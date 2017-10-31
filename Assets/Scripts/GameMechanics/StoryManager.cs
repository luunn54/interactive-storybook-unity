// StoryManager loads a scene based on a SceneDescription, including loading
// images, audio files, and drawing colliders and setting up callbacks to
// handle trigger events. StoryManager uses methods in TinkerText and
// SceneObjectManipulator for setting up these callbacks.

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using System.Collections.Generic;

public class StoryManager : MonoBehaviour {

    // We may want to call methods on GameController or add to the task queue.
    public GameController gameController;

	public GameObject portraitGraphicsPanel;
    public GameObject portraitTextPanel;
    public GameObject landscapeGraphicsPanel;
    public GameObject landscapeTextPanel;
	public GameObject landscapeWideGraphicsPanel;
	public GameObject landscapeWideTextPanel;

    public GameObject portraitTitlePanel;
    public GameObject landscapeTitlePanel;

    // Used for internal references.
    private GameObject graphicsPanel;
    private GameObject textPanel;
    private GameObject titlePanel;
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

        this.gameController = GetComponent<GameController>();

        this.setDisplayMode(DisplayMode.Landscape);
        this.tinkerTexts = new List<GameObject>();
        this.stanzas = new List<GameObject>();
        this.sceneObjects = new Dictionary<string, GameObject>();
    }

    // Main function to be called by GameController.
    // Passes in a description received over ROS or hardcoded.
    // LoadScene is responsible for loading all resources and putting them in
    // place, and attaching callbacks to created GameObjects, where these
    // callbacks involve functions from SceneManipulatorAPI.
    public void LoadPage(SceneDescription description) {
        this.setDisplayMode(description.displayMode);

        // Special case for title page.
        if (description.isTitle) {
            this.loadTitlePage(description);
            return;
        }

        // Load image.
        this.loadImage(description.storyImageFile);

        // Load all words as TinkerText. Begin at beginning of a stanza.
        this.remainingStanzaWidth = 0;
        foreach (string word in description.text.Split(' ')) {
            this.loadTinkerText(word);
        }

        // TODO: figure out how to load audio. WHat object to attach it to?
        // Its own invisible object? How to set up the triggering of words?
        // Give it timestamps and while playing whenever it passes those
        // timestamps it knows what triggers to make.

        // Load all scene objects.
        foreach (SceneObject sceneObject in description.sceneObjects) {
            this.loadSceneObject(sceneObject);
        }

        // Load triggers.
        foreach (Trigger trigger in description.triggers) {
            this.loadTrigger(trigger);
        }

    }

    private void loadTitlePage(SceneDescription description) {
        // Load the into the title panel without worrying about anything except
        // for fitting the space and making the aspect ratio correct.
        // Basically the same as first half of loadImage() function.
        string imageFile = description.storyImageFile;
        string storyName = imageFile.Substring(0,
            imageFile.LastIndexOf("_", StringComparison.CurrentCulture)
        );
        GameObject newObj = new GameObject();
        newObj.AddComponent<Image>();
        newObj.AddComponent<AspectRatioFitter>();
        newObj.transform.SetParent(this.titlePanel.transform, false);
        newObj.transform.localPosition = Vector3.zero;
        newObj.GetComponent<AspectRatioFitter>().aspectMode =
                  AspectRatioFitter.AspectMode.FitInParent;
        newObj.GetComponent<AspectRatioFitter>().aspectRatio =
                  this.graphicsPanelAspectRatio;
        string fullImagePath = "StoryPages/" + storyName + "/" + imageFile;
        Sprite sprite = Resources.Load<Sprite>(fullImagePath);
        newObj.GetComponent<Image>().sprite = sprite;
        newObj.GetComponent<Image>().preserveAspect = true;
        this.storyImage = newObj;

        // TODO: load the audio.
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
        newTinkerText.GetComponent<TinkerText>()
                     .Init(this.tinkerTexts.Count, preferredWidth);
		newTinkerText.transform.SetParent(this.currentStanza.transform, false);
        this.remainingStanzaWidth -= preferredWidth;
        this.remainingStanzaWidth -= STANZA_SPACING;
        this.tinkerTexts.Add(newTinkerText);
    }

    // Adds a SceneObject to the story scene.
    private void loadSceneObject(SceneObject sceneObject) {
        // TODO: handle multiple objects per label. For now, only allow one.
        if (this.sceneObjects.ContainsKey(sceneObject.label)) {
            return;
        }
        GameObject newObj = 
            Instantiate((GameObject)Resources.Load("Prefabs/SceneObject"));
        newObj.transform.SetParent(this.graphicsPanel.transform, false);
        newObj.GetComponent<RectTransform>().SetAsLastSibling();
        // Set the position.
        SceneObjectManipulator manip =
            newObj.GetComponent<SceneObjectManipulator>();
        Position pos = sceneObject.position;
        manip.label = sceneObject.label;
        manip.MoveToPosition(
            new Vector3(this.storyImageX + pos.left * this.imageScaleFactor,
                        this.storyImageY - pos.top * this.imageScaleFactor)
        )();
        manip.ChangeSize(
            new Vector2(pos.width * this.imageScaleFactor,
                        pos.height * this.imageScaleFactor)
        )();
        // Add a dummy handler to check things.
        manip.AddClickHandler(() =>
        {
            Logger.Log("SceneObject clicked " +
                       manip.label);
        });
        newObj.name = sceneObject.label;
        this.sceneObjects[sceneObject.label] = newObj;
    }

    // Sets up a trigger between TinkerTexts and SceneObjects.
    private void loadTrigger(Trigger trigger) {
        Logger.Log(trigger.sceneObjectLabel);
        SceneObjectManipulator manip = 
            this.sceneObjects[trigger.sceneObjectLabel]
                .GetComponent<SceneObjectManipulator>();
        TinkerText tinkerText = this.tinkerTexts[trigger.textId]
                                    .GetComponent<TinkerText>();
        Action action = new Action(() => {});
        // TODO: move some of this logic into SceneObjectManipulator
        // instead of in loadTrigger. Right now, the switch statements grow
        // linearly with the number of action types and condition types.
        switch (trigger.action.type) {
            case ActionType.Highlight:
                Logger.Log(trigger.action.args.color.ToString());
                action = manip.Highlight(trigger.action.args.color);
                break;
            case ActionType.MoveToPosition:
                break;
            case ActionType.ChangeSize:
                break;
            default:
                Logger.LogError("Unknown trigger action: " +
                                trigger.action.type.ToString());
                break;
        }
        switch (trigger.condition.type) {
            case ConditionType.Click:
                Logger.Log(tinkerText.GetId());
                Logger.Log(manip.gameObject.name);
                tinkerText.AddClickHandler(action);
                break;
            default:
                Logger.LogError("Unknown condition type: " +
                                trigger.condition.type.ToString());
                break;
        }
    }


    // Called by GameController when we should remove all elements we've added
    // to this page (usually in preparration for the creation of another page).
    public void ClearPage() {
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

    // Update the display mode. We need to update our internal references to
    // textPanel and graphicsPanel.
    public void setDisplayMode(DisplayMode newMode) {
        if (this.displayMode != newMode) {
            this.displayMode = newMode;
            switch (this.displayMode)
            {
                case DisplayMode.Landscape:
                    this.graphicsPanel = this.landscapeGraphicsPanel;
                    this.textPanel = this.landscapeTextPanel;
                    this.titlePanel = this.landscapeTitlePanel;
                    break;
                case DisplayMode.LandscapeWide:
                    this.graphicsPanel = this.landscapeWideGraphicsPanel;
                    this.textPanel = this.landscapeWideTextPanel;
                    this.titlePanel = this.landscapeTitlePanel;
                    break;
                case DisplayMode.Portrait:
                    this.graphicsPanel = this.portraitGraphicsPanel;
                    this.textPanel = this.portraitTextPanel;
                    this.titlePanel = this.portraitTitlePanel;
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

}
