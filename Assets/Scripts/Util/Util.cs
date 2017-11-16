// General utility functions that are useful throughout the app.

using System;
using UnityEngine;

public static class Util {
    // TODO: should include comma or not? Sometimes that makes it too vertical.
    public static string[] punctuation = {";", ".", "?", "\"", "!" };

    public static string FileNameToStoryName(string fileName) {
        return fileName.Substring(0,
            fileName.LastIndexOf("_", StringComparison.CurrentCulture)
        );
    }

    // Returns true if the given word should be the last word of a stanza,
    // such as if that word ends a phrase or sentence.
    public static bool WordShouldEndStanza(string word) {
        foreach (string p in punctuation) {
            if (word.EndsWith(p, StringComparison.CurrentCulture)) {
                return true;
            }
        }
        return false;
    }

    // Gets the sprite for a particular story image file.
    public static Sprite GetStorySprite(string imageFile) {
        string storyName = Util.FileNameToStoryName(imageFile);
        string fullImagePath = "StoryPages/" + storyName + "/" + imageFile;
        return Resources.Load<Sprite>(fullImagePath);
    }


    // Turns something like "the_hungry_toad" into "The Hungry Toad"
    public static string HumanReadableStoryName(string story) {
        string[] words = story.Split('_');
        string humanReadable = "";
        foreach (string word in words) {
            if (word.Length > 1) {
                humanReadable += char.ToUpper(word[0]) + word.Substring(1);
            } else {
                humanReadable += word.ToUpper();
            }
            humanReadable += " ";
        }
        return humanReadable.TrimEnd(' ');
    }

    // Get the title page sprite for a story name.
    public static Sprite GetTitleSprite(string story) {
        return Util.GetStorySprite(story + "_title");
    }
}
