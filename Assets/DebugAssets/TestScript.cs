using UnityEngine;
using System.Runtime.InteropServices;
using System.IO;

public class TestScript : MonoBehaviour {
    [DllImport("TestDll2", EntryPoint = "Initialize")]
    public static extern void Initialize();

    [DllImport("TestDll2", EntryPoint = "FreeMemory")]
    public static extern void FreeMemory();

    [DllImport("TestDll2", EntryPoint = "ProcessFrame")]
    public static extern byte[] ProcessFrame(int width, int height);

    VideoPanel videoPanelUI;

    // Use this for initialization
    void Start () {
        Initialize();
        videoPanelUI = GameObject.FindObjectOfType<VideoPanel>();
        videoPanelUI.SetResolution(640, 480);
    }
	
	// Update is called once per frame
	void Update () {
        byte[] imgData = ProcessFrame(640, 480);
        videoPanelUI.SetBytes(imgData);
        FreeMemory();
    }
}
