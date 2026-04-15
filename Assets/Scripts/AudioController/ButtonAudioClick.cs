using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonAudioClick : MonoBehaviour
{
    [SerializeField] private AudioClip _audioClip;
    [SerializeField] private AudioSource _audioSource;

    public void PlayButtonClickSound()
    {
        _audioSource.PlayOneShot(_audioClip);
    }
}
