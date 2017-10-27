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
    public Button portraitNextButton;
    public Button portraitBackButton;

    public Button startReadButton;

    public GameObject landscapePanel;
    public GameObject portraitPanel;

    // Objects for ROS connection.
    public GameObject splashScreen;
    public Button connectButton;
    private RosManager ros;

    // Reference to SceneManager so we can load and manipulate story scenes.
    private StoryManager storyManager;

    // Stores the scene descriptions for the current story.
    private string storyName;
    private List<SceneDescription> storyPages;
    private int currentPageNumber = 0; // 0-indexed, index into this.storyPages.
    // Orientations of each story. TODO: read from file, for now just hardcode.
    private Dictionary<string, Orientation> orientations;

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
        // Start the ROS message watcher.
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

        this.storyPages = new List<SceneDescription>();
        this.orientations = new Dictionary<string, Orientation>();

        this.storyManager = GetComponent<StoryManager>();

        // TODO: Check if we are using ROS or not.
        // Either launch the splash screen to connect to ROS, or go straight
        // into the story selection process.

        // Set up the orientations.
        this.orientations["the_hungry_toad"] = Orientation.Landscape;
        this.orientations["possum_and_the_peeper"] = Orientation.Landscape;

        this.selectStory("the_hungry_toad");

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
        // TODO: Need to handle the progression of speech/animation here?
        // Yes, Cynthia said there's a tool that will get the timing for me,
        // for highlighting. So just play audio and call highlighting.
        // This should probably happen in StoryManager and not GameController.
    }

    private void selectStory(string story) {
        this.storyName = story;
        DirectoryInfo dir = new DirectoryInfo(Application.dataPath +
                                              "/SceneDescriptions/" + story);
        FileInfo[] files = dir.GetFiles("*.json");
        Logger.Log(files.Length);
        // Sort to ensure pages are in order.
        Array.Sort(files, (f1, f2) => string.Compare(f1.Name, f2.Name));
        this.storyPages.Clear();
        // Figure out the orientation of this story and tell SceneDescription.
        SceneDescription.SetOrientation(this.orientations[storyName]);
        foreach (FileInfo file in files) {
            this.storyPages.Add(new SceneDescription(file.Name));          
        }
    }

    private void switchToLandscapeMode() {
        this.portraitPanel.GetComponent<CanvasGroup>().interactable = false;
        this.portraitPanel.GetComponent<CanvasGroup>().alpha = 0;
        this.portraitPanel.SetActive(false);

		this.landscapePanel.GetComponent<CanvasGroup>().interactable = true;
		this.landscapePanel.GetComponent<CanvasGroup>().alpha = 1;
        this.landscapePanel.SetActive(true);

        // TODO: is this necessary?
        Screen.orientation = ScreenOrientation.Landscape;
    }

    private void switchToPortraitMode() {
		this.landscapePanel.GetComponent<CanvasGroup>().interactable = false;
		this.landscapePanel.GetComponent<CanvasGroup>().alpha = 0;
        this.landscapePanel.SetActive(false);

		this.portraitPanel.GetComponent<CanvasGroup>().interactable = true;
		this.portraitPanel.GetComponent<CanvasGroup>().alpha = 1;
        this.portraitPanel.SetActive(true);

        Screen.orientation = ScreenOrientation.Portrait;
    }

    // All UI handlers.
    private void onNextButtonClick()
    {
        Logger.Log("Next Button clicked.");

        this.storyManager.LoadPage(this.storyPages[this.currentPageNumber]);
	}

    private void onBackButtonClick()
    {
        Logger.Log("Back Button clicked.");
        this.switchToLandscapeMode();
        this.storyManager.ClearScene();
    }

    private void onStartRead()
    {

    }

    // All ROS message handlers.
    // They should add tasks to the task queue.
    // Don't worry about this yet.

    void onStopReadingReceived() {
        // Robot wants to intervene, so we should stop the automatic reading.    
    }
}
