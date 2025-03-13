using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMusic : MonoBehaviour
{
    public AudioSource audioSource; // AudioSource 연결
    public AudioClip backgroundMusic; // 배경음악 클립

    private void Start()
    {
        audioSource.clip = backgroundMusic;
        audioSource.loop = true; // 반복 재생
        audioSource.Play(); // 배경음악 재생
    }
}
