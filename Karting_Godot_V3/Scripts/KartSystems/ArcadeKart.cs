using Godot;

namespace KartSystems
{
	public partial class ArcadeKart : RigidBody3D {

		//-----------------------------------------
		//----------------- STATS -----------------
		//-----------------------------------------

		public class StatPowerup
		{
			public ArcadeKart.Stats modifiers;
			public string PowerUpID;
			public float ElapsedTime;
			public float MaxTime;
		}

		public struct Stats
		{
			[ExportGroup("Movement Settings")]

			[Export(PropertyHint.Range, "0.001f,,")] 
			/// <summary>
			/// Top speed attainable when moving forward.
			/// </summary>
			public float TopSpeed;

			[Export]
			/// <summary>
			/// How quickly the kart reaches top speed.
			/// </summary>
			public float Acceleration;

			[Export(PropertyHint.Range, "0.001f,,")]
			/// <summary>
			/// Top speed attainable when moving backward.
			/// </summary>
			public float ReverseSpeed;

			[Export]
			/// <summary>
			/// How quickly the kart reaches top speed, when moving backward.
			/// </summary>
			public float ReverseAcceleration;

			[Export(PropertyHint.Range, "0.2f, 1,")]
			/// <summary>
			/// How quickly the kart starts accelerating from 0. A higher number means it accelerates faster sooner.
			/// </summary>
			public float AccelerationCurve;

			[Export]
			/// <summary>
			/// How quickly the kart slows down when the brake is applied.
			/// </summary>
			public float Braking;

			[Export]
			/// <summary>
			/// How quickly the kart will reach a full stop when no inputs are made.
			/// </summary>
			public float CoastingDrag;

			[Export(PropertyHint.Range, "0.0f, 1.0f,")]
			/// <summary>
			/// The amount of side-to-side friction.
			/// </summary>
			public float Grip;

			[Export]
			/// <summary>
			/// How tightly the kart can turn left or right.
			/// </summary>
			public float Steer;

			[Export]
			/// <summary>
			/// Additional gravity for when the kart is in the air.
			/// </summary>
			public float AddedGravity;

			// allow for stat adding for powerups.
			public static Stats operator +(Stats a, Stats b)
			{
				return new Stats
				{
					Acceleration = a.Acceleration + b.Acceleration,
					AccelerationCurve = a.AccelerationCurve + b.AccelerationCurve,
					Braking = a.Braking + b.Braking,
					CoastingDrag = a.CoastingDrag + b.CoastingDrag,
					AddedGravity = a.AddedGravity + b.AddedGravity,
					Grip = a.Grip + b.Grip,
					ReverseAcceleration = a.ReverseAcceleration + b.ReverseAcceleration,
					ReverseSpeed = a.ReverseSpeed + b.ReverseSpeed,
					TopSpeed = a.TopSpeed + b.TopSpeed,
					Steer = a.Steer + b.Steer,
				};
			}
		}

		public Rigidbody Rigidbody { get; private set; }
		public InputData Input { get; private set; }
		public float AirPercent { get; private set; }
		public float GroundPercent { get; private set; }

		public ArcadeKart.Stats baseStats = new ArcadeKart.Stats
		{
			TopSpeed = 10f,
			Acceleration = 5f,
			AccelerationCurve = 4f,
			Braking = 10f,
			ReverseAcceleration = 5f,
			ReverseSpeed = 5f,
			Steer = 5f,
			CoastingDrag = 4f,
			Grip = .95f,
			AddedGravity = 1f,
		};


		[ExportGroup("Vehicle Visual")]
		public List<MeshInstance> m_VisualWheels;
	}
}
