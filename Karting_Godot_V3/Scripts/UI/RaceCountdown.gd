extends Node

enum CountdownState {
    START,
    THREE,
    TWO,
    ONE,
    GO,
    FINISHED
}

# Preload the audio streams
var soundTick: AudioStreamSample = preload("res://Audio/SFX/StartSound01.wav")
var soundGo: AudioStreamSample = preload("res://Audio/SFX/StartSound02.wav")
var audioPlayer: AudioStreamPlayer

var countdownState = CountdownState.START
var countdownDuration = 1.0  # Total countdown duration in seconds
var resumeDuration = 1.0  # Duration to display "Go" after countdown

# called from GameFlowManager
func _on_trigger_race_countdown():
    $CountdownTimer.start(countdownDuration)  # Start the timer and don't autostart
    $CenterContainer/Three.visible = true
    $ObjectiveMessage.visible = true
    countdownState = CountdownState.THREE

func _on_CountdownTimer_timeout():
    audioPlayer = $AudioStreamPlayer
    audioPlayer.stream = soundTick
    match countdownState:
        CountdownState.THREE:
            audioPlayer.play()
            $CenterContainer/Three.visible = false
            $CenterContainer/Two.visible = true
            countdownState = CountdownState.TWO
        CountdownState.TWO:
            audioPlayer.play()
            $CenterContainer/Two.visible = false
            $CenterContainer/One.visible = true
            countdownState = CountdownState.ONE
        CountdownState.ONE:
            audioPlayer.play()
            $CenterContainer/One.visible = false
            $CenterContainer/GO.visible = true
            countdownState = CountdownState.GO
            audioPlayer.stream = soundGo
            audioPlayer.play()
            $CountdownTimer.stop()
            $CountdownTimer.start(resumeDuration)  # Start a timer for the "Go" duration
            #get_tree().paused = false
        CountdownState.GO:
            $CenterContainer/GO.visible = false
            #$GameGoal.visible = false
            $ObjectiveMessage.visible = false
            #$TextureRect.visible = false
            #$GameGoal.visible = false
            countdownState = CountdownState.FINISHED
            # Resume the game here or perform any other actions when the countdown finishes.
