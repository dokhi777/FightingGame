using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpSound : MonoBehaviour
{
    public AudioSource audioSource; // ���׹̳� ���� ����� �ҽ�
    public AudioClip spSound; // ���׹̳� ȸ�� ȿ����

    public void PlaySound()
    {
        audioSource.clip = spSound;
        audioSource.Play(); // ���׹̳� ȿ���� ���
    }
}