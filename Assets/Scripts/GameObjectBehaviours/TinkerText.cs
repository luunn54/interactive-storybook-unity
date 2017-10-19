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

    // UnityActions for various UI interactions (e.g. clicking).
    private UnityAction clickUnityAction;

    // These numbers should match the prefab, putting them here is just for
    // convenience when setting sizeDelta.
    private float TINKER_TEXT_HEIGHT = 300; // Height of entire TinkerText, including graphic.
    private float BUTTON_TEXT_HEIGHT = 120; // Height of the button and text components.

    // Set up click handler.
    // TODO: Consider using MouseDown and MouseUp instead of Click?
    void Start() {
        this.textButton.GetComponent<Button>().onClick.AddListener(this.clickUnityAction);
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
    public void Init(float newWidth)
    {
        gameObject.SetActive(true);
        // Update size of TinkerText.
        GetComponent<RectTransform>().sizeDelta = new Vector2(newWidth, this.TINKER_TEXT_HEIGHT);
        // Update size of Button.
        this.textButton.GetComponent<RectTransform>().sizeDelta = new Vector2(newWidth, this.BUTTON_TEXT_HEIGHT);

        //    Action action = this.HelloWorld();
        //    this.AddClickHandler(action);
    }

    // Add a new action to the UnityAction click handler.
    public void AddClickHandler(Action action)
    {
        this.clickUnityAction += new UnityAction(action);
    }

    // Test using lambda expressions.
    private Action HelloWorld()
    {
        return () => { Logger.Log("hello world action "); };
    }

}
