// This static class contains the API that SceneManager can use to manipulate
// the scene. These manipulations mostly include changing appearances or
// positions of scene objects, or calling functions on the objects.

using UnityEngine;

public static class SceneManipulatorAPI {
    
    // Highlight the given game object.
    // Just an example, probably need to pass more information, like
    // the position to highlight, what color, more extendable basically.
    //
    // Question: Return true on success, or should throw errors on failure?
    //
    // Make a specific type of GameObject that knows how to highlight itself?
    public static void Highlight(GameObject obj) {
        // 
    }
}
