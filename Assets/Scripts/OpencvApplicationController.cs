using System.Collections;

using UnityEngine;
using UnityEngine.XR.WSA.Input;
using UnityEngine.Video;
using HoloToolkit.Unity.InputModule;
using Vuforia;

[RequireComponent(typeof(KeyboardRecognitionManager))]
public class OpencvApplicationController : ApplicationController {
    public GameObject instructionText;
    public GameObject indicator;
    public GameObject mainContent;
    public GameObject mainMenu;
    public GameObject finalizeAlignmentBtn;
    public GameObject pianoKeyboard;
    public GameObject tutorialPanel;
    public VideoClip song1;
    public VideoClip song2;

    bool isRecognizing;
    VideoPlayer tutorialVideo;
    KeyboardRecognitionManager keyboardRecognitionManager;
    HandDraggable handDraggable;
    GestureRecognizer gestureRecognizer;

    // Start recognition mode again
    // Show the indicator and the instruction text, hide the main content
    public override void RestartAlignment() {
        keyboardRecognitionManager.StartRecognition();
        isRecognizing = true;
        gestureRecognizer.StartCapturingGestures();

        mainMenu.SetActive(false);
        finalizeAlignmentBtn.SetActive(true);
        tutorialPanel.SetActive(false);
        pianoKeyboard.SetActive(true);
        mainContent.SetActive(false);
        instructionText.SetActive(true);
        indicator.SetActive(true);
        handDraggable.enabled = true;
    }

    // Disable hand draggable feature
    // Show the main menu and tutorial panels, hide the piano keyboard
    public override void FinishAlignment() {
        mainMenu.SetActive(true);
        finalizeAlignmentBtn.SetActive(false);
        pianoKeyboard.SetActive(false);
        tutorialPanel.SetActive(true);
        handDraggable.enabled = false;
    }

    // Play or stop the tutorial
    public override void ToggleTutorial() {
        if (tutorialVideo.isPlaying) {
            tutorialVideo.Stop();
        }
        else {
            tutorialVideo.Play();
        }
    }

    public override void SkipIntro() {
        throw new System.NotImplementedException();
    }

    // Set the clip according to song number
    public override void ChooseSong(int songNo) {
        if (tutorialVideo.isPlaying) {
            tutorialVideo.Stop();
        }
        tutorialVideo.clip = songNo == 1 ? song1 : song2;
    }

    // Mute or unmute the tutorial
    public override void MuteTutorial(bool muted) {
        tutorialVideo.GetTargetAudioSource(0).mute = muted;
    }

    // Set tutorial speed
    public override void SetTutorialSpeed(float speed) {
        tutorialVideo.playbackSpeed = 1 + speed;
    }

    void Start () {
        isRecognizing = true;
        VuforiaBehaviour.Instance.enabled = false;
        keyboardRecognitionManager = GetComponent<KeyboardRecognitionManager>();
        tutorialVideo = tutorialPanel.GetComponent<VideoPlayer>();
        handDraggable = mainContent.GetComponent<HandDraggable>();
        

        mainMenu.SetActive(false);
        tutorialPanel.SetActive(false);
        mainContent.SetActive(false);

        // Start listening to tap gesture
        gestureRecognizer = new GestureRecognizer();
        gestureRecognizer.TappedEvent += (source, tapCount, headRay) => { StartCoroutine(OnTap(headRay)); };
        gestureRecognizer.SetRecognizableGestures(GestureSettings.Tap);
        gestureRecognizer.StartCapturingGestures();
    }
	
	void Update () {
        if (!isRecognizing)
            return;

        // If recognition mode is on
        // Update the position and orientation of the indicater every frame
        Vector3 forward = Vector3.Cross(keyboardRecognitionManager.GetKeyboardOrientation(), Vector3.up);
        Quaternion rotation = Quaternion.LookRotation(forward);
        indicator.transform.position = keyboardRecognitionManager.GetKeyboardPosition();
        indicator.transform.rotation = rotation;
    }

    // On any tap event
    IEnumerator OnTap(Ray headRay) {
        yield return new WaitForSeconds(0.65f);
        RaycastHit hitInfo;
        if (Physics.Raycast(headRay, out hitInfo)) {

            // If the recognition mode is on, stop recognition
            // Set the position and orientation of the main content to be the same as the indicator
            // Hide the indicator and the instruction text, show the main content
            if (isRecognizing) {
                keyboardRecognitionManager.StopRecognition();
                isRecognizing = false;
                gestureRecognizer.StopCapturingGestures();

                mainContent.transform.position = indicator.transform.position + new Vector3(0f, 0.1f, 0f);
                mainContent.transform.rotation = indicator.transform.rotation;

                indicator.SetActive(false);
                instructionText.SetActive(false);
                mainContent.SetActive(true);
            }
        }
    }
}
