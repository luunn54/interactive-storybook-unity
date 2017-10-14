// SceneManager loads a scene based on a SceneDescription, including loading
// images, audio files, and drawing colliders and setting up callbacks to
// handle trigger events. SceneManager uses SceneManipulationAPI for setting up
// these callbacks.

using UnityEngine;
using UnityEngine.UI;

public class SceneManager : MonoBehaviour {

	public GameObject portraitGraphicsPanel;
    public GameObject portraitTextPanel;

    public GameObject landscapeGraphicsPanel;
    public GameObject landscapeTextPanel;
	public GameObject landscapeWideGraphicsPanel;
	public GameObject landscapeWideTextPanel;

    private string displayMode = "landscape"; // TODO: use enum.

	void Awake() {
        Logger.Log("SceneManager awake");
    }

    void Start() {
        Logger.Log("SceneManager start");
        // Get all references as needed.
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
        Logger.Log(description);
    }

    public void LoadImage() {
        GameObject newObj = new GameObject();
        newObj.AddComponent<RawImage>();
        newObj.AddComponent<AspectRatioFitter>();
        newObj.transform.SetParent(landscapeGraphicsPanel.transform, false);
        newObj.GetComponent<AspectRatioFitter>().aspectMode = AspectRatioFitter.AspectMode.FitInParent;
        Texture2D texture = Resources.Load("StoryPages/the_hungry_toad/the_hungry_toad_01") as Texture2D;
        newObj.GetComponent<RawImage>().texture = texture;
        Logger.Log("finished loading resource");
    }

    // GameController can tell us to rotate mode.
    public void setDisplayMode(string newMode) {
        this.displayMode = newMode;
    }

}
