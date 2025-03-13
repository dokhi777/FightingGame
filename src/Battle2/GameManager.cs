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
    private bool isRoundProcessing = false; // ���� ó�� ������ Ȯ��

    private void Start()
    {
        InitializeRound();
    }

    private void Update()
    {
        if (isGameOver || isRoundProcessing) return; // ���� ���� �Ǵ� ���� ó�� ���̸� �ƹ��͵� ���� ����

        if (roundManager == null)
        {
            return;
        }

        if (roundManager.IsRoundOver())
        {
            string winner = roundManager.GetWinner();
            if (!string.IsNullOrEmpty(winner)) // null�� �ƴ� ��쿡�� ó��
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

        isRoundProcessing = false; // ���� ó�� ���� �ʱ�ȭ
        Debug.Log($"Initializing Round {currentRound + 1}");

        roundText.text = $"Round {currentRound + 1}";
        resultText.text = "";

        PlayerInitializer playerInitializer = FindObjectOfType<PlayerInitializer>();
        playerInitializer?.ResetCharacters(); // �÷��̾�� AI �ʱ�ȭ

        roundManager.InitializeTimer(60f); // Ÿ�̸� �ʱ�ȭ
        roundManager.ResetHpManagers(); // ü�� �ʱ�ȭ

        currentRound++;
        Debug.Log("Round initialized.");
    }

    public void RoundResult(string winner)
    {
        if (isRoundProcessing) return; // �̹� ���� ó�� ���� ��� ����
        isRoundProcessing = true; // ���� ó�� ���� ����

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

        // ���� ���� ���� Ȯ��
        if (playerWins >= 2 || aiWins >= 2)
        {
            EndMatch(); // ���� ���� ó��
        }
        else
        {
            // ���� ���ᰡ �ƴ� ��쿡�� ���� ���� �ʱ�ȭ
            Invoke(nameof(InitializeRound), 7f);
        }
    }

    private void EndMatch()
    {
        if (isGameOver) return;

        isGameOver = true;
        Debug.Log("Game Over");

        string message = playerWins >= 2
            ? "Congratuation!\nYou Win!" // �¸� �޽���
            : "That's too bad...\nYou lose!"; // �й� �޽���

        // �˾�â Ȱ��ȭ
        ActivateGameOverPanel(message);
    }

    private void ActivateGameOverPanel(string message)
    {
        gameOverPanel.SetActive(true); // �г� Ȱ��ȭ
        gameOverText.text = message; // �ؽ�Ʈ ����
    }

    public void LoadTitleScene()
    {
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            if (obj.scene.name == "DontDestroyOnLoad")
            {
                Destroy(obj); // DontDestroyOnLoad�� �ִ� ������Ʈ ����
            }
        }
        SceneManager.LoadSceneAsync("Title", LoadSceneMode.Single); // Title �� �ε�
    }
}