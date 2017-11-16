// This file contains the main Game Controller class.
//
// GameController handles the logic for the initial connection to ROS,
// as well as other metadata about the storybook interaction. GameController
// does not have to communicate over Ros, change behavior by setting the value
// of Constants.USE_ROS.
//
// GameController controls the high level progression of the story, and tells
// StoryManager which scenes to load.
//
// GameController is a singleton.

using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

    // The singleton instance.
    public static GameController instance = null;

    // Task queue.
    private Queue<Action> taskQueue = new Queue<Action>();

    // UI GameObjects. Make public so that they can be attached in Unity.
    public Button landscapeNextButton;
    public Button landscapeBackButton;
    public Button landscapeFinishButton;
    public Button portraitNextButton;
    public Button portraitBackButton;
    public Button portraitFinishButton;

    public Button toggleAudioButton;

    private Button nextButton;
    private Button backButton;
    private Button finishButton;

    public Button startStoryButton;

    public GameObject landscapePanel;
    public GameObject portraitPanel;

    // Objects for Splash Screen, Story Selection and Mode Selection.
    public GameObject splashPanel;
    public Dropdown storyDropdown;

    // Objects for ROS connection.
    public GameObject rosPanel;
    public Button connectButton;
    private RosManager ros;

    // Reference to SceneManager so we can load and manipulate story scenes.
    private StoryManager storyManager;

    // List of stories to populate dropdown.
    private List<string> stories;

    // Stores the scene descriptions for the current story.
    private string storyName;
    private ScreenOrientation orientation;
    private List<SceneDescription> storyPages;
    private int currentPageNumber = 0; // 0-indexed, index into this.storyPages.
    // Orientations of each story. TODO: read from file, for now just hardcode.
    private Dictionary<string, ScreenOrientation> orientations;

    void Awake()
    {
        // Enforce singleton pattern.
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Logger.Log("duplicate GameController, destroying");
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        // Set up all UI elements. (SetActive, GetComponent, etc.)
        // Get references to objects if necessary.
        Logger.Log("Game Controller start");
        this.landscapeNextButton.interactable = true;
        this.landscapeNextButton.onClick.AddListener(onNextButtonClick);
        this.portraitNextButton.interactable = true;
        this.portraitNextButton.onClick.AddListener(onNextButtonClick);

        this.landscapeBackButton.interactable = true;
        this.landscapeBackButton.onClick.AddListener(onBackButtonClick);
        this.portraitBackButton.interactable = true;
        this.portraitBackButton.onClick.AddListener(onBackButtonClick);

        this.landscapeFinishButton.interactable = true;
        this.landscapeFinishButton.onClick.AddListener(onFinishButtonClick);
        this.portraitFinishButton.interactable = true;
        this.portraitFinishButton.onClick.AddListener(onFinishButtonClick);

        this.startStoryButton.onClick.AddListener(onStartStoryClicked);

        this.toggleAudioButton.onClick.AddListener(toggleAudio);

        this.storyPages = new List<SceneDescription>();
        this.orientations = new Dictionary<string, ScreenOrientation>();

        this.storyManager = GetComponent<StoryManager>();

        this.stories = new List<string>();
        // TODO: read this from a file.
        this.stories.Add("the_hungry_toad");
        this.stories.Add("possum_and_the_peeper");

        // TODO: Check if we are using ROS or not.
        // Either launch the splash screen to connect to ROS, or go straight
        // into the story selection process.

        // Set up the orientations.
        // TODO: read in a file here instead.
        this.orientations["the_hungry_toad"] = ScreenOrientation.Landscape;
        this.orientations["possum_and_the_peeper"] = ScreenOrientation.Landscape;

        this.storyManager.SetAutoplay(true);

        // Set up the dropdown, load splash screen.
        this.setupStoryDropdown();
        this.showSplashScreen(true);
    }

    // Update() is called once per frame.
    void Update()
    {
        // Pop all tasks from the task queue and perform them.
        // Tasks are added from other threads, usually in response to ROS msgs.
        while (this.taskQueue.Count > 0) {
            try {
                this.taskQueue.Dequeue().Invoke();
            } catch (Exception e) {
                Logger.LogError("Error invoking action on main thread!\n" + e);
            }
        }
    }

    private void startStory(string story) {
        this.storyName = story;
        DirectoryInfo dir = new DirectoryInfo(Application.dataPath +
                                              "/SceneDescriptions/" + story);
        FileInfo[] files = dir.GetFiles("*.json");
        // Sort to ensure pages are in order.
        Array.Sort(files, (f1, f2) => string.Compare(f1.Name, f2.Name));
        this.storyPages.Clear();
        // Figure out the orientation of this story and tell SceneDescription.
        this.setOrientation(this.orientations[this.storyName]);
        SceneDescription.SetOrientation(this.orientation);
        foreach (FileInfo file in files) {
            this.storyPages.Add(new SceneDescription(file.Name));          
        }
        this.setOrientation(this.orientation);
        this.changeButtonText(this.nextButton, "Begin Story!");
        this.hideElement(this.backButton.gameObject);
        this.storyManager.LoadPage(this.storyPages[this.currentPageNumber]);
    }

    private void changeButtonText(Button button, string text) {
        button.GetComponentInChildren<Text>().text = text;
    }

    private void showElement(GameObject go) {
        go.SetActive(true);
    }

    private void hideElement(GameObject go) {
        go.SetActive(false);
    }

    private void setOrientation(ScreenOrientation o) {
        this.orientation = o;
        switch (o) {
            case ScreenOrientation.Landscape:
                this.setLandscapeOrientation();
                break;
            case ScreenOrientation.Portrait:
                this.setPortraitOrientation();
                break;
            default:
                Logger.LogError("No orientation: " + o.ToString());
                break;
        }
    }

    private void setLandscapeOrientation() {
        Logger.Log("Changing to Landscape orientation");
        this.portraitPanel.SetActive(false);
        this.landscapePanel.SetActive(true);

        this.nextButton = this.landscapeNextButton;
        this.backButton = this.landscapeBackButton;
        this.finishButton = this.landscapeFinishButton;

        // TODO: is this necessary?
        Screen.orientation = ScreenOrientation.Landscape;
    }

    private void setPortraitOrientation() {
        Logger.Log("Changing to Portrait orientation");
        this.landscapePanel.SetActive(false);
        this.portraitPanel.SetActive(true);

        this.nextButton = this.portraitNextButton;
        this.backButton = this.portraitBackButton;
        this.finishButton = this.portraitFinishButton;
        Screen.orientation = ScreenOrientation.Portrait;
    }

    // All UI handlers.
    private void onNextButtonClick() {
        Logger.Log("Next Button clicked.");
        this.currentPageNumber += 1;
        this.storyManager.ClearPage();
        this.storyManager.LoadPage(this.storyPages[this.currentPageNumber]);
        if (this.currentPageNumber == 1) {
            // Special case, need to change the text and show the back button.
            this.changeButtonText(this.nextButton, "Next Page");
            this.showElement(this.backButton.gameObject);
        }
        if (this.currentPageNumber == this.storyPages.Count - 1) {
            this.hideElement(this.nextButton.gameObject);
            this.showElement(this.finishButton.gameObject);
        }
	}

    private void onFinishButtonClick() {
        // For now, just return to the splash screen.
        this.showSplashScreen(true);
    }

    private void onBackButtonClick() {
        Logger.Log("Back Button clicked.");
        this.currentPageNumber -= 1;
        this.storyManager.ClearPage();
        this.storyManager.LoadPage(this.storyPages[this.currentPageNumber]);
        if (this.currentPageNumber == 0) {
            // Hide the back button because we're at the beginning.
            this.hideElement(this.backButton.gameObject);
        }
    }

    private void onStartStoryClicked() {
        // Read the selected value of the story dropdown and start that story.
        int selectedIdx = this.storyDropdown.value;
        this.showSplashScreen(false);
        this.startStory(this.stories[selectedIdx]);
    }

    // All ROS message handlers.
    // They should add tasks to the task queue.
    // Don't worry about this yet. Use ROS Manager class to handle this.

    private void onStopReadingReceived() {
        // Robot wants to intervene, so we should stop the automatic reading.    
    }

    private void toggleAudio() {
        this.storyManager.ToggleAudio();
    }

    // Show human readable story names and pull title images when possible.
    private void setupStoryDropdown() {
        this.storyDropdown.ClearOptions();
        List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
        foreach (string story in this.stories) {
            // Get human readable text and load the image.
            Dropdown.OptionData newOption = new Dropdown.OptionData();
            newOption.text = Util.HumanReadableStoryName(story);
            newOption.image = Util.GetTitleSprite(story);
            options.Add(newOption);
        }

        this.storyDropdown.AddOptions(options);
    }

    private void showSplashScreen(bool show) {
        if (show) {
            this.splashPanel.SetActive(true);
            this.landscapePanel.SetActive(false);
            this.portraitPanel.SetActive(false);
        } else {
            this.splashPanel.SetActive(false);
        }
    }

}
