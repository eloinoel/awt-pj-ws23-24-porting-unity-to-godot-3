using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;

public class ArcadeKartVehicleBody : VehicleBody
{

	private Vector3 targetLinearVelocity;
	private Vector3 targetAngularVelocity;
	private float linearDamping = 0.1f;
	private float angularDamping = 0.5f;
	private float collisionSlowdownFactor = 0.8f; // Slowdown factor (50% of current speed)

	private bool prevHasCollision = false;

	//-----------------------------------------
	//----------------- STATS -----------------
	//-----------------------------------------

	public class StatPowerup
	{
		public ArcadeKartVehicleBody.Stats modifiers;
		public string PowerUpID;
		public float ElapsedTime;
		public float MaxTime;
	}

	public struct Stats
	{
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

	public RigidBody Rigidbody { get; private set; }
	public float AirPercent { get; private set; }
	public float GroundPercent { get; private set; }

	//original
	/* public ArcadeKartVehicleBody.Stats baseStats = new ArcadeKartVehicleBody.Stats
	{
		// I hard set the settings hiw they were overwritten by the unity editor
		TopSpeed = 15f, //10f,
		Acceleration = 7f, //5f,
		AccelerationCurve = 0.5f, //4f,
		Braking = 16f, //10f,
		ReverseAcceleration = 3f, //5f,
		ReverseSpeed = 10f, //5f,
		Steer = 4f, //5f,
		CoastingDrag = 5f, //4f,
		Grip = 0.97f, //.95f,
		AddedGravity = 1f,
	}; */

	//tuning effort
	public ArcadeKartVehicleBody.Stats baseStats = new ArcadeKartVehicleBody.Stats
	{
		// I hard set the settings hiw they were overwritten by the unity editor
		TopSpeed = 17f, //10f,
		Acceleration = 7f, //5f,
		AccelerationCurve = 0.5f, //4f,
		Braking = 16f, //10f,
		ReverseAcceleration = 3f, //5f,
		ReverseSpeed = 10f, //5f,
		Steer = 5f, //5f,
		CoastingDrag = 5f, //4f,
		Grip = 0.99f, //.95f,
		AddedGravity = 1.5f,
	};

	//-----------------------------------------
	//----Additionally Exported Parameters-----
	//-----------------------------------------

	[Export]
	public Godot.Collections.Array<Godot.NodePath> m_VisualWheels;


	[Export]
	/// <summary>
	/// The transform that determines the position of the kart's mass.
	/// </summary>
	public Transform CenterOfMass;

	[Export(PropertyHint.Range, "0.0f, 20.0f,")]
	/// <summary>
	/// Coefficient used to reorient the kart in the air. The higher the number, the faster the kart will readjust itself along the horizontal plane.
	/// </summary>
	public float AirborneReorientationCoefficient = 3.0f;


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

	[Export(PropertyHint.Range, "-0.1f, 0.1f,")]
	/// <summary>
	/// Vertical to move the trails up or down and ensure they are above the ground.
	/// </summary>
	public float DriftTrailVerticalOffset;

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


	[Export]
	public Godot.NodePath FrontLeftWheelPath;
	public Godot.VehicleWheel FrontLeftWheel;

	[Export]
	public Godot.NodePath FrontRightWheelPath;
	public Godot.VehicleWheel FrontRightWheel;

	[Export]
	public Godot.NodePath RearLeftWheelPath;
	public Godot.VehicleWheel RearLeftWheel;

	[Export]
	public Godot.NodePath RearRightWheelPath;
	public Godot.VehicleWheel RearRightWheel;

	//-----------------------------------------
	//---------- Internal Parameters ----------
	//-----------------------------------------

	// the input sources that can control the kart
	const float k_NullInput = 0.01f;
	const float k_NullSpeed = 0.01f;
	Godot.Vector3 m_VerticalReference = Vector3.Up;

