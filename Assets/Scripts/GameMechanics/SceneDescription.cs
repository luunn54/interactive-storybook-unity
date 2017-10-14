// SceneDescription is a serializable struct that describes a storybook scene.
//
// It contains definitions and descriptions of the objects in the scene,
// some metadata about the scene, and specifies triggers among objects.
// SceneManager takes a given SceneDescription and loads it, setting up
// necessarily colliders and handlers to enusre that the triggers occur.

using System;

// Describes a scene object.
// Contains the asset file to load the image from, the position to load
// the image, and an identifying name that is unique among other SceneObject
// objects in this scene.
struct SceneObject {
    string name;
    string asset;
    int x;
    int y;
    int z;
    int scale_x;
    int scale_y;
    int scale_z;
}

// SceneDescription can be serialized to and from JSON.
// This is necessary so that we can describe scenes as text, so that they
// can be stored easily as Assets and can be sent over the network.
[Serializable]
public struct SceneDescription {
    string display; // Either "landscape" or "portrait".
    SceneObject[] objects; // Will turn each of these into a GameObject, which for now is maybe an invisible box.
    string text; // All of the text. Code will take care of displaying.

    // TODO: Relationships between words and objects.
}
