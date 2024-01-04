extends VehicleBody

var max_rpm = 1200
var max_torque = 200

func _physics_process(delta):
	steering = lerp(steering, Input.get_axis("right","left") * 0.5, 10 * delta)
	var acceleration = Input.get_axis("backward","forward") * 12
	var rpm = $back_left_wheel.get_rpm()
	$back_left_wheel.engine_force = acceleration * max_torque * ( 1 - rpm / max_rpm)
	rpm = $back_right_wheel.get_rpm()
	$back_right_wheel.engine_force = acceleration * max_torque * ( 1 - rpm / max_rpm)