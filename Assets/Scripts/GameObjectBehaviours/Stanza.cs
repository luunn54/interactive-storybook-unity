using UnityEngine;
using System.Collections.Generic;


// A Stanza is a sequence of TinkerTexts.
// Stanza is mostly a structural concept used to organize TinkerTexts.
// It has a reference to the audio, so that it is able to support operations
// such as playAudio when it is swiped.
// This Stanza script is automatically attached to each stanza object.
public class Stanza : MonoBehaviour {

    // Know which TinkerTexts belong to this stanza.
    private int firstTinkerTextIndex; // Global index.
    private int lastTinkerTextIndex; // Global index.

    // Know which sentence this stanza is a part of.
    private int sentenceIndex;

    // References to all of the TinkerText objects tha belong to this stanza.
    private List<GameObject> tinkerTexts;

    void Start() {
        
    }

    void Update() {
        
    }
}
