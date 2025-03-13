using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float moveSpeed = 3f; // 이동 속도
    public float sprintSpeed = 6f;
    public Vector3 attackColliderSize = new Vector3(1, 1.8f, 2);
    private Vector3 hit2ColliderSize = new Vector3(1f, 1.8f, 0.5f);

    public float basicAttackDamage = 3f; // 기본 일반 공격 데미지
    public float comboAttackDamage = 6f; // 기본 콤보 공격 데미지
    public float ultimateAttackDamage = 20f; // 기본 필살기 데미지


    private TextMeshProUGUI actionText;
    private float damageMultiplier = 1f; // 데미지 배율 (기본값 1배)
    private Rigidbody rb; // Rigidbody 참조
    private float initialZPosition;
    private Animator animator;
    private BoxCollider boxCollider;
    private Vector3 defaultColliderSize = new Vector3(1, 1.8f, 1);
    private Vector3 defaultColliderCenter = new Vector3(0, 0.8f, 0);

    private float comboTimeout = 1.0f; // 콤보 입력 제한 시간
    private float lastInputTime = 0.0f; // 마지막 입력 시간
    private string currentCombo = ""; // 현재 입력된 콤보 문자열
    private bool isMoving = false;
    private bool isGuarding = false;
    private bool isSprinting = false;
    private bool isInvincible = false;

    private void Awake()
    {
        // Rigidbody 추가
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

        // 콤보 타임아웃 체크
        if (Time.time - lastInputTime > comboTimeout)
        {
            ResetCombo(); // 시간 초과 시 콤보 초기화
        }

        // 입력 처리
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

        // 좌우 이동 처리
        float moveInput = Input.GetAxis("Horizontal");
        Vector3 move = new Vector3(moveInput * speed * Time.deltaTime, 0, 0);
        rb.MovePosition(rb.position + move);

        transform.position = new Vector3(transform.position.x, transform.position.y, initialZPosition);

        if (moveInput > 0)
        {
            transform.rotation = Quaternion.Euler(0, 90, 0); // 오른쪽 방향
        }
        else if (moveInput < 0)
        {
            transform.rotation = Quaternion.Euler(0, -90, 0); // 왼쪽 방향
        }

        // 방향키 입력 시 Walk 애니메이션
        if (Mathf.Abs(moveInput) > 0) 
        {
            if (!isMoving)
            {
                animator.SetBool("IsWalking", true);
                animator.SetFloat("SpeedMultiplier", isSprinting ? 3f : 1f); // 애니메이션 속도 증가
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
        // 공격 실행
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

        // 가드 실행
        if (Input.GetKeyDown(KeyCode.G) && !isGuarding)
        {
            PerformGuard();
            ShowAction("-> Guard!");
        }

        // 필살기 실행
        if (Input.GetKeyDown(KeyCode.Q))
        {
            PerformUltimate();
            ShowAction("-> Ultimate!!!");
        }
    }
    private void HandleInput(string key)
    {
        // 키 입력 시 텍스트 출력
        ShowAction(key);

        // 콤보 입력 업데이트
        currentCombo += key;
        lastInputTime = Time.time; // 입력 시간 갱신

        // 콤보 처리
        if (currentCombo == "JK") // J → K
        {
            // Combo1 실행
            animator.SetTrigger("Combo1");
            ShowAction("-> Combo1!");
            ResetCombo(); // 콤보 초기화
        }
        else if (currentCombo == "HHJ") // H → H → J
        {
            // Combo2 실행
            animator.SetTrigger("Combo2");
            ShowAction("-> Combo2!");
            ResetCombo(); // 콤보 초기화
        }
        else if (currentCombo.Length > 3) // 입력 초과 시 초기화
        {
            ResetCombo();
        }
    }
    private void PerformGuard()
    {
        animator.SetTrigger("Guard");
        isGuarding = true;

        // 2초 후 가드 해제
        Invoke(nameof(StopGuarding), 2.8f);
    }
    private void StopGuarding()
    {
        isGuarding = false;  // 가드 상태 해제
    }
    private void ShowAction(string action)
    {
        actionText.text = action; // 텍스트 설정
        actionText.gameObject.SetActive(true); // 텍스트 활성화
        CancelInvoke(nameof(HideAction)); // 기존 숨기기 예약 취소
        Invoke(nameof(HideAction), 3.0f); // 3초 후 텍스트 숨김
    }

    private void HideAction()
    {
        actionText.gameObject.SetActive(false); // 텍스트 비활성화
    }
    private void ResetCombo()
    {
        currentCombo = ""; // 입력된 콤보 초기화
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
    private void UpdateCollider(Vector3 size)
    {
        boxCollider.size = size;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("AI")) // AI 캐릭터와 충돌 시
        {
            HpManager playerHpManager = GetComponent<HpManager>();
            SpManager playerSpManager = GetComponent<SpManager>();
            Animator aiAnimator = collision.gameObject.GetComponent<Animator>();

            // 현재 실행 중인 애니메이션 상태 확인
            AnimatorStateInfo stateInfo = aiAnimator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.IsName("Attack1") || stateInfo.IsName("Attack2") || stateInfo.IsName("Attack3"))
            {
                // AI가 Attack 애니메이션 중이면 Hit 트리거 실행
                // 일반 공격 처리
                if (isGuarding && !isInvincible)
                {
                    playerSpManager.RecoverSp(50f); // 가드 성공 시 스태미나 회복
                    Debug.Log("Player: 가드 성공! 스태미나 회복");
                }
                else if (!isGuarding && !isInvincible)
                {
                    animator.SetTrigger("Hit");
                    float damage = basicAttackDamage * damageMultiplier;
                    playerHpManager.TakeDamage(damage); // 가드와 무적 상태가 아닐 때만 데미지 적용
                }
                else
                {
                    Debug.Log("Player: 무적 상태로 공격 무효");
                }
            }   // 콤보 공격 처리
            else if (stateInfo.IsName("Combo1") || stateInfo.IsName("Combo2"))
            {
                // AI가 Combo 애니메이션 중이면 Hit2 트리거 실행
                float damage = comboAttackDamage * damageMultiplier;
                playerHpManager.TakeDamage(damage);
            }   // 필살기 처리
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
                // Bodybuilder의 필살기: 데미지 무효화
                Debug.Log("Player가 Bodybuilder의 필살기 공격을 받음. 데미지 무효화!");
                break;

            case "Rosales":
                // Rosales의 필살기: 데미지 30 적용
                playerHpManager.TakeDamage(30);
                Debug.Log("Player가 Rosales의 필살기 공격을 받음. 데미지 30!");
                break;

            case "Mutant":
                // Mutant의 필살기: 데미지 무효화
                Debug.Log("Player가 Mutant의 필살기 공격을 받음. 데미지 무효화!");
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
        transform.position = new Vector3(0, transform.position.y, initialZPosition); // 초기 위치로 이동
        transform.rotation = Quaternion.Euler(0, 90, 0); // 초기 방향으로 설정

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