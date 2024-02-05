extends Camera


# Declare member variables here. Examples:
# var a = 2
# var b = "text"
onready var camera_pivot = get_parent()
onready var vehicle = get_parent().get_parent().get_parent()

var look_at_target

# Called when the node enters the scene tree for the first time.
func _ready():
	look_at_target = vehicle.global_transform.origin
	self.set_perspective(60, 0.1, 5000)

func _physics_process(delta):
	# detach camera from car in editor

	# cam follows car, smooth out any sudden movements
	camera_pivot.global_transform.origin = camera_pivot.global_transform.origin.linear_interpolate(vehicle.global_transform.origin, delta * 20)
	camera_pivot.global_transform.basis = camera_pivot.transform.basis.slerp(vehicle.transform.basis, delta * 5)

	# sway camera to where we are going with the car
	if(not _is_driving_backwards()):
		look_at_target = look_at_target.linear_interpolate(vehicle.global_transform.origin + vehicle.linear_velocity, delta*5)
		self.look_at(look_at_target, self.global_transform.basis.y)
		# in this case the camera sometimes feels a little bit unintuitive
	else:
		look_at_target = look_at_target.linear_interpolate(vehicle.global_transform.origin, delta*5)
		self.look_at(look_at_target, self.global_transform.basis.y)



func _is_driving_backwards():
	return vehicle.linear_velocity.dot(vehicle.global_transform.basis.z) < 0
