using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartButton : MonoBehaviour
{
    public Button startButton;  // 시작 버튼
    public Transform[] spawnPoints; // 스폰 포인트 배열

    private void Start()
    {
        startButton.onClick.AddListener(OnStartButtonClicked);
    }

    private void Update()
    {
        bool allSpawnPointsFilled = true;

        for (int i = 0; i < spawnPoints.Length; i++)
        {
            if (CharacterSelection.selectedCharacters[i] == null)
            {
                allSpawnPointsFilled = false;
                break;
            }
        }

        startButton.interactable = allSpawnPointsFilled;
    }

    private void OnStartButtonClicked()
    {
        // 다음 씬으로 전환
        SceneManager.LoadSceneAsync("Battle1", LoadSceneMode.Single);
    }
}
