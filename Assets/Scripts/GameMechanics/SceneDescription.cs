// SceneDescription is a serializable struct that describes a storybook scene.
//
// It contains definitions and descriptions of the objects in the scene,
// some metadata about the scene, and specifies triggers among objects.
// SceneManager takes a given SceneDescription and loads it, setting up
// necessarily colliders and handlers to enusre that the triggers occur.

// TODO: access control - adjust things so not everything is public.

using UnityEngine;
using System;
using System.IO;

// Describes the position of a scene object. Uses the same format as the
// output from our Mechanical Turk HITs.
[Serializable]
public struct Position {
    public int left;
    public int top;
    public int width;
    public int height;
}

// Describes a scene object.
// Contains the asset file to load the image from, the position to load
// the image, and an identifying name that is unique among other SceneObject
// objects in this scene.
[Serializable]
public struct SceneObject {
    public string label;
    // Can be empty. This means there's no sprite to load.
    public string asset;
    public Position position;
}

[Serializable]
public struct TriggerAction {
    
}

[Serializable]
public struct TriggerCondition {
    
}

[Serializable]
public struct Trigger {
    public string textId;
    public string sceneObjectLabel;
    public TriggerCondition condition;
    public TriggerAction action;
}

// SceneDescription can be serialized to and from JSON.
// This is necessary so that we can describe scenes in plaintext, so that they
// can be stored easily as JSON files and can be sent over the network.
[Serializable]
public class SceneDescription {
    // TODO: use the enum.
    public string displayMode;

    // E.g. // "the_hungry_toad_01".
    public string storyImageFile;

    // All of the text. StoryManager will create TinkerText.
    public string text;

    // List of scene objects to place.
    public SceneObject[] sceneObjects;

    // Triggers to coordinate connections among SceneObjects and TinkerTexts.
    public Trigger[] triggers;

    public SceneDescription() {
        // Empty constructor if no JSON file is passed.
    }

    public SceneDescription(string jsonFile) {
        this.loadFromJSON(jsonFile);
    }

    // Populate this SceneDescription with JSON data.
    private void loadFromJSON(string jsonFile) {
        string storyName = jsonFile.Substring(0,
            jsonFile.LastIndexOf("_", StringComparison.CurrentCulture)
        );
        string dataAsJson = File.ReadAllText("Assets/SceneDescriptions/" +
                                             storyName + "/" + jsonFile);
		JsonUtility.FromJsonOverwrite(dataAsJson, this);
    }

}
