extends Node

onready var camera_pivot = $CameraPivot
onready var vehicle = get_parent()
onready var camera = $CameraPivot/Camera

export var isMainCamera : bool = false

var look_at_target: Vector3
# default position where the camera looks at
var target_anchor: Vector3
# for acceleration computation
var previous_velocity_z: float

var timer = 0 # debug

# Called when the node enters the scene tree for the first time.
func _ready():
    camera.current = isMainCamera
    target_anchor = _calculate_target_anchor()
    look_at_target = vehicle.to_global(target_anchor)
    previous_velocity_z = 0.0

    # original Unity settings for camera
    camera.set_perspective(60, 0.1, 5000)

func _physics_process(delta: float):
    timer+=delta
    # detach camera from car in editor by adding a simple normal node as parent

    # cam follows car, smooth out any sudden movements
    camera_pivot.global_transform.origin = camera_pivot.global_transform.origin.linear_interpolate(vehicle.global_transform.origin, delta * 20)
    #camera_pivot.global_transform.basis = camera_pivot.global_transform.basis.slerp(vehicle.global_transform.basis, delta * 5)
    var y_rotation = vehicle.transform.basis.get_euler().y
    var y_rot_pivot = camera_pivot.transform.basis.get_euler().y
    camera_pivot.transform.basis = camera_pivot.transform.basis.rotated(camera_pivot.transform.basis.y, y_rotation - y_rot_pivot)
    #camera_pivot.rotate_y(deg2rad(- y_rot_pivot + y_rotation))
    #var slerped_basis = camera_pivot.transform.basis.slerp(vehicle.transform.basis, delta * 5)
    #camera_pivot.transform.basis.y = slerped_basis.y


    # DebugDrawingGD.draw_sphere(camera_pivot.global_transform.origin, 0.5, Color(0, 0, 1, 1))


    #compute acceleration
    var localVelocity = vehicle.transform.basis.xform_inv(vehicle.linear_velocity)
    var acceleration = _get_acceleration(localVelocity.z, delta)
    previous_velocity_z = localVelocity.z

    # sway the camera to where we are currently d
    var local_target: Vector3
    # TODO: camera acceleration is slow when accelerating
    if localVelocity.z < 0:
        local_target = Vector3(target_anchor.x, target_anchor.y, target_anchor.z)
    else:
        # determine the ratio between the current z speed and the maximum possible z speed (We get a value in the range [0,1], 0: current_speed == 0; 1: current_speed == max_speed)
        # we then use pass this value through an easing function and multiply its result with the size of the desired range [0, max_speed]
        # this is then used as an offset to the position the camera is looking at
        var vehicle_max_local_speed = 15.0
        var percent_of_offset_range = localVelocity.z / vehicle_max_local_speed;
        local_target = Vector3(target_anchor.x, target_anchor.y, target_anchor.z + _ease_in_sine(percent_of_offset_range) * vehicle_max_local_speed)

    # simple implementation without easing when slowing down
    #local_target = Vector3(target_anchor.x, target_anchor.y, max(target_anchor.z, localVelocity.z + target_anchor.z))
    look_at_target = look_at_target.linear_interpolate(vehicle.to_global(local_target), delta * 30)

    # TODO: remove debug
    """ if(timer >= 0.5):
        #print(localVelocity.z)
        #print(target_anchor.z)
        #print("final: " + str(local_target.z))
        #print(percent_of_max_speed)
        #print(_ease_in_sine(percent_of_max_speed))
        #print("----------")
        timer = 0 """
    #DebugDrawingGD.draw_sphere(look_at_target, 0.5, Color(0, 0, 1, 1))
    #DebugDrawingGD.draw_line(vehicle_position, vehicle_position + camera.global_transform.basis.y)
    #camera.look_at(look_at_target, Vector3.UP)

func _is_driving_backwards():
    return vehicle.linear_velocity.dot(vehicle.global_transform.basis.z) < 0

# computes the starting point the camera is looking at in global coordinates
func _calculate_target_anchor():
    var camera_pivot_position = camera_pivot.transform.origin

    # calculate z offset depending on initial camera position and rotation relative to camera pivot
    var cam_z_offset = camera.transform.origin.z
    var cam_y_offset = camera.transform.origin.y
    var rotation_angle= camera.transform.basis.get_euler().x

    # tanh(angle) = GK / AK with GK = cam_z_offset + z_offset
    var gk = tan(deg2rad(rotation_angle)) * cam_y_offset
    var z_offset = gk - cam_z_offset
    var target = Vector3(camera_pivot_position.x, camera_pivot_position.y, camera_pivot_position.z + z_offset)

    return target

func _ease_in_sine(x: float):
    return 1 - cos((x * PI) / 2)

func _ease_in_quad(x: float):
    return x * x

func _ease_in_out_quart(x: float):
    return 1 - cos((x * PI) / 2)

func _get_acceleration(currentVelocity_z: float , deltaTime: float):
    return (currentVelocity_z - previous_velocity_z) / deltaTime

func _is_accelerating(acceleration: float):
    return acceleration > 0
