using UnityEngine;
using System.Collections.Generic;


// A Stanza is a sequence of TinkerTexts.
// Stanza is mostly a structural concept used to organize TinkerTexts.
// It has a reference to the audio, so that it is able to support operations
// such as playAudio when it is swiped.
// This Stanza script is automatically attached to each stanza object.
public class Stanza : MonoBehaviour {

    public static bool allowSwipe;

    public GameObject stanzaPanel;
    private RectTransform rect;
    private StoryAudioManager audioManager;

    // Know boundaries of the stanza.
    private float leftX;
    private float topY;
    private float bottomY;

    // Detect swipe positions.
    private Vector2 mouseDownPos;
    private Vector2 mouseUpPos;

    // Know which TinkerTexts belong to this stanza.
    private int firstTinkerTextIndex; // Global index.
    private int lastTinkerTextIndex; // Global index.

    private float startTimestamp;
    private float endTimestamp;

    // Know which sentence this stanza is a part of.
    private int sentenceIndex;

    // References to all of the TinkerText objects that belong to this stanza.
    private List<GameObject> tinkerTexts;

    public void Init(StoryAudioManager audio, Vector2 textPanelPos) {
        this.audioManager = audio;
        this.leftX = textPanelPos.x;
        float posY = this.rect.localPosition.y;
        float height = this.rect.sizeDelta.y;
        // TODO: might need to change this depending on orientation.
        this.topY = textPanelPos.y + posY + height / 2.0f;
        this.bottomY = 
    }

    private void Start() {
        Logger.Log("Stanza start");
        this.stanzaPanel = gameObject;
        this.rect = this.stanzaPanel.GetComponent<RectTransform>();
        Stanza.allowSwipe = true;
    }

    void Update() {
        // Check for swipes. Start reading if the swipe starts to the left and
        // then ends inside the stanza. Must be vertically within the stanza
        // the entire time.
        if (Stanza.allowSwipe) {
            if (Input.GetMouseButtonDown(0)) {
                this.mouseDownPos = Input.mousePosition;
                print("mouseDown " + this.mouseDownPos.ToString()); 
            }
            if (Input.GetMouseButtonUp(0)) {
                this.mouseUpPos = Input.mousePosition;
                print("mouseUp " + this.mouseDownPos.ToString()); 

            }
            // Check if they are vertically within stanza, and that the swipe
            // started to the left and then moved into the stanza.
            if (this.stanzaWasSwiped()) {
                this.audioManager.PlayInterval(this.startTimestamp,
                                               this.endTimestamp);
            }
        }
    }

    public void SetStartTimestamp(float start) {
        this.startTimestamp = start;
    }

    public void SetEndTimestamp(float end) {
        this.endTimestamp = end;
    }

    public void PlayStanza() {
        this.audioManager.PlayInterval(this.startTimestamp, this.endTimestamp);
    }

    private bool stanzaWasSwiped() {
        
    }
}
