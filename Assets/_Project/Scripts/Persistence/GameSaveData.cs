using System;

/// <summary>
/// Plain serialisable container for everything that persists between
/// play sessions. Field names map 1:1 to the JSON keys, so renaming
/// here invalidates existing save files.
/// </summary>
[Serializable]
public class GameSaveData
{
    public int highScore;
    public int bestWave;
    public float masterVolume = 1f;
}