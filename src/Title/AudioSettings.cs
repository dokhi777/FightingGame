using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioSettings : MonoBehaviour
{
    public AudioMixer audioMixer; // AudioMixer ����
    public Slider musicSlider;    // ���� ���� �����̴�
    public Slider effectsSlider;  // ȿ���� ���� �����̴�

    private void Start()
    {
        // �ʱ� �����̴� �� ���� (����Ʈ ������ 0dB)
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 0.75f);
        effectsSlider.value = PlayerPrefs.GetFloat("EffectsVolume", 0.75f);

        // AudioMixer �ʱ� �� �ݿ�
        SetMusicVolume(musicSlider.value);
        SetEffectsVolume(effectsSlider.value);
    }

    public void SetMusicVolume(float value)
    {
        if (value <= 0.01f) // �����̴��� �ּҰ��� �������� ��
        {
            audioMixer.SetFloat("MusicVolume", -80f); // ���� ����
        }
        else
        {
            audioMixer.SetFloat("MusicVolume", Mathf.Log10(value) * 20); // �Ϲ� ���� ����
        }

        PlayerPrefs.SetFloat("MusicVolume", value); // �����̴� �� ����
    }

    public void SetEffectsVolume(float value)
    {
        if (value <= 0.01f) // �����̴��� �ּҰ��� �������� ��
        {
            audioMixer.SetFloat("EffectsVolume", -80f); // ���� ����
        }
        else
        {
            audioMixer.SetFloat("EffectsVolume", Mathf.Log10(value) * 20); // �Ϲ� ���� ����
        }

        PlayerPrefs.SetFloat("EffectsVolume", value); // �����̴� �� ����
    }
}
