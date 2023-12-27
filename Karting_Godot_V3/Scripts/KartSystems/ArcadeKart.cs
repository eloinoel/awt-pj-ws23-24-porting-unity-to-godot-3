using Godot;

public class ArcadeKart {

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
		//[ExportGroup("Movement Settings")]
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

/* 	public Rigidbody Rigidbody { get; private set; } //TODO: change type */
/* 	public InputData Input { get; private set; }//TODO: change type */
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

	//-----------------------------------------
	//----Additionally Exported Parameters-----
	//-----------------------------------------

	//[ExportGroup("Vehicle Visual")]
	[Export]
	public Godot.Collections.Array<Godot.NodePath> m_VisualWheels;


	//[ExportGroup("Vehicle Physics")]
	[Export]
	/// <summary>
	/// The transform that determines the position of the kart's mass.
	/// </summary>
	public Transform CenterOfMass; //TODO: change type

	[Export(PropertyHint.Range, "0.0f, 20.0f,")]
	/// <summary>
	/// Coefficient used to reorient the kart in the air. The higher the number, the faster the kart will readjust itself along the horizontal plane.
	/// </summary>
	public float AirborneReorientationCoefficient = 3.0f;


	//[ExportGroup("Drifting")]
	[Export(PropertyHint.Range, "0.01f, 1.0f,")]
	/// <summary>
	/// The grip value when drifting.
	/// </summary>
	public float DriftGrip = 0.4f;

	[Export(PropertyHint.Range, "0.0f, 10.0f,")]
	/// <summary>
	/// Additional steer when the kart is drifting.
	/// </summary>
	public float DriftAdditionalSteer = 5.0f;

	[Export(PropertyHint.Range, "1.0f, 30.0f,")]
	/// <summary>
	/// The higher the angle, the easier it is to regain full grip.
	/// </summary>
	public float MinAngleToFinishDrift = 10.0f;

	[Export(PropertyHint.Range, "0.01f, 0.99f,")]
	/// <summary>
	/// Mininum speed percentage to switch back to full grip.
	/// </summary>
	public float MinSpeedPercentToFinishDrift = 0.5f;

	[Export(PropertyHint.Range, "1.0f, 20.0f,")]
	/// <summary>
	/// The higher the value, the easier it is to control the drift steering.
	/// </summary>
	public float DriftControl = 10.0f;

	[Export(PropertyHint.Range, "0.0f, 20.0f,")]
	/// <summary>
	/// The lower the value, the longer the drift will last without trying to control it by steering.
	/// </summary>
	public float DriftDampening = 10.0f;


	//[ExportGroup("VFX")]
	/*[Export]
	/// <summary>
	/// VFX that will be placed on the wheels when drifting.
	/// </summary>
 	public ParticleSystem DriftSparkVFX; //TODO: change type */

	[Export(PropertyHint.Range, "0.0f, 0.2f,")]
	/// <summary>
	/// Offset to displace the VFX to the side.
	/// </summary>
	public float DriftSparkHorizontalOffset = 0.1f;

	[Export(PropertyHint.Range, "0.0f, 90.0f,")]
	/// <summary>
	/// Angle to rotate the VFX.
	/// </summary>
	public float DriftSparkRotation = 17.0f;

	/*[Export]
	/// <summary>
	/// VFX that will be placed on the wheels when drifting.
	/// </summary>
 	public GameObject DriftTrailPrefab; //TODO: change type */

	[Export(PropertyHint.Range, "-0.1f, 0.1f,")]
	/// <summary>
	/// Vertical to move the trails up or down and ensure they are above the ground.
	/// </summary>
	public float DriftTrailVerticalOffset;

	/*[Export]
	/// <summary>
	/// VFX that will spawn upon landing, after a jump.
	/// </summary>
 	public GameObject JumpVFX; //TODO: change type */

	/*[Export]
	/// <summary>
	/// VFX that is spawn on the nozzles of the kart.
	/// </summary>
 	public GameObject NozzleVFX; //TODO: change type */

	/*[Export]
	/// <summary>
	/// List of the kart's nozzles.
	/// </summary>
 	public Godot.Collections.Array<Transform> Nozzles; //TODO: change type */
	

