using Godot;
using System;

enum CountdownState {
	START,
	THREE,
	TWO,
	ONE,
	GO,
	FINISHED
}

public class RaceCountdown : Node
{
	AudioStreamSample soundTick;
	AudioStreamSample soundGo;
	AudioStreamPlayer audioPlayer;

	CountdownState countdownState = CountdownState.START;
	float countdownDuration = 1.0f;

	Timer countdownTimer;
	Node2D objectiveMessage;
	Label One;
	Label Two;
	Label Three;
	Label Go;

	[Export]
	NodePath helperFunctionsPath;
	HelperFunctions helperFunctions;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		soundTick = GD.Load<AudioStreamSample>("res://Audio/SFX/StartSound01.wav");
		soundGo = GD.Load<AudioStreamSample>("res://Audio/SFX/StartSound02.wav");

		if(soundTick == null || soundGo == null)
		{
			GD.PrintErr("RaceCountdown: Could not load audio files");
		}

		audioPlayer = GetNode<AudioStreamPlayer>("AudioStreamPlayer");
		countdownTimer = GetNode<Timer>("CountdownTimer");
		objectiveMessage = GetNode<Node2D>("ObjectiveMessage");
		One = GetNode<Label>("CenterContainer/One");
		Two = GetNode<Label>("CenterContainer/Two");
		Three = GetNode<Label>("CenterContainer/Three");
		Go = GetNode<Label>("CenterContainer/GO");
		helperFunctions = GetNode<HelperFunctions>(helperFunctionsPath);
	}

	public void TriggerRaceCountdown()
	{
		countdownTimer.Start(countdownDuration);
		audioPlayer.Stream = soundTick;
		audioPlayer.Play();
		helperFunctions.FadeIn(Three);
		helperFunctions.FadeIn(objectiveMessage, 0.5f);
		//Three.Visible = true;
		//objectiveMessage.Visible = true;
		countdownState = CountdownState.THREE;
	}

	private void _OnCoundownTimerTimeout()
	{
		switch(countdownState)
		{
			case CountdownState.THREE:
				countdownTimer.Start(countdownDuration);
				audioPlayer.Play();
				//helperFunctions.FadeOut(Three);
				helperFunctions.FadeIn(Two);
				Three.Visible = false;
				//Two.Visible = true;
				countdownState = CountdownState.TWO;
				break;
			case CountdownState.TWO:
				countdownTimer.Start(countdownDuration);
				audioPlayer.Play();
				//helperFunctions.FadeOut(Two);
				helperFunctions.FadeIn(One);
				Two.Visible = false;
				//One.Visible = true;
				countdownState = CountdownState.ONE;
				break;
			case CountdownState.ONE:
				countdownTimer.Start(countdownDuration);
				audioPlayer.Stream = soundGo;
				audioPlayer.Play();
				//helperFunctions.FadeOut(One);
				helperFunctions.FadeIn(Go);
				One.Visible = false;
				//Go.Visible = true;
				countdownState = CountdownState.GO;
				break;
			case CountdownState.GO:
				helperFunctions.FadeOut(Go);
				helperFunctions.FadeOut(objectiveMessage, 0.5f);
				//Go.Visible = false;
				//objectiveMessage.Visible = false;
				countdownState = CountdownState.FINISHED;
				break;
			default:
				break;
		}
	}
}
