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

    // Dynamically created TinkerTexts specific to this scene.
    private List<GameObject> tinkerTexts;

    // Dynamically created SceneObjects, keyed by their human-given name.
    private Dictionary<string, GameObject> sceneObjects;

    private string displayMode = "landscape"; // TODO: use enum?

	void Awake() {
        Logger.Log("StoryManager awake");
    }

    void Start() {
        Logger.Log("StoryManager start");

        this.graphicsPanel = landscapeGraphicsPanel;
        this.textPanel = landscapeTextPanel;
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

        // Load all words as tinkertext.
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
    }

    // Add a new TinkerText ofor the given word.
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
            this.currentStanza = newStanza;
            // Reset the remaining stanza width.
            this.remainingStanzaWidth = this.textPanel.GetComponent<RectTransform>().sizeDelta.x;
        }
		// Initialize the TinkerText width correctly.
        // Set new TinkerText parent to be the stanza.
		newTinkerText.GetComponent<TinkerText>().Init(preferredWidth);
		newTinkerText.transform.SetParent(this.currentStanza.transform, false);
        this.remainingStanzaWidth -= preferredWidth;
        this.tinkerTexts.Add(newTinkerText);
    }

    private void loadBoundingBox() {
        // Create empty game object and try to position it correct.
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

    // GameController can tell us to rotate mode.
    public void SetDisplayMode(string newMode) {
        if (newMode != this.displayMode){
            this.displayMode = newMode;
            if (this.displayMode == "landscape") {
                this.graphicsPanel = this.landscapeGraphicsPanel;
                this.textPanel = this.landscapeTextPanel;
            } else if (this.displayMode == "landscapeWide") {
                this.graphicsPanel = this.landscapeWideGraphicsPanel;
                this.textPanel = this.landscapeWideTextPanel;
            } else if (this.displayMode == "portrait") {
                this.graphicsPanel = this.portraitGraphicsPanel;
                this.textPanel = this.portraitTextPanel;
            } else {
                Logger.LogError("unknown display mode " + newMode);
            }
        }
    }

}
