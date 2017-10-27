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

    // UnityActions for various UI interactions (e.g. clicking).
    private UnityAction clickUnityAction;

    void Start() {
        Logger.Log("started scene object manipulator");
        // TODO: add necessary components in a prefab.
        this.clickUnityAction = new UnityAction(() => { });
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
        };
    }

    public Action Move(Vector3 localPosition) {
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
