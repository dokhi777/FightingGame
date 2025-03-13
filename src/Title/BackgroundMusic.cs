using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMusic : MonoBehaviour
{
    public AudioSource audioSource; // AudioSource ����
    public AudioClip backgroundMusic; // ������� Ŭ��

    private void Start()
    {
        audioSource.clip = backgroundMusic;
        audioSource.loop = true; // �ݺ� ���
        audioSource.Play(); // ������� ���
    }
}
