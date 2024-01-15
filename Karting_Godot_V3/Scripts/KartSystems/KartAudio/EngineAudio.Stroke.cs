using System;
using Godot;

namespace KartGame.KartSystems
{
    public partial class EngineAudio
    {
        /// <summary>
        /// Represents audio data for a single stroke of an engine (2 strokes per revolution)
        /// </summary>
        /* [System.Serializable] */ public struct Stroke
        {
            public AudioStreamOGGVorbis clip;
            [Export(PropertyHint.Range, "0.0f, 1.0f")]
            public float gain;
            internal float[] buffer;
            internal int position;

            internal void Reset () => position = 0;

            internal float Sample ()
            {
                if (position < buffer.Length)
                {
                    var s = buffer[position];
                    position++;
                    return s * gain;
                }

                return 0;
            }

            internal void Init ()
            {
                //if no clip is available use a noisy sine wave as a place holder.
                //else initialise buffer of samples from clip data.
                if (clip == null)
                {
                    buffer = new float[4096];
                    for (var i = 0; i < buffer.Length; i++)
                        buffer[i] = Mathf.Sin (i * (1f / 44100) * 440) + (float) GD.RandRange(-1.0f, 1.0f) * 0.05f;
                }
                else
                {
                    buffer = new float[clip.Data.Length];
                    //PREV: clip.GetData (buffer, 0);
                    for (var i = 0; i < clip.Data.Length; i++)
                        buffer[i] = clip.Data[i];
                }
            }
        }
    }
}