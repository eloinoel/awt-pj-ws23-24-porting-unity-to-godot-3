extends Node

var left_trail: Spatial
var right_trail: Spatial

var trailUptime: float = -1.0
var trailsActive: bool = false

# Called when the node enters the scene tree for the first time.
func _ready():
    var kart = get_parent()
    left_trail = kart.get_node_or_null("back_left_wheel/Trail3D")
    right_trail = kart.get_node_or_null("back_right_wheel/Trail3D")

    if left_trail != null:
        left_trail.visible = false
    if right_trail != null:
        right_trail.visible = false

    pass # Replace with function body.

func _process(delta):
    if trailsActive:
        if trailUptime > 0:
            trailUptime -= delta;
        else:
            disableLaserTrails()

func enableLaserTrails():
    if left_trail == null or right_trail == null:
        printerr("Lasertrails are null")
        return

    #left_trail.emit = true
    #right_trail.emit = true
    left_trail.visible = true
    right_trail.visible = true

    trailsActive = true
    trailUptime = 3.0
    pass

func disableLaserTrails():
    #left_trail.emit = false
    #right_trail.emit = false
    if left_trail == null or right_trail == null:
        printerr("Lasertrails are null")
        return
    left_trail.visible = false
    right_trail.visible = false
    trailsActive = false

