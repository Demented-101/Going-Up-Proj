using System;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicTrack : MonoBehaviour
{
    public bool isSelected = false;
    private AudioSource musicSource;

    [SerializeField] private AudioClip musicClip;
    [SerializeField] private float maxVolume = 1.0f;

    private const float volUpSpeed = 0.2f;
    private const float volDownSpeed = 0.35f;

    private void Start()
    {
        musicSource = GetComponent<AudioSource>();
        musicSource.clip = musicClip;
    }

    public void Select()
    {
        isSelected = true;
        musicSource.Play();
    }

    public void Stop()
    {
        isSelected = false;
    }

    private void Update()
    {
        if (isSelected)
        {
            musicSource.volume += volUpSpeed * maxVolume * Time.deltaTime;
            if (musicSource.volume >= maxVolume) { musicSource.volume = maxVolume; } 
        }
        else
        {
            if (musicSource.volume <= 0) { musicSource.Stop(); return; }
            musicSource.volume -= volDownSpeed * maxVolume * Time.deltaTime;
        }
    }
}
