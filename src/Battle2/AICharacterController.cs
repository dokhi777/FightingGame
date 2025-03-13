using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AICharacterController : MonoBehaviour
{
    public float basicAttackDamage = 3f; // �⺻ �Ϲ� ���� ������
    public float comboAttackDamage = 6f; // �⺻ �޺� ���� ������
    public float ultimateAttackDamage = 20f; // �⺻ �ʻ�� ������
    private float damageMultiplier = 1f; // ������ ���� (�⺻�� 1��)
  
    private Animator animator;
    private BoxCollider boxCollider;
    private float initialZPosition;
    private Quaternion initialRotation;
    private bool isGuarding = false;
    private bool isSprinting = false;
    private bool isInvincible = false;
    
    private Vector3 defaultColliderSize = new Vector3(1f, 1.8f, 1f);
    private Vector3 attackColliderSize = new Vector3(1f, 1.8f, 2f);
    private Vector3 hit2ColliderSize = new Vector3(1f, 1.8f, 0.5f);

    private void Awake()
    {
        animator = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider>();

        initialRotation = transform.rotation;
        initialZPosition = transform.position.z;
    }
    private void Start()
    {
        // 3�� �������� ���� �ൿ ����
        InvokeRepeating(nameof(PerformRandomAction), 0f, 3f);
    }
    private void Update()
    {
        transform.rotation = initialRotation;
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

        HandleUltimateSkill();
    }
    private void UpdateCollider(Vector3 size)
    {
        boxCollider.size = size;
    }
    private void PerformRandomAction()
    {
        if (animator == null) return;

        // 100% Ȯ�� �й�
        int randomValue = Random.Range(0, 100);

        if (randomValue < 40)
        {
            // �Ϲ� ���� (40%)
            PerformAttack();
        }
        else if (randomValue < 55)
        {
            // Combo1 (15%)
            animator.SetTrigger("Combo1");
            Debug.Log("AI : Combo1");
        }
        else if (randomValue < 70)
        {
            // Combo2 (15%)
            animator.SetTrigger("Combo2");
            Debug.Log("AI : Combo2");
        }
        else if (randomValue < 90)
        {
            // �ȱ� �Ǵ� �ٱ� ����
            StartWalkingOrRunning();
        }
        else
        {
            // ���� (10%)
            PerformGuard();
        }
    }
    private void PerformAttack()
    {
        // �Ϲ� ���� �� ���� ����
        int randomAttack = Random.Range(0, 3);

        if (randomAttack == 0)
        {
            animator.SetTrigger("Attack1");
            Debug.Log("AI : Attack1");
        }
        else if (randomAttack == 1)
        {
            animator.SetTrigger("Attack2");
            Debug.Log("AI : Attack2");
        }
        else if (randomAttack == 2)
        {
            animator.SetTrigger("Attack3");
            Debug.Log("AI : Attack3");
        }
    }
    private void HandleUltimateSkill()
    {
        SpManager aiSpManager = GetComponent<SpManager>();

        // ���¹̳��� ���� á���� �ʻ�� ����
        if (aiSpManager.IsStaminaFull())
        {
            aiSpManager.ResetStamina(); // ���¹̳� �ʱ�ȭ

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
    }
    private void StartWalkingOrRunning()
    {
        // ������Ʈ ���θ� ���� ����
        isSprinting = Random.Range(0, 100) < 40; // 40% Ȯ���� ������Ʈ

        float speedMultiplier = isSprinting ? 3f : 1f; // ������Ʈ �ӵ� ����
        animator.SetBool("IsWalking", true);
        animator.SetFloat("SpeedMultiplier", speedMultiplier);

        Debug.Log(isSprinting ? "AI : Running" : "AI : Walking");

        // 3�� �� ���� ����
        Invoke(nameof(StopWalkingOrRunning), 3f);
    }
    private void StopWalkingOrRunning()
    {
        animator.SetBool("IsWalking", false);
        Debug.Log("AI : Stopped Walking/Running");
    }
    private void PerformGuard()
    {
        // ���� �ִϸ��̼� ����
        animator.SetTrigger("Guard");
        isGuarding = true;

        // ����� 2.8�� ���� ����
        Invoke(nameof(StopGuarding), 2.8f);
    }
    private void StopGuarding()
    {
        isGuarding = false; // ���� ���� ����
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
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player")) // Player ĳ���Ϳ� �浹 ��
        {
            HpManager aiHpManager = GetComponent<HpManager>();
            SpManager aiSpManager = GetComponent<SpManager>();
            Animator playerAnimator = collision.gameObject.GetComponent<Animator>();

            // ���� ���� ���� �ִϸ��̼� ���� Ȯ��
            AnimatorStateInfo stateInfo = playerAnimator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.IsName("Attack1") || stateInfo.IsName("Attack2") || stateInfo.IsName("Attack3"))
            {
                // Player�� Attack �ִϸ��̼� ���̸� Hit Ʈ���� ����
                // �Ϲ� ���� ó��
                if (isGuarding && !isInvincible)
                {
                    aiSpManager.RecoverSp(50f); // ���� ���� �� ���¹̳� ȸ��
                    Debug.Log("AI: ���� ����! ���¹̳� ȸ��");
                }
                else if (!isGuarding && !isInvincible)
                {
                    animator.SetTrigger("Hit");
                    float damage = basicAttackDamage * damageMultiplier;
                    aiHpManager.TakeDamage(damage); // ����� ���� ���°� �ƴ� ���� ������ ����
                }
                else
                {
                    Debug.Log("AI: ���� ���·� ���� ��ȿ");
                }
            }   // �޺� ���� ó��
            else if (stateInfo.IsName("Combo1") || stateInfo.IsName("Combo2"))
            {
                // Player�� Combo �ִϸ��̼� ���̸� Hit2 Ʈ���� ����
                animator.SetTrigger("Hit2");
                float damage = comboAttackDamage * damageMultiplier;
                aiHpManager.TakeDamage(damage);
            }   // �ʻ�� ó��
            else if (stateInfo.IsName("Ultimate"))
            {
                HandleUltimateCollision(collision.gameObject, aiHpManager);
            }
        }
    }

    private void HandleUltimateCollision(GameObject player, HpManager aiHpManager)
    {
        string playerName = player.name.Replace("(Clone)", "").Trim();

        switch (playerName)
        {
            case "Bodybuilder":
                // Bodybuilder�� �ʻ��: ������ ��ȿȭ
                Debug.Log("AI�� Bodybuilder�� �ʻ�� ������ ����. ������ ��ȿȭ!");
                break;

            case "Rosales":
                // Rosales�� �ʻ��: ������ 30 ����
                aiHpManager.TakeDamage(30);
                Debug.Log("AI�� Rosales�� �ʻ�� ������ ����. ������ 30!");
                break;

            case "Mutant":
                // Mutant�� �ʻ��: ������ ��ȿȭ
                Debug.Log("AI�� Mutant�� �ʻ�� ������ ����. ������ ��ȿȭ!");
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
        transform.position = new Vector3(5, transform.position.y, initialZPosition); // �ʱ� ��ġ�� �̵�
        transform.rotation = Quaternion.Euler(0, -90, 0); // �ʱ� �������� ����

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