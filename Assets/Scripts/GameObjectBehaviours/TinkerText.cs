using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System;
using System.Collections;


// TinkerText is the script added to all GameObjects dynamically created and
// that are intended to be text.
//
// A TinkerText consists of a simple text box and in the future potentially an
// associated animated gif.
//
// A TinkerText can also be paired with other sprites on the story image, and
// triggers between the TinkerText and the sprites are orchestrated by
// StoryManager via methods in TinkerText and SceneObjectManipulator. 
public class TinkerText : MonoBehaviour
{

    // Have a reference tot he children objects in this TinkerText.
    public GameObject textButton;
    public GameObject text;
    public GameObject graphicPanel;
    private AnimationClip graphic;

    // These numbers should match the prefab, putting them here is just for
    // convenience when setting sizeDelta.
	private float TINKER_TEXT_HEIGHT = 300; // Height of entire TinkerText, including graphic.
    private float BUTTON_TEXT_HEIGHT = 120; // Height of the button and text components.

	// Necessary layout set up for all TinkerTexts.
	void Start() {
        Logger.Log("TinkerText start");
	}

    // Shouldn't need this, should handle events through OnMouseDown and
    // OnMouseUp.
	void Update() {
			
	}

    // StoryManager will call this to give TinkerText a chance to readjust the
    // width of all the text and the textButton and the overall TinkerText.
    // Also need to set the component to active. 
    // Don't need to know anything about its position, the layout groups
    // should automatically handle that.
    public void Init(float newWidth) {
        gameObject.SetActive(true);
        // Update size of TinkerText.
		GetComponent<RectTransform>().sizeDelta = new Vector2(newWidth, this.TINKER_TEXT_HEIGHT);
        // Update size of Button.
        this.textButton.GetComponent<RectTransform>().sizeDelta = new Vector2(newWidth, this.BUTTON_TEXT_HEIGHT);
        Logger.Log("  updated size of button and tinkertext");
        //UnityAction action = new UnityAction(this.HelloWorld()); ; 
        //this.AddClickHandler(action);
        this.textButton.GetComponent<Button>().onClick.AddListener(this.Testing);
    }

    // UnityAction is conceptually just an array of functions to be called
    // sequentially.
    public void AddClickHandler(UnityAction unityAction) {
        unityAction += new UnityAction(this.HelloWorld());
        unityAction += this.Testing;
        this.textButton.GetComponent<Button>().onClick.AddListener(unityAction);
        //this.textButton.GetComponent<Button>().onClick.AddListener(this.Testing);
    }

    private Action HelloWorld() {
        return () => { Logger.Log("hello world action"); };
    }

    public void Testing() {
        Logger.Log("testing");
    }

}
