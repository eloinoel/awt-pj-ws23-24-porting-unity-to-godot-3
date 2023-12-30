using Godot;
using System;

public class ArcadeKart : RigidBody
{

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

	/* 	public Rigidbody Rigidbody { get; private set; } //TODO: probably not needed because this instance is the Rigidbody */
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


	//[ExportGroup("Physical Wheels")]
	[Export]
	public Godot.NodePath FrontLeftWheelPath;
    public Godot.VehicleWheel3D FrontLeftWheel;

	[Export]
	public Godot.NodePath FrontRightWheelPath;
    public Godot.VehicleWheel3D FrontRightWheel;

	[Export]
	public Godot.NodePath RearLeftWheelPath;
    public Godot.VehicleWheel3D RearLeftWheel;

	[Export]
	public Godot.NodePath RearRightWheelPath;
    public Godot.VehicleWheel3D RearRightWheel;

//	[Export]
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

    

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }





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

    private void UpdateDriftVFXOrientation()
    {
        /* foreach (var vfx in m_DriftSparkInstances)
        {
            vfx.sparks.transform.position = vfx.wheel.transform.position - (vfx.wheel.radius * Vector3.up) + (DriftTrailVerticalOffset * Vector3.up) + (transform.right * vfx.horizontalOffset);
            vfx.sparks.transform.rotation = transform.rotation * Quaternion.Euler(0.0f, 0.0f, vfx.rotation);
        }

        foreach (var trail in m_DriftTrailInstances)
        {
            trail.trailRoot.transform.position = trail.wheel.transform.position - (trail.wheel.radius * Vector3.up) + (DriftTrailVerticalOffset * Vector3.up);
            trail.trailRoot.transform.rotation = transform.rotation;
        } */
    }

    //TODO:
    void UpdateSuspensionParams(VehicleWheel3D wheel)
    {
        /* wheel.suspensionDistance = SuspensionHeight;
        wheel.center = new Vector3(0.0f, WheelsPositionVerticalOffset, 0.0f);
        JointSpring spring = wheel.suspensionSpring;
        spring.spring = SuspensionSpring;
        spring.damper = SuspensionDamp;
        wheel.suspensionSpring = spring; */
    }

    // Called when the node enters the scene tree for the first time.
    // Replaces Awake Method from Unity
	public override void _Ready()
	{
		GD.Print("ArcadeKart script is loaded"); // TODO: remove debug

        // Rigidbody = GetComponent<Rigidbody>(); --> this class is the Rigidbody
        // m_Inputs = GetComponents<IInput>(); --> don't know what this is used for

        // set properties from given node paths
        FrontLeftWheel = GetNode<VehicleWheel3D>(FrontLeftWheelPath);
        FrontRightWheel = GetNode<VehicleWheel3D>(FrontRightWheelPath);
        RearLeftWheel = GetNode<VehicleWheel3D>(RearLeftWheelPath);
        RearRightWheel = GetNode<VehicleWheel3D>(RearRightWheelPath);

        //apply code to properties
        if(FrontRightWheel != null && FrontLeftWheel != null && RearLeftWheel != null && RearRightWheel != null)
        {
            UpdateSuspensionParams(FrontLeftWheel);
            UpdateSuspensionParams(FrontRightWheel);
            UpdateSuspensionParams(RearLeftWheel);
            UpdateSuspensionParams(RearRightWheel);
        }
        else
        {
            GD.PrintErr("_Ready: Wheel Colliders were null");
        }

        m_CurrentGrip = baseStats.Grip;

        //TODO: add in vfx when necessary
        /* if (DriftSparkVFX != null)
        {
            AddSparkToWheel(RearLeftWheel, -DriftSparkHorizontalOffset, -DriftSparkRotation);
            AddSparkToWheel(RearRightWheel, DriftSparkHorizontalOffset, DriftSparkRotation);
        } */

        /* if (DriftTrailPrefab != null)
        {
            AddTrailToWheel(RearLeftWheel);
            AddTrailToWheel(RearRightWheel);
        } */

        /* if (NozzleVFX != null)
        {
            foreach (var nozzle in Nozzles)
            {
                Instantiate(NozzleVFX, nozzle, false);
            }
        } */
	}

	public override void _IntegrateForces(PhysicsDirectBodyState state)
	{
		UpdateSuspensionParams(FrontLeftWheel);
		UpdateSuspensionParams(FrontRightWheel);
		UpdateSuspensionParams(RearLeftWheel);
		UpdateSuspensionParams(RearRightWheel);

		/*GatherInputs();

		// apply our powerups to create our finalStats
		TickPowerups(); */

		// apply our physics properties
		/* Unitys rigidbodies have a centerOfMass property that influences how collisions play out (Godots do not) */
		/* https://www.reddit.com/r/godot/comments/vgi42d/is_it_possible_to_change_center_of_mass_for/
			Changing a RigidBody's center of mass is supported in 4.0.alpha, but not in Godot 3.x.
			Since it relied on lots of internal changes (some of them backwards-incompatible),
			this can't be backported to Godot 3.x without a complete rewrite.
		*/
		/* CenterOfMass = this.transform.InverseTransformPoint(CenterOfMass.position); */

		/* TODO: Unity script also fills a WheelHit structure. But i couldnt find any place it is used in. So I think this is equivalent. */
		int groundedCount = 0;
		if (FrontLeftWheel.is_in_contact() && FrontLeftWheel.get_contact_body() != null):
			groundedCount++;
		if (FrontRightWheel.is_in_contact() && FrontRightWheel.get_contact_body() != null):
			groundedCount++;
		if (RearLeftWheel.is_in_contact() && RearLeftWheel.get_contact_body() != null):
			groundedCount++;
		if (RearRightWheel.is_in_contact() && RearRightWheel.get_contact_body() != null):
			groundedCount++;

		// calculate how grounded and airbone we are
		GroundPercent = (float) groundedCount / 4.0f;
		AirPercent = 1 - GroundPercent;

		// apply vehicle physics
		if (m_CanMove)
		{
			MoveVehicle(Input.getActionStrength("forward"), Input.getActionStrength("backward"), Input.getAxis("right","left"));
		}
		GroundAirborne();

		m_PreviousGroundPercent = GroundPercent;
		
		UpdateDriftVFXOrientation();
	}

	void TickPowerups()
	{
		/* // remove all elapsed powerups
		m_ActivePowerupList.RemoveAll((p) => { return p.ElapsedTime > p.MaxTime; });
		//Debug.Log(m_ActivePowerupList.Count);

		// zero out powerups before we add them all up
		var powerups = new Stats();

		// add up all our powerups
		for (int i = 0; i < m_ActivePowerupList.Count; i++)
		{
			var p = m_ActivePowerupList[i];

			// add elapsed time
			p.ElapsedTime += Time.fixedDeltaTime;

			// add up the powerups
			powerups += p.modifiers;
		}

		// add powerups to our final stats
		m_FinalStats = baseStats + powerups;

		// clamp values in finalstats
		m_FinalStats.Grip = Mathf.Clamp(m_FinalStats.Grip, 0, 1); */
	}

	void GroundAirborne()
	{
		// while in the air, fall faster
		if (AirPercent >= 1)
		{
			/* Rigidbody.velocity += Physics.gravity * Time.fixedDeltaTime * m_FinalStats.AddedGravity; */
		}
	}

	public void Reset()
	{
        /* Vector3 euler = transform.rotation.eulerAngles;
		euler.x = euler.z = 0f;
		transform.rotation = Quaternion.Euler(euler); */
	}

	public float LocalSpeed()
	{
/* 		if (m_CanMove)
		{
			float dot = Vector3.Dot(transform.forward, Rigidbody.velocity);
			if (MinAngleToFinishDrift.Abs(dot) > 0.1f)
			{
				float speed = Rigidbody.velocity.magnitude;
				return DriftControl < 0 ? -(speed / m_FinalStats.ReverseSpeed) : (speed / m_FinalStats.TopSpeed);
			}
			return 0f;
		}
		else
		{
			// use this value to play kart sound when it is waiting the race start countdown.
			return Input.Accelerate ? 1.0f : 0.0f;
		} */
	}

	void MoveVehicle(bool accelerate, bool brake, float turnInput)
	{
		float accelInput = (accelerate ? 1.0f : 0.0f) - (brake ? 1.0f : 0.0f);

		// manual acceleration curve coefficient scalar
		float accelerationCurveCoeff = 5;
		Vector3 localVel = transform.InverseTransformVector(Rigidbody.velocity);

		bool accelDirectionIsFwd = accelInput >= 0;
		bool localVelDirectionIsFwd = localVel.z >= 0;

		// use the max speed for the direction we are going--forward or reverse.
		float maxSpeed = localVelDirectionIsFwd ? m_FinalStats.TopSpeed : m_FinalStats.ReverseSpeed;
		float accelPower = accelDirectionIsFwd ? m_FinalStats.Acceleration : m_FinalStats.ReverseAcceleration;

		float currentSpeed = Rigidbody.velocity.magnitude;
		float accelRampT = currentSpeed / maxSpeed;
		float multipliedAccelerationCurve = m_FinalStats.AccelerationCurve * accelerationCurveCoeff;
		float accelRamp = MinAngleToFinishDrift.Lerp(multipliedAccelerationCurve, 1, accelRampT * accelRampT)

		bool isBraking = (localVelDirectionIsFwd && brake) || (!localVelDirectionIsFwd && accelerate);

		// if we are braking (moving reverse to where we are going)
		// use the braking acceleration instead
		float finalAccelPower = isBraking ? m_FinalStats.Braking : accelPower;

		float finalAcceleration = finalAccelPower * accelRamp;

		// apply inputs to forward/backward
		float turningPower = IsDrifting ? m_DriftTurningPower : turnInput * m_FinalStats.Steer;

		Quaternion turnAngle = Quaternion.AngleAxis(turningPower, transform.up);
		Vector3 fwd = turnAngle * transform.forward;
		Vector3 movement = fwd * accelInput * finalAcceleration *((m_HasCollision || GroundPercent > 0.0f) ? 1.0f : 0.0f);

		// forward movement
		bool wasOverMaxSpeed = currentSpeed >= maxSpeed;

		// if over max speed, cannot accelerate faster.
		if (wasOverMaxSpeed && !isBraking)
			movement *= 0.0f;

		Vector3 newVelocity = Rigidbody.velocity + movement * Time.fixedDeltaTime;
		newVelocity.y = Rigidbody.velocity.y;

		// clamp max speed if we are on ground
		if (GroundPercent > 0.0f && !wasOverMaxSpeed)
		{
			newVelocity = Vector3.ClampMagnitude(newVelocity, maxSpeed);
		}

		// coasting is when we aren't touching accelerate
		if (Mathf.Abs(accelInput) < k_NullInput && GroundPercent > 0.0f)
		{
			newVelocity = Vector3.MoveTowards(newVelocity, new Vector3(0, Rigidbody.velocity.y, 0), Time.fixedDeltaTime * m_FinalStats.CoastingDrag)
		}

		Rigidbody.velocity = newVelocity;

		// Drift
		if (GroundPercent > 0.0f)
		{
			if (m_inAir)
			{
				m_InAir = false;
				Instantiate(JumpVFX, transfrom.position, Quaternion.identity);
			}

			// manual angular velocity coefficient
			float angulatVelocitySteering = 0.4f;
			float angularVelocitySmoothSpeed = 20f;

			// turning is reversed if we're going in reverse and pressing reverse
			if (!localVelDirectionIsFwd && !accelDirectionIsFwd)
				angularVelocitySteering *= -1.0f;
				
			var angularVel = Rigidbody.angularVelocity;

			// move the Y angular velocity towards our target
			angularVel.y = Mathf.MoveTowards(angularVel.y, turningPower * angularVelocitySteering, Time.fixedDeltaTime * angularVelocitySmoothSpeed);

			// apply the angular velocity
			Rigidbody.angularVelocity = angularVel;

			// rotate rigidbody's velocity as well to generate immediate velocity redirection
			// maual velocity steering coefficient
			float velocitySteering = 25f;

			// If the karts lands with a forward not in the velocity direction, we start the drift
			if (GroundPercent >= 0.0f && m_PreviousGroundPercent < 0.1f)
			{
				Vector3 flattenVelocity = Vector3.ProjectOnPlane(Rigidbody.velocity, m_VerticalReference).normalized;
				if (Vector3.Dot(flattenVelocity, transform.forward * Mathf.Sign(accelInput)) < Mathf.Cos(MinAngleToFinishDrift * Mathf.Deg2Rad))
				{
					IsDrifting = true;
					m_CurrentGrip = DriftGrip;
					m_DriftTurningPower = 0.0f;
				}
			}

			// Drift Management
			if (!IsDrifting)
			{
				if ((WantsToDrift || isBraking) && currentSpeed > maxSpeed * MinSpeedPercentToFinishDrift)
				{
					IsDrifting = true;
					m_DriftTurningPower = turningPower + (Mathf.Sign(turningPower) * DriftAdditionalSteer);
					m_CurrentGrip = DriftGrip;

					ActivateDriftVFX(true);
				}
			}

			if (IsDrifting)
			{
				float turnInputAbs = Mathf.Abs(turnInput);
				if (turnInputAbs < k_NullInput)
					m_DriftTurningPower = Mathf.MoveTowards(m_DriftTurningPower, 0.0f, Mathf.Clamp01(DriftDampening * Time.fixedDeltaTime));

				// Update the turning power based on input
				float driftMaxSteerValue = m_FinalStats.Steer + DriftAdditionalSteer;
				m_DriftTurningPower = Mathf.Clamp(m_DriftTurningPower + (turnInput * Mathf.Clamp01(DriftControl * Time.fixedDeltaTime)), -driftMaxSteerValue, driftMaxSteerValue);

				bool facingVelocity = Vector3.Dot(Rigidbody.velocity.normalized, transform.forward * Mathf.Sign(accelInput)) > Mathf.Cos(MinAngleToFinishDrift * Mathf.Deg2Rad);

				bool canEndDrift = true;
				if (isBraking)
					canEndDrift = false;
				else if (!facingVelocity)
					canEndDrift = false;
				else if (turnInputAbs >= k_NullInput && currentSpeed > maxSpeed * MinSpeedPercentToFinishDrift)
					canEndDrift = false;

				if (canEndDrift || currentSpeed < k_NullSpeed)
				{
					// No Input, and car aligned with speed direction => Stop the drift
					IsDrifting = false;
					m_CurrentGrip = m_FinalStats.Grip;
				}

			}

			// rotate our velocity based on current steer value
			Rigidbody.velocity = Quaternion.AngleAxis(turningPower * Mathf.Sign(localVel.z) * velocitySteering * m_CurrentGrip * Time.fixedDeltaTime, transform.up) * Rigidbody.velocity;
		}
		else
		{
			m_InAir = true;
		}

		bool validPosition = false;
		if (Physics.Raycast(transform.position + (transform.up * 0.1f), -transform.up, out RaycastHit hit, 3.0f, 1 << 9 | 1 << 10 | 1 << 11)) // Layer: ground (9) / Environment(10) / Track (11)
		{
			Vector3 lerpVector = (m_HasCollision && m_LastCollisionNormal.y > hit.normal.y) ? m_LastCollisionNormal : hit.normal;
			m_VerticalReference = Vector3.Slerp(m_VerticalReference, lerpVector, Mathf.Clamp01(AirborneReorientationCoefficient * Time.fixedDeltaTime * (GroundPercent > 0.0f ? 10.0f : 1.0f)));    // Blend faster if on ground
		}
		else
		{
			Vector3 lerpVector = (m_HasCollision && m_LastCollisionNormal.y > 0.0f) ? m_LastCollisionNormal : Vector3.up;
			m_VerticalReference = Vector3.Slerp(m_VerticalReference, lerpVector, Mathf.Clamp01(AirborneReorientationCoefficient * Time.fixedDeltaTime));
		}

		validPosition = GroundPercent > 0.7f && !m_HasCollision && Vector3.Dot(m_VerticalReference, Vector3.up) > 0.9f;

		// Airborne / Half on ground management
		if (GroundPercent < 0.7f)
		{
			Rigidbody.angularVelocity = new Vector3(0.0f, Rigidbody.angularVelocity.y * 0.98f, 0.0f);
			Vector3 finalOrientationDirection = Vector3.ProjectOnPlane(transform.forward, m_VerticalReference);
			finalOrientationDirection.Normalize();
			if (finalOrientationDirection.sqrMagnitude > 0.0f)
			{
				Rigidbody.MoveRotation(Quaternion.Lerp(Rigidbody.rotation, Quaternion.LookRotation(finalOrientationDirection, m_VerticalReference), Mathf.Clamp01(AirborneReorientationCoefficient * Time.fixedDeltaTime)));
			}
		}
		else if (validPosition)
		{
			m_LastValidPosition = transform.position;
			m_LastValidRotation.eulerAngles = new Vector3(0.0f, transform.rotation.y, 0.0f);
		}

		ActivateDriftVFX(IsDrifting && GroundPercent > 0.0f);
	}
	//TODO: continue porting methods here
}
