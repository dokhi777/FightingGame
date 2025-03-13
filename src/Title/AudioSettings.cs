using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioSettings : MonoBehaviour
{
    public AudioMixer audioMixer; // AudioMixer 연결
    public Slider musicSlider;    // 음악 볼륨 슬라이더
    public Slider effectsSlider;  // 효과음 볼륨 슬라이더

    private void Start()
    {
        // 초기 슬라이더 값 설정 (디폴트 볼륨은 0dB)
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 0.75f);
        effectsSlider.value = PlayerPrefs.GetFloat("EffectsVolume", 0.75f);

        // AudioMixer 초기 값 반영
        SetMusicVolume(musicSlider.value);
        SetEffectsVolume(effectsSlider.value);
    }

    public void SetMusicVolume(float value)
    {
        if (value <= 0.01f) // 슬라이더가 최소값에 도달했을 때
        {
            audioMixer.SetFloat("MusicVolume", -80f); // 완전 무음
        }
        else
        {
            audioMixer.SetFloat("MusicVolume", Mathf.Log10(value) * 20); // 일반 볼륨 조절
        }

        PlayerPrefs.SetFloat("MusicVolume", value); // 슬라이더 값 저장
    }

    public void SetEffectsVolume(float value)
    {
        if (value <= 0.01f) // 슬라이더가 최소값에 도달했을 때
        {
            audioMixer.SetFloat("EffectsVolume", -80f); // 완전 무음
        }
        else
        {
            audioMixer.SetFloat("EffectsVolume", Mathf.Log10(value) * 20); // 일반 볼륨 조절
        }

        PlayerPrefs.SetFloat("EffectsVolume", value); // 슬라이더 값 저장
    }
}
