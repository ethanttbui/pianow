//  
// Copyright (c) 2017 Vulcan, Inc. All rights reserved.  
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
//

using System;
using System.Runtime.InteropServices;

using UnityEngine;
using UnityEngine.XR.WSA;

using HoloLensCameraStream;

[RequireComponent(typeof(CameraStreamHelper))]
public class KeyboardRecognitionManager : MonoBehaviour {
    Vector3 keyboardPosition, keyboardOrientation;
    bool stopVideoMode;
    byte[] latestImageBytes;
    HoloLensCameraStream.Resolution resolution;
    VideoCapture videoCapture;
    IntPtr spatialCoordinateSystemPtr;
    // VideoPanel videoPanelUI;

    [DllImport("KeyboardRecognition", EntryPoint = "ProcessFrame")]
    public static extern IntPtr ProcessFrame(IntPtr inBytes, int width, int height);

    [DllImport("KeyboardRecognition", EntryPoint = "FreeMemory")]
    public static extern void FreeMemory();

    public Vector3 GetKeyboardPosition () {
        return keyboardPosition;
    }

    public Vector3 GetKeyboardOrientation () {
        return keyboardOrientation;
    }

    public void StopRecognition () {
        stopVideoMode = true;
    }

    public void StartRecognition () {
        stopVideoMode = false;
        OnVideoCaptureCreated(videoCapture);
    }

    void Start() {
        stopVideoMode = false;

        //Fetch a pointer to Unity's spatial coordinate system if you need pixel mapping
        spatialCoordinateSystemPtr = WorldManager.GetNativeISpatialCoordinateSystemPtr();

        //Call this in Start() to ensure that the CameraStreamHelper is already "Awake".
        CameraStreamHelper.Instance.GetVideoCaptureAsync(OnVideoCaptureCreated);

        // videoPanelUI = GameObject.FindObjectOfType<VideoPanel>();
    }

    void OnDestroy()
    {
        if (videoCapture != null)
        {
            videoCapture.FrameSampleAcquired -= OnFrameSampleAcquired;
            videoCapture.Dispose();
        }
    }

    void OnVideoCaptureCreated(VideoCapture videoCapture)
    {
        if (videoCapture == null)
        {
            Debug.LogError("Did not find a video capture object. You may not be using the HoloLens.");
            return;
        }

        this.videoCapture = videoCapture;

        //Request the spatial coordinate ptr if you want fetch the camera and set it if you need to 
        CameraStreamHelper.Instance.SetNativeISpatialCoordinateSystemPtr(spatialCoordinateSystemPtr);

        resolution = CameraStreamHelper.Instance.GetLowestResolution();
        float frameRate = CameraStreamHelper.Instance.GetLowestFrameRate(resolution);
        videoCapture.FrameSampleAcquired += OnFrameSampleAcquired;

        //You don't need to set all of these params.
        //I'm just adding them to show you that they exist.
        CameraParameters cameraParams = new CameraParameters();
        cameraParams.cameraResolutionHeight = resolution.height;
        cameraParams.cameraResolutionWidth = resolution.width;
        cameraParams.frameRate = Mathf.RoundToInt(frameRate);
        cameraParams.pixelFormat = CapturePixelFormat.BGRA32;
        cameraParams.enableHolograms = false;

        // UnityEngine.WSA.Application.InvokeOnAppThread(() => { videoPanelUI.SetResolution(resolution.width, resolution.height); }, false);

        videoCapture.StartVideoModeAsync(cameraParams, OnVideoModeStarted);
    }


    void OnVideoModeStarted(VideoCaptureResult result)
    {
        if (result.success == false)
        {
            Debug.LogWarning("Could not start video mode.");
            return;
        }
    }

    void OnVideoModeStopped(VideoCaptureResult result)
    {
        if (result.success == false)
        {
            Debug.LogWarning("Could not stop video mode.");
            return;
        }
    }

    void OnFrameSampleAcquired(VideoCaptureSample sample)
    {
        //When copying the bytes out of the buffer, you must supply a byte[] that is appropriately sized.
        //You can reuse this byte[] until you need to resize it (for whatever reason).
        if (latestImageBytes == null || latestImageBytes.Length < sample.dataLength)
        {
            latestImageBytes = new byte[sample.dataLength];
        }
        sample.CopyRawImageDataIntoBuffer(latestImageBytes);

        //If you need to get the cameraToWorld matrix for purposes of compositing you can do it like this
        float[] outMatrix;
        if (sample.TryGetCameraToWorldMatrix(out outMatrix) == false)
        {
            return;
        }
        Matrix4x4 cameraToWorldMatrix = LocatableCameraUtils.ConvertFloatArrayToMatrix4x4(outMatrix);

        //If you need to get the projection matrix for purposes of compositing you can do it like this
        if (sample.TryGetProjectionMatrix(out outMatrix) == false)
        {
            return;
        }
        Matrix4x4 projectionMatrix = LocatableCameraUtils.ConvertFloatArrayToMatrix4x4(outMatrix);

        sample.Dispose();

        //This is where we actually use the image data
        UnityEngine.WSA.Application.InvokeOnAppThread(() =>
        {
            int length = latestImageBytes.Length;
            int size = Marshal.SizeOf(latestImageBytes[0]) * length;
            IntPtr inPtr = Marshal.AllocHGlobal(size);
            try
            {
                Marshal.Copy(latestImageBytes, 0, inPtr, length);
                IntPtr outPtr = ProcessFrame(inPtr, resolution.width, resolution.height);
                if (outPtr != IntPtr.Zero)
                {
                    int[] cvOutput = new int[4];
                    Marshal.Copy(outPtr, cvOutput, 0, 4);

                    // Get the world coordinates of the keyboard center
                    Vector2 pixelCoordinates = new Vector2(cvOutput[0], cvOutput[1]);
                    Vector3 worldDirection = LocatableCameraUtils.PixelCoordToWorldCoord(cameraToWorldMatrix, projectionMatrix, resolution, pixelCoordinates);
                    Ray ray = new Ray(cameraToWorldMatrix.GetColumn(3), worldDirection);
                    RaycastHit hitInfo;
                    if(Physics.Raycast(ray, out hitInfo))
                    {
                        keyboardPosition = hitInfo.point;
                    }

                    // Get the world coordinates of the keyboard's eigenvector
                    pixelCoordinates = new Vector2(cvOutput[2], cvOutput[3]);
                    worldDirection = LocatableCameraUtils.PixelCoordToWorldCoord(cameraToWorldMatrix, projectionMatrix, resolution, pixelCoordinates);
                    ray = new Ray(cameraToWorldMatrix.GetColumn(3), worldDirection);
                    if (Physics.Raycast(ray, out hitInfo))
                    {
                        keyboardOrientation = hitInfo.point - keyboardPosition;
                    }
                }
                /*byte[] outBytes = new byte[length];
                Marshal.Copy(outPtr, outBytes, 0, length);
                videoPanelUI.SetBytes(outBytes);*/
            }
            finally
            {
                // Clean memory
                Marshal.FreeHGlobal(inPtr);
                FreeMemory();
                GC.Collect();
            }
        }, false);

        if (stopVideoMode)
        {
            videoCapture.StopVideoModeAsync(OnVideoModeStopped);
        }
    }
}
