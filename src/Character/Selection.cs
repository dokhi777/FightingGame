using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class CharacterSelection : MonoBehaviour
{
    public GameObject[] characterPrefabs; // 캐릭터 프리팹 배열
    public Transform[] spawnPoints;       // Spawn Point 배열
    public Button[] buttons;              // 버튼 배열
    public int[] buttonToSpawnPointMap;   // 버튼과 Spawn Point 매핑 배열
    public TextMeshProUGUI[] characterNameText;  // 캐릭터 이름 표시 텍스트

    private GameObject[] currentCharacters; // 현재 활성화된 캐릭터들
    public static GameObject[] selectedCharacters = new GameObject[2]; // 선택된 캐릭터 저장

    private void Start()
    {
        // 활성화된 캐릭터 배열 초기화
        currentCharacters = new GameObject[spawnPoints.Length];

        // 버튼 클릭 이벤트 연결
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
        // 버튼 인덱스를 캐릭터 인덱스로 매핑
        if (buttonIndex == 3 || buttonIndex == 7) // 무작위 캐릭터 버튼
        {
            return UnityEngine.Random.Range(0, characterPrefabs.Length);
        }

        return buttonIndex % 4;
    }

    public void SpawnCharacter(int characterIndex, int buttonIndex)
    {
        int spawnPointIndex = buttonToSpawnPointMap[buttonIndex];

        // 이전 캐릭터 제거
        if (currentCharacters[spawnPointIndex] != null)
        {
            Destroy(currentCharacters[spawnPointIndex]);
        }

        // 새로운 캐릭터 생성
        currentCharacters[spawnPointIndex] = Instantiate(
            characterPrefabs[characterIndex],
            spawnPoints[spawnPointIndex].position,
            spawnPoints[spawnPointIndex].rotation
        );

        currentCharacters[spawnPointIndex].transform.SetParent(null);

        Transform characterTransform = currentCharacters[spawnPointIndex].transform;

        // 특정 캐릭터의 위치와 회전 강제 적용
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

        // 캐릭터별 Animator Controller 연결
        RuntimeAnimatorController controller = null;
        switch (characterIndex)
        {
            case 0: // 첫 번째 캐릭터
                controller = Resources.Load<RuntimeAnimatorController>("AnimatorControllers/Bodybuilder");
                break;
            case 1: // 두 번째 캐릭터
                controller = Resources.Load<RuntimeAnimatorController>("AnimatorControllers/Rosales");
                break;
            case 2: // 세 번째 캐릭터
                controller = Resources.Load<RuntimeAnimatorController>("AnimatorControllers/Mutant");
                break;
        }

        if (controller != null)
        {
            animator.runtimeAnimatorController = controller;
        }

        // 기본 애니메이션 실행
        animator.Play("Idle");

        // 선택된 캐릭터 저장
        selectedCharacters[spawnPointIndex] = currentCharacters[spawnPointIndex];
        DontDestroyOnLoad(selectedCharacters[spawnPointIndex]);

        // 레이어 설정
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
