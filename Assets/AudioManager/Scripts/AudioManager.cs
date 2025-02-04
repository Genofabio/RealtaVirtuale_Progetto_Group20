using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;

    private AudioMixerSnapshot soundtrackSnapshot;
    private AudioMixerSnapshot environmentSnapshot;
    private AudioMixerSnapshot voiceSnapshot;

    private float transitionSpeed = 4.0f;

    void Start()
    {
        soundtrackSnapshot = audioMixer.FindSnapshot("Soundtrack");
        environmentSnapshot = audioMixer.FindSnapshot("Environment");
        voiceSnapshot = audioMixer.FindSnapshot("Audio");
    }

    public void SetSoundtrackSnapshot()
    {
        soundtrackSnapshot.TransitionTo(transitionSpeed);
    }

    public void SetEnvironmentSnapshot()
    {
        environmentSnapshot.TransitionTo(transitionSpeed);
    } 

    public void SetVoiceSnapshot()
    {
        voiceSnapshot.TransitionTo(transitionSpeed);
    }
}
