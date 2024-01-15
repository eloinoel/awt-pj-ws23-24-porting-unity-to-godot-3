using System;
using System.Collections;
using Godot;

public class AudioManager
{
	public AudioStreamPlayer audioMixer;

	public void EnsureSFXDestruction(AudioStream source)
	{
		// StartCoroutine("DelayedSFXDestruction", source);
	}

	private IEnumerator DelayedSFXDestruction(AudioStream source)
	{
/*         while (source.isPlaying)
		{
			yield return null;
		}

		GameObject.Destroy(source.gameObject); */
		return null;
	}
}
