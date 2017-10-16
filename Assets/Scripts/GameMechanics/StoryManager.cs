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

    // public GameObject testObject;

    // Dynamically created GameObjects specific to this scene,
    // each keyed by their human-given name.
    private Dictionary<string, GameObject> sceneObjects;

    private string displayMode = "landscape"; // TODO: use enum.

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
        this.LoadImage(description.getStoryImageFile());

        //// Triggers
        //foreach (Trigger trigger in description.getTriggers()) {
        //    this.loadTrigger(trigger);
        //}

    }

    // Argument imageFile should be something like "the_hungry_toad_01" and then
    // this function will find it in the Resources directory and load it.
    private void LoadImage(string imageFile) {
        string storyName = imageFile.Substring(0, imageFile.LastIndexOf("_", StringComparison.CurrentCulture));
        GameObject newObj = new GameObject();
        newObj.AddComponent<Image>();
        newObj.AddComponent<AspectRatioFitter>();
        newObj.transform.SetParent(this.landscapeGraphicsPanel.transform, false);
        newObj.transform.localPosition = Vector3.zero;
        newObj.GetComponent<AspectRatioFitter>().aspectMode = AspectRatioFitter.AspectMode.FitInParent;
		string fullImagePath = "StoryPages/" + storyName + "/" + imageFile;
        Logger.Log(fullImagePath);
        newObj.GetComponent<Image>().sprite = Resources.Load(fullImagePath) as Sprite;
        newObj.GetComponent<Image>().preserveAspect = true;
        Logger.Log("finished loading resource");

		// SceneObjects - for now just text.
		GameObject testObject = new GameObject();
		testObject.transform.SetParent(newObj.transform, false);
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
        this.displayMode = newMode;
    }

}
