// General utility functions that are useful throughout the app.

using System;

public static class Util {
    public static string GetStoryName(string fileName) {
        return fileName.Substring(0,
            fileName.LastIndexOf("_", StringComparison.CurrentCulture)
        );
    }
}
