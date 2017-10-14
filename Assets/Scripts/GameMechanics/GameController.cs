// This file contains the main Game Controller class.
//
// GameController handles the logic for the initial connection to ROS,
// as well as other metadata about the storybook interaction. GameController
// does not have to communicate over Ros, change behavior by setting the value
// of Constants.USE_ROS.
//
// Game Controller is a singleton?

using System;
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

    public Button startReadButton; // Reference always the same.
    public GameObject landscapePanel; // Reference always the same.
    public GameObject portraitPanel; // Reference always the same.

    // Objects for ROS connection.
    public Button connectButton;
    private RosManager ros;
    private SceneManager sceneManager;

    void Awake()
    {
        // Enforce singleton pattern.
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Logger.Log("destroy");
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        // Start the ROS message watcher.
        // Set up all UI elements. (SetActive, GetComponent, etc.)
        // Get references to objects if necessary.

        this.landscapeNextButton.interactable = true;
        this.landscapeNextButton.onClick.AddListener(onNextButtonClick);
        this.portraitNextButton.interactable = true;
        this.portraitNextButton.onClick.AddListener(onNextButtonClick);

        this.landscapeBackButton.interactable = true;
        this.landscapeBackButton.onClick.AddListener(onBackButtonClick);
        this.portraitBackButton.interactable = true;
        this.portraitBackButton.onClick.AddListener(onBackButtonClick);

        this.sceneManager = GetComponent<SceneManager>();
        this.sceneManager.HelloWorld();
        this.sceneManager.LoadImage();

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
        // Probably should do that, with timers that tell how long
        // before switching over to the next word - how to do audio for that?
        // Start simple, just play/pause audio, don't worry about highlighting.
    }

    void switchToLandscapeMode() {
		// Update references for nextButton and backButton.
		// Hide the canvas group associated with portrait.
		// Show the canvas group associated with landscape.
		// Toggle screen orientation.

        this.portraitPanel.GetComponent<CanvasGroup>().interactable = false;
        this.portraitPanel.GetComponent<CanvasGroup>().alpha = 0;

		this.landscapePanel.GetComponent<CanvasGroup>().interactable = true;
		this.landscapePanel.GetComponent<CanvasGroup>().alpha = 1;

        Screen.orientation = ScreenOrientation.Landscape;
    }

    void switchToPortraitMode() {

		this.landscapePanel.GetComponent<CanvasGroup>().interactable = false;
		this.landscapePanel.GetComponent<CanvasGroup>().alpha = 0;

		this.portraitPanel.GetComponent<CanvasGroup>().interactable = true;
		this.portraitPanel.GetComponent<CanvasGroup>().alpha = 1;

        Screen.orientation = ScreenOrientation.Portrait;
    }

    // All UI handlers.
    void onNextButtonClick()
    {

        Debug.Log("Next Button clicked.");
        this.switchToPortraitMode();

	}

    void onBackButtonClick()
    {
        Debug.Log("Back Button clicked.");
        this.switchToLandscapeMode();
    }

    void onStartRead()
    {

    }

    // All ROS message handlers.
    // They should add tasks to the task queue.
    // Don't worry about this yet.

    void onStopReadingReceived() {
        // Robot wants to intervene, so we should stop the automatic reading.    
    }
}