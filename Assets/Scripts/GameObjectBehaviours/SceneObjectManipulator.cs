using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

// ObjectManipulator is a script that should be added to all game objects that
// are created dynamically by the StoryManager and that will be manipulated
// during the story interaction. StoryManager can call methods on the
// ObjectManipulator to set click handlers, deal with paired objects. The
// handlers will likely be functions from SceneManipulatorAPI.
public class SceneObjectManipulator : MonoBehaviour
{

    // TODO: in the future allow multiple paired objects using dictionary.
    private GameObject pairedObject;
    // private List<ObjectAction> onMouseDownHandlers;
    private string TestString = "blah";

    void Start() {
        Logger.Log("started scene object manipulator");
        // this.onMouseDownHandlers = new List<Action<GameObject, object>>();
    }

    void Update()
    {
        // Unneeded.
    }


    // Call all OnMouseDown actions.
    public void OnMouseDown() {
        //foreach (ObjectAction action in this.onMouseDownHandlers) {
        //    action.func(gameObject, action.args);
        //}
        Logger.Log("OnMouseUpAsButton");
        Logger.Log(gameObject.transform.localPosition);
        //SceneManipulatorAPI.Highlight(gameObject, "red");
    }

    public Action Highlight(string color) {
        return () =>
        {
            gameObject.GetComponent<Image>().color = Color.blue;
            gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(200, 200);
            gameObject.GetComponent<RectTransform>().anchorMin = new Vector2(0, 1);
            gameObject.GetComponent<RectTransform>().anchorMax = new Vector2(0, 1);
            gameObject.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
            gameObject.GetComponent<RectTransform>().SetAsLastSibling();
            Logger.Log(gameObject.GetComponent<RectTransform>().localPosition);
            Logger.Log(gameObject.GetComponent<RectTransform>().anchoredPosition);
            Logger.Log("Highlight: " + color + " " + this.TestString);
        };
    }

    public Action MyTest(Action<object[]> action, params object[] args) {
        return () =>
        {
            action(args);
        };
    }

}
