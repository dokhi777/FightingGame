using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RoundManager : MonoBehaviour
{
    public TextMeshProUGUI timerText;              // 타이머를 표시할 UI 텍스트

    private HpManager playerHpManager;  // 플레이어의 HpManager
    private HpManager aiHpManager;      // AI의 HpManager

    private float timeRemaining = 60f; // 60초 타이머
    private bool isRoundOver = false;

    private void Start()
    {
        // 동적으로 생성된 캐릭터 탐지
        InvokeRepeating(nameof(FindHpManagers), 0f, 1f); // 1초 간격으로 HpManager를 탐색
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

        // HpManager를 모두 찾으면 반복 중단
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
            int seconds = Mathf.CeilToInt(timeRemaining); // 남은 시간을 정수로 변환
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
                aiHpManager.TakeDamage(aiHpManager.GetCurrentHp()); // AI 체력을 0으로 설정
                winnerAnimator = playerHpManager.GetComponent<Animator>();
                winnerAnimator?.SetTrigger("Victory");
                return "Player";
            }
            else if (aiHp > playerHp)
            {
                playerHpManager.TakeDamage(playerHpManager.GetCurrentHp()); // 플레이어 체력을 0으로 설정
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

