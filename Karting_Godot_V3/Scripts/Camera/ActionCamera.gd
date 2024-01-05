extends Node

onready var camera_pivot = $CameraPivot
onready var vehicle = get_parent()
onready var camera = $CameraPivot/Camera

export var isMainCamera : bool = false

var look_at_target: Vector3
# default position where the camera looks at
var target_anchor: Vector3
var timer = 0 # debug

# Called when the node enters the scene tree for the first time.
func _ready():
    camera.current = isMainCamera
    target_anchor = _calculate_target_anchor()
    look_at_target = vehicle.to_global(target_anchor)

    # original Unity settings for camera
    camera.set_perspective(60, 0.1, 5000)

func _physics_process(delta):
    timer+=delta
    # detach camera from car in editor by adding a simple normal node as parent

    # cam follows car, smooth out any sudden movements
    camera_pivot.global_transform.origin = camera_pivot.global_transform.origin.linear_interpolate(vehicle.global_transform.origin, delta * 20)
    camera_pivot.global_transform.basis = camera_pivot.transform.basis.slerp(vehicle.transform.basis, delta * 5)


    # sway camera to where we are going with the car
    #DebugDrawingGD.draw_sphere(look_at_target)
    var vehicle_position = vehicle.global_transform.origin
    #DebugDrawingGD.draw_line(vehicle_position, vehicle_position + vehicle.linear_velocity, Color(1,0,0,1), 0.1)
    #look_at_target = look_at_target.linear_interpolate(vehicle.global_transform.origin + vehicle.linear_velocity, delta*5)
    #DebugDrawingGD.draw_sphere(look_at_target)
    #DebugDrawingGD.draw_line(vehicle_position, vehicle_position + camera.global_transform.basis.y)
    var localVelocity = vehicle.transform.basis.xform_inv(vehicle.linear_velocity)
    var local_target = Vector3(target_anchor.x, target_anchor.y, max(target_anchor.z, localVelocity.z))
    look_at_target = look_at_target.linear_interpolate(vehicle.to_global(local_target), delta * 30)

    #look_at_target = vehicle.to_global(local_target)

    """ if(timer >= 0.5):
        print(localVelocity.z)
        print(target_anchor.z)
        print("final: " + str(local_target.z))
        print("----------")
        timer = 0 """
    DebugDrawingGD.draw_sphere(look_at_target, 0.5, Color(0, 0, 1, 1))
    camera.look_at(look_at_target, Vector3.UP)

    """ if(not _is_driving_backwards()):
        look_at_target = look_at_target.linear_interpolate(vehicle_position + vehicle.linear_velocity, delta*5)
        camera.look_at(look_at_target, Vector3.UP)
        # camera.look_at(look_at_target, camera.global_transform.basis.y)
        # in this case the camera sometimes feels a little bit unintuitive
    else:
        look_at_target = look_at_target.linear_interpolate(vehicle_position, delta*5)
        camera.look_at(look_at_target, Vector3.UP)
        #camera.look_at(look_at_target, camera.global_transform.basis.y) """



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
