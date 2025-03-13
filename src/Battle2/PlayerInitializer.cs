using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class PlayerInitializer : MonoBehaviour
{
    public GameObject[] selectedCharacters; // DontDestroyOnLoad�� ĳ���� �迭
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


    public Vector3 character1Position = new Vector3(-5, 0.6f, -0.5f); // ù ��° ĳ���� �ʱ� ��ġ
    public Vector3 character2Position = new Vector3(5, 0.6f, -0.5f);  // �� ��° ĳ���� �ʱ� ��ġ
    public Vector3 character1Rotation = new Vector3(0, 90, 0); // ù ��° ĳ���� �ʱ� ����
    public Vector3 character2Rotation = new Vector3(0, -90, 0); // �� ��° ĳ���� �ʱ� ����

    private float initialZPosition;

    private void Start()
    {
        selectedCharacters = CharacterSelection.selectedCharacters; // �ڵ����� ĳ���͸� ����

        // ù ��° ĳ����(PlayerMove�� �߰��� ĳ����)
        if (selectedCharacters.Length > 0 && selectedCharacters[0] != null)
        {
            GameObject firstCharacter = selectedCharacters[0];

            LoadCharacterImage(firstCharacter.name, characterImage1);

            // ù ��° ĳ���� �ʱ� ��ġ�� ���� ����
            firstCharacter.transform.position = character1Position;
            firstCharacter.transform.rotation = Quaternion.Euler(character1Rotation);

            firstCharacter.tag = "Player";

            // Animator ����
            SetupAnimator(firstCharacter);

            // PlayerMove ��ũ��Ʈ �߰� �� Animator ����
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

            // HpManager, SpManager �߰�
            AddHpManager(firstCharacter, playerHpBar, 100);
            AddSpManager(firstCharacter, playerSpBar, 0);

            // �ؽ�Ʈ ����
            playerMove.SetActionText(actionText);  
        }

        // �� ��° ĳ����(�÷��̾� �������� ���� ĳ����)
        if (selectedCharacters.Length > 1 && selectedCharacters[1] != null)
        {
            GameObject secondCharacter = selectedCharacters[1];

            LoadCharacterImage(secondCharacter.name, characterImage2);

            // �� ��° ĳ���� �ʱ� ��ġ�� ���� ����
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
                boxCollider.size = new Vector3(1f, 1.8f, 1f); // �⺻ ũ�� ����
                boxCollider.center = new Vector3(0f, 0.8f, 0f); // �߽� ����
            }

            // Animator ����
            SetupAnimator(secondCharacter);

            if (secondCharacter.GetComponent<AICharacterController>() == null)
            {
                secondCharacter.AddComponent<AICharacterController>();
            }

            // HpManager, SpManager �߰�
            AddHpManager(secondCharacter, aiHpBar, 100);
            AddSpManager(secondCharacter, aiSpBar, 0);
        }
    }

    private void LoadCharacterImage(string characterName, Image targetImage)
    {
        // "(Clone)" ���� �� ���� Ʈ��
        characterName = characterName.Replace("(Clone)", "").Trim();

        // ��� ����
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

        // ĳ���� �̸��� ���� Animator Controller ����
        string characterName = character.name.Replace("(Clone)", "").Trim(); // "(Clone)" ����
        RuntimeAnimatorController controller = null;

        if (characterName == "Rosales")
        {
            // Rosales�� �Ϲ� AnimatorController
            controller = GetAnimatorController(characterName, isOverride: false);
        }
        else
        {
            // Mutant�� Bodybuilder�� AnimatorOverrideController
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
        hpManager.Initialize(hpBarSlider, maxHp); // �ʱ�ȭ

        GameObject hitSoundObject = GameObject.Find("HitSound");
        HitSound hitSound = hitSoundObject.GetComponent<HitSound>();
        hpManager.hitSound = hitSound;
    }

    private void AddSpManager(GameObject character, GameObject spBarObject, float minSp)
    {
        SpManager spManager = character.AddComponent<SpManager>();
        Slider spBarSlider = spBarObject.GetComponentInChildren<Slider>();
        spManager.Initialize(spBarSlider, minSp); // �ʱ�ȭ

        GameObject spSoundObject = GameObject.Find("SpSound");
        SpSound spSound = spSoundObject.GetComponent<SpSound>();
        spManager.spSound = spSound;
    }
    private RuntimeAnimatorController GetAnimatorController(string characterName, bool isOverride)
    {
        string path = $"AnimatorControllers/{characterName}Animator";

        if (isOverride)
        {
            // AnimatorOverrideController�� �ε�
            AnimatorOverrideController overrideController = Resources.Load<AnimatorOverrideController>(path);

            if (overrideController == null)
                return null;
            return overrideController;
        }
        else
        {
            // �Ϲ� RuntimeAnimatorController�� �ε�
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

        Debug.Log("�÷��̾�� AI ���� �ʱ�ȭ �Ϸ�");
    }
}