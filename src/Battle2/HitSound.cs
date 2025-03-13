using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitSound : MonoBehaviour
{
    public AudioSource audioSource; // AudioSource ����
    public AudioClip hitSound; // ��ư Ŭ�� ȿ����

    public void PlaySound()
    {
        audioSource.clip = hitSound; // AudioClip ����
        audioSource.Play(); // AudioMixer�� �ݿ��� ���·� ���
    }
}
