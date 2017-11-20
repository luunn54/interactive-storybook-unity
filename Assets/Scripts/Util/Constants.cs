// This file contains constants for the interactive storybook.

public static class Constants
{
    // General dimensions.
    public static float SCREEN_WIDTH = 2560;
    public static float SCREEN_HEIGHT = 1600;

    // ROS connection.
    public static bool USE_ROS = true;
    public static string DEFAULT_ROSBRIDGE_IP = "192.168.1.149";
    public static string DEFAULT_ROSBRIDGE_PORT = "9090";

    // ROS topics.


}

// Display Modes.
// Related to ScreenOrientation but also deals with layout of the scene.
public enum DisplayMode
{
    LandscapeWide,
    Landscape,
    Portrait
};
