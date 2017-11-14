// General utility functions that are useful throughout the app.

using System;

public static class Util {
    // TODO: should include comma or not? Sometimes that makes it too vertical.
    public static string[] punctuation = {";", ".", "?", "\"", "!" };

    public static string GetStoryName(string fileName) {
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
}
