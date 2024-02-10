extends Node2D

# Declare member variables here. Examples:
# var a = 2
# var b = "text"
export var race_scene_path := "res://Scenes/RaceScene/RaceScene.tscn" setget set_path

func set_path(new_path : String) -> void:
	race_scene_path = new_path
	

# Called when the node enters the scene tree for the first time.
func _ready():
	$Control2/ControlsImage.visible = false


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass
#	var texture = $Viewport.get_texture()
#	$Screen.texture = texture


func _on_PlayButton_pressed():
	get_tree().change_scene(race_scene_path)

func _on_MenuButton_pressed():
	get_tree().change_scene("res://Scenes/Menues/IntroMenu/MainMenu.tscn")


func _on_ControlButton_pressed():
	$Control2/ControlsImage.visible = true

func _on_CloseControls_pressed():
	$Control2/ControlsImage.visible = false
