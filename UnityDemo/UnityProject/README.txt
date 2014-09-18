Camera Artifact Rendering v0.2.0 - 2014-09-17
=============================================================

==UnityProject Folder==

1) To test CAR open the Unity project in this folder.
2) Open scene MainScene
3) Print AugmentedResistance_Marker.jpg
4) In the ARCamera object in the Web Cam Behaviour script choose your camera device
5) Run and point your camera at the printed marker and move. You should see how the character is blurred when you move fast.


Notes
- The project is currenlty configured for a 4:3 camera. You can change the x-scale of the VideoTexturePlane and the game aspect to configure for other aspect ratios.
- Use the CAR Linear Blur script in the ARCamera to configure the blur:
-- Stabilizer Strengh: Temporal blur stabilization
-- Blur Multipliert: Blur strength
- You can also add blur to the backgound using the CAR Linear Blur script attached to the BackgroundCamera