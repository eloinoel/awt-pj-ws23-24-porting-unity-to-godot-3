using System;
using System.Collections;
using Godot;

public class AudioManager
{
    public AudioStreamPlayer audioMixer;

    public void EnsureSFXDestruction(AudioStream source)
    {
        StartCoroutine("DelayedSFXDestruction", source);
    }

    private IEnumerator DelayedSFXDestruction(AudioSource source)
    {
        while (source.isPlaying)
        {
            yield return null;
        }

        GameObject.Destroy(source.gameObject);
    }
}
