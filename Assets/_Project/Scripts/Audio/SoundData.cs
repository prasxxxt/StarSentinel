using UnityEngine;

/// <summary>
/// Designer-friendly wrapper around a sound clip with playback parameters.
/// AudioManager looks these up by ID to play sound by name without touching
/// AudioClip references everywhere.
/// </summary>
[CreateAssetMenu(fileName = "SoundData", menuName = "StarSentinel/Sound Data")]
public class SoundData : ScriptableObject
{
    public string id;
    public AudioClip clip;
    [Range(0f, 1f)] public float volume = 1f;
    [Range(0.5f, 2f)] public float minPitch = 1f;
    [Range(0.5f, 2f)] public float maxPitch = 1f;
}
