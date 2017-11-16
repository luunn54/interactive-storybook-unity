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
	}

    // Use Update for handling when to trigger actions in other objects.
	private void Update() {
        // Check our current timestamp, and compare against timestamps of
        // the triggers we have, in order to cause specific actions to happen.
        this.currentTimestamp = this.audioSource.time;
        float maxCutoffTime = this.currentTimestamp;
        // Watch for special case where the audio has finished and we need to
        // make sure we call any outstanding triggers.
        if (this.currentTimestamp < this.lastTimestamp) {
            maxCutoffTime = float.MaxValue;
        }

        foreach (KeyValuePair<float, Action> trigger in this.triggers) {
            if (trigger.Key > this.lastTimestamp &&
                trigger.Key <= maxCutoffTime) {
                // Invoke this trigger's action.
                trigger.Value();
            }
        }

        this.lastTimestamp = this.currentTimestamp;
	}

    // Load an audio file.
    public void LoadAudio(string audioFile) {
        string storyName = Util.FileNameToStoryName(audioFile);
        this.audioClip = Resources.Load("StoryAudio/" + storyName + "/" +
                                        audioFile) as AudioClip;
        this.audioSource.clip = this.audioClip;
    }

    // For StoryManager to call when it's setting up the scene.
    public void AddTrigger(float timestamp, Action action) {
        if (!this.triggers.ContainsKey(timestamp)) {
            this.triggers[timestamp] = () => { };
        }
        this.triggers[timestamp] += action;
    }

    // Start playing the audio (in a separate thread?).
    public void PlayAudio() {
        this.StopAudio();
        this.audioSource.Play();
    }

    public void UnpauseAudio() {
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
            this.UnpauseAudio();
        }
    }

    // Plays the audio between start seconds and end seconds.
    public void PlayInterval(float start, float end) {
        this.StopAudio();
        this.audioSource.time = start; // TODO: maybe backtrack a tiny bit?
        this.PlayAudio();
    }

    public void ClearTriggersAndReset() {
        this.triggers.Clear();
        this.resetInternalTimestamps();
    }

    private void resetInternalTimestamps() {
        this.lastTimestamp = float.MinValue;
        this.currentTimestamp = 0.0f;
        this.stopTimestamp = float.MaxValue;
    }

}
