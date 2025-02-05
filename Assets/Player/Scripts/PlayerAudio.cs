using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerAudio", menuName = "Scriptable Objects/PlayerAudio")]
public class PlayerAudio : ScriptableObject
{
    [System.Serializable]
    public class AudioEntry
    {
        public PlayerAudioKey key;
        public AudioClip clip;
    }

    public List<AudioEntry> audioEntries = new List<AudioEntry>();

    public AudioClip GetAudioClip(PlayerAudioKey key)
    {
        foreach (var entry in audioEntries)
        {
            if (entry.key == key)
                return entry.clip;
        }
        return null; 
    }
}

public enum PlayerAudioKey
{
    PickUp,
    Drop
}