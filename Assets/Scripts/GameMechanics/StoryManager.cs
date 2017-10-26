// StoryManager loads a scene based on a SceneDescription, including loading
// images, audio files, and drawing colliders and setting up callbacks to
// handle trigger events. StoryManager uses SceneManipulationAPI for setting up
// these callbacks.

using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class StoryManager : MonoBehaviour {

	public GameObject portraitGraphicsPanel;
    public GameObject portraitTextPanel;

    public GameObject landscapeGraphicsPanel;
    public GameObject landscapeTextPanel;
	public GameObject landscapeWideGraphicsPanel;
	public GameObject landscapeWideTextPanel;

    // Used for internal references.
    private GameObject graphicsPanel;
    private GameObject textPanel;
    private GameObject currentStanza;

    private float graphicsPanelWidth;
    private float graphicsPanelHeight;
    private float graphicsPanelAspectRatio;

    // Variables for loading TinkerTexts.
    private float STANZA_SPACING = 20; // Matches Prefab.
    private float MIN_TINKER_TEXT_WIDTH = TinkerText.MIN_WIDTH;
    private float TEXT_HEIGHT = TinkerText.BUTTON_TEXT_HEIGHT;
    private float remainingStanzaWidth = 0; // For loading TinkerTexts.

    // Dynamically created Stanzas.
    private List<GameObject> stanzas;
    // Dynamically created TinkerTexts specific to this scene.
    private List<GameObject> tinkerTexts;
    // Dynamically created SceneObjects, keyed by their human-given name.
    private Dictionary<string, GameObject> sceneObjects;
    // The image we loaded for this scene.
    private GameObject storyImage;
    // Need to know the actual dimensions of the background image, so that we
    // can correctly place new SceneObjects on the background.
    private float storyImageWidth;
    private float storyImageHeight;
    // The (x,y) coordinates of the upper left corner of the image in relation
    // to the upper left corner of the encompassing GameObject.
    private float storyImageX;
    private float storyImageY;
    // Ratio of the story image to the original texture size.
    private float imageScaleFactor;
    private DisplayMode displayMode;

	void Awake() {
        Logger.Log("StoryManager awake");
    }

    void Start() {
        Logger.Log("StoryManager start");

        this.SetDisplayMode(DisplayMode.Landscape);
        this.tinkerTexts = new List<GameObject>();
        this.stanzas = new List<GameObject>();
        this.sceneObjects = new Dictionary<string, GameObject>();
    }

    // Main function to be called by GameController.
    // Passes in a description received over ROS or hardcoded.
    // LoadScene is responsible for loading all resources and putting them in
    // place, and attaching callbacks to created GameObjects, where these
    // callbacks involve functions from SceneManipulatorAPI.
    public void LoadScene(SceneDescription description) {
        Logger.Log(description.storyImageFile);
        this.loadImage(description.storyImageFile);

        // Load all words as TinkerText. Begin at beginning of a stanza.
        this.remainingStanzaWidth = 0;
        foreach (string word in description.text.Split(' ')) {
            this.loadTinkerText(word);
        }

        // Load all scene objects.
        foreach (SceneObject sceneObject in description.sceneObjects) {
            this.loadSceneObject(sceneObject);
        }

        // Load triggers.
        //foreach (Trigger trigger in description.triggers) {
        //    this.loadTrigger(trigger);
        //}

    }

    // Argument imageFile should be something like "the_hungry_toad_01" and then
    // this function will find it in the Resources directory and load it.
    private void loadImage(string imageFile) {
        string storyName = imageFile.Substring(0,
            imageFile.LastIndexOf("_", StringComparison.CurrentCulture)
        );
        GameObject newObj = new GameObject();
        newObj.AddComponent<Image>();
        newObj.AddComponent<AspectRatioFitter>();
        newObj.transform.SetParent(this.graphicsPanel.transform, false);
        newObj.transform.localPosition = Vector3.zero;
        newObj.GetComponent<AspectRatioFitter>().aspectMode =
                  AspectRatioFitter.AspectMode.FitInParent;
        newObj.GetComponent<AspectRatioFitter>().aspectRatio =
                  this.graphicsPanelAspectRatio;
        string fullImagePath = "StoryPages/" + storyName + "/" + imageFile;
        Sprite sprite = Resources.Load<Sprite>(fullImagePath);
        newObj.GetComponent<Image>().sprite = sprite;
        newObj.GetComponent<Image>().preserveAspect = true;
        // Figure out sizing so that later scene objects can be loaded relative
        // to the background image for accurate overlay.
        Texture texture = Resources.Load<Texture>(fullImagePath);
        Logger.Log(texture.width);
        Logger.Log(texture.height);
        float imageAspectRatio = (float)texture.width / (float)texture.height;
        if (imageAspectRatio > this.graphicsPanelAspectRatio) {
            // Width is the constraining factor.
            this.storyImageWidth = this.graphicsPanelWidth;
            this.storyImageHeight = this.graphicsPanelWidth / imageAspectRatio;
            this.storyImageX = 0;
            this.storyImageY = 
                -(this.graphicsPanelHeight - this.storyImageHeight) / 2;
        } else {
            // Height is the constraining factor.
            this.storyImageHeight = this.graphicsPanelHeight;
            this.storyImageWidth = this.graphicsPanelHeight * imageAspectRatio;
            this.storyImageY = 0;
            this.storyImageX = 
                (this.graphicsPanelWidth - this.storyImageWidth) / 2;
        }
        this.imageScaleFactor = this.storyImageWidth / texture.width;
        this.storyImage = newObj;
    }

    // Add a new TinkerText for the given word.
    private void loadTinkerText(string word) {
        if (word.Length == 0) {
            return;
        }
		// Create a new stanza if we there's not enough stanza width left.
		// Figure out how wide this word will be, first create the word.
		GameObject newTinkerText =
            Instantiate((GameObject)Resources.Load("Prefabs/TinkerText"));
        GameObject newText = newTinkerText.GetComponent<TinkerText>().text;
        newText.GetComponent<Text>().text = word;
        float preferredWidth =
            LayoutUtility.GetPreferredWidth(
                newText.GetComponent<RectTransform>()
            );
        preferredWidth = Math.Max(preferredWidth, this.MIN_TINKER_TEXT_WIDTH);
        newText.GetComponent<RectTransform>().sizeDelta =
                   new Vector2(preferredWidth, this.TEXT_HEIGHT);
        //Logger.Log(word + " " + preferredWidth.ToString() +
                   //" " + remainingStanzaWidth.ToString());
        if (preferredWidth > this.remainingStanzaWidth){
            // Add a new stanza.
            GameObject newStanza =
                Instantiate((GameObject)Resources.Load("Prefabs/StanzaPanel"));
            newStanza.transform.SetParent(this.textPanel.transform, false);
            this.stanzas.Add(newStanza);
            this.currentStanza = newStanza;
            // Reset the remaining stanza width.
            this.remainingStanzaWidth =
                    this.textPanel.GetComponent<RectTransform>().sizeDelta.x;
        }
		// Initialize the TinkerText width correctly.
        // Set new TinkerText parent to be the stanza.
        // TODO: set TinkerText id based on the info from JSON SceneDescription.
        // Probably just set them in order or something.
		newTinkerText.GetComponent<TinkerText>().Init(1, preferredWidth);
		newTinkerText.transform.SetParent(this.currentStanza.transform, false);
        this.remainingStanzaWidth -= preferredWidth;
        this.remainingStanzaWidth -= STANZA_SPACING;
        //Logger.Log("remaining: " + remainingStanzaWidth.ToString());
        this.tinkerTexts.Add(newTinkerText);
    }

    // Adds a SceneObject to the story scene.
    private void loadSceneObject(SceneObject sceneObject) {
        GameObject newObj = 
            Instantiate((GameObject)Resources.Load("Prefabs/SceneObject"));
        newObj.transform.SetParent(this.graphicsPanel.transform, false);
        newObj.GetComponent<RectTransform>().SetAsLastSibling();
        this.sceneObjects[sceneObject.label] = newObj;
        // Set the position.
        SceneObjectManipulator manip =
            newObj.GetComponent<SceneObjectManipulator>();
        Position pos = sceneObject.position;
        manip.Move(
            new Vector3(this.storyImageX + pos.left * this.imageScaleFactor,
                        this.storyImageY - pos.top * this.imageScaleFactor)
        )();
        manip.ChangeSize(
            new Vector2(pos.width * this.imageScaleFactor,
                        pos.height * this.imageScaleFactor)
        )();
        // Test adding a click handler.
        this.tinkerTexts[0].GetComponent<TinkerText>()
            .AddClickHandler(manip.Highlight(Color.blue));
    }

    // Called by GameController when we should remove all elements we've added
    // to this page (usually in preparration for the creation of another page).
    public void ClearScene() {
        // Destroy stanzas.
        foreach (GameObject stanza in this.stanzas) {
            Destroy(stanza);
        }
        this.stanzas.Clear();
        // Destroy TinkerText objects we have a reference to, and reset list.
        foreach (GameObject tinkertext in this.tinkerTexts) {
            Destroy(tinkertext);
        }
        this.tinkerTexts.Clear();
        // Destroy SceneObjects we have a reference to, and empty dictionary.
        foreach (KeyValuePair<string,GameObject> obj in this.sceneObjects) {
            Destroy(obj.Value);
        }
        this.sceneObjects.Clear();
        // Remove all images.
        Destroy(this.storyImage.gameObject);
        this.storyImage = null;
    }

    // Called by GameController to rotate display mode. We need to update our
    // internal references to textPanel and graphicsPanel.
    public void SetDisplayMode(DisplayMode newMode) {
        this.displayMode = newMode;
        switch (this.displayMode)
        {
            case DisplayMode.Landscape:
                this.graphicsPanel = this.landscapeGraphicsPanel;
                this.textPanel = this.landscapeTextPanel;
                break;
            case DisplayMode.LandscapeWide:
                this.graphicsPanel = this.landscapeWideGraphicsPanel;
                this.textPanel = this.landscapeWideTextPanel;
                break;
            case DisplayMode.Portrait:
                this.graphicsPanel = this.portraitGraphicsPanel;
                this.textPanel = this.portraitTextPanel;
                break;
            default:
                Logger.LogError("unknown display mode " + newMode);
                break;
        }
        Vector2 rect =
            this.graphicsPanel.GetComponent<RectTransform>().sizeDelta;
        this.graphicsPanelWidth = (float)rect.x;
        this.graphicsPanelHeight = (float)rect.y;
        this.graphicsPanelAspectRatio =
            this.graphicsPanelWidth / this.graphicsPanelHeight;
    }

}
