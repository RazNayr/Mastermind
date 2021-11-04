using System;
using UnityEngine;
using Random = UnityEngine.Random;


[CreateAssetMenu(menuName ="Audio Events/Simple")]
public class SimpleAudioEvent : AudioEvent
{
    public AudioClip clip;

    public RangedFloat volume;

    [MinMaxRange(0, 2)]
    public RangedFloat pitch;

    public override void Play(AudioSource source)
    {
        if (clip == null) return;

        source.clip = clip;
        source.volume = Random.Range(volume.minValue, volume.maxValue);
        source.pitch = Random.Range(pitch.minValue, pitch.maxValue);
        source.Play();
    }
}
