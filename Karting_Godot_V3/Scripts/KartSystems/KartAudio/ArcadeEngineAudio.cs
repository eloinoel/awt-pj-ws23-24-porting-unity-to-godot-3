using Godot;
using System;

namespace KartGame.KartSystems
{
	/// <summary>
	/// This class produces audio for various states of the vehicle's movement.
	/// </summary>
	public class ArcadeEngineAudio : Node
	{
		[Export(hintString: "What audio clip should play when the kart starts?")]
		public NodePath StartSoundPath;
		public AudioStreamPlayer3D StartSound;
		[Export(hintString: "What audio clip should play when the kart does nothing?")]
		public NodePath IdleSoundPath;
		AudioStreamPlayer3D IdleSound;
		[Export(hintString: "What audio clip should play when the kart moves around?")]
		public NodePath RunningSoundPath;
		public AudioStreamPlayer3D RunningSound;
		[Export(hintString: "What audio clip should play when the kart is drifting")]
		public NodePath DriftPath;
		public AudioStreamPlayer3D Drift;
		[Export(PropertyHint.Range, "0.1f, 1.0f")]
		public float RunningSoundMaxVolume = 1.0f;
		[Export(PropertyHint.Range, "0.1f, 2.0f")]
		public float RunningSoundMaxPitch = 1.0f;
		[Export(hintString: "What audio clip should play when the kart moves in Reverse?")]
		public NodePath ReverseSoundPath;
		public AudioStreamPlayer3D ReverseSound;
		[Export(PropertyHint.Range, "0.1f, 1.0f")]
		public float ReverseSoundMaxVolume = 0.5f;
		[Export(PropertyHint.Range, "0.1f, 2.0f")]
		public float ReverseSoundMaxPitch = 0.6f;
		// NOTE: instead of searching for the parents arcadeKart in _Ready(),
		// we simply assume that the user sets the exported property arcadeKart
		// to the same Kart in both scripts
		// TODO: check if this needs an extra export now
		[Export(hintString: "The Kart to which the played sound effects belong")]
		public NodePath arcadeKartPath;
		public ArcadeKartVehicleBody arcadeKart;

		public override void _Ready()
		{
			StartSound = GetNode<AudioStreamPlayer3D>(StartSoundPath);
			IdleSound = GetNode<AudioStreamPlayer3D>(IdleSoundPath);
			RunningSound = GetNode<AudioStreamPlayer3D>(RunningSoundPath);
			Drift = GetNode<AudioStreamPlayer3D>(DriftPath);
			ReverseSound = GetNode<AudioStreamPlayer3D>(ReverseSoundPath);

			arcadeKart = GetNode<ArcadeKartVehicleBody>(arcadeKartPath);
		}

		public override void _Process(float delta)
		{
			float kartSpeed = 0.0f;
			if (arcadeKart != null)
			{
				kartSpeed = arcadeKart.LocalSpeed();
				Drift.UnitDb = arcadeKart.IsDrifting && arcadeKart.GroundPercent > 0.0f ? GD.Linear2Db(arcadeKart.LinearVelocity.Length() / arcadeKart.GetMaxSpeed()) : GD.Linear2Db(0.0f);
			}

			IdleSound.UnitDb = GD.Linear2Db(Mathf.Lerp(0.6f, 0.0f, Mathf.Clamp(kartSpeed * 4, 0.0f, 1.0f)));

			if (kartSpeed < 0.0f)
			{
				// In reverse
				RunningSound.UnitDb = GD.Linear2Db(0.0f);
				ReverseSound.UnitDb = GD.Linear2Db(Mathf.Lerp(0.1f, ReverseSoundMaxVolume, Mathf.Clamp(-kartSpeed * 1.2f, 0.0f, 1.0f)));
				ReverseSound.PitchScale = Mathf.Lerp(0.1f, ReverseSoundMaxPitch, -kartSpeed + (Mathf.Sin(HelperFunctions.GetTime()) * .1f));
			}
			else
			{
				// Moving forward
				ReverseSound.UnitDb = GD.Linear2Db(0.0f);
				RunningSound.UnitDb = GD.Linear2Db(Mathf.Lerp(0.1f, RunningSoundMaxVolume, Mathf.Clamp(kartSpeed * 1.2f, 0.0f, 1.0f)));
				RunningSound.PitchScale = Mathf.Lerp(0.3f, RunningSoundMaxPitch, kartSpeed + (Mathf.Sin(HelperFunctions.GetTime()) * .1f));
			}
		}
	}
}

