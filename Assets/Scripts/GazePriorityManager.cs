using UnityEngine;
using HoloToolkit.Unity.InputModule;

public class GazePriorityManager : MonoBehaviour {
    public LayerMask prioritizedLayerMask;

	void Start () {
        int nonPrioritized = Physics.DefaultRaycastLayers & ~prioritizedLayerMask;
        GazeManager.Instance.RaycastLayerMasks = new LayerMask[] { prioritizedLayerMask, nonPrioritized };
    }
}