	float kartTurnInput = 0.0f;
	// Drift params
	public bool WantsToDrift { get; private set; } = false;
	public bool IsDrifting { get; private set; } = false;
	float m_CurrentGrip = 1.0f;
	float m_DriftTurningPower = 0.0f;
	float m_PreviousGroundPercent = 1.0f;
/*  readonly Godot.Collections.Array<(GameObject trailRoot, WheelCollider wheel, TrailRenderer trail)> m_DriftTrailInstances = new List<(GameObject, WheelCollider, TrailRenderer)>();*/
/*  readonly Godot.Collections.Array<(WheelCollider wheel, float horizontalOffset, float rotation, ParticleSystem sparks)> m_DriftSparkInstances = new List<(WheelCollider, float, float, ParticleSystem)>();*/

	// can the kart move?
	bool m_CanMove = true;
	List<StatPowerup> m_ActivePowerupList = new List<StatPowerup>();
	ArcadeKartVehicleBody.Stats m_FinalStats;

	Godot.Quat m_LastValidRotation;
	Godot.Vector3 m_LastValidPosition;
	Godot.Vector3 m_LastCollisionNormal;
	bool m_HasCollision;
	bool m_InAir = false;

	//-----------------------------------------
	//--------------- Functions ---------------
	//-----------------------------------------

	public void AddPowerup(StatPowerup statPowerup)
	{
		m_ActivePowerupList.Add(statPowerup);
	}

	public void SetCanMove(bool move) => m_CanMove = move;
	public float GetMaxSpeed() => Mathf.Max(m_FinalStats.TopSpeed, m_FinalStats.ReverseSpeed);

	//NOTE: needs adjustment for collection maneuvering
	private void ActivateDriftVFX(bool active)
	{
		/*foreach (var vfx in m_DriftSparkInstances)
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
			trail.Item3.emitting = active && trail.wheel.GetGroundHit(out WheelHit hit);*/
	}

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

	void UpdateSuspensionParams(VehicleWheel wheel)
	{
		/* wheel.suspensionDistance = SuspensionHeight;
		wheel.center = new Vector3(0.0f, WheelsPositionVerticalOffset, 0.0f);
		JointSpring spring = wheel.suspensionSpring;
		spring.spring = SuspensionSpring;
		spring.damper = SuspensionDamp;
		wheel.suspensionSpring = spring; */
	}

	// Smooth axis values like in unity
	float smoothInput(float currentInput, float turnInput, float delta)
	{
		float gravity = 3.0f;
		float sensitivity = 3.0f;
		// return to resting Input 0.0f
		if(turnInput == 0.0f && currentInput < 0.0f) {
			return Mathf.Clamp(currentInput + gravity*delta, -1.0f, 0.0f);
		} else if(turnInput == 0.0f && currentInput > 0.0f) {
			return Mathf.Clamp(currentInput - gravity*delta, 0.0f, 1.0f);
		}
		// This implements unitys Snap property
		if((currentInput > 0.0f && turnInput < 0.0f) || (currentInput < 0.0f && turnInput > 0.0f)) {
			currentInput = 0.0f;
		}
		// smoothly transition to target input with the linear speed defined by sensitivity
		return Mathf.Clamp(currentInput + sensitivity*delta*turnInput, -1.0f, 1.0f);
	}

	// Called when the node enters the scene tree for the first time.
	// Replaces Awake Method from Unity
	public override void _Ready()
	{
		Rigidbody = this;

		// set properties from given node paths
		FrontLeftWheel = GetNode<VehicleWheel>(FrontLeftWheelPath);
		FrontRightWheel = GetNode<VehicleWheel>(FrontRightWheelPath);
		RearLeftWheel = GetNode<VehicleWheel>(RearLeftWheelPath);
		RearRightWheel = GetNode<VehicleWheel>(RearRightWheelPath);

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

		//setup collision callbacks with the CollisionShape
		Connect("body_entered", this, "_OnCollisionEnter");
		Connect("body_exited", this, "_OnCollisionExit");
	}

	private void ApplyStabilizingImpulse(PhysicsDirectBodyState state)
	{
		// Define the stabilizing impulse magnitude
		float angularImpulseMagnitude = 10.0f;
		float linearImpulseMagnitude = 5.0f;

		// Apply an angular impulse to stop any unwanted rotation
		Vector3 angularImpulse = -state.AngularVelocity * angularImpulseMagnitude;
		state.ApplyImpulse(Vector3.Zero, angularImpulse);

		// Apply a linear impulse to correct tilt
		Vector3 upDirection = Vector3.Up;
		Vector3 correctionImpulse = (upDirection - state.Transform.basis.y) * linearImpulseMagnitude;
		state.ApplyImpulse(Vector3.Zero, correctionImpulse);
	}


	public override void _IntegrateForces(PhysicsDirectBodyState state)
	{

		/* if (m_HasCollision && !prevHasCollision)
		{
			// stabalize Kart in collision
			ApplyStabilizingImpulse(state);
			
			// add slowdown effect when colliding
			targetLinearVelocity = state.LinearVelocity * (1.0f - linearDamping);
			targetAngularVelocity = state.AngularVelocity * (1.0f - angularDamping);

			state.LinearVelocity = targetLinearVelocity;
			state.AngularVelocity = targetAngularVelocity;

		}
		// so stat stablising is not applied every frame the collision is detected
		prevHasCollision = m_HasCollision; */

		//base._IntegrateForces(state);

		UpdateSuspensionParams(FrontLeftWheel);
		UpdateSuspensionParams(FrontRightWheel);
		UpdateSuspensionParams(RearLeftWheel);
		UpdateSuspensionParams(RearRightWheel);

		// apply our powerups to create our finalStats
		TickPowerups(state);

		// apply our physics properties
		/* Unitys rigidbodies have a centerOfMass property that influences how collisions play out (Godots do not) */
		/* https://www.reddit.com/r/godot/comments/vgi42d/is_it_possible_to_change_center_of_mass_for/
			Changing a RigidBody's center of mass is supported in 4.0.alpha, but not in Godot 3.x.
			Since it relied on lots of internal changes (some of them backwards-incompatible),
			this can't be backported to Godot 3.x without a complete rewrite.
		*/
		/* CenterOfMass = this.transform.InverseTransformPoint(CenterOfMass.position); */

		int groundedCount = 0;
		if (FrontLeftWheel.IsInContact() && FrontLeftWheel.GetContactBody() != null)
			groundedCount++;
		if (FrontRightWheel.IsInContact() && FrontRightWheel.GetContactBody() != null)
			groundedCount++;
		if (RearLeftWheel.IsInContact() && RearLeftWheel.GetContactBody() != null)
			groundedCount++;
		if (RearRightWheel.IsInContact() && RearRightWheel.GetContactBody() != null)
			groundedCount++;

		//check collisions with kart (in Unity onCollisionStay method)
		_OnCollisionStay(state);

		// calculate how grounded and airbone we are
		GroundPercent = (float) groundedCount / 4.0f;
		AirPercent = 1 - GroundPercent;

		// apply vehicle physics
		if (m_CanMove)
		{
			kartTurnInput = smoothInput(kartTurnInput, Input.GetAxis("right","left"), state.Step);
			MoveVehicle(Input.IsActionPressed("forward"), Input.IsActionPressed("backward"), kartTurnInput, state);
			//MoveVehicle(Input.IsActionPressed("forward"), Input.IsActionPressed("backward"), Input.GetAxis("right","left"), state);
		}
		GroundAirborne(state);

		m_PreviousGroundPercent = GroundPercent;

		UpdateDriftVFXOrientation();
	}

	void TickPowerups(PhysicsDirectBodyState state)
	{
		// remove all elapsed powerups
		//m_ActivePowerupList.RemoveAll((p) => { return p.ElapsedTime > p.MaxTime; });
/*      for (int i = m_ActivePowerupList.Count-1; i >= 0; i--)
		{
			StatPowerup powerup = m_ActivePowerupList[i];
			if (powerup.ElapsedTime > powerup.MaxTime)
			{
				m_ActivePowerupList.Remove(powerup);
			}
		} */
		if (m_ActivePowerupList.Count > 0)
		{
			StatPowerup powerup = m_ActivePowerupList[0];
			if (powerup.ElapsedTime > powerup.MaxTime)
			{
				m_ActivePowerupList.Clear();
			}
		}

		// zero out powerups before we add them all up
		var powerups = new Stats();

		// add up all our powerups
		for (int i = 0; i < m_ActivePowerupList.Count; i++)
		{
			var p = m_ActivePowerupList[i];

			// add elapsed time
			p.ElapsedTime += state.Step;

			// add up the powerups
			powerups += p.modifiers;
		}

		// add powerups to our final stats
		m_FinalStats = baseStats + powerups;

		// clamp values in finalstats
		m_FinalStats.Grip = Mathf.Clamp(m_FinalStats.Grip, 0, 1);
	}

