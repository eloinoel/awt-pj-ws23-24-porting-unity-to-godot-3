extends Node2D

var is_paused = false setget set_is_paused

func _ready():
	is_paused = false
	visible = false


func _unhandled_input(event):
	if event.is_action_pressed("pause"):
		self.is_paused = !is_paused


func set_is_paused(value):
	is_paused = value
	get_tree().paused = is_paused
	visible = is_paused


func _on_Resume_pressed():
	self.is_paused = false


func _on_Controls_pressed():
	pass # Replace with function body.
