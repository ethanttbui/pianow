<p align="center">
  <img src="https://github.com/ethanttbui/Pianow/blob/master/Assets/Textures/logo.png" alt="Pianow's logo">
</p>

# About Pianow

**Created by**: Bui Thanh Tung, Tsai Yi-Ting  
**Supervisor**: Dr. Dirk Schnieders  
Department of Computer Science, the University of Hong Kong

Pianow is a mixed reality application that aims to offer beginners a new way to practice playing the piano by themselves, which is easy, fun, and interactive. Pianow is developed for the HoloLens. It can effectively align virtual content to the physical piano keyboard and display piano tutorials consisting of falling notes and colored keys. More information can be found at [Pianow's website](http://i.cs.hku.hk/fyp/2017/fyp17011/).

# Documentation

Pianow is a Unity project. Inside the [Assets](Assets) folder, there is a number of directories that might be of interest to you:

**[HoloToolkit & HoloToolkit-Examples](https://github.com/Microsoft/MixedRealityToolkit-Unity):** A collection of useful components for application development on the HoloLens, developed and maintained by Microsoft. Some important components used in this project are:

* [InputManager.prefab](https://github.com/Microsoft/MixedRealityToolkit-Unity/blob/master/Assets/HoloToolkit/Input/README.md#prefabs): Manages user input on the HoloLens, except voice input.

* [SpatialMapping.prefab](https://github.com/Microsoft/MixedRealityToolkit-Unity/blob/master/Assets/HoloToolkit/SpatialMapping/README.md): Manages the spatial mapping ability of the application.

* [CursorWithFeedback.prefab](https://github.com/Microsoft/MixedRealityToolkit-Unity/blob/master/Assets/HoloToolkit/Input/README.md#prefabs): The cursor used in this application.

* [SpeechInputSource.cs & SpeechInputHandler.cs](https://github.com/Microsoft/MixedRealityToolkit-Unity/blob/master/Assets/HoloToolkit/Input/README.md#scripts): Handles speech input.

* [Tagalong.cs & Billboard.cs](https://github.com/Microsoft/MixedRealityToolkit-Unity/blob/master/Assets/HoloToolkit/Utilities/README.md): Allows a hologram to tag along and always face toward the user as the user moves around the space.

* [HandDraggable.cs](https://github.com/Microsoft/MixedRealityToolkit-Unity/blob/master/Assets/HoloToolkit/Input/README.md#scripts): Allows a hologram to be dragged and dropped by the user.

* [InteractiveElements Example](https://github.com/Microsoft/MixedRealityToolkit-Unity/blob/master/Assets/HoloToolkit-Examples/UX/Readme/README_InteractiveButtonComponents.md): The prefabs of this example are adapted to build the control menu and some other components of Pianow.

**[Vuforia](https://www.vuforia.com/):** A widely popular augmented reality SDK, developed by PTC Inc. Two important components used in this project are ARCamera and Image (i.e. image target).

**Plugins:** This folder contains some native plugins written in C++ and imported to the project as DLL files.
* [HoloLensCameraStream](https://github.com/VulcanTechnologies/HoloLensCameraStream): A solution to stream data from the HoloLens' locatable camera at runtime, developed and maintained by Vulcan Technologies.

* [opencv-hololens](https://github.com/sylvain-prevost/opencv-hololens): A modified version of OpenCV that is compatible with Universal Windows platform on the HoloLens, developed and maintained by Sylvain Prevost.

* KeyboardRecognition: A plugin created by the project team specifically for the purpose of recognizing the position and orientation of the piano keyboard. The source Visual Studio solution is included in [KeyboardRecognition](KeyboardRecognition) folder.

**Scripts:** This folder contains most of the major scripts used in this project. Which script is adapted from other sources is clearly stated below.

* ApplicationController.cs: An abstract class that defines all handlers for UI events.

* VuforiaApplicationController.cs: Implementeation of ApplicationController class for the version of the application that uses Vuforia marker detection method.

* OpencvApplicationController.cs: Implementeation of ApplicationController class for the version of the application that uses OpenCV keyboard recognition and manual alignment method.

* KeyboardRecognitionManager.cs: This script streams data from the HoloLens' locatable camera and calls the KeyboardRecognition plugin to identify the position and orientation of the piano keyboard in each frame. It also projects these coordinates from pixel to world coordinate system. The implementation of this script is partially adapted from the [CamStream Example](https://github.com/VulcanTechnologies/HoloLensCameraStream/tree/master/HoloLensVideoCaptureExample/Assets/CamStream) inside [HoloLensCameraStream](https://github.com/VulcanTechnologies/HoloLensCameraStream) repository. It requires CameraStreamHelper.cs and LocatableCameraUtils.cs to work.

* CameraStreamHelper.cs & LocatableCameraUtils.cs: Required components for KeyboardRecognitionManager.cs, fully adapted from the [CamStream Example](https://github.com/VulcanTechnologies/HoloLensCameraStream/tree/master/HoloLensVideoCaptureExample/Assets/CamStream).

* ButtonLabelManager.cs & SliderUpdateManager.cs: These short scripts define the behaviors of some specific UI elements (i.e. button label and slider).

* GazePriorityManager.cs: Prioritize raycast hit on GazePrioritized layer mask. This is important for manual alignment method with drag-and-drop as it ensures that the draggable hologram can always be selected.

The meanings and purposes of other directories in the Assets folder are easy to understand based on their names and contents. Apart from the directories, there are also two scenes provided: one for the version of the application that uses Vuforia marker detection method (i.e. Vuforia scene) and the other for the version of the application that uses OpenCV keyboard recognition and manual alignment method (i.e. OpenCV scene). The appropriate scene should be used in the build according to which version of the application is to be generated.
