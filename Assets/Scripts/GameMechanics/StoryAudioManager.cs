using UnityEngine;
using System.Collections.Generic;
using System;

// AudioManager is given a single audio file and timestamp file, and is able
// to support operations like playing the audio between timestamps, and
// has triggers that can be triggered at specific timestamps, i.e. cause a
// particular TinkerText to light up when a certain word is said.
//
// This AudioManager script is attached to an invisible GameObject that always
// exists in the scene.
public class StoryAudioManager : MonoBehaviour {

    // Need an AudioSource and an AudioClip to play audio.
    public AudioSource audioSource;
    private AudioClip audioClip;

    // TODO: Keep track of timestamped triggers in a balanced BST, which can then
    // support range search queries in O(log(n) + k) time.
    // Or to simplify, can use a sorted array and keep track of index to
    // start searching at.
    Dictionary<float, Action> triggers;
    float lastTimestamp;
    float currentTimestamp;
    float stopTimestamp; // If we want to stop the audio early.

	// Use this for initialization
	private void Start() {
        Logger.Log("StoryAudioManager start");
        this.resetInternalTimestamps();
        this.triggers = new Dictionary<float, Action>();
        this.triggers.Add(0.5f, () => { Logger.Log("0 current Timestamp: " + this.currentTimestamp.ToString()); });
        this.triggers.Add(1.0f, () => { Logger.Log("1 current Timestamp: " + this.currentTimestamp.ToString()); });
        this.triggers.Add(1.5f, () => { Logger.Log("2 current Timestamp: " + this.currentTimestamp.ToString()); });
        this.triggers.Add(2.0f, () => { Logger.Log("3 current Timestamp: " + this.currentTimestamp.ToString()); });
	}

    // Use Update for handling when to trigger actions in other objects.
	private void Update() {
        // Check our current timestamp, and compare against timestamps of
        // the triggers we have, in order to cause specific actions to happen.
        this.currentTimestamp = this.audioSource.time;
        // TODO: the end time might be very far in the future for the last
        // words, can check for end by checking when currentTimestamp becomes
        // less than lastTimestamp, and make sure all remaining events trigger.
        float maxCutoffTime = this.currentTimestamp;
        if (this.currentTimestamp < this.lastTimestamp) {
            maxCutoffTime = float.MaxValue;
        }

        foreach (KeyValuePair<float, Action> trigger in this.triggers) {
            if (trigger.Key > this.lastTimestamp && trigger.Key < maxCutoffTime) {
                trigger.Value();
            }
        }
        this.lastTimestamp = this.currentTimestamp;
	}

    // Load an audio file and the timestamps.
    // This function will set up the triggers based on the timestamp -> triggers
    // map. This function should be called whenever a new page is loaded.
    public void LoadAudioAndTimestamps(string audioFile, string timestampFile) {
        string storyName = Util.GetStoryName(audioFile);
        this.audioClip = Resources.Load("StoryAudio/" + storyName + "/" +
                                        audioFile) as AudioClip;
        this.audioSource.clip = this.audioClip;
        this.parseTimestamps(timestampFile);
    }

    // Start playing the audio (in a separate thread?).
    public void PlayAudio() {
        this.audioSource.Play();
    }

    // Pause audio in a way that allows for resume.
    public void PauseAudio() {
        this.audioSource.Pause();
    }

    // Stop audio from playing (and reset timestamp back to 0).
    public void StopAudio() {
        this.audioSource.Stop();
        this.resetInternalTimestamps();
    }

    public void ToggleAudio() {
        if (this.audioSource.isPlaying) {
            this.PauseAudio();
        } else {
            this.PlayAudio();
        }
    }

    // Plays the audio between start seconds and end seconds.
    public void PlayInterval(float start, float end) {
        this.audioSource.time = start; // TODO: maybe backtrack a tiny bit?
        this.PlayAudio();
    }

    private void resetInternalTimestamps() {
        this.lastTimestamp = float.MinValue;
        this.currentTimestamp = 0.0f;
        this.stopTimestamp = float.MaxValue;
    }

    // Parse timestamps file and set up the triggers.
    private void parseTimestamps(string filename) {
        // this.triggers.Clear();
        // Depending on how we design the json file for these triggers, we
        // will need to deserialize and turn it into representable objects.
    }

}