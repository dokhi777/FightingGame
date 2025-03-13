using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpSound : MonoBehaviour
{
    public AudioSource audioSource; // 스테미나 사운드 오디오 소스
    public AudioClip spSound; // 스테미나 회복 효과음

    public void PlaySound()
    {
        audioSource.clip = spSound;
        audioSource.Play(); // 스테미나 효과음 재생
    }
}