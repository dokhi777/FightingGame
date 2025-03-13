using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class CharacterSelection : MonoBehaviour
{
    public GameObject[] characterPrefabs; // ĳ���� ������ �迭
    public Transform[] spawnPoints;       // Spawn Point �迭
    public Button[] buttons;              // ��ư �迭
    public int[] buttonToSpawnPointMap;   // ��ư�� Spawn Point ���� �迭
    public TextMeshProUGUI[] characterNameText;  // ĳ���� �̸� ǥ�� �ؽ�Ʈ

    private GameObject[] currentCharacters; // ���� Ȱ��ȭ�� ĳ���͵�
    public static GameObject[] selectedCharacters = new GameObject[2]; // ���õ� ĳ���� ����

    private void Start()
    {
        // Ȱ��ȭ�� ĳ���� �迭 �ʱ�ȭ
        currentCharacters = new GameObject[spawnPoints.Length];

        // ��ư Ŭ�� �̺�Ʈ ����
        for (int i = 0; i < buttons.Length; i++)
        {
            int index = i;

            buttons[i].onClick.AddListener(() =>
            {
                int characterIndex = GetCharacterIndex(index);
                SpawnCharacter(characterIndex, index);
                UpdateCharacterName(characterIndex, index);
            });
        }
    }
    private void UpdateCharacterName(int characterIndex, int buttonIndex)
    {
        int spawnPointIndex = buttonToSpawnPointMap[buttonIndex];

        if (characterNameText[spawnPointIndex] != null)
        {
            characterNameText[spawnPointIndex].text = characterPrefabs[characterIndex].name;
        }
    }

    private int GetCharacterIndex(int buttonIndex)
    {
        // ��ư �ε����� ĳ���� �ε����� ����
        if (buttonIndex == 3 || buttonIndex == 7) // ������ ĳ���� ��ư
        {
            return UnityEngine.Random.Range(0, characterPrefabs.Length);
        }

        return buttonIndex % 4;
    }

    public void SpawnCharacter(int characterIndex, int buttonIndex)
    {
        int spawnPointIndex = buttonToSpawnPointMap[buttonIndex];

        // ���� ĳ���� ����
        if (currentCharacters[spawnPointIndex] != null)
        {
            Destroy(currentCharacters[spawnPointIndex]);
        }

        // ���ο� ĳ���� ����
        currentCharacters[spawnPointIndex] = Instantiate(
            characterPrefabs[characterIndex],
            spawnPoints[spawnPointIndex].position,
            spawnPoints[spawnPointIndex].rotation
        );

        currentCharacters[spawnPointIndex].transform.SetParent(null);

        Transform characterTransform = currentCharacters[spawnPointIndex].transform;

        // Ư�� ĳ������ ��ġ�� ȸ�� ���� ����
        if (characterIndex == 0) // Bodybuilder
        {
            characterTransform.position = spawnPoints[spawnPointIndex].position + new Vector3(0, 0.5f, 0);
        }
        else if (characterIndex == 2) // Mutant
        {
            characterTransform.position = spawnPoints[spawnPointIndex].position + new Vector3(-0.1f, 0.3f, 0);
        }

        Animator animator = currentCharacters[spawnPointIndex].GetComponent<Animator>();
        if (animator == null)
        {
            animator = currentCharacters[spawnPointIndex].AddComponent<Animator>();
        }

        // ĳ���ͺ� Animator Controller ����
        RuntimeAnimatorController controller = null;
        switch (characterIndex)
        {
            case 0: // ù ��° ĳ����
                controller = Resources.Load<RuntimeAnimatorController>("AnimatorControllers/Bodybuilder");
                break;
            case 1: // �� ��° ĳ����
                controller = Resources.Load<RuntimeAnimatorController>("AnimatorControllers/Rosales");
                break;
            case 2: // �� ��° ĳ����
                controller = Resources.Load<RuntimeAnimatorController>("AnimatorControllers/Mutant");
                break;
        }

        if (controller != null)
        {
            animator.runtimeAnimatorController = controller;
        }

        // �⺻ �ִϸ��̼� ����
        animator.Play("Idle");

        // ���õ� ĳ���� ����
        selectedCharacters[spawnPointIndex] = currentCharacters[spawnPointIndex];
        DontDestroyOnLoad(selectedCharacters[spawnPointIndex]);

        // ���̾� ����
        SetLayerRecursive(currentCharacters[spawnPointIndex], LayerMask.NameToLayer("Character"));
    }

    private void SetLayerRecursive(GameObject obj, int newLayer)
    {
        if (obj == null) return;
        obj.layer = newLayer;

        foreach (Transform child in obj.transform)
        {
            SetLayerRecursive(child.gameObject, newLayer);
        }
    }
}