	//[ExportGroup("Suspensions")]
	[Export(PropertyHint.Range, "0.0f, 1.0f,")]
	/// <summary>
	/// The maximum extension possible between the kart's body and the wheels.
	/// </summary>
	public float SuspensionHeight = 0.2f;

	[Export(PropertyHint.Range, "10.0f, 100000.0f,")]
	/// <summary>
	/// The higher the value, the stiffer the suspension will be.
	/// </summary>
	public float SuspensionSpring = 20000.0f;

	[Export(PropertyHint.Range, "0.0f, 5000.0f,")]
	/// <summary>
	/// The higher the value, the faster the kart will stabilize itself.
	/// </summary>
	public float SuspensionDamp = 500.0f;

	[Export(PropertyHint.Range, "-1.0f, 1.0f,")]
	/// <summary>
	/// Vertical offset to adjust the position of the wheels relative to the kart's body.
	/// </summary>
	public float WheelsPositionVerticalOffset = 0.0f;


/* 	//[ExportGroup("Physical Wheels")]
	[Export]
	public WheelCollider FrontLeftWheel; //TODO: change type

	[Export]
	public WheelCollider FrontRightWheel; //TODO: change type

	[Export]
	public WheelCollider RearLeftWheel; //TODO: change type

	[Export]
	public WheelCollider RearRightWheel; //TODO: change type */

	[Export]
	/// <summary>
	/// Which layers the wheels will detect.
	/// </summary>
/* 	public LayerMask GroundLayers = Physics.DefaultRaycastLayers; //TODO: change type */


	//-----------------------------------------
	//---------- Internal Parameters ----------
	//-----------------------------------------

	// the input sources that can control the kart
/* 	IInput[] m_Inputs; //TODO: change type */

	const float k_NullInput = 0.01f;
	const float k_NullSpeed = 0.01f;
/* 	Godot.Vector3 m_VerticalReference = Vector3.up; */

	// Drift params
	public bool WantsToDrift { get; private set; } = false;
	public bool IsDrifting { get; private set; } = false;
	float m_CurrentGrip = 1.0f;
	float m_DriftTurningPower = 0.0f;
	float m_PreviousGroundPercent = 1.0f;
/* 	readonly Godot.Collections.Array<(GameObject trailRoot, WheelCollider wheel, TrailRenderer trail)> m_DriftTrailInstances = new List<(GameObject, WheelCollider, TrailRenderer)>(); //TODO: change types */
/* 	readonly Godot.Collections.Array<(WheelCollider wheel, float horizontalOffset, float rotation, ParticleSystem sparks)> m_DriftSparkInstances = new List<(WheelCollider, float, float, ParticleSystem)>(); //TODO: change types */

	// can the kart move?
	bool m_CanMove = true;
	Godot.Collections.Array<StatPowerup> m_ActivePowerupList = new Godot.Collections.Array<StatPowerup>();
	ArcadeKart.Stats m_FinalStats;

/* 	Godot.Quaternion m_LastValidRotation; */
	Godot.Vector3 m_LastValidPosition;
	Godot.Vector3 m_LastCollisionNormal;
	bool m_HasCollision;
	bool m_InAir = false;

	//-----------------------------------------
	//--------------- Functions ---------------
	//-----------------------------------------

	public void AddPowerup(StatPowerup statPowerup)
	{
		//Debug.Log("add Powerup");
		m_ActivePowerupList.Add(statPowerup);
	}

	public void SetCanMove(bool move) => m_CanMove = move;
	public float GetMaxSpeed() => Mathf.Max(m_FinalStats.TopSpeed, m_FinalStats.ReverseSpeed);

	//TODO: needs adjustment for collection maneuvering
/* 	private void ActivateDriftVFX(bool active)
	{
		foreach (var vfx in m_DriftSparkInstances)
		{
			if (active && vfx.wheel.GetGroundHit(out WheelHit hit))
			{
				if (!vfx.sparks.isPlaying)
					vfx.sparks.Play();
			}
			else
			{
				if (vfx.sparks.isPlaying)
					vfx.sparks.Stop(true, ParticleSystemStopBehavior.StopEmitting);
			}
		}

		foreach (var trail in m_DriftTrailInstances)
			trail.Item3.emitting = active && trail.wheel.GetGroundHit(out WheelHit hit);
	} */

	//TODO: continue porting methods here

}
