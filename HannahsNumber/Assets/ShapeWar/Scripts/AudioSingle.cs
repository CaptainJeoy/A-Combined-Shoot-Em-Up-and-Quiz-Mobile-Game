using UnityEngine;

public class AudioSingle : Singleton<AudioSingle>
{
    [SerializeField] private AudioSource audioSource;

    [SerializeField] private AudioClip audioClip;

    public void Play()
    {
        audioSource.clip = audioClip;
        audioSource.Play();
    }

    public void StopAudio()
    {
        audioSource.Stop();
    }
}
