extends RigidBody2D

""" Resolve: public class StatPowerup
{
	public ArcadeKart.Stats modifiers;
	public string PowerUpID;
	public float ElapsedTime;
	public float MaxTime;
} """

""" Resolve: public Rigidbody Rigidbody { get; private set; }
public InputData Input { get; private set; }
public float AirPercent { get; private set; }
public float GroundPercent { get; private set; } """

# TODO: Export stats into godot editor, so that they can be changed there as properties of this script
# TODO: Depending on its merits, wrap all Kart properties within a class (like it was done in the unity script)
var base_stats = {
	top_speed = 10,
	acceleration = 5,
	reverse_speed = 5,
	reverse_acceleration = 5,
	acceleration_curve = 4,
	braking_deceleration = 10,
	coasting_drag = 4,
	grip = 0.95,
	steer = 5,
	added_gravity = 1
}

# Vehicle Visual
""" Resolve: public List<GameObject> m_VisualWheels; """

# Vehicle Physics
""" Resolve: public Transform CenterOfMass; """

var airborne_reorientation_coefficient: float = 3

# Drifting stats
var drift_grip: float = 0.4
var drift_additional_steer: float = 5
var min_angle_to_finish_drift: float = 10
var min_speed_percent_to_finish_drift: float = 0.5
var drift_control: float = 10
var drift_dampening: float = 10

# VFX
""" Resolve: public ParticleSystem DriftSparkVFX;
public float DriftSparkHorizontalOffset = 0.1f;
public float DriftSparkRotation = 17.0f;
public GameObject DriftTrailPrefab;
public float DriftTrailVerticalOffset;
public GameObject JumpVFX;
public GameObject NozzleVFX;
public List<Transform> Nozzles; """

# Suspensions
var suspension_height: float = 0.2
var suspension_spring: float = 20000
var suspension_damp: float = 500
var wheels_position_vertical_offset: float = 0

# Physical Wheels
""" Resolve: public WheelCollider FrontLeftWheel;
public WheelCollider FrontRightWheel;
public WheelCollider RearLeftWheel;
public WheelCollider RearRightWheel;

public LayerMask GroundLayers = Physics.DefaultRaycastLayers; """

# Input sources that can control the Kart
""" Resolve: IInput[] m_Inputs;

const float k_NullInput = 0.01f;
const float k_NullSpeed = 0.01f;
Vector3 m_VerticalReference = Vector3.up; """

# Drift parameters
var wants_to_drift: bool = false
var is_drifting: bool = false
var m_current_grip: float = 1
var m_drift_turning_power: float = 0
var m_previous_ground_percent = 1
""" Resolve: readonly List<(GameObject trailRoot, WheelCollider wheel, TrailRenderer trail)> m_DriftTrailInstances = new List<(GameObject, WheelCollider, TrailRenderer)>();
readonly List<(WheelCollider wheel, float horizontalOffset, float rotation, ParticleSystem sparks)> m_DriftSparkInstances = new List<(WheelCollider, float, float, ParticleSystem)>(); """

# can the Kart move?
var m_can_move: bool = true
""" Resolve: List<StatPowerup> m_ActivePowerupList = new List<StatPowerup>(); """
var m_final_stats = base_stats.duplicate(true)

""" Resolve: Quaternion m_LastValidRotation;
Vector3 m_LastValidPosition;
Vector3 m_LastCollisionNormal;
bool m_HasCollision;
bool m_InAir = false; """

# TODO: implement method for adding stats (used for powerups)
func add_stats():
	return {}

# TODO: implement method for adding a new powerup into the active powerup list
func add_powerup():
	return {}

func set_can_move(move: bool): m_can_move = move
func get_max_speed(): return max(m_final_stats.top_speed, m_final_stats.reverse_speed)

""" Resolve: private void ActivateDriftVFX(bool active) { ... } """
""" Resolve: private void UpdateDriftVFXOrientation() { ... } """
""" Resolve: void UpdateSuspensionParams(WheelCollider wheel) { ... } """

func _ready():
	""" Resolve: Rigidbody = GetComponent<Rigidbody>();
	m_Inputs = GetComponents<IInput>();

	UpdateSuspensionParams(FrontLeftWheel);
	UpdateSuspensionParams(FrontRightWheel);
	UpdateSuspensionParams(RearLeftWheel);
	UpdateSuspensionParams(RearRightWheel);

	m_CurrentGrip = baseStats.Grip;

	if (DriftSparkVFX != null)
	{
		AddSparkToWheel(RearLeftWheel, -DriftSparkHorizontalOffset, -DriftSparkRotation);
		AddSparkToWheel(RearRightWheel, DriftSparkHorizontalOffset, DriftSparkRotation);
	}

	if (DriftTrailPrefab != null)
	{
		AddTrailToWheel(RearLeftWheel);
		AddTrailToWheel(RearRightWheel);
	}

	if (NozzleVFX != null)
	{
		foreach (var nozzle in Nozzles)
		{
			Instantiate(NozzleVFX, nozzle, false);
		}
	} """
	return {}

	""" Resolve: void AddTrailToWheel(WheelCollider wheel) { ... } """

	""" Resolve: void AddSparkToWheel(WheelCollider wheel, float horiz) { ... } """

func _integrate_forces(state):
	""" Resolve: UpdateSuspensionParams(FrontLeftWheel);
	UpdateSuspensionParams(FrontRightWheel);
	UpdateSuspensionParams(RearLeftWheel);
	UpdateSuspensionParams(RearRightWheel);

	// apply our powerups to create our finalStats
	TickPowerups(); """

	""" Resolve: // apply our physics properties
	Rigidbody.centerOfMass = transform.InverseTransformPoint(CenterOfMass.position);

	int groundedCount = 0;
	if (FrontLeftWheel.isGrounded && FrontLeftWheel.GetGroundHit(out WheelHit hit))
		groundedCount++;
	if (FrontRightWheel.isGrounded && FrontRightWheel.GetGroundHit(out hit))
		groundedCount++;
	if (RearLeftWheel.isGrounded && RearLeftWheel.GetGroundHit(out hit))
		groundedCount++;
	if (RearRightWheel.isGrounded && RearRightWheel.GetGroundHit(out hit))
		groundedCount++;

	// calculate how grounded and airborne we are
	GroundPercent = (float) groundedCount / 4.0f;
	AirPercent = 1 - GroundPercent;

	// apply vehicle physics
	if (m_CanMove)
	{
		# TODO: Implement MoveVehicle
		# previously was MoveVehicle(Input.accelerate, Input.Brake, Input.TurnInput)
		# TODO: Ensure, that Godots & Unitys Inputs are scaled/parametrized the same way
		MoveVehicle(Input.get_axis("backward","forward"), Input.get_axis("right","left"));
	}
	GroundAirbourne();

	m_PreviousGroundPercent = GroundPercent; """

	""" Resolve: UpdateDriftVFXOrientation(); """
