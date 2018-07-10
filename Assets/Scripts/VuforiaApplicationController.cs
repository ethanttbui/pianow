using System.Collections;

using HoloToolkit.Unity.InputModule;
using UnityEngine;
using UnityEngine.Video;
using Vuforia;

[RequireComponent(typeof(VideoPlayer))]
public class VuforiaApplicationController : ApplicationController{
    public GameObject instruction;
    public GameObject imageTarget;
    public GameObject indicator;
    public GameObject mainContent;
    public GameObject tutorialIntro;
    public SpeechInputSource speechInputSource;
    public VideoClip song1;
    public VideoClip song2;

    VideoPlayer videoPlayer;
    bool playIntro = true;
    int currentSongNo = 1;

    // Disable Vuforia
    // Set the position and orientation of the main content to be the same as the indicator
    // The main content whould always be upright (no rotation along Y axis)
    // Hide the indicator + instruction and show the main content
    public override void FinishAlignment() {
        VuforiaBehaviour.Instance.enabled = false;
        mainContent.transform.position = indicator.transform.position;
        Quaternion upRotation = Quaternion.FromToRotation(indicator.transform.up, Vector3.up);
        mainContent.transform.rotation = upRotation * indicator.transform.rotation;

        speechInputSource.StopKeywordRecognizer();
        imageTarget.SetActive(false);
        instruction.SetActive(false);
        mainContent.SetActive(true);
    }

    // Re-enable Vuforia
    // Show the indicator + instruction and hide the main content
    public override void RestartAlignment() {
        videoPlayer.Stop();
        mainContent.SetActive(false);
        VuforiaBehaviour.Instance.enabled = true;
        speechInputSource.StartKeywordRecognizer();
        instruction.SetActive(true);
        imageTarget.SetActive(true);
    }

    // Play or stop the tutorial
    public override void ToggleTutorial() {
        if (playIntro) {
            StartCoroutine(PlayTutorialWithIntro());
            playIntro = false;
            return;
        }

        if (videoPlayer.isPlaying) {
            videoPlayer.Stop();
        } else {
            videoPlayer.Play();
        }
    }

    public override void SkipIntro() {
        tutorialIntro.GetComponent<VideoPlayer>().Stop();
        tutorialIntro.SetActive(false);
        playIntro = false;
        if (!videoPlayer.isPlaying) {
            videoPlayer.Play();
        }
    }

    // Set the clip according to song number
    public override void ChooseSong(int songNo) {
        currentSongNo = songNo;
        if (videoPlayer.isPlaying) {
            videoPlayer.Stop();
            videoPlayer.clip = songNo == 1 ? song1 : song2;
            videoPlayer.Play();
        } else {
            videoPlayer.clip = songNo == 1 ? song1 : song2;
        }
    }

    // Mute or unmute the tutorial
    public override void MuteTutorial(bool muted) {
        videoPlayer.GetTargetAudioSource(0).mute = muted;
    }

    // Set tutorial speed
    public override void SetTutorialSpeed(float speed) {
        videoPlayer.playbackSpeed = 1 + speed;
    }

    void Awake() {
        videoPlayer = GetComponent<VideoPlayer>();
        mainContent.SetActive(false);
        tutorialIntro.SetActive(false);
    }

    void Start() {
        speechInputSource.StartKeywordRecognizer();
    }

    IEnumerator PlayTutorialWithIntro() {
        tutorialIntro.SetActive(true);
        mainContent.GetComponent<AudioSource>().Stop();
        tutorialIntro.GetComponent<VideoPlayer>().Play();
        yield return new WaitForSeconds(40);
        tutorialIntro.SetActive(false);
        if (!videoPlayer.isPlaying) {
            videoPlayer.Play();
        }
    }
}