	void GroundAirborne(PhysicsDirectBodyState state)
	{
		// while in the air, fall faster
		if (AirPercent >= 1)
		{
			state.LinearVelocity += new Vector3(0, -10, 0) * state.Step * m_FinalStats.AddedGravity;
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
		Vector3 Forward = GlobalTransform.basis.z.Normalized();
		if (m_CanMove)
		{
			float dot = Forward.Dot(this.LinearVelocity);
			if (Mathf.Abs(dot) > 0.1f)
			{
				float speed = this.LinearVelocity.Length();
				return dot < 0 ? -(speed / m_FinalStats.ReverseSpeed) : (speed / m_FinalStats.TopSpeed);
			}
			return 0f;
		}
		else
		{
			// use this value to play kart sound when it is waiting the race start countdown.
			return Input.IsActionPressed("forward") ? 1.0f : 0.0f;
		}
		return 0f;
	}

	// see https://stackoverflow.com/questions/69728827/how-do-i-detect-collisions-in-godot
	private void _OnCollisionEnter(Node body)
	{
		m_HasCollision = true;
	}

	private void _OnCollisionExit(Node body)
	{
		m_HasCollision = false;
	}

	// this method isn't a real callback but is called every frame in _integrateForces (naming according to unity code)
	private void _OnCollisionStay(PhysicsDirectBodyState state)
	{
		m_LastCollisionNormal = Vector3.Zero;
		float dot = -1.0f;
		int contactCount = state.GetContactCount();

		if (contactCount > 0)
		{
			m_HasCollision = true;
			for(int i = 0; i < contactCount; i++)
			{
				Vector3 contactNormal = state.GetContactLocalNormal(i);
				if(contactNormal.Dot(Vector3.Up) > dot)
				{
					m_LastCollisionNormal = contactNormal;
				}
			}
		}
	}

	/*
	void OnCollisionStay(Collision collision)
	{
		m_HasCollision = true;
		m_LastCollisionNormal = Vector3.zero;
		float dot = -1.0f;

		foreach (var contact in collision.contacts)
		{
			if (Vector3.Dot(contact.normal, Vector3.up) > dot)
				m_LastCollisionNormal = contact.normal;
		}
	} */
	void MoveVehicle(bool accelerate, bool brake, float turnInput, PhysicsDirectBodyState state)
	{
		float accelInput = (accelerate ? 1.0f : 0.0f) - (brake ? 1.0f : 0.0f);

		// manual acceleration curve coefficient scalar
		float accelerationCurveCoeff = 5;
		Vector3 localVel = state.Transform.basis.XformInv(state.LinearVelocity);

		bool accelDirectionIsFwd = accelInput >= 0;
		bool localVelDirectionIsFwd = localVel.z >= 0;

		// use the max speed for the direction we are going--forward or reverse.
		float maxSpeed = localVelDirectionIsFwd ? m_FinalStats.TopSpeed : m_FinalStats.ReverseSpeed;
		float accelPower = accelDirectionIsFwd ? m_FinalStats.Acceleration : m_FinalStats.ReverseAcceleration;

		float currentSpeed = state.LinearVelocity.Length();
		float accelRampT = currentSpeed / maxSpeed;
		float multipliedAccelerationCurve = m_FinalStats.AccelerationCurve * accelerationCurveCoeff;
		float accelRamp = Mathf.Lerp(multipliedAccelerationCurve, 1, accelRampT * accelRampT);

		bool isBraking = (localVelDirectionIsFwd && brake) || (!localVelDirectionIsFwd && accelerate);

		// if we are braking (moving reverse to where we are going)
		// use the braking acceleration instead
		float finalAccelPower = isBraking ? m_FinalStats.Braking : accelPower;

		float finalAcceleration = finalAccelPower * accelRamp;

		// apply inputs to forward/backward
		float turningPower = IsDrifting ? m_DriftTurningPower : turnInput * m_FinalStats.Steer;

		/* In stack overflow we trust:
		https://stackoverflow.com/questions/48438273/godot-3d-get-forward-vector */
		Vector3 Up = GlobalTransform.basis.y.Normalized();
		Vector3 Forward = GlobalTransform.basis.z.Normalized();

		Vector3 fwd = Forward.Rotated(Up, Mathf.Deg2Rad(turningPower));
		Vector3 movement = fwd * accelInput * finalAcceleration * ((m_HasCollision || GroundPercent > 0.0f) ? 1.0f : 0.0f);

		// forward movement
		bool wasOverMaxSpeed = currentSpeed >= maxSpeed;

		// if over max speed, cannot accelerate faster.
		if (wasOverMaxSpeed && !isBraking)
			movement *= 0.0f;

		//changed to state instead of Rigidbody.LinearVelocity
		Vector3 newVelocity = state.LinearVelocity + movement * state.Step;
		newVelocity.y = state.LinearVelocity.y;

		//  clamp max speed if we are on ground
		if (GroundPercent > 0.0f && !wasOverMaxSpeed)
		{
			newVelocity = newVelocity.LimitLength(maxSpeed);
		}

		// coasting(Leerlauf) is when we aren't touching accelerate
		if (Mathf.Abs(accelInput) < k_NullInput && GroundPercent > 0.0f)
		{
			newVelocity = newVelocity.MoveToward(new Vector3(0, state.LinearVelocity.y, 0), state.Step * m_FinalStats.CoastingDrag);
		}
		state.LinearVelocity = newVelocity;

		// Drift
		if (GroundPercent > 0.0f)
		{
			if (m_InAir)
			{
				m_InAir = false;
			}

			// manual angular velocity coefficient
			float angularVelocitySteering = 0.4f;
			float angularVelocitySmoothSpeed = 20f;

			// turning is reversed if we're going in reverse and pressing reverse
			if (!localVelDirectionIsFwd && !accelDirectionIsFwd)
				angularVelocitySteering *= -1.0f;

			var angularVel = state.AngularVelocity;

			// move the Y angular velocity towards our target
			angularVel.y = Mathf.MoveToward(angularVel.y, turningPower * angularVelocitySteering, state.Step * angularVelocitySmoothSpeed);

			// apply the angular velocity
			state.AngularVelocity = angularVel;

			// rotate rigidbody's velocity as well to generate immediate velocity redirection
			// manual velocity steering coefficient
			float velocitySteering = 25f;

			// If the karts lands with a forward not in the velocity direction, we start the drift
			/*if (GroundPercent >= 0.0f && m_PreviousGroundPercent < 0.1f)
			{
				Vector3 flattenVelocity = Vector3.ProjectOnPlane(Rigidbody.velocity, m_VerticalReference).normalized;
				if (Vector3.Dot(flattenVelocity, transform.forward * Mathf.Sign(accelInput)) < Mathf.Cos(MinAngleToFinishDrift * Mathf.Deg2Rad))
				{
					IsDrifting = true;
					m_CurrentGrip = DriftGrip;
					m_DriftTurningPower = 0.0f;
				}
			}*/

			// Drift Management
			/*if (!IsDrifting)
			{
				if ((WantsToDrift || isBraking) && currentSpeed > maxSpeed * MinSpeedPercentToFinishDrift)
				{
					IsDrifting = true;
					m_DriftTurningPower = turningPower + (Mathf.Sign(turningPower) * DriftAdditionalSteer);
					m_CurrentGrip = DriftGrip;

					ActivateDriftVFX(true);
				}
			}*/

			/*if (IsDrifting)
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

			}*/

			// rotate our velocity based on current steer value
			localVel = state.Transform.basis.XformInv(state.LinearVelocity);
			state.LinearVelocity = state.LinearVelocity.Rotated(Up, Mathf.Deg2Rad(turningPower * Mathf.Sign(localVel.z) * velocitySteering * m_CurrentGrip * state.Step));
		}
		else
		{
			m_InAir = true;
		}

		bool validPosition = false;
		/*else if (validPosition)
		{
			m_LastValidPosition = transform.position;
			m_LastValidRotation.eulerAngles = new Vector3(0.0f, transform.rotation.y, 0.0f);
		}

		ActivateDriftVFX(IsDrifting && GroundPercent > 0.0f);*/
		// NOTE: lastvalidRotation still buggy/often misset, can lead to teleport off map glitch
		/* var spacestate = GetWorld().DirectSpaceState;
		var ignoreCollision = new Godot.Collections.Array { this };
		var intersection = spacestate.IntersectRay(Rigidbody.GlobalTransform.origin, -Up, ignoreCollision);
		if (intersection.Count > 0)
		{
			Vector3 hitNormal = (Vector3) intersection["normal"];
			Vector3 lerpVector = (m_HasCollision && m_LastCollisionNormal.y > hitNormal.y) ? m_LastCollisionNormal : hitNormal;
			float slerpRatio = Mathf.Clamp(AirborneReorientationCoefficient * state.Step * (GroundPercent > 0.0f ? 10.0f : 1.0f), 0.0f, 1.0f);
			m_VerticalReference = QuatSlerp(m_VerticalReference, lerpVector, slerpRatio);
		}
		else
		{
			Vector3 lerpVector = (m_HasCollision && m_LastCollisionNormal.y > 0.0f) ? m_LastCollisionNormal : Vector3.Up;
			float slerpRatio = Mathf.Clamp(AirborneReorientationCoefficient * state.Step, 0.0f, 1.0f);
			m_VerticalReference = QuatSlerp(m_VerticalReference, lerpVector, slerpRatio);
		}
		validPosition = GroundPercent > 0.7f && !m_HasCollision && m_VerticalReference.Dot(Vector3.Up) > 0.9f;

		// Airborne / Half on ground management
		if (GroundPercent < 0.7f)
		{
			state.AngularVelocity = new Vector3(0.0f, state.AngularVelocity.y * 0.98f, 0.0f);
			Forward = Rigidbody.GlobalTransform.basis.z;
			Plane projectionPlane = new Plane(m_VerticalReference, 0.0f);
			Vector3 finalOrientationDirection = projectionPlane.Project(Forward);
			finalOrientationDirection = finalOrientationDirection.Normalized();
			if (finalOrientationDirection.LengthSquared() > 0.0f)
			{
				float slerpRatio = Mathf.Clamp(AirborneReorientationCoefficient * state.Step, 0.0f, 1.0f);
				Vector3 Rotation = state.Transform.basis.GetEuler();
				//Quat QuatRotation = new Quat(Rotation);
				Vector3 LateralAxis = finalOrientationDirection.Cross(m_VerticalReference);
				Basis basis = new Basis(LateralAxis, m_VerticalReference, finalOrientationDirection);
				Quat lookAt = basis.Quat().Normalized();
				Quat lerpQuat = new Quat(QuatSlerp(Rotation, lookAt.GetEuler(), slerpRatio));
				state.Transform = state.Transform.LookingAt(finalOrientationDirection, m_VerticalReference);
				//state.Transform.basis.RotationQuat = lerpQuat;
			}
		}
		else if (validPosition)
		{
			m_LastValidPosition = state.Transform.origin;
			m_LastValidRotation = new Quat(state.Transform.basis.GetEuler());
		} */

		ActivateDriftVFX(IsDrifting && GroundPercent > 0.0f);
	}

	Vector3 QuatSlerp(Vector3 from, Vector3 to, float slerpRatio)
	{
		Quat fromQuat = new Quat(from);
		Quat toQuat = new Quat(to);
		fromQuat = fromQuat.Slerp(toQuat, slerpRatio);
		return fromQuat.GetEuler();
	}

	Vector3 FloorVec(Vector3 vector, int numDecimals)
	{
		Vector3 result = new Vector3(vector.x, vector.y, vector.z);

		result.x = ((int) (result.x * 10 * numDecimals)) / (numDecimals * 10.0f);
		result.y = ((int) (result.y * 10 * numDecimals)) / (numDecimals * 10.0f);
		result.z = ((int) (result.z * 10 * numDecimals)) / (numDecimals * 10.0f);

		return result;
	}
}
