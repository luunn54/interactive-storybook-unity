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

    private Button nextButton;
    private Button backButton;
    private Button finishButton;

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

        this.storyPages = new List<SceneDescription>();
        this.orientations = new Dictionary<string, ScreenOrientation>();

        this.storyManager = GetComponent<StoryManager>();

        // TODO: Check if we are using ROS or not.
        // Either launch the splash screen to connect to ROS, or go straight
        // into the story selection process.

        // Set up the orientations.
        // TODO: read in a file here instead.
        this.orientations["the_hungry_toad"] = ScreenOrientation.Landscape;
        this.orientations["possum_and_the_peeper"] = ScreenOrientation.Landscape;

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

        this.setOrientation(this.orientations[this.storyName]);
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

    private void setOrientation(ScreenOrientation orientation) {
        switch (orientation) {
            case ScreenOrientation.Landscape:
                this.setLandscapeOrientation();
                break;
            case ScreenOrientation.Portrait:
                this.setPortraitOrientation();
                break;
            default:
                Logger.LogError("No orientation: " + orientation.ToString());
                break;
        }
    }

    private void setLandscapeOrientation() {
        this.portraitPanel.GetComponent<CanvasGroup>().interactable = false;
        this.portraitPanel.GetComponent<CanvasGroup>().alpha = 0;
        this.portraitPanel.SetActive(false);

		this.landscapePanel.GetComponent<CanvasGroup>().interactable = true;
		this.landscapePanel.GetComponent<CanvasGroup>().alpha = 1;
        this.landscapePanel.SetActive(true);

        this.nextButton = this.landscapeNextButton;
        this.backButton = this.landscapeBackButton;
        this.finishButton = this.landscapeFinishButton;

        // TODO: is this necessary?
        Screen.orientation = ScreenOrientation.Landscape;
    }

    private void setPortraitOrientation() {
		this.landscapePanel.GetComponent<CanvasGroup>().interactable = false;
		this.landscapePanel.GetComponent<CanvasGroup>().alpha = 0;
        this.landscapePanel.SetActive(false);

		this.portraitPanel.GetComponent<CanvasGroup>().interactable = true;
		this.portraitPanel.GetComponent<CanvasGroup>().alpha = 1;
        this.portraitPanel.SetActive(true);

        this.nextButton = this.portraitNextButton;
        this.backButton = this.portraitBackButton;
        this.finishButton = this.portraitFinishButton;
        Screen.orientation = ScreenOrientation.Portrait;
    }

    // All UI handlers.
    private void onNextButtonClick() {
        Logger.Log("Next Button clicked.");
        if (this.currentPageNumber == 0) {
            // Special case, need to change the text and show the back button.
            this.changeButtonText(this.nextButton, "Next Page");
            this.showElement(this.backButton.gameObject);
        }
        if (this.currentPageNumber == this.storyPages.Count - 2) {
            this.hideElement(this.nextButton.gameObject);
            this.showElement(this.finishButton.gameObject);
        }
        this.currentPageNumber += 1;
        this.storyManager.ClearPage();
        this.storyManager.LoadPage(this.storyPages[this.currentPageNumber]);
	}

    private void onFinishButtonClick() {
        
    }

    private void onBackButtonClick()
    {
        Logger.Log("Back Button clicked.");
        this.currentPageNumber -= 1;
        this.storyManager.ClearPage();
        this.storyManager.LoadPage(this.storyPages[this.currentPageNumber]);
    }

    private void onStartReadClicked() {
        // Tell story manager to begin playing the audio on this page,
        // and story manager is responsible for telling the tinker texts
        // to light up at the correct time.
    }

    private void onStopReadClicked() {
        // Can be caused by a ROS message if the robot wants to do something,
        // or can be because the child clicked something that we have set up
        // to stop the reading.
    }

    // All ROS message handlers.
    // They should add tasks to the task queue.
    // Don't worry about this yet.

    void onStopReadingReceived() {
        // Robot wants to intervene, so we should stop the automatic reading.    
    }
}
