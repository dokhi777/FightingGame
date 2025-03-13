using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RoundManager : MonoBehaviour
{
    public TextMeshProUGUI timerText;              // Ÿ�̸Ӹ� ǥ���� UI �ؽ�Ʈ

    private HpManager playerHpManager;  // �÷��̾��� HpManager
    private HpManager aiHpManager;      // AI�� HpManager

    private float timeRemaining = 60f; // 60�� Ÿ�̸�
    private bool isRoundOver = false;

    private void Start()
    {
        // �������� ������ ĳ���� Ž��
        InvokeRepeating(nameof(FindHpManagers), 0f, 1f); // 1�� �������� HpManager�� Ž��
    }

    private void Update()
    {
        if (isRoundOver) return;

        timeRemaining -= Time.deltaTime;
        UpdateTimerUI();

        if (timeRemaining <= 0f || playerHpManager.IsDead() || aiHpManager.IsDead())
        {
            isRoundOver = true;
        }
    }

    private void FindHpManagers()
    {
        if (playerHpManager == null)
        {
            playerHpManager = FindHpManagerWithTag("Player");
        }

        if (aiHpManager == null)
        {
            aiHpManager = FindHpManagerWithTag("AI");
        }

        // HpManager�� ��� ã���� �ݺ� �ߴ�
        if (playerHpManager != null && aiHpManager != null)
        {
            CancelInvoke(nameof(FindHpManagers));
        }
    }

    private HpManager FindHpManagerWithTag(string tag)
    {
        GameObject obj = GameObject.FindGameObjectWithTag(tag);
        return obj != null ? obj.GetComponent<HpManager>() : null;
    }

    private void UpdateTimerUI()
    {
        if (timerText != null)
        {
            int seconds = Mathf.CeilToInt(timeRemaining); // ���� �ð��� ������ ��ȯ
            timerText.text = $"{seconds}";
        }
    }
    public bool IsRoundOver()
    {
        return isRoundOver;
    }

    public string GetWinner()
    {
        if (playerHpManager == null || aiHpManager == null)
        {
            Debug.LogError("HpManager is null, cannot determine winner.");
            return null;
        }

        Animator winnerAnimator = null;

        if (playerHpManager.IsDead())
        {
            winnerAnimator = aiHpManager.GetComponent<Animator>();
            winnerAnimator?.SetTrigger("Victory");
            return "AI";
        }
        if (aiHpManager.IsDead())
        {
            winnerAnimator = playerHpManager.GetComponent<Animator>();
            winnerAnimator?.SetTrigger("Victory");
            return "Player";
        }

        if (timeRemaining <= 0f)
        {
            float playerHp = playerHpManager.GetCurrentHp();
            float aiHp = aiHpManager.GetCurrentHp();

            if (playerHp > aiHp)
            {
                aiHpManager.TakeDamage(aiHpManager.GetCurrentHp()); // AI ü���� 0���� ����
                winnerAnimator = playerHpManager.GetComponent<Animator>();
                winnerAnimator?.SetTrigger("Victory");
                return "Player";
            }
            else if (aiHp > playerHp)
            {
                playerHpManager.TakeDamage(playerHpManager.GetCurrentHp()); // �÷��̾� ü���� 0���� ����
                winnerAnimator = aiHpManager.GetComponent<Animator>();
                winnerAnimator?.SetTrigger("Victory");
                return "AI";
            }
        }

        return "Draw";
    }
    public void InitializeTimer(float duration)
    {
        Debug.Log($"Timer initialized with duration: {duration}");
        timeRemaining = duration;
        isRoundOver = false;
    }

    public void ResetHpManagers()
    {
        if (playerHpManager != null)
        {
            playerHpManager.ResetHp();
        }

        if (aiHpManager != null)
        {
            aiHpManager.ResetHp();
        }
    }
}

