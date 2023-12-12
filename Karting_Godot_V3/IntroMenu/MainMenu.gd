extends Node2D


# Declare member variables here. Examples:
# var a = 2
# var b = "text"


# Called when the node enters the scene tree for the first time.
func _ready():
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass
#	var texture = $Viewport.get_texture()
#	$Screen.texture = texture


func _on_PlayButton_pressed():
	print('play')
	get_tree().change_scene("res://RaceScene.tscn")

func _on_MenuButton_pressed():
	print('Menu')
	get_tree().change_scene("res://IntroMenu/MainMenu.tscn")
