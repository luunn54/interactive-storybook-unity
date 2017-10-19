// This static class contains the API that SceneManager can use to manipulate
// the scene. These manipulations mostly include changing appearances or
// positions of scene objects, or calling functions on the objects.

using UnityEngine;
using System;

// The static functions return objects of generic type Action, which can then
// be attached to various GameObjects through their ObjectManipulator, so that
// the functions can be called from other sources.
public static class SceneManipulatorAPI {
    
    // Highlight the given game object.
    // Just an example, probably need to pass more information, like
    // the position to highlight, what color, more extendable basically.
    //
    // Question: Return true on success, or should throw errors on failure?
    //
    // Make a specific type of GameObject that knows how to highlight itself?

    //private static Action HighlightAction(GameObject obj, string arg) {
    //    Logger.Log("Highlighted!" + arg);
    //    return MakeAction(obj, HighlightAction, )
    //}

    // Transform an action that takes a parameter into one that doesn't.
    private static Action MakeAction(SceneObjectManipulator target, Action<SceneObjectManipulator, object[]> action, params object[] args) {
        return () =>
        {
            action(target, args);
        };
    }
}
