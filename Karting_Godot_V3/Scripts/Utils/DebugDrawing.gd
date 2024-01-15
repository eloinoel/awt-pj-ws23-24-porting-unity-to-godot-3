extends Node

func draw_sphere(position: Vector3, radius: float = 0.1, color: Color = Color(1, 0, 0, 1), duration: float = 20):
	"""
	instantiate MeshInstance3D with a SphereMesh at a specified target position
	"""
	var mesh_instance = _setup_sphere_mesh(position, radius, color)

	get_tree().get_root().add_child(mesh_instance)

	_free_mesh_after_delay(mesh_instance, duration)

func draw_line(pos1: Vector3, pos2: Vector3, color: Color = Color(1, 0, 0, 1), duration: float = 20):
	#var mesh_instance = MeshInstance.new()
	var immediate_geometry = ImmediateGeometry.new()
	var material = SpatialMaterial.new()

	material.albedo_color = color

	immediate_geometry.begin(Mesh.PRIMITIVE_LINES, material)
	immediate_geometry.add_vertex(pos1)
	immediate_geometry.add_vertex(pos2)
	immediate_geometry.add_vertex(Vector3(pos1.x + 0.001, pos1.y, pos1.z))
	immediate_geometry.add_vertex(Vector3(pos2.x + 0.001, pos2.y, pos2.z))
	immediate_geometry.end()

	#mesh_instance.mesh = immediate_geometry
	#mesh_instance.cast_shadow = false

	#get_tree().get_root().add_child(mesh_instance)
	get_tree().get_root().add_child(immediate_geometry)

	#_free_mesh_after_delay(mesh_instance, duration)
	_free_mesh_after_delay(immediate_geometry, duration)

# ----------------------------
# ---- private functions -----
# ----------------------------


func _setup_sphere_mesh(position: Vector3, radius: float = 0.1, color: Color = Color(1, 0, 0, 1)):
	var mesh_instance = MeshInstance.new()
	mesh_instance.transform.origin = position
	mesh_instance.cast_shadow = false

	# setup mesh for the MeshInstance node
	var mesh = SphereMesh.new()
	mesh.radius = radius
	mesh.height = radius * 2
	mesh_instance.mesh = mesh

	var material = SpatialMaterial.new()
	material.albedo_color = color
	mesh_instance.material_override = material

	return mesh_instance

func _free_mesh_after_delay(mesh_instance: MeshInstance, delay: float):
	yield(get_tree().create_timer(delay), "timeout")
	if(is_instance_valid(mesh_instance)):
		mesh_instance.queue_free()
