extends Node

onready var camera_pivot = $CameraPivot
onready var vehicle = get_parent()
onready var camera = $CameraPivot/Camera

export var isMainCamera : bool = false

var look_at_target: Vector3
# transform of how the camera is positioned as a default in the local coordinates of the car
var anchor: Transform

# Called when the node enters the scene tree for the first time.
func _ready():
    look_at_target = vehicle.global_transform.origin
    camera.current = isMainCamera

    # original Unity settings for camera
    camera.set_perspective(60, 0.1, 5000)

func _physics_process(delta):
    # detach camera from car in editor

    # cam follows car, smooth out any sudden movements
    camera_pivot.global_transform.origin = camera_pivot.global_transform.origin.linear_interpolate(vehicle.global_transform.origin, delta * 20)
    camera_pivot.global_transform.basis = camera_pivot.transform.basis.slerp(vehicle.transform.basis, delta * 5)

    # sway camera to where we are going with the car
    #DebugDrawingGD.draw_sphere(vehicle.global_transform.origin)
    var vehicle_position = vehicle.global_transform.origin
    #DebugDrawingGD.draw_line(vehicle_position, vehicle_position + vehicle.linear_velocity, Color(1,0,0,1), 0.1)
    look_at_target = look_at_target.linear_interpolate(vehicle.global_transform.origin, delta*5)
    #camera.look_at(look_at_target, camera.global_transform.basis.y)
    """ if(not _is_driving_backwards()):
        look_at_target = look_at_target.linear_interpolate(vehicle.global_transform.origin + vehicle.linear_velocity, delta*5)
        camera.look_at(look_at_target, camera.global_transform.basis.y)
        # in this case the camera sometimes feels a little bit unintuitive
    else:
        look_at_target = look_at_target.linear_interpolate(vehicle.global_transform.origin, delta*5)
        camera.look_at(look_at_target, camera.global_transform.basis.y) """



func _is_driving_backwards():
    return vehicle.linear_velocity.dot(vehicle.global_transform.basis.z) < 0
