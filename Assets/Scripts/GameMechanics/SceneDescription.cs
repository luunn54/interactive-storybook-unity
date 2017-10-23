// SceneDescription is a serializable struct that describes a storybook scene.
//
// It contains definitions and descriptions of the objects in the scene,
// some metadata about the scene, and specifies triggers among objects.
// SceneManager takes a given SceneDescription and loads it, setting up
// necessarily colliders and handlers to enusre that the triggers occur.

using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;

// Describes a scene object.
// Contains the asset file to load the image from, the position to load
// the image, and an identifying name that is unique among other SceneObject
// objects in this scene.
struct SceneObject {
    string name;
    string asset; // Can be empty. This means there's no sprite to load.
    int x;
    int y;
    int scale_x;
    int scale_y;
}

// SceneDescription can be serialized to and from JSON.
// This is necessary so that we can describe scenes in plaintext, so that they
// can be stored easily as JSON files and can be sent over the network.
[Serializable]
// This file contains the format in which we describe a scene's layout
// and components.

public struct TriggerAction {
    
}

public struct TriggerCondition {
    
}

public struct Trigger {
    
}



public class SceneDescription {
    public string displayMode; // Either "landscape" or "portrait".
    public string storyImageFile; // E.g. "the_hungry_toad_01".
    public string text; // All of the text. StoryManager will create TinkerText.

    public SceneDescription() {
        
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

    // Getters

    public string getDisplayMode() {
        return this.displayMode;
    }

    public string getStoryImageFile() {
        return this.storyImageFile;
    }

    public string getText() {
        return this.text;
    }

    // TODO: Relationships between words and objects.
    // Dictionary<string, SceneObject> objects;

}
