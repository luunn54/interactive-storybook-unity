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
    private GameObject currentStanza; // Stanza is just for disaply, we keep references to all TinkerText so it's not necessary to reference through a stanza.

    // Variables for loading TinkerTexts.
    private float MIN_TINKER_TEXT_WIDTH = 200; // Based on size of gifs.
    private float TEXT_HEIGHT = 100; // The actual text is smaller than whole TinkerText.
    private float remainingStanzaWidth = 0; // Width remaining in current stanza.

    // Dynamically created Stanzas.
    private List<GameObject> stanzas;
    // Dynamically created TinkerTexts specific to this scene.
    private List<GameObject> tinkerTexts;
    // Dynamically created SceneObjects, keyed by their human-given name.
    private Dictionary<string, GameObject> sceneObjects;
    // The image we loaded for this scene.
    private GameObject storyImage;

    private GameObject testObj;

    private string displayMode = "landscape"; // TODO: use enum?

	void Awake() {
        Logger.Log("StoryManager awake");
    }

    void Start() {
        Logger.Log("StoryManager start");

        this.graphicsPanel = landscapeGraphicsPanel;
        this.textPanel = landscapeTextPanel;
        this.tinkerTexts = new List<GameObject>();
        this.stanzas = new List<GameObject>();
        this.sceneObjects = new Dictionary<string, GameObject>();

        this.testObj = new GameObject();
        testObj.AddComponent<RectTransform>();
        testObj.transform.SetParent(this.graphicsPanel.transform, false);
        //testObj.GetComponent<RectTransform>().localPosition = Vector3.zero;
        testObj.GetComponent<RectTransform>().sizeDelta = new Vector2(50, 50);
        testObj.AddComponent<Image>();
        testObj.GetComponent<Image>().color = Color.red;

        this.testObj.AddComponent<SceneObjectManipulator>();
    }

    public void HelloWorld() {
        Logger.Log("I can call methods");
    }

    // Main function to be called by GameController.
    // Passes in a description received over ROS or hardcoded.
    // LoadScene is responsible for loading all resources and putting them in
    // place, and attaching callbacks to created GameObjects, where these
    // callbacks involve functions from SceneManipulatorAPI.
    public void LoadScene(SceneDescription description) {
        Logger.Log(description.getStoryImageFile());
        this.loadImage(description.getStoryImageFile());

        // Load all words as TinkerText. Begin at beginning of a stanza.
        this.remainingStanzaWidth = 0;
        foreach (string word in description.getText().Split(' ')) {
            this.loadTinkerText(word);
        }

        // Triggers
        //foreach (Trigger trigger in description.getTriggers()) {
        //    this.loadTrigger(trigger);
        //}

    }

    // Argument imageFile should be something like "the_hungry_toad_01" and then
    // this function will find it in the Resources directory and load it.
    private void loadImage(string imageFile) {
        string storyName = imageFile.Substring(0, imageFile.LastIndexOf("_", StringComparison.CurrentCulture));
        GameObject newObj = new GameObject();
        newObj.AddComponent<Image>();
        newObj.AddComponent<AspectRatioFitter>();
        newObj.transform.SetParent(this.graphicsPanel.transform, false);
        newObj.transform.localPosition = Vector3.zero;
        newObj.GetComponent<AspectRatioFitter>().aspectMode = AspectRatioFitter.AspectMode.FitInParent;
		string fullImagePath = "StoryPages/" + storyName + "/" + imageFile;
        Sprite sprite = Resources.Load<Sprite>(fullImagePath);
        newObj.GetComponent<Image>().sprite = sprite;
        newObj.GetComponent<Image>().preserveAspect = true;
        this.storyImage = newObj;
    }

    // Add a new TinkerText for the given word.
    private void loadTinkerText(string word) {
        if (word.Length == 0) {
            return;
        }
        Logger.Log("got word: " + word);
		// Create a new stanza if we there's not enough stanza width left.
		// Figure out how wide this word will be, first create the word.
		GameObject newTinkerText = (GameObject)Instantiate((GameObject)Resources.Load("Prefabs/TinkerText"));
        GameObject newText = newTinkerText.GetComponent<TinkerText>().text;
        newText.GetComponent<Text>().text = word;
        float preferredWidth = LayoutUtility.GetPreferredWidth(newText.GetComponent<RectTransform>());
        preferredWidth = Math.Max(preferredWidth, this.MIN_TINKER_TEXT_WIDTH);
        newText.GetComponent<RectTransform>().sizeDelta = new Vector2(preferredWidth, this.TEXT_HEIGHT);
        if (preferredWidth > this.remainingStanzaWidth){
            // Add a new stanza.
            GameObject newStanza = (GameObject)Instantiate((GameObject)Resources.Load("Prefabs/StanzaPanel"));
            newStanza.transform.SetParent(this.textPanel.transform, false);
            this.stanzas.Add(newStanza);
            this.currentStanza = newStanza;
            // Reset the remaining stanza width.
            this.remainingStanzaWidth = this.textPanel.GetComponent<RectTransform>().sizeDelta.x;
        }
		// Initialize the TinkerText width correctly.
        // Set new TinkerText parent to be the stanza.
		newTinkerText.GetComponent<TinkerText>().Init(preferredWidth);
		newTinkerText.transform.SetParent(this.currentStanza.transform, false);
        SceneObjectManipulator manip = testObj.GetComponent<SceneObjectManipulator>();
        newTinkerText.GetComponent<TinkerText>().AddClickHandler(manip.Highlight("blue"));
        this.remainingStanzaWidth -= preferredWidth;
        this.tinkerTexts.Add(newTinkerText);
    }

    private void loadBoundingBox() {
        // Create empty game object and try to position it correctly.
        GameObject testObject = new GameObject();
        testObject.transform.SetParent(this.graphicsPanel.transform, false);
        Logger.Log(testObject.transform.parent.gameObject.transform.name);
        //testObject.AddComponent<RectTransform>();
        //testObject.GetComponent<RectTransform>().sizeDelta = new Vector2(100, 100);
        testObject.AddComponent<BoxCollider2D>();
        testObject.GetComponent<BoxCollider2D>().enabled = true;
        testObject.transform.localPosition = Vector3.zero;
        testObject.transform.localScale = new Vector3(1, 1, 1);
        testObject.AddComponent<SceneObjectManipulator>();
    }

    // Called by GameController when we should remove all elements we've added
    // to this page (usually in preparration for the creation of another page).
    public void ClearScene() {
        // Destroy stanzas.
        foreach (GameObject stanza in this.stanzas) {
            Destroy(stanza);
        }
        // Destroy TinkerText objects we have a reference to, and reset list.
        foreach (GameObject tinkertext in this.tinkerTexts) {
            Destroy(tinkertext);
        }
        this.tinkerTexts = new List<GameObject>();
        // Destroy SceneObjects we have a reference to, and empty dictionary.
        foreach (KeyValuePair<string,GameObject> obj in this.sceneObjects) {
            Destroy(obj.Value);
            this.sceneObjects.Remove(obj.Key);
        }
        // Remove all images.
        Destroy(this.storyImage.gameObject);
        this.storyImage = null;
    }

    // Called by GameController to rotate display mode. We need to update our
    // internal references to textPanel and graphicsPanel.
    public void SetDisplayMode(string newMode) {
        if (newMode != this.displayMode){
            this.displayMode = newMode;
            switch (this.displayMode)
            {
                case "landscape":
                    this.graphicsPanel = this.landscapeGraphicsPanel;
                    this.textPanel = this.landscapeTextPanel;
                    break;
                case "landscapeWide":
                    this.graphicsPanel = this.landscapeWideGraphicsPanel;
                    this.textPanel = this.landscapeWideTextPanel;
                    break;
                case "portrait":
                    this.graphicsPanel = this.portraitGraphicsPanel;
                    this.textPanel = this.portraitTextPanel;
                    break;
                default:
                    Logger.LogError("unknown display mode " + newMode);
                    break;
            }
        }
    }

}
