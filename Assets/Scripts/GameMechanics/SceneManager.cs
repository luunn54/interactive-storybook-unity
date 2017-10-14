// SceneManager loads a scene based on a SceneDescription, including loading
// images, audio files, and drawing colliders and setting up callbacks to
// handle trigger events. SceneManager uses SceneManipulationAPI for setting up
// these callbacks.

using UnityEngine;
using UnityEngine.UI;
using System;

public class SceneManager : MonoBehaviour {

	public GameObject portraitGraphicsPanel;
    public GameObject portraitTextPanel;

    public GameObject landscapeGraphicsPanel;
    public GameObject landscapeTextPanel;
	public GameObject landscapeWideGraphicsPanel;
	public GameObject landscapeWideTextPanel;

    // Used for internal references.
    private GameObject graphicsPanel;
    private GameObject textPanel;

    private string displayMode = "landscape"; // TODO: use enum.

	void Awake() {
        Logger.Log("SceneManager awake");
    }

    void Start() {
        Logger.Log("SceneManager start");

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

    }

    private void LoadImage(string imageFile) {
        string storyName = imageFile.Substring(0, imageFile.LastIndexOf("_"));
        GameObject newObj = new GameObject();
        newObj.AddComponent<RawImage>();
        newObj.AddComponent<AspectRatioFitter>();
        newObj.transform.SetParent(this.landscapeGraphicsPanel.transform, false);
        newObj.GetComponent<AspectRatioFitter>().aspectMode = AspectRatioFitter.AspectMode.FitInParent;
		string fullImagePath = "StoryPages/" + storyName + "/" + imageFile;
		Texture2D texture = Resources.Load(fullImagePath) as Texture2D;
        Logger.Log(fullImagePath);
        newObj.GetComponent<RawImage>().texture = texture;
        Logger.Log("finished loading resource");
    }

    // GameController can tell us to rotate mode.
    public void SetDisplayMode(string newMode) {
        this.displayMode = newMode;
    }

}
