using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

// ObjectManipulator is a script that should be added to all game objects that
// are created dynamically by the StoryManager and that will be manipulated
// during the story interaction. StoryManager can call methods on the
// ObjectManipulator to set click handlers.
//
// The public methods of SceneObjectManipulator return Actions that will serve
// as the callbacks for click handlers. These Actions can also be called
// immediately if we want to invoke the effects at the calling time.
public class SceneObjectManipulator : MonoBehaviour
{

    // Components that all SceneObjects should have.
    public Button button;
    public Image image;
    public RectTransform rectTransform;
    public string label { get; set; }

    // TODO: add the concept of variables, so that variables can be
    // saved between scenes. This also implies that we should not Destroy
    // SceneObjects as we change pages, otherwise we lose that memory, and
    // we should have an active set per page, and activate or deactivate the
    // page, and only delete everything when we end the story.

    // UnityActions for various UI interactions (e.g. clicking).
    private UnityAction clickUnityAction;

    void Start() {
        Logger.Log("started scene object manipulator");
        // TODO: add audio and animation to the prefab, then include them.

        // It's important to do += here and not = for clickUnityAction.
        this.clickUnityAction += () => { };
        this.button.onClick.AddListener(this.clickUnityAction);
    }

    public void AddClickHandler(Action action) {
        this.clickUnityAction += new UnityAction(action);
    }

    public Action Highlight(Color color) {
        return () =>
        {
            gameObject.GetComponent<Image>().color = color;
            Logger.Log("Highlight: " + color.ToString());
            // After some amount of time, remove highlighting.

        };
    }

    public Action MoveToPosition(Vector3 localPosition) {
        return () =>
        {
            this.rectTransform.localPosition = localPosition;
            Logger.Log("Moved: " + localPosition.ToString());
            this.GetComponent<RectTransform>().SetAsLastSibling();
        };
    }

    public Action ChangeSize(Vector2 newSize) {
        return () =>
        {
            this.rectTransform.sizeDelta = newSize;
        };
    }

    //public Action PlayAnimation() {
    //    return () =>
    //    {
    //        this.animation.play();
    //    };
    //}

}
