using UnityEngine;
using System.Collections.Generic;

// A sentence is a wrapper around at least one stanza.
// Sentences support the play function, and are given a reference
// to the audio object.
public class Sentence {

    private List<GameObject> stanzas;
    private StoryAudioManager audio;

    // Keep track of the time interval in the audio that this sentence
    // corresponds to.
    private int earliestTimestamp;
    private int latestTimestamp;

    public Sentence(StoryAudioManager audio) {
        this.audio = audio;
    }

    public void AddStanza(GameObject stanza) {
        this.stanzas.Add(stanza);
    }
	
}