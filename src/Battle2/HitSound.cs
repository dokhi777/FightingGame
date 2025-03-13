using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitSound : MonoBehaviour
{
    public AudioSource audioSource; // AudioSource 연결
    public AudioClip hitSound; // 버튼 클릭 효과음

    public void PlaySound()
    {
        audioSource.clip = hitSound; // AudioClip 설정
        audioSource.Play(); // AudioMixer에 반영된 상태로 재생
    }
}
