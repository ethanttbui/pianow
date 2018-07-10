using UnityEngine;
using HoloToolkit.Examples.InteractiveElements;

[RequireComponent(typeof(SliderGestureControl))]
public class SliderUpdateManager : MonoBehaviour {
    public ApplicationController applicationController;
    SliderGestureControl sliderControl;

    public void UpdateTutorialSpeed() {
        applicationController.SetTutorialSpeed(sliderControl.SliderValue);
    }

    void Awake() {
        sliderControl = GetComponent<SliderGestureControl>();
    }
}
