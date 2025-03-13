using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float moveSpeed = 3f; // �̵� �ӵ�
    public float sprintSpeed = 6f;
    public Vector3 attackColliderSize = new Vector3(1, 1.8f, 2);
    private Vector3 hit2ColliderSize = new Vector3(1f, 1.8f, 0.5f);

    public float basicAttackDamage = 3f; // �⺻ �Ϲ� ���� ������
    public float comboAttackDamage = 6f; // �⺻ �޺� ���� ������
    public float ultimateAttackDamage = 20f; // �⺻ �ʻ�� ������


    private TextMeshProUGUI actionText;
    private float damageMultiplier = 1f; // ������ ���� (�⺻�� 1��)
    private Rigidbody rb; // Rigidbody ����
    private float initialZPosition;
    private Animator animator;
    private BoxCollider boxCollider;
    private Vector3 defaultColliderSize = new Vector3(1, 1.8f, 1);
    private Vector3 defaultColliderCenter = new Vector3(0, 0.8f, 0);

    private float comboTimeout = 1.0f; // �޺� �Է� ���� �ð�
    private float lastInputTime = 0.0f; // ������ �Է� �ð�
    private string currentCombo = ""; // ���� �Էµ� �޺� ���ڿ�
    private bool isMoving = false;
    private bool isGuarding = false;
    private bool isSprinting = false;
    private bool isInvincible = false;

    private void Awake()
    {
        // Rigidbody �߰�
        rb = gameObject.AddComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.useGravity = true;

        boxCollider = GetComponent<BoxCollider>();
        if (boxCollider == null)
        {
            boxCollider = gameObject.AddComponent<BoxCollider>();
            boxCollider.size = defaultColliderSize;
            boxCollider.center = defaultColliderCenter;
        }

        initialZPosition = transform.position.z;
    }
    private void Update()
    {
        if (animator == null) return;

        HandleMovement();
        HandleActions();

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        // ���� �ִϸ��̼� ���� ���̸� Collider ũ�� ũ�� ����
        if (stateInfo.IsName("Attack1") || stateInfo.IsName("Attack2") || stateInfo.IsName("Attack3")
            || stateInfo.IsName("Combo1") || stateInfo.IsName("Combo2") || stateInfo.IsName("Ultimate"))
        {
            UpdateCollider(attackColliderSize);
        }
        // ��ų �ǰ� �ִϸ��̼� ���� ���̸� Collider ũ�� �۰� ����
        else if (stateInfo.IsName("Hit2"))
        {
            UpdateCollider(hit2ColliderSize);
        }
        else
        {
            UpdateCollider(defaultColliderSize);
        }

        // �޺� Ÿ�Ӿƿ� üũ
        if (Time.time - lastInputTime > comboTimeout)
        {
            ResetCombo(); // �ð� �ʰ� �� �޺� �ʱ�ȭ
        }

        // �Է� ó��
        if (Input.GetKeyDown(KeyCode.H))
        {
            HandleInput("H");
        }
        else if (Input.GetKeyDown(KeyCode.J))
        {
            HandleInput("J");
        }
        else if (Input.GetKeyDown(KeyCode.K))
        {
            HandleInput("K");
        }
    }
    public void SetAnimator(Animator newAnimator)
    {
        animator = newAnimator;
    }
    public void SetActionText(TextMeshProUGUI actionText)
    {
        this.actionText = actionText;
    }
    private void HandleMovement()
    {
        isSprinting = Input.GetKey(KeyCode.LeftShift);
        float speed = isSprinting ? sprintSpeed : moveSpeed;

        // �¿� �̵� ó��
        float moveInput = Input.GetAxis("Horizontal");
        Vector3 move = new Vector3(moveInput * speed * Time.deltaTime, 0, 0);
        rb.MovePosition(rb.position + move);

        transform.position = new Vector3(transform.position.x, transform.position.y, initialZPosition);

        if (moveInput > 0)
        {
            transform.rotation = Quaternion.Euler(0, 90, 0); // ������ ����
        }
        else if (moveInput < 0)
        {
            transform.rotation = Quaternion.Euler(0, -90, 0); // ���� ����
        }

        // ����Ű �Է� �� Walk �ִϸ��̼�
        if (Mathf.Abs(moveInput) > 0) 
        {
            if (!isMoving)
            {
                animator.SetBool("IsWalking", true);
                animator.SetFloat("SpeedMultiplier", isSprinting ? 3f : 1f); // �ִϸ��̼� �ӵ� ����
                isMoving = true;
            }
        }
        else
        {
            if (isMoving)
            {
                animator.SetBool("IsWalking", false);
                isMoving = false;
            }
        }
    }
    private void HandleActions()
    {
        // ���� ����
        if (Input.GetKeyDown(KeyCode.H))
        {
            animator.SetTrigger("Attack1");
            ShowAction("-> H"); 
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            animator.SetTrigger("Attack2");
            ShowAction("-> J"); 
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            animator.SetTrigger("Attack3");
            ShowAction("-> K"); 
        }

        // ���� ����
        if (Input.GetKeyDown(KeyCode.G) && !isGuarding)
        {
            PerformGuard();
            ShowAction("-> Guard!");
        }

        // �ʻ�� ����
        if (Input.GetKeyDown(KeyCode.Q))
        {
            PerformUltimate();
            ShowAction("-> Ultimate!!!");
        }
    }
    private void HandleInput(string key)
    {
        // Ű �Է� �� �ؽ�Ʈ ���
        ShowAction(key);

        // �޺� �Է� ������Ʈ
        currentCombo += key;
        lastInputTime = Time.time; // �Է� �ð� ����

        // �޺� ó��
        if (currentCombo == "JK") // J �� K
        {
            // Combo1 ����
            animator.SetTrigger("Combo1");
            ShowAction("-> Combo1!");
            ResetCombo(); // �޺� �ʱ�ȭ
        }
        else if (currentCombo == "HHJ") // H �� H �� J
        {
            // Combo2 ����
            animator.SetTrigger("Combo2");
            ShowAction("-> Combo2!");
            ResetCombo(); // �޺� �ʱ�ȭ
        }
        else if (currentCombo.Length > 3) // �Է� �ʰ� �� �ʱ�ȭ
        {
            ResetCombo();
        }
    }
    private void PerformGuard()
    {
        animator.SetTrigger("Guard");
        isGuarding = true;

        // 2�� �� ���� ����
        Invoke(nameof(StopGuarding), 2.8f);
    }
    private void StopGuarding()
    {
        isGuarding = false;  // ���� ���� ����
    }
    private void ShowAction(string action)
    {
        actionText.text = action; // �ؽ�Ʈ ����
        actionText.gameObject.SetActive(true); // �ؽ�Ʈ Ȱ��ȭ
        CancelInvoke(nameof(HideAction)); // ���� ����� ���� ���
        Invoke(nameof(HideAction), 3.0f); // 3�� �� �ؽ�Ʈ ����
    }

    private void HideAction()
    {
        actionText.gameObject.SetActive(false); // �ؽ�Ʈ ��Ȱ��ȭ
    }
    private void ResetCombo()
    {
        currentCombo = ""; // �Էµ� �޺� �ʱ�ȭ
    }
    private void PerformUltimate()
    {
        SpManager playerSpManager = GetComponent<SpManager>();
        if (!playerSpManager.IsStaminaFull()) return;

        playerSpManager.ResetStamina();

        string characterName = gameObject.name.Replace("(Clone)", "").Trim();

        switch (characterName)
        {
            case "Bodybuilder":
                animator.SetTrigger("Ultimate");
                EnableInvincibility(10f);
                Debug.Log("Bodybuilder �ʻ�� : 10�� ���� ���� ���� ��ȿȭ");
                break;

            case "Rosales":
                animator.SetTrigger("Ultimate");
                Debug.Log("Rosales �ʻ�� : �ǰݽ� ������ 30");
                break;

            case "Mutant":
                animator.SetTrigger("Ultimate");
                if (!isInvincible)
                {
                    StartCoroutine(EnableDamageBoost(10f)); // 10�� ���� ������ ���� 2��
                }
                Debug.Log("Mutant �ʻ�� : 10�� ���� ���ݷ� �� ��");
                break;

            default:
                Debug.LogError($"{characterName} �ʻ�Ⱑ ���ǵ��� �ʾҽ��ϴ�.");
                break;
        }
    }
    // 10�� ���� ���� Ȱ��ȭ (Bodybuilder)
    private void EnableInvincibility(float duration)
    {
        isInvincible = true;
        Debug.Log("Bodybuilder : 10�� ���� ���� ����");
        Invoke(nameof(DisableInvincibility), duration);
    }

    private void DisableInvincibility()
    {
        isInvincible = false;
        Debug.Log("Bodybuilder : ���� ���� ����");
    }

    // 10�� ��ȭ ���� Ȱ��ȭ (Mutant)
    private IEnumerator EnableDamageBoost(float duration)
    {
        damageMultiplier = 2f;  // ������ ���� 2��� ����
        Debug.Log("Mutant : 10�� ���� ��� ������ 2�� �������� �����ϴ�!");

        yield return new WaitForSeconds(duration);

        damageMultiplier = 1f; // ���� ������� ����
        Debug.Log("Mutant : ������ ������ ���� ���·� �����Ǿ����ϴ�.");
    }
    private void UpdateCollider(Vector3 size)
    {
        boxCollider.size = size;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("AI")) // AI ĳ���Ϳ� �浹 ��
        {
            HpManager playerHpManager = GetComponent<HpManager>();
            SpManager playerSpManager = GetComponent<SpManager>();
            Animator aiAnimator = collision.gameObject.GetComponent<Animator>();

            // ���� ���� ���� �ִϸ��̼� ���� Ȯ��
            AnimatorStateInfo stateInfo = aiAnimator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.IsName("Attack1") || stateInfo.IsName("Attack2") || stateInfo.IsName("Attack3"))
            {
                // AI�� Attack �ִϸ��̼� ���̸� Hit Ʈ���� ����
                // �Ϲ� ���� ó��
                if (isGuarding && !isInvincible)
                {
                    playerSpManager.RecoverSp(50f); // ���� ���� �� ���¹̳� ȸ��
                    Debug.Log("Player: ���� ����! ���¹̳� ȸ��");
                }
                else if (!isGuarding && !isInvincible)
                {
                    animator.SetTrigger("Hit");
                    float damage = basicAttackDamage * damageMultiplier;
                    playerHpManager.TakeDamage(damage); // ����� ���� ���°� �ƴ� ���� ������ ����
                }
                else
                {
                    Debug.Log("Player: ���� ���·� ���� ��ȿ");
                }
            }   // �޺� ���� ó��
            else if (stateInfo.IsName("Combo1") || stateInfo.IsName("Combo2"))
            {
                // AI�� Combo �ִϸ��̼� ���̸� Hit2 Ʈ���� ����
                float damage = comboAttackDamage * damageMultiplier;
                playerHpManager.TakeDamage(damage);
            }   // �ʻ�� ó��
            else if (stateInfo.IsName("Ultimate"))
            {
                HandleUltimateCollision(collision.gameObject, playerHpManager);
            }
        }
    }
    private void HandleUltimateCollision(GameObject player, HpManager playerHpManager)
    {
        string playerName = player.name.Replace("(Clone)", "").Trim();

        switch (playerName)
        {
            case "Bodybuilder":
                // Bodybuilder�� �ʻ��: ������ ��ȿȭ
                Debug.Log("Player�� Bodybuilder�� �ʻ�� ������ ����. ������ ��ȿȭ!");
                break;

            case "Rosales":
                // Rosales�� �ʻ��: ������ 30 ����
                playerHpManager.TakeDamage(30);
                Debug.Log("Player�� Rosales�� �ʻ�� ������ ����. ������ 30!");
                break;

            case "Mutant":
                // Mutant�� �ʻ��: ������ ��ȿȭ
                Debug.Log("Player�� Mutant�� �ʻ�� ������ ����. ������ ��ȿȭ!");
                break;

            default:
                Debug.LogWarning($"{playerName} �ʻ�� ȿ���� ���ǵ��� �ʾҽ��ϴ�.");
                break;
        }

        // �ʻ�� �� ���� ���� (Hit2 Ʈ����)
        animator.SetTrigger("Hit2");
    }

    // ���� ���۽� �ʿ�
    public void ResetState()
    {
        // ��ġ �� ȸ�� �ʱ�ȭ
        transform.position = new Vector3(0, transform.position.y, initialZPosition); // �ʱ� ��ġ�� �̵�
        transform.rotation = Quaternion.Euler(0, 90, 0); // �ʱ� �������� ����

        // �ִϸ��̼� �ʱ�ȭ
        if (animator != null)
        {
            animator.ResetTrigger("Hit");
            animator.ResetTrigger("Dead");
            animator.SetBool("IsWalking", false);
            animator.SetBool("IsDead", false);
            animator.Play("Idle"); // Idle �ִϸ��̼� ���·� ��ȯ
        }

        Debug.Log($"{gameObject.name} ���� �ʱ�ȭ �Ϸ�");
    }
}