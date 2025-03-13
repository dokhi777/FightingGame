using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonSound : MonoBehaviour
{
    public AudioSource audioSource; // AudioSource ����
    public AudioClip buttonClickSound; // ��ư Ŭ�� ȿ����

    public void PlaySound()
    {
        audioSource.clip = buttonClickSound; // AudioClip ����
        audioSource.Play(); // AudioMixer�� �ݿ��� ���·� ���
    }
}

