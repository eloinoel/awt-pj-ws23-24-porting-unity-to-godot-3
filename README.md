# AWT-Porting-Unity-To-Godot

Porting a Unity 3D Application into the Godot Engine

Unity is a game engine that is popular with game developers for mobile, desktop, web or console devices. It is easy to get started, the engine and many assets are available for free and paid license options. Recently the company behind Unity tried to change the license and charge developers for every install of a game on a client device. The company has since partly retracted these plans, nevertheless other game engines have since elevated attention with developers looking for a safe licensing option. The Godot game engine is an open source alternative to Unity that tries to offer as many features and seems to have the needed maturity. Thus Godot looks like a worthwhile option to port existing games/scenarios from Unity to Godot while preventing any current or future license issues.

## Project Objective

    - Find an existing or develop a minimalistic 3D game for the Unity engine, meaning it needs to have some 
      game logic and not just be a scene with basic movement for Unity that can be exported to run in the webbrowser
    - Port that same Unity game to use Godot instead, it should also run in the webbrowser
    - Evaluate the porting process, your workflows, the tools you have used for porting, both of the engines - compare 
      everything involved during the process

## Play the Game

### Unity Version

You can play our Unity game in your web browser at: https://eloinoel.github.io/UnityRacingGame/ 

If you want to run the game locally, download this repository and open the KartingUnity/ directory with Unity version 2022.3.14f1. You can then build the game by selecting 'File->Build Settings', then choosing the desired platform (for example WebGL for deployment in your browser). Then select 'Build and Run' at the bottom right of the dialog window. Alternatively, you could 'Build' the game and then open it with your own web server, e.g. running 'python -m http.server' in your build directory where the 'index.html' resides.

### Godot Version

You can play the ported Godot game in its final state at: https://eloinoel.github.io/awt-pj-ws23-24-porting-unity-to-godot-3/ 

To run the game locally, first the [.Net version of Godot 3.5](https://godotengine.org/download/3.x/windows/) with C# support has to be downloaded. A guide on how to set up Godot with C# support can be found [here](https://docs.godotengine.org/en/3.5/tutorials/scripting/c_sharp/c_sharp_basics.html). Installing the latest stable version of the [.NET SDK](https://dotnet.microsoft.com/en-us/download) is required. Open Godot and 'Import' the 'project.godot' file found in './Karting_Godot_V3/'. After importing the project, go to 'Project->Tools->C#->Create C# solution'. You should now see a '.csproj' and '.sln' file next to the 'project.godot' file. Furthermore, at the top right, set the engine rendering method to 'GLES3'. You should now be able to build and run the game. 
Exporting the game for the web can be done by going to 'Project->Export'. Click 'Add...' and select 'HTML5'. Finalise by clicking 'Export Project'. Further information can be found [here](https://docs.godotengine.org/en/3.5/tutorials/export/exporting_for_web.html#doc-exporting-for-web).

The 'Karting_Godot_V4' directory only contains ported assets and was used for testing with Godot 4 in earlier stages of the project. 
