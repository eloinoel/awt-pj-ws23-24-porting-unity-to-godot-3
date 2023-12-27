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
	top_speed: float = 10,
	acceleration: float = 5,
	reverse_speed: float = 5,
	reverse_acceleration: float = 5,
	acceleration_curve: float = 4,
	braking_deceleration: float = 10,
	coasting_drag: float = 4,
	grip: float = 0.95,
	steer: float = 5,
	added_gravity: float = 1
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
var final_stats = base_stats.duplicate(true)

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
func get_max_speed(): return max(m.final_stats.top_speed, m.final_stats.reverse_speed)

""" Resolve: private void ActivateDriftVFX(bool active) { ... } """
""" Resolve: private void UpdateDriftVFXOrientation() { ... } """
""" Resolve: void UpdateSuspensionParams(WheelCollider wheel) { ... } """
# TODO: I left of here, last time (line 236 in the unity script)

func _ready():
	return {}

func _integrate_forces(state):
	# TODO:
	return {}
