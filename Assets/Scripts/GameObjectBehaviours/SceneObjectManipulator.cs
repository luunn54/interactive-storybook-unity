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


    void Start()
    {
        Logger.Log("started manipulator");
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
        SceneManipulatorAPI.Highlight(gameObject, "red");
    }


}
