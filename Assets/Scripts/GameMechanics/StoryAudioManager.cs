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

    // Need to keep track of timestamp info. Better data structure to use?
    Dictionary<int, Action> triggers;
    int lastTimestamp;
    int currentTimestamp;

	// Use this for initialization
	private void Start() {
        Logger.Log("StoryAudioManager start");
        this.triggers = new Dictionary<int, Action>();
	}

    // Use Update for handling when to trigger actions in other objects.
	private void Update() {
	    // Check our current timestamp, and compare against timestamps of
        // the triggers we have, in order to cause specific actions to happen.
        //
        // Use audioSource.timeSamples for finer grain control.
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
        Logger.Log("loaded audio file " + audioFile);
    }

    // Start playing the audio (in a separate thread?).
    public void StartAudio() {
        Logger.Log("about to call play audio");
        this.audioSource.Play();
        Logger.Log("just called play audio");
    }

    // Pause audio in a way that allows for resume.
    public void PauseAudio() {
        this.audioSource.Pause();
    }

    // Stop audio from playing (and reset timestamp back to 0).
    public void StopAudio() {
        this.audioSource.Stop();
    }

    public void ToggleAudio() {
        if (this.audioSource.isPlaying) {
            this.PauseAudio();
        } else {
            this.StartAudio();
        }
    }

    // Parse timestamps file and set up the triggers.
    private void parseTimestamps(string filename) {
        this.triggers.Clear();
        // Depending on how we design the json file for these triggers, we
        // will need to deserialize and turn it into representable objects.
    }

    // Convert from a timestamp in seconds to a timestamp in samples.
    private int timeToSamples(int seconds) {
        return -1;
    }
}
