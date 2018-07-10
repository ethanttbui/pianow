using UnityEngine;
using UnityEngine.Video;
using HoloToolkit.Examples.InteractiveElements;

[RequireComponent(typeof(LabelTheme))]
public class ButtonLabelManager : MonoBehaviour {
    public VideoPlayer tutorialVideo;

    LabelTheme labelTheme;

    void Awake() {
        labelTheme = GetComponent<LabelTheme>();
    }

    void Update() {
        labelTheme.Default = tutorialVideo.isPlaying ? "Stop Tutorial!" : "Start Tutorial!";
    }
}
