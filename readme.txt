==================================
Settings: (server address etc)
--> MmoEngine.GetDefaultSettings()

==================================
Install:
1) download the unity3d island demo
   http://unity3d.com/support/resources/example-projects/islanddemo
   http://unity3d.com/files/live-demos/tropical-paradise/IslandDemo.zip
2) unzip into this folder (merge folders + do not overwrite existing files)
3) open scene Islands.unity 

================================== added by cnsoft =======================
4) check out our patch files. and copy to  YOUR SDKFOLDER\src-server\Mmo\Photon.MmoDemo.Client.UnityIsland\Assets
   override some old files. 
5) open it with Unity4.x. it will works now. 

If you want to talk about mmo tech or unity or game development , free to send mail to cnsoft@gmail.com. 
These files are fixed when i review photon mmo demo server.so i shared it with you guys who interested it also.

++++++++++++++++++++++++++++++++++ Provided by Photon ++++++++++++++++++
Troubleshooting:

Problem: "BCE0022: Cannot convert 'UnityEngine.GameObject' to 'float'."
Cause: Unity 3.4
Solution: remove "if(water) waterLevel = water.gameObject;" from Assets/Scripts/UnderwaterEffects.js

Problem: "BCE0031: Language feature not implemented: UnityEditor."
Cause: Unity 3.4
Solution: remove the 2 occurences of "UnityEditor.TerrainLightmapper.UpdateTreeColor" from Assets/Editor/UpdateTreeColors.js

Pronlem: "MonoBehaviour is attached to the game object 'MmoEnginePrefab' but the game object does not contain the component. Destroying component".
Cause: Unity 3.4
Solution: Remove MmoEnginePrefab from scene and add it again. Then compile.

Problem: "Broken Scene"
Solution:
1) unzip unity3d island and overwrite existing files
2) add Photon/MmoEnginePrefab, Photon/PlayerNamePrefab and Photon/ViewDistancePrefab to scene
3) assign shader Resources/Font to Resources/arialbd/font material
4) Resources/ActorName: assign Text Mesh font Resources/arialbd
5) Resources/ActorName: assign Mesh Renderer Material Resources/arialbd/font material

Problem: "No server communication"
Cause: "Missing (Mono Script)" in MmoEnginePrefab
Solution: reattach script MmoEngine to MmoEnginePrefab

Problem: "Opponent names not readable"
Cause: font and material of prefab Resources/ActorName broken
Solution:
1) Resources/ActorName: assign Text Mesh font Resources/arialbd
2) Resources/ActorName: assign Mesh Renderer Material Resources/arialbd/font material

Problem: "Opponent names shown in front of everything else"
Cause: shader of font material is not "Guid/Text Shader Z"
Solution: assign shader Resources/Font to Resources/arialbd/font material
