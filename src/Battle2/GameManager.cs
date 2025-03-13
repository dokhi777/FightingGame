using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public RoundManager roundManager;
    public TextMeshProUGUI resultText;
    public TextMeshProUGUI roundText;
    public GameObject gameOverPanel;
    public TextMeshProUGUI gameOverText;

    private int playerWins = 0;
    private int aiWins = 0;
    private int currentRound = 0;
    private bool isGameOver = false;
    private bool isRoundProcessing = false; // 라운드 처리 중인지 확인

    private void Start()
    {
        InitializeRound();
    }

    private void Update()
    {
        if (isGameOver || isRoundProcessing) return; // 게임 종료 또는 라운드 처리 중이면 아무것도 하지 않음

        if (roundManager == null)
        {
            return;
        }

        if (roundManager.IsRoundOver())
        {
            string winner = roundManager.GetWinner();
            if (!string.IsNullOrEmpty(winner)) // null이 아닌 경우에만 처리
            {
                RoundResult(winner);
            }
            Debug.Log($"Wins Count: {playerWins}, {aiWins}");
        }
    }

    private void InitializeRound()
    {
        if (isGameOver)
        {
            return;
        }

        isRoundProcessing = false; // 라운드 처리 상태 초기화
        Debug.Log($"Initializing Round {currentRound + 1}");

        roundText.text = $"Round {currentRound + 1}";
        resultText.text = "";

        PlayerInitializer playerInitializer = FindObjectOfType<PlayerInitializer>();
        playerInitializer?.ResetCharacters(); // 플레이어와 AI 초기화

        roundManager.InitializeTimer(60f); // 타이머 초기화
        roundManager.ResetHpManagers(); // 체력 초기화

        currentRound++;
        Debug.Log("Round initialized.");
    }

    public void RoundResult(string winner)
    {
        if (isRoundProcessing) return; // 이미 라운드 처리 중인 경우 종료
        isRoundProcessing = true; // 라운드 처리 상태 설정

        if (winner == "Player")
        {
            playerWins++;
            Debug.Log($"Player Wins Count: {playerWins}");
            resultText.text = "Player Wins!";
        }
        else if (winner == "AI")
        {
            aiWins++;
            Debug.Log($"AI Wins Count: {aiWins}");
            resultText.text = "AI Wins!";
        }
        else
        {
            resultText.text = "Draw!";
        }

        // 게임 종료 조건 확인
        if (playerWins >= 2 || aiWins >= 2)
        {
            EndMatch(); // 게임 종료 처리
        }
        else
        {
            // 게임 종료가 아닌 경우에만 다음 라운드 초기화
            Invoke(nameof(InitializeRound), 7f);
        }
    }

    private void EndMatch()
    {
        if (isGameOver) return;

        isGameOver = true;
        Debug.Log("Game Over");

        string message = playerWins >= 2
            ? "Congratuation!\nYou Win!" // 승리 메시지
            : "That's too bad...\nYou lose!"; // 패배 메시지

        // 팝업창 활성화
        ActivateGameOverPanel(message);
    }

    private void ActivateGameOverPanel(string message)
    {
        gameOverPanel.SetActive(true); // 패널 활성화
        gameOverText.text = message; // 텍스트 설정
    }

    public void LoadTitleScene()
    {
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            if (obj.scene.name == "DontDestroyOnLoad")
            {
                Destroy(obj); // DontDestroyOnLoad에 있는 오브젝트 삭제
            }
        }
        SceneManager.LoadSceneAsync("Title", LoadSceneMode.Single); // Title 씬 로드
    }
}