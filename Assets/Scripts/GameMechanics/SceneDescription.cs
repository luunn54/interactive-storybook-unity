// SceneDescription is a serializable struct that describes a storybook scene.
//
// It contains definitions and descriptions of the objects in the scene,
// some metadata about the scene, and specifies triggers among objects.
// SceneManager takes a given SceneDescription and loads it, setting up
// necessarily colliders and handlers to enusre that the triggers occur.

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
    public string name;
    public string asset; // Can be empty. This means there's no sprite to load.
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
    public string sceneObjectName;
    public TriggerCondition condition;
    public TriggerAction action;
}

// SceneDescription can be serialized to and from JSON.
// This is necessary so that we can describe scenes in plaintext, so that they
// can be stored easily as JSON files and can be sent over the network.
[Serializable]
// This file contains the format in which we describe a scene's layout
// and components.
public class SceneDescription {
    public string displayMode; // Either "landscape" or "portrait".
    public string storyImageFile; // E.g. "the_hungry_toad_01".
    public string text; // All of the text. StoryManager will create TinkerText.
    public SceneObject[] sceneObjects; // Scene objects.
    //public Trigger[] triggers;

    public SceneDescription() {
        // Empty constructor if no JSON file is passed.
    }

    public SceneDescription(string jsonFile) {
        this.loadFromJSON(jsonFile);
    }

    // Populate this SceneDescription with JSON data.
    private void loadFromJSON(string jsonFile) {
        string storyName = jsonFile.Substring(0, jsonFile.LastIndexOf("_", StringComparison.CurrentCulture));
        string dataAsJson = File.ReadAllText("Assets/SceneDescriptions/" + storyName + "/" + jsonFile);
		JsonUtility.FromJsonOverwrite(dataAsJson, this);
    }

    // Getters.
    public string getDisplayMode() {
        return this.displayMode;
    }

    public string getStoryImageFile() {
        return this.storyImageFile;
    }

    public string getText() {
        return this.text;
    }

    public SceneObject[] getSceneObjects() {
        return this.sceneObjects;
    }

    //public Trigger[] getTriggers() {
    //    return this.triggers;
    //}

}
