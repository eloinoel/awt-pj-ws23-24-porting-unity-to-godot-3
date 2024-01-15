extends Node

enum CountdownState {
	THREE,
	TWO,
	ONE,
	GO,
	FINISHED
}

# Preload the audio streams
var soundTick: AudioStreamSample = preload("res://Audio/SFX/StartSound01.wav")
var soundGo: AudioStreamSample = preload("res://Audio/SFX/StartSound02.wav")
var audioPlayer: AudioStreamPlayer2D

var countdownState = CountdownState.THREE
var countdownDuration = 1.0  # Total countdown duration in seconds
var resumeDuration = 2.0  # Duration to display "Go" after countdown

func _ready():
	#yield(get_tree().create_timer(0.1), "timeout")
	$CountdownTimer.start(countdownDuration)  # Start the timer and don't autostart
	$Three.visible = true
	$GameGoal.visible = true
	#get_tree().paused = true

func _on_CountdownTimer_timeout():
	audioPlayer = $AudioStreamPlayer2D
	audioPlayer.stream = soundTick
	match countdownState:
		CountdownState.THREE:
			audioPlayer.play()
			$Three.visible = false
			$Two.visible = true
			countdownState = CountdownState.TWO
			
		CountdownState.TWO:
			audioPlayer.play()
			$Two.visible = false
			$One.visible = true
			countdownState = CountdownState.ONE
		CountdownState.ONE:
			audioPlayer.play()
			$One.visible = false
			$Go.visible = true
			countdownState = CountdownState.GO
			audioPlayer.stream = soundGo
			audioPlayer.play()
			$CountdownTimer.stop()
			$CountdownTimer.start(resumeDuration)  # Start a timer for the "Go" duration
			#get_tree().paused = false
		CountdownState.GO:
			$Go.visible = false
			$GameGoal.visible = false
			$TextureRect.visible = false
			$GameGoal.visible = false
			countdownState = CountdownState.FINISHED
			# Resume the game here or perform any other actions when the countdown finishes.
			
