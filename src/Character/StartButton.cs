using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartButton : MonoBehaviour
{
    public Button startButton;  // ���� ��ư
    public Transform[] spawnPoints; // ���� ����Ʈ �迭

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
        // ���� ������ ��ȯ
        SceneManager.LoadSceneAsync("Battle1", LoadSceneMode.Single);
    }
}
