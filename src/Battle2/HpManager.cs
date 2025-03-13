using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HpManager : MonoBehaviour
{
    public Slider HpBar; // ü�¹� UI
    public float maxHp = 100f; // �ִ� ü��
    public HitSound hitSound;

    private float currentHp;
    private Animator animator;

    public void Initialize(Slider hpBarSlider, float maxHpValue)
    {
        HpBar = hpBarSlider;
        maxHp = maxHpValue;
        currentHp = maxHp;
        animator = GetComponent<Animator>();
        UpdateHpBar();
    }

    public void TakeDamage(float damage)
    {
        if (currentHp <= 0) return;

        currentHp -= damage;
        currentHp = Mathf.Clamp(currentHp, 0, maxHp);
        hitSound.PlaySound();
        UpdateHpBar();

        if (currentHp <= 0)
        {
            HandleDeath();
        }
    }

    public void ResetHp()
    {
        currentHp = maxHp;
        UpdateHpBar();
    }

    public bool IsDead()
    {
        return currentHp <= 0; // ���� ü���� 0 �����̸� ���
    }
    private void UpdateHpBar()
    {
        HpBar.value = currentHp / maxHp;
    }
    private void HandleDeath()
    {
        animator.SetTrigger("Dead");
        animator.SetBool("IsDead", true);
        Debug.Log($"Dead : {gameObject.name}");
    }
    public float GetCurrentHp()
    {
        return currentHp; // ���� ü���� ��ȯ
    }
}
