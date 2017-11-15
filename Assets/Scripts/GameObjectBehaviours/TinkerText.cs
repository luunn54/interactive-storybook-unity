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

    private int id;
    private float textWidth;
    public float audioStartTime, audioEndTime;

    // Have a reference tot he children objects in this TinkerText.
    public GameObject textButton;
    public GameObject text;
    public GameObject graphicPanel;
    private AnimationClip graphic;

    // UnityActions for various UI interactions (e.g. clicking).
    private UnityAction clickUnityAction;

    // These numbers should match the prefab, putting them here is just for
    // convenience when setting sizeDelta.
    // Height of entire TinkerText, including graphic.
    public static float TINKER_TEXT_HEIGHT = 165;
    public static float MIN_WIDTH = 120; // Based on size of GIFs.
    // Height of the button and text components.
    public static float BUTTON_TEXT_HEIGHT = 85;

    // Set up click handler.
    // TODO: Consider using MouseDown and MouseUp instead of Click?
    void Start() {
        // It's important to do += here and not = for clickUnityAction.
        // Need to initialize otherwise will get NullReferenceException.
        this.clickUnityAction += () => { };
        this.textButton.GetComponent<Button>()
            .onClick.AddListener(this.clickUnityAction);
    }

    public int GetId() {
        return this.id;
    }

    // StoryManager will call this to give TinkerText a chance to readjust the
    // width of all the text and the textButton and the overall TinkerText.
    // Also need to set the component to active. 
    // Don't need to know anything about its position, the layout groups
    // should automatically handle that.
    public void Init(int id, string word, AudioTimestamp timestamp) {
        this.id = id;
        this.text.GetComponent<Text>().text = word;
        this.audioStartTime = timestamp.start;
        this.audioEndTime = timestamp.end;
        gameObject.SetActive(true);
    }

    public void SetWidth(float newWidth) {
        // Update size of TinkerText.
        GetComponent<RectTransform>().sizeDelta =
            new Vector2(newWidth, TINKER_TEXT_HEIGHT);
        // Update size of Button.
        this.textButton.GetComponent<RectTransform>().sizeDelta =
            new Vector2(newWidth, BUTTON_TEXT_HEIGHT);
        this.text.GetComponent<RectTransform>().sizeDelta =
            new Vector2(newWidth, BUTTON_TEXT_HEIGHT);
        this.textWidth = newWidth;
    }

    // Set whether or not the TinkerText is clickable.
    // (E.g. turn off clicking when auto reading is happening, then turn back
    // on when in explore mode on the page.
    public void SetClickable(bool clickable) {
        this.textButton.GetComponent<Button>().interactable = clickable;
    }

    // Add a new action to the UnityAction click handler.
    public void AddClickHandler(Action action) {
        this.clickUnityAction += new UnityAction(action);
    }

    public void OnStartAudioTrigger() {
        Logger.Log(id.ToString() + " start" + this.ToString());
        // Change the text color.
        this.text.GetComponent<Text>().color = Color.magenta;
    }

    public void OnEndAudioTrigger() {
        Logger.Log(id.ToString() + " end");
        this.text.GetComponent<Text>().color = Color.black;
    }
}
