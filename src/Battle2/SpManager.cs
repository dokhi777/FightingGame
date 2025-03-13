using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpManager : MonoBehaviour
{
    public Slider SpBar; // 스태미나 UI Slider
    public float maxSp = 100; // 최대 스태미나

    private float currentSp;
    public SpSound spSound;

    public void Initialize(Slider spBarSlider, float minSpValue)
    {
        SpBar = spBarSlider;
        currentSp = 0;
        UpdateSpBar();
    }

    public void RecoverSp(float amount)
    {
        currentSp += amount;
        currentSp = Mathf.Clamp(currentSp, 0, maxSp);
        spSound.PlaySound();
        UpdateSpBar();
    }

    public bool IsStaminaFull()
    {
        return currentSp >= maxSp;
    }

    public void ResetStamina()
    {
        currentSp = 0;
        UpdateSpBar();
    }

    private void UpdateSpBar()
    {
        SpBar.value = currentSp / maxSp;
    }
}