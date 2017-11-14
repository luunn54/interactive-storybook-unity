using UnityEngine;
using System.Collections.Generic;


// A Stanza is a sequence of TinkerTexts.
// Stanza is mostly a structural concept used to organize TinkerTexts.
// It has a reference to the audio, so that it is able to support operations
// such as playAudio when it is swiped.
// This Stanza script is automatically attached to each stanza object.
public class Stanza : MonoBehaviour {

    public GameObject stanzaPanel;

    // Know which TinkerTexts belong to this stanza.
    private int firstTinkerTextIndex; // Global index.
    private int lastTinkerTextIndex; // Global index.

    private float startTimestamp;
    private float endTimestamp;

    // Know which sentence this stanza is a part of.
    private int sentenceIndex;

    // References to all of the TinkerText objects that belong to this stanza.
    private List<GameObject> tinkerTexts;

    private void Start() {
        Logger.Log("Stanza start");
        this.stanzaPanel = gameObject;
    }

    void Update() {
        // TODO: maybe need to check for swipes or something here?
    }

    public void SetStartTimestamp(float start) {
        this.startTimestamp = start;
    }

    public void SetEndTimestamp(float end) {
        this.endTimestamp = end;
    }

}
