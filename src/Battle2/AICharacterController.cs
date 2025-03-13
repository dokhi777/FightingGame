using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AICharacterController : MonoBehaviour
{
    public float basicAttackDamage = 3f; // 기본 일반 공격 데미지
    public float comboAttackDamage = 6f; // 기본 콤보 공격 데미지
    public float ultimateAttackDamage = 20f; // 기본 필살기 데미지
    private float damageMultiplier = 1f; // 데미지 배율 (기본값 1배)
  
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
        // 3초 간격으로 랜덤 행동 실행
        InvokeRepeating(nameof(PerformRandomAction), 0f, 3f);
    }
    private void Update()
    {
        transform.rotation = initialRotation;
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        // 공격 애니메이션 실행 중이면 Collider 크기 크게 변경
        if (stateInfo.IsName("Attack1") || stateInfo.IsName("Attack2") || stateInfo.IsName("Attack3")
            || stateInfo.IsName("Combo1") || stateInfo.IsName("Combo2") || stateInfo.IsName("Ultimate"))
        {
            UpdateCollider(attackColliderSize);
        }
        // 스킬 피격 애니메이션 실행 중이면 Collider 크기 작게 변경
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

        // 100% 확률 분배
        int randomValue = Random.Range(0, 100);

        if (randomValue < 40)
        {
            // 일반 공격 (40%)
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
            // 걷기 또는 뛰기 결정
            StartWalkingOrRunning();
        }
        else
        {
            // 가드 (10%)
            PerformGuard();
        }
    }
    private void PerformAttack()
    {
        // 일반 공격 중 랜덤 선택
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

        // 스태미나가 가득 찼으면 필살기 실행
        if (aiSpManager.IsStaminaFull())
        {
            aiSpManager.ResetStamina(); // 스태미나 초기화

            string characterName = gameObject.name.Replace("(Clone)", "").Trim();

            switch (characterName)
            {
                case "Bodybuilder":
                    animator.SetTrigger("Ultimate");
                    EnableInvincibility(10f);
                    Debug.Log("Bodybuilder 필살기 : 10초 동안 적의 공격 무효화");
                    break;

                case "Rosales":
                    animator.SetTrigger("Ultimate");
                    Debug.Log("Rosales 필살기 : 피격시 데미지 30");
                    break;

                case "Mutant":
                    animator.SetTrigger("Ultimate");
                    if (!isInvincible)
                    {
                        StartCoroutine(EnableDamageBoost(10f)); // 10초 동안 데미지 배율 2배
                    }
                    Debug.Log("Mutant 필살기 : 10초 동안 공격력 두 배");
                    break;

                default:
                    Debug.LogError($"{characterName} 필살기가 정의되지 않았습니다.");
                    break;
            }
        }
    }
    private void StartWalkingOrRunning()
    {
        // 스프린트 여부를 랜덤 결정
        isSprinting = Random.Range(0, 100) < 40; // 40% 확률로 스프린트

        float speedMultiplier = isSprinting ? 3f : 1f; // 스프린트 속도 조정
        animator.SetBool("IsWalking", true);
        animator.SetFloat("SpeedMultiplier", speedMultiplier);

        Debug.Log(isSprinting ? "AI : Running" : "AI : Walking");

        // 3초 후 동작 멈춤
        Invoke(nameof(StopWalkingOrRunning), 3f);
    }
    private void StopWalkingOrRunning()
    {
        animator.SetBool("IsWalking", false);
        Debug.Log("AI : Stopped Walking/Running");
    }
    private void PerformGuard()
    {
        // 가드 애니메이션 시작
        animator.SetTrigger("Guard");
        isGuarding = true;

        // 가드는 2.8초 동안 지속
        Invoke(nameof(StopGuarding), 2.8f);
    }
    private void StopGuarding()
    {
        isGuarding = false; // 가드 상태 해제
    }

    // 10초 무적 상태 활성화 (Bodybuilder)
    private void EnableInvincibility(float duration)
    {
        isInvincible = true;
        Debug.Log("Bodybuilder : 10초 동안 무적 상태");
        Invoke(nameof(DisableInvincibility), duration);
    }

    private void DisableInvincibility()
    {
        isInvincible = false;
        Debug.Log("Bodybuilder : 무적 상태 해제");
    }

    // 10초 강화 상태 활성화 (Mutant)
    private IEnumerator EnableDamageBoost(float duration)
    {
        damageMultiplier = 2f;  // 데미지 배율 2배로 설정
        Debug.Log("Mutant : 10초 동안 모든 공격이 2배 데미지를 가집니다!");

        yield return new WaitForSeconds(duration);

        damageMultiplier = 1f; // 배율 원래대로 복구
        Debug.Log("Mutant : 데미지 배율이 원래 상태로 복구되었습니다.");
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player")) // Player 캐릭터와 충돌 시
        {
            HpManager aiHpManager = GetComponent<HpManager>();
            SpManager aiSpManager = GetComponent<SpManager>();
            Animator playerAnimator = collision.gameObject.GetComponent<Animator>();

            // 현재 실행 중인 애니메이션 상태 확인
            AnimatorStateInfo stateInfo = playerAnimator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.IsName("Attack1") || stateInfo.IsName("Attack2") || stateInfo.IsName("Attack3"))
            {
                // Player가 Attack 애니메이션 중이면 Hit 트리거 실행
                // 일반 공격 처리
                if (isGuarding && !isInvincible)
                {
                    aiSpManager.RecoverSp(50f); // 가드 성공 시 스태미나 회복
                    Debug.Log("AI: 가드 성공! 스태미나 회복");
                }
                else if (!isGuarding && !isInvincible)
                {
                    animator.SetTrigger("Hit");
                    float damage = basicAttackDamage * damageMultiplier;
                    aiHpManager.TakeDamage(damage); // 가드와 무적 상태가 아닐 때만 데미지 적용
                }
                else
                {
                    Debug.Log("AI: 무적 상태로 공격 무효");
                }
            }   // 콤보 공격 처리
            else if (stateInfo.IsName("Combo1") || stateInfo.IsName("Combo2"))
            {
                // Player가 Combo 애니메이션 중이면 Hit2 트리거 실행
                animator.SetTrigger("Hit2");
                float damage = comboAttackDamage * damageMultiplier;
                aiHpManager.TakeDamage(damage);
            }   // 필살기 처리
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
                // Bodybuilder의 필살기: 데미지 무효화
                Debug.Log("AI가 Bodybuilder의 필살기 공격을 받음. 데미지 무효화!");
                break;

            case "Rosales":
                // Rosales의 필살기: 데미지 30 적용
                aiHpManager.TakeDamage(30);
                Debug.Log("AI가 Rosales의 필살기 공격을 받음. 데미지 30!");
                break;

            case "Mutant":
                // Mutant의 필살기: 데미지 무효화
                Debug.Log("AI가 Mutant의 필살기 공격을 받음. 데미지 무효화!");
                break;

            default:
                Debug.LogWarning($"{playerName} 필살기 효과가 정의되지 않았습니다.");
                break;
        }

        // 필살기 중 공통 동작 (Hit2 트리거)
        animator.SetTrigger("Hit2");
    }

    // 라운드 시작시 필요
    public void ResetState()
    {
        // 위치 및 회전 초기화
        transform.position = new Vector3(5, transform.position.y, initialZPosition); // 초기 위치로 이동
        transform.rotation = Quaternion.Euler(0, -90, 0); // 초기 방향으로 설정

        // 애니메이션 초기화
        if (animator != null)
        {
            animator.ResetTrigger("Hit");
            animator.ResetTrigger("Dead");
            animator.SetBool("IsWalking", false);
            animator.SetBool("IsDead", false);
            animator.Play("Idle"); // Idle 애니메이션 상태로 전환
        }

        Debug.Log($"{gameObject.name} 상태 초기화 완료");
    }
}