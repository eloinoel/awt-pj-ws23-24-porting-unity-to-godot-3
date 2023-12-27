# AWT-Porting-Unity-To-Godot

Porting Unity 3D Application into Godot Engine

Unity is a game engine that is popular with game developers for mobile, desktop, web or console devices. It is easy to get started, the engine and many assets are available for free and paid license options. Recently the company behind Unity tried to change the license and charge developers for every install of a game on a client device. The company has since partly retracted these plans, nevertheless other game engines have since elevated attention with developers looking for a safe licensing option. The Godot game engine is an open source alternative to Unity that tries to offer as many features and seems to have the needed maturity. Thus Godot looks like a worthwhile option to port existing games/scenarios from Unity to Godot while preventing any current or future license issues.

## Project Objective

    - Find an existing or develop a minimalistic 3D game for the Unity engine, meaning it needs to have some 
      game logic and not just be a scene with basic movement for Unity that can be exported to run in the webbrowser
    - Port that same Unity game to use Godot instead, it should also run in the webbrowser
    - Evaluate the porting process, your workflows, the tools you have used for porting, both of the engines - compare 
      everything involved during the process

## Get the Game

You can play our game in your web browser at: https://eloinoel.github.io/awt-pj-ws23-24-porting-unity-to-godot-3/

If you want to run the game locally, download this repository and open the Karting directory with Unity version 2022.3.14f1. You can then build the game by selecting 'File->Build Settings', then choosing the desired platform (for example WebGL for deployment in your browser). Then select 'Build and Run' at the bottom right of the dialog window. Alternatively, you could 'Build' the game and then open it with your own web server, e.g. running 'python -m http.server' in your build directory where the 'index.html' resides.