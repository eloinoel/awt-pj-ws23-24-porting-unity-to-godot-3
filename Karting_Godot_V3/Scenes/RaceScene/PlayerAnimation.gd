extends Spatial

var rotation_speed = 90.0
var tilt_speed = 900.0

var max_rotation_degrees = 15.0
var max_tilt_degrees = 100.0

var target_rotation_degrees = 0.0
var target_tilt_degrees = 0.0

func _process(delta):
    if Input.is_action_pressed('left'): 
        target_rotation_degrees = min(max_rotation_degrees, target_rotation_degrees + rotation_speed * delta)
        target_tilt_degrees = min(max_tilt_degrees, tilt_speed * delta)
    elif Input.is_action_pressed('right'):
        target_rotation_degrees = max(-max_rotation_degrees, target_rotation_degrees - rotation_speed * delta)
        target_tilt_degrees = max(-max_tilt_degrees, -tilt_speed * delta) 
    else:

        target_rotation_degrees = lerp(target_rotation_degrees, 0, 0.1)
        target_tilt_degrees = lerp(target_tilt_degrees, 0, 0.1)

    rotation.y = lerp(rotation.y, deg2rad(target_rotation_degrees), 0.1)
    rotation.z = lerp(rotation.z, deg2rad(-target_tilt_degrees), 0.1)
