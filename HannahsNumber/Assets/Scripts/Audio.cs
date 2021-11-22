using UnityEngine;

public class Audio : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;

    [SerializeField] private AudioClip audioClip;

    public void PlayGun()
    {
        audioSource.clip = audioClip;
        audioSource.Play();
    }

    public void StopAudio()
    {
        audioSource.Stop();
    }

    public void SetVolume(float vol)
    {
        audioSource.volume = vol;
    }
}
