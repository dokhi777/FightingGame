using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class PlayerInitializer : MonoBehaviour
{
    public GameObject[] selectedCharacters; // DontDestroyOnLoad된 캐릭터 배열
    public Image characterImage1;
    public Image characterImage2;
    public GameObject playerHpBar;
    public GameObject playerSpBar;
    public GameObject aiHpBar;
    public GameObject aiSpBar;
    public RuntimeAnimatorController rosalesAnimator;
    public AnimatorOverrideController bodybuilderAnimator;
    public AnimatorOverrideController mutantAnimator;
    public TextMeshProUGUI actionText;


    public Vector3 character1Position = new Vector3(-5, 0.6f, -0.5f); // 첫 번째 캐릭터 초기 위치
    public Vector3 character2Position = new Vector3(5, 0.6f, -0.5f);  // 두 번째 캐릭터 초기 위치
    public Vector3 character1Rotation = new Vector3(0, 90, 0); // 첫 번째 캐릭터 초기 각도
    public Vector3 character2Rotation = new Vector3(0, -90, 0); // 두 번째 캐릭터 초기 각도

    private float initialZPosition;

    private void Start()
    {
        selectedCharacters = CharacterSelection.selectedCharacters; // 자동으로 캐릭터를 설정

        // 첫 번째 캐릭터(PlayerMove를 추가할 캐릭터)
        if (selectedCharacters.Length > 0 && selectedCharacters[0] != null)
        {
            GameObject firstCharacter = selectedCharacters[0];

            LoadCharacterImage(firstCharacter.name, characterImage1);

            // 첫 번째 캐릭터 초기 위치와 각도 설정
            firstCharacter.transform.position = character1Position;
            firstCharacter.transform.rotation = Quaternion.Euler(character1Rotation);

            firstCharacter.tag = "Player";

            // Animator 설정
            SetupAnimator(firstCharacter);

            // PlayerMove 스크립트 추가 및 Animator 전달
            PlayerMove playerMove = firstCharacter.GetComponent<PlayerMove>();
            if (playerMove == null)
            {
                playerMove = firstCharacter.AddComponent<PlayerMove>();
                Animator animator = firstCharacter.GetComponent<Animator>();
                if (animator != null)
                {
                    playerMove.SetAnimator(animator);
                }
            }

            // HpManager, SpManager 추가
            AddHpManager(firstCharacter, playerHpBar, 100);
            AddSpManager(firstCharacter, playerSpBar, 0);

            // 텍스트 연결
            playerMove.SetActionText(actionText);  
        }

        // 두 번째 캐릭터(플레이어 움직임이 없는 캐릭터)
        if (selectedCharacters.Length > 1 && selectedCharacters[1] != null)
        {
            GameObject secondCharacter = selectedCharacters[1];

            LoadCharacterImage(secondCharacter.name, characterImage2);

            // 두 번째 캐릭터 초기 위치와 각도 설정
            secondCharacter.transform.position = character2Position;
            secondCharacter.transform.rotation = Quaternion.Euler(character2Rotation);

            secondCharacter.tag = "AI";

            Rigidbody rb2 = secondCharacter.GetComponent<Rigidbody>();
            if (rb2 == null)
            {
                rb2 = secondCharacter.AddComponent<Rigidbody>();
                rb2.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ;
                rb2.useGravity = true;
            }

            BoxCollider boxCollider = secondCharacter.GetComponent<BoxCollider>();
            if (boxCollider == null)
            {
                boxCollider = secondCharacter.AddComponent<BoxCollider>();
                boxCollider.size = new Vector3(1f, 1.8f, 1f); // 기본 크기 설정
                boxCollider.center = new Vector3(0f, 0.8f, 0f); // 중심 설정
            }

            // Animator 설정
            SetupAnimator(secondCharacter);

            if (secondCharacter.GetComponent<AICharacterController>() == null)
            {
                secondCharacter.AddComponent<AICharacterController>();
            }

            // HpManager, SpManager 추가
            AddHpManager(secondCharacter, aiHpBar, 100);
            AddSpManager(secondCharacter, aiSpBar, 0);
        }
    }

    private void LoadCharacterImage(string characterName, Image targetImage)
    {
        // "(Clone)" 제거 및 공백 트림
        characterName = characterName.Replace("(Clone)", "").Trim();

        // 경로 설정
        string imagePath = $"Images/{characterName}";

        Sprite characterSprite = Resources.Load<Sprite>(imagePath);

        targetImage.sprite = characterSprite;
    }
    private void SetupAnimator(GameObject character)
    {
        Animator animator = character.GetComponent<Animator>();

        if (animator == null)
        {
            animator = character.AddComponent<Animator>();
        }

        // 캐릭터 이름에 따른 Animator Controller 설정
        string characterName = character.name.Replace("(Clone)", "").Trim(); // "(Clone)" 제거
        RuntimeAnimatorController controller = null;

        if (characterName == "Rosales")
        {
            // Rosales는 일반 AnimatorController
            controller = GetAnimatorController(characterName, isOverride: false);
        }
        else
        {
            // Mutant와 Bodybuilder는 AnimatorOverrideController
            controller = GetAnimatorController(characterName, isOverride: true);
        }

        if (controller != null)
        {
            animator.runtimeAnimatorController = controller;
        }
    }
    private void AddHpManager(GameObject character, GameObject hpBarObject, float maxHp)
    {
        HpManager hpManager = character.AddComponent<HpManager>();
        Slider hpBarSlider = hpBarObject.GetComponentInChildren<Slider>();
        hpManager.Initialize(hpBarSlider, maxHp); // 초기화

        GameObject hitSoundObject = GameObject.Find("HitSound");
        HitSound hitSound = hitSoundObject.GetComponent<HitSound>();
        hpManager.hitSound = hitSound;
    }

    private void AddSpManager(GameObject character, GameObject spBarObject, float minSp)
    {
        SpManager spManager = character.AddComponent<SpManager>();
        Slider spBarSlider = spBarObject.GetComponentInChildren<Slider>();
        spManager.Initialize(spBarSlider, minSp); // 초기화

        GameObject spSoundObject = GameObject.Find("SpSound");
        SpSound spSound = spSoundObject.GetComponent<SpSound>();
        spManager.spSound = spSound;
    }
    private RuntimeAnimatorController GetAnimatorController(string characterName, bool isOverride)
    {
        string path = $"AnimatorControllers/{characterName}Animator";

        if (isOverride)
        {
            // AnimatorOverrideController를 로드
            AnimatorOverrideController overrideController = Resources.Load<AnimatorOverrideController>(path);

            if (overrideController == null)
                return null;
            return overrideController;
        }
        else
        {
            // 일반 RuntimeAnimatorController를 로드
            RuntimeAnimatorController animatorController = Resources.Load<RuntimeAnimatorController>(path);

            if (animatorController != null)
                return animatorController;
            return null;
        }
    }

    public void ResetCharacters()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        GameObject ai = GameObject.FindGameObjectWithTag("AI");

        if (player != null)
        {
            PlayerMove playerMove = player.GetComponent<PlayerMove>();
            playerMove?.ResetState();
        }

        if (ai != null)
        {
            AICharacterController aiController = ai.GetComponent<AICharacterController>();
            aiController?.ResetState();
        }

        Debug.Log("플레이어와 AI 상태 초기화 완료");
    }
}