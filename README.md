Camera Artifact Rendering v0.3.0 - 2015-04-01
=============================================================
This repository contains the following Reality Mixer SEs of the FIcontent Pervasive Games Platform:
  * Camera Artifact Rendering SE 
  
For documentation please refer to the FIcontent Wiki at http://wiki.ficontent.eu/ in particular
the documentation of the Pervasive Games Platform at http://wiki.ficontent.eu/doku.php/ficontent.gaming.architecture

=============================================================
Unity Demo
=============================================================
Tested with Unity 4.6.0 and Vuforia SDK 3.0.9

1) To run the Unity demo open the folder "FIcontent.Gaming.Enabler.RealityMixer.CameraArtifactRendering\UnityDemo\UnityProject" in Unity version 4.6.0.

2) Open scene "MainScene".

3) Print AugmentedResistance_Marker.jpg image in "FIcontent.Gaming.Enabler.RealityMixer.CameraArtifactRendering\UnityDemo\Marker".

4) On the object "ARCamera" in the "Web Cam Behaviour" component choose your camera device, for example your webcam.

5) Run, point your webcam at the printed marker, and move the camera around. You should see how the character is blurred when you move fast.

6) To change the strength of the camera blur change the "Blur Multiplier" value in the "CarLinear Blur" component on the "ARCamera" object.


Notes
- The project is configured for a 4:3 camera and a 16:9 screen. You can change the scale of the VideoTexturePlane and the game aspect to configure for other aspect ratios.
- You can also add blur to the background image using the "CARLinear Blur" component attached to the "BackgroundCamera" object.
- In order to use these camera effects for your own project simply start with the provided Unity demo project and add your content and functionality.

=============================================================
XML3D Demo
=============================================================

A live-demo of this project can be seen here:
http:\graphics.ethz.ch\research\argroup\FIcontent.Gaming.Enabler.RealityMixer.CameraArtifactRendering\XML3dDemo\carDemo.html

Use a bright light source as marker, point it into your camera, and move it around. The cube will follow the light source and it will be blurred according to its velocity.

To deploy the XML3D demo project copy the folder "FIcontent.Gaming.Enabler.RealityMixer.CameraArtifactRendering\XML3dDemo" to your webserver and browse to carDemo.html. 

Notes:
In the file carDemo.html you find two parameters to configure the blur on line 169 and 170:
	var STRENGHT = 1.0;			// Defines the strength of the blur. Should be set depending 								on the amount of light in the scene
	var STABILIZING = 0.2;		// Defines the temporal smoothness