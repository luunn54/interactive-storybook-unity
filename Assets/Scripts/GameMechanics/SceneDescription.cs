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
public enum ConditionType {
    Click
}

[Serializable]
public enum ActionType {
    Highlight,
    Move,
    ChangeSize
}

// All possible arguments we'd want to be passed along with a trigger action.
[Serializable]
public struct ActionArgs
{
    public Color color;
    public int alpha, r, g, b; // For colors, 0-255.
    public int x, y; // For positions or sizing.
}

[Serializable]
public struct TriggerAction {
    public ActionType type;
    public string typeString;
    public ActionArgs args;
}

[Serializable]
public struct TriggerCondition {
    public ConditionType type;
    public string typeString;
}

[Serializable]
public struct Trigger {
    public int textId;
    public string sceneObjectLabel;
    public TriggerCondition condition;
    public TriggerAction action;
}

// SceneDescription can be serialized to and from JSON.
// This is necessary so that we can describe scenes in plaintext, so that they
// can be stored easily as JSON files and can be sent over the network.
[Serializable]
public class SceneDescription {
    private static ScreenOrientation orientation; // To be set by GameController.

    public DisplayMode displayMode;
    public string displayModeString; // Easier for deserialization.

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

    public static void SetOrientation(ScreenOrientation o) {
        SceneDescription.orientation = o;
    }

    // Populate this SceneDescription with JSON data.
    private void loadFromJSON(string jsonFile) {
        string storyName = jsonFile.Substring(0,
            jsonFile.LastIndexOf("_", StringComparison.CurrentCulture)
        );
        string dataAsJson = File.ReadAllText("Assets/SceneDescriptions/" +
                                             storyName + "/" + jsonFile);
		JsonUtility.FromJsonOverwrite(dataAsJson, this);

        // Decide if the image should be in landscape or portrait mode.
        this.setDisplayMode();

        // Process triggers.
        for (int i = 0; i < this.triggers.Length; i++)
        {
            Trigger trigger = this.triggers[i];
            if (trigger.condition.typeString != null) {
                string cType = trigger.condition.typeString
                                      .Substring(0, 1).ToUpper() +
                                      trigger.condition.typeString.Substring(1);
                this.triggers[i].condition.type = (ConditionType)Enum.Parse(
                    typeof(ConditionType), cType);
            }
            if (trigger.action.typeString != null) {
                string aType = trigger.action.typeString
                                  .Substring(0, 1).ToUpper() +
                                  trigger.action.typeString.Substring(1);
                this.triggers[i].action.type = (ActionType)Enum.Parse(
                    typeof(ActionType), aType);
            }
            // Convert from RGB to Color if necessary.
            if (this.triggers[i].action.type == ActionType.Highlight)
            {
                // If no alpha provided, assume it's 1.
                float alpha = 1;
                if (trigger.action.args.alpha > 0) {
                    alpha = (float)trigger.action.args.alpha / 255;
                }
                this.triggers[i].action.args.color = new Color(
                    (float)trigger.action.args.r / 255,
                    (float)trigger.action.args.g / 255,
                    (float)trigger.action.args.b / 255,
                    alpha
                );
            }
        }
    }

    private void setDisplayMode() {
        string storyName = this.storyImageFile.Substring(0,
            this.storyImageFile.LastIndexOf("_",StringComparison.CurrentCulture)
        );
        string fullImagePath = "StoryPages/" + storyName + "/" +
            this.storyImageFile;
        Texture texture = Resources.Load<Texture>(fullImagePath);
        float imageAspectRatio = (float)texture.width / (float)texture.height;
        if (SceneDescription.orientation == ScreenOrientation.Landscape) {
            if (imageAspectRatio > 2) {
                this.displayMode = DisplayMode.LandscapeWide;
            } else {
                this.displayMode = DisplayMode.Landscape;
            }
    
        } else if (SceneDescription.orientation == ScreenOrientation.Portrait) {
            this.displayMode = DisplayMode.Portrait;
        }
        Logger.Log(this.storyImageFile);
        Logger.Log("hello displayMode is " + this.displayMode.ToString());
    }

}
