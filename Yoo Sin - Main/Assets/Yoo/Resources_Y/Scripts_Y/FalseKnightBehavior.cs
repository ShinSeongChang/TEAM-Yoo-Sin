using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FalseKnightBehavior : MonoBehaviour
{
    // 매직넘버들 상수 처리
    private const int ATTACK_RANGE = 0;
    private const int STUN = 1;

    private const float SHOCKWAVE_CHARGE_TIME = 1.4f;
    private const float TURN_TIME = 0.167f;
    private const float RUN_JUMP_TIME = 0.25f;
    private const float RUNNING = 0.7f;
    private const float ATTACK_REMAIN_TIME = 0.25f;
    private const float HEAD_OPEN_TIME = 0.417f;
    private const float BEFORE_HEAD_OPEN_TIME = 1.283f;

    private const float RUN_DELAY = 5f;
    private const float ATTACK_DELAY = 2.5f;
    private const float JUMP_FORCE = 400f;
    // 매직넘버들 상수 처리

    public GameObject shockWavePrefab;
    private GameObject shockWave;
    private Rigidbody2D falseKnightRigidbody;
    private Animator falseKnightAni;
    private BoxCollider2D detectRange;
    private CapsuleCollider2D falseKnightCollider;
    private GameObject player;
    private Collider2D attackRange;

    private Quaternion toLeft = Quaternion.Euler(0, 180, 0);
    private Quaternion toRight = Quaternion.Euler(0, 0, 0);

    private int hp = 13;
    private int randomNumber;
    private int stunCount;
    private Skill whatToDo;

    private float distance;
    private float timeAfterAttack;
    private float timeAfterRun;

    private WaitForSeconds shockWaveDelay = new WaitForSeconds(SHOCKWAVE_CHARGE_TIME);
    private WaitForSeconds turnDelay = new WaitForSeconds(TURN_TIME);
    private WaitForSeconds runAndJumpDelay = new WaitForSeconds(RUN_JUMP_TIME);
    private WaitForSeconds running = new WaitForSeconds(RUNNING);
    private WaitForSeconds attackRemain = new WaitForSeconds(ATTACK_REMAIN_TIME);
    private WaitForSeconds headOpenDelay = new WaitForSeconds(HEAD_OPEN_TIME);
    private WaitForSeconds beforeHeadOpen = new WaitForSeconds(BEFORE_HEAD_OPEN_TIME);

    private FalseKnightStunHead head;

    private bool playerOnRight;
    private bool lookRight;

    enum Skill
    {
        jump = 0,
        shockWave = 1,
        takeDown = 2,
        backJumpShockWave = 3
    }

    void Start()
    {
        head = transform.GetChild(STUN).gameObject.transform.GetComponentInChildren<FalseKnightStunHead>();
        falseKnightRigidbody = GetComponent<Rigidbody2D>();
        falseKnightCollider = GetComponent<CapsuleCollider2D>();
        falseKnightAni = GetComponent<Animator>();
        detectRange = gameObject.GetComponent<BoxCollider2D>();
        attackRange = transform.GetChild(ATTACK_RANGE).gameObject.GetComponent<Collider2D>();
        player = null;
        distance = 0;
        lookRight = true;
        timeAfterAttack = 0;
        timeAfterRun = 0;
        whatToDo = 0;
        stunCount = 0;
        randomNumber = 0;
    }

    void Update()
    {
        // 사망 상태가 false이고, 스턴 상태가 false 이면
        if (falseKnightAni.GetBool("IsDead") == false && falseKnightAni.GetBool("IsStun") == false)
        {
            CheckStun();
            // 플레이어가 감지됐다면
            if (player != null)
            {
                // 땅에 붙어있음이 false 이고, 상승하는 힘이 0미만이라면
                if (falseKnightAni.GetBool("IsGround") == false && falseKnightRigidbody.velocity.y < 0)
                {
                    // 중력값을 5로 초기화(빨리 떨어지도록 함)
                    falseKnightRigidbody.gravityScale = 5;
                }
                // 공격, 달리기 이후의 시간을 델타타임만큼 더함
                timeAfterAttack += Time.deltaTime;
                timeAfterRun += Time.deltaTime;
                PositionDiff();
                CheckTurn();
                Move();
                Attack();
            }
        }

        // 사망 상태가  false이고, 스턴 상태가 true 이면
        if (falseKnightAni.GetBool("IsDead") == false && falseKnightAni.GetBool("IsStun") == true)
        {
            CheckWakeUp();
        }
    }

    // 플레이어와 기사 사이의 거리를 계산 및 플레이어가 오른쪽에 있는지 확인하는 함수
    private void PositionDiff()
    {
        if (transform.position.x < player.transform.position.x)
        {
            distance = -1 * (transform.position.x - player.transform.position.x);
            playerOnRight = true;
        }
        else if (transform.position.x >= player.transform.position.x)
        {
            distance = transform.position.x - player.transform.position.x;
            playerOnRight = false;
        }
        //Debug.LogFormat(playerOnRight + ", 플레이어의 x좌표 : " + player.transform.position.x + "\n" + "기사의 x좌표 : " + transform.position.x + "\n");
    }

    // 기사가 방향전환을 해야 하는지 체크하는 함수
    private void CheckTurn()
    {
        // 기사가 땅에 붙어있고, 기본 상태일 때
        if (falseKnightAni.GetBool("IsGround") == true && falseKnightAni.GetBool("IsIdle") == true)
        {
            // 플레이어가 오른쪽에 있지않으나 기사가 오른쪽을 바라보고 있다면
            if (playerOnRight == false && lookRight == true)
            {
                StartCoroutine(Turn());
            }
            // 플레이어가 오른쪽에 있으나 기사가 오른쪽을 바라보고 있지않다면
            else if (playerOnRight == true && lookRight == false)
            {
                StartCoroutine(Turn());
            }
        }
    }

    // 플레이어가 기사로부터 일정거리 이상 멀어지면 실행하는 함수
    private void Move()
    {
        // 기사가 기본 상태이고 플레이어와의 거리가 11이상 떨어졌다면
        if (falseKnightAni.GetBool("IsIdle") == true && distance >= 11)
        {
            // 달리기의 대기시간보다 달린 후의 시간이 더 크다면 (달리기의 대기시간이 지났다면)
            if (timeAfterRun >= RUN_DELAY)
            {
                StartCoroutine(Run());
            }
        }
    }

    // 플레이어가 기사의 일정범위 내에 있을 때 실행하는 공격 함수
    private void Attack()
    {
        // 기사가 기본 상태이고 다른 행동을 하는 중이 아니라면
        if (falseKnightAni.GetBool("IsIdle") == true && falseKnightAni.GetBool("IsRun") == false
            && falseKnightAni.GetBool("IsTakeDown") == false && falseKnightAni.GetBool("IsShockWave") == false)
        {
            // 공격 한 후의 시간이 공격 대기시간보다 크고 (공격 대기시간이 지났고)
            if (timeAfterAttack >= ATTACK_DELAY)
            {
                // 거리가 0이상 5미만일 경우 점프, 내려찍기, 백점프 충격파 중 하나 사용
                if(0 <= distance && distance < 5)
                {
                    randomNumber = Random.Range(0, 3);
                    if (randomNumber == 0)
                    {
                        whatToDo = Skill.jump;
                    }
                    else if (randomNumber == 1)
                    {
                        whatToDo = Skill.takeDown;
                    }
                    else if(randomNumber == 2)
                    {
                        whatToDo = Skill.backJumpShockWave;
                    }

                    if (whatToDo == Skill.jump)
                    {
                        StartCoroutine(Jump());
                    }
                    else if (whatToDo == Skill.takeDown)
                    {
                        StartCoroutine(TakeDown());
                    }
                    else if (whatToDo == Skill.backJumpShockWave)
                    {
                        StartCoroutine(BackShockWave());
                    }
                }
                // 거리가 5이상 8미만 일경우 점프, 내려찍기, 충격파 중 하나 사용
                else if (5 <= distance && distance < 8)
                {
                    randomNumber = Random.Range(0, 2);
                    if (randomNumber == 0)
                    {
                        whatToDo = Skill.shockWave;
                    }
                    else if (randomNumber == 1)
                    {
                        whatToDo = Skill.takeDown;
                    }
                    else if (randomNumber == 2)
                    {
                        whatToDo = Skill.jump;
                    }

                    if (whatToDo == Skill.shockWave)
                    {
                        StartCoroutine(ShockWave());
                    }
                    else if (whatToDo == Skill.takeDown)
                    {
                        StartCoroutine(TakeDown());
                    }
                    else if (whatToDo == Skill.jump)
                    {
                        StartCoroutine(Jump());
                    }
                }
                // 플레이어와 기사의 거리가 8이상 11미만이라면 충격파
                else if (8 <= distance && distance < 11)
                {
                    StartCoroutine(ShockWave());
                }
            }
        }
    }

    // 기사가 스턴이 되어야 하는지 체크하는 CheckStun함수
    private void CheckStun()
    {
        // 기사의 체력이 0이하라면
        if (hp <= 0)
        {
            // Stun함수 실행
            Stun();
        }
    }

    // 기사가 스턴에서 깨어나야 하는지 체크하는 CheckWakeUp함수
    private void CheckWakeUp()
    {
        // 기사의 스턴 상태의 머리의 체력이 0이하라면
        if (head.hp <= 0)
        {
            // WakeUp함수 실행
            WakeUp();
        }
    }

    // 기사가 스턴 상태로 되는 Stun함수 (스턴 횟수가 일정 횟수가 되면 사망으로 변경)
    private void Stun()
    {
        // 스턴 횟수를 1 추가하고 기사의 애니메이터의 기본상태임을 false, 스턴상태임을 true로 초기화, StunStart트리거 활성화
        stunCount += 1;
        if (stunCount == 3)
        {
            falseKnightAni.SetBool("IsDead", true);
        }
        falseKnightAni.SetBool("IsIdle", false);
        falseKnightAni.SetBool("IsStun", true);
        falseKnightAni.SetTrigger("StunStart");
        // 오른쪽을 보고 있다면
        if (lookRight == true)
        {
            // 왼쪽으로 일정거리 날아감
            falseKnightRigidbody.velocity = Vector2.zero;
            falseKnightRigidbody.AddForce(new Vector2((-1 * 120), JUMP_FORCE / 2));
        }
        // 오른쪽을 보고 있지 않다면
        else if (playerOnRight == false)
        {
            // 오른쪽으로 일정거리 날아감
            falseKnightRigidbody.velocity = Vector2.zero;
            falseKnightRigidbody.AddForce(new Vector2(120, JUMP_FORCE / 2));
        }
    }

    // 기사가 스턴 상태에서 깨어나는 WakeUp함수
    private void WakeUp()
    {
        // 기사의 콜라이더를 키고 리지드 바디의 프리즈 값을 초기에 설정된 값으로 맞춰줌
        falseKnightCollider.enabled = true;
        falseKnightRigidbody.constraints = RigidbodyConstraints2D.None;
        falseKnightRigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
        //falseKnightRigidbody.gravityScale = 1;
        // 기사의 애니메이터의 투구열림을 false, 스턴상태임을 false, 기본상태임을 true로 초기화
        falseKnightAni.SetBool("IsHeadOpen", false);
        falseKnightAni.SetBool("IsStun", false);
        falseKnightAni.SetBool("IsIdle", true);
        // 기사의 체력 다시 회복
        hp = 13;
        // 기사의 공격 시간 0으로 다시 지정
        timeAfterAttack = 0;
        timeAfterRun = 0;
    }

    // 기사가 방향 전환을 하도록 만드는 Turn 코루틴
    IEnumerator Turn()
    {
        falseKnightAni.SetBool("IsIdle", false);
        falseKnightAni.SetBool("IsTurn", true);
        // Turn애니메이션이 실행되는 시간 만큼 기다린 후 다음 행 너머 부터 실행
        yield return turnDelay;
        // 플레이어가 기사 오른쪽에 있지않으나 기사가 오른쪽을 보고 있다면
        if (playerOnRight == false && lookRight == true)
        {
            // 기사가 왼쪽을 보도록하고, 기사가 오른쪽을 보고있지않다고 저장함
            transform.rotation = toLeft;
            lookRight = false;
        }
        // 플레이어가 기사 오른쪽에 있으나 기사가 오른쪽을 보고 있지않다면
        else if (playerOnRight == true && lookRight == false)
        {
            // 기사가 오른쪽을 보도록하고, 기사가 오른쪽을 보고있다고 저장함
            transform.rotation = toRight;
            lookRight = true;
        }
        falseKnightAni.SetBool("IsIdle", true);
        falseKnightAni.SetBool("IsTurn", false);
        // 여기서 코루틴 종료
        yield break;
    }

    // 기사가 달리도록하는 Run 코루틴
    IEnumerator Run()
    {
        falseKnightAni.SetBool("IsIdle", false);
        falseKnightAni.SetBool("IsRun", true);
        // 달리기와 점프 준비 애니메이션이 실행되는 시간 만큼 기다린 후 다음 행 너머 부터 실행
        yield return runAndJumpDelay;
        // 플레이어가 오른쪽에 있다면
        if (playerOnRight == true)
        {
            // 기사가 거리 차이 * 50 만큼의 힘을 받음 (오른쪽으로 가기 위해)
            falseKnightRigidbody.velocity = Vector2.zero;
            falseKnightRigidbody.AddForce(new Vector2((distance * 50), 0));
        }
        // 플레이어가 오른쪽에 있지않다면
        else if (playerOnRight == false)
        {
            // 기사가 거리 차이 * 50 * -1 만큼의 힘을 받음 (왼쪽으로 가기 위해)
            falseKnightRigidbody.velocity = Vector2.zero;
            falseKnightRigidbody.AddForce(new Vector2(-1 * (distance * 50), 0));
        }
        // 달리기 지속시간 동안 기다린 후 다음 행 너머 부터 실행
        yield return running;
        // 기사가 멈추도록함
        falseKnightRigidbody.velocity = Vector2.zero;
        falseKnightAni.SetBool("IsIdle", true);
        falseKnightAni.SetBool("IsRun", false);
        // 달린 후의 시간을 0으로 초기화
        timeAfterRun = 0;
        // 여기서 코루틴 종료
        yield break;
    }

    // 기사가 점프공격 하도록 하는 Jump 코루틴
    IEnumerator Jump()
    {
        falseKnightAni.SetBool("IsIdle", false);
        falseKnightAni.SetBool("IsJump", true);
        // 공격 시작 후의 시간을 0으로 초기화
        timeAfterAttack = 0;
        // 달리기와 점프 준비 애니메이션이 실행되는 시간 만큼 기다린 후 다음 행 너머 부터 실행
        yield return runAndJumpDelay;
        // 플레이어가 오른쪽에 있다면
        if (playerOnRight == true)
        {
            // 오른쪽으로 플레이어와 거리차이에 따라 힘을 줌 (점프 시작 시점의 플레이어 위치를 향한 힘)
            falseKnightRigidbody.velocity = Vector2.zero;
            falseKnightRigidbody.AddForce(new Vector2(distance * 40, JUMP_FORCE));
        }
        // 플레이어가 오른쪽에 있지않다면
        else if (playerOnRight == false)
        {
            // 왼쪽으로 플레이어와 거리차이에 따라 힘을 줌 (점프 시작 시점의 플레이어 위치를 향한 힘)
            falseKnightRigidbody.velocity = Vector2.zero;
            falseKnightRigidbody.AddForce(new Vector2(-1 * distance * 40, JUMP_FORCE));
        }
        falseKnightAni.SetBool("IsIdle", true);
        falseKnightAni.SetBool("IsJump", false);
        // 여기서 코루틴 종료
        yield break;
    }

    // 기사가 내려찍기 공격을 하도록 하는 TakeDown 코루틴
    IEnumerator TakeDown()
    {
        falseKnightAni.SetBool("IsIdle", false);
        falseKnightAni.SetBool("IsTakeDown", true);
        // 공격 시작 후의 시간을 0으로 초기화
        timeAfterAttack = 0;
        // 달리기와 점프 준비 애니메이션이 실행되는 시간 만큼 기다린 후 다음 행 너머 부터 실행
        yield return runAndJumpDelay;
        // 플레이어가 오른쪽에 있다면
        if (playerOnRight == true)
        {
            // 오른쪽으로 플레이어와 거리차이에 따라 힘을 줌 (점프 시작 시점의 플레이어 위치의 왼쪽을 향한 힘)
            falseKnightRigidbody.velocity = Vector2.zero;
            falseKnightRigidbody.AddForce(new Vector2(distance * 40 - 100, JUMP_FORCE));
        }
        // 플레이어가 오른쪽에 있지않다면
        else if (playerOnRight == false)
        {
            // 왼쪽으로 플레이어와 거리차이에 따라 힘을 줌 (점프 시작 시점의 플레이어 위치의 오른쪽을 향한 힘)
            falseKnightRigidbody.velocity = Vector2.zero;
            falseKnightRigidbody.AddForce(new Vector2(-1 * (distance * 40 - 100), JUMP_FORCE));
        }
        // 여기서 코루틴 종료
        yield break;
    }

    // 기사가 충격파 공격을 하도록 하는 ShockWave 코루틴
    IEnumerator ShockWave()
    {
        falseKnightAni.SetBool("IsIdle", false);
        falseKnightAni.SetBool("IsShockWave", true);
        // 공격 시작 후의 시간을 0으로 초기화
        timeAfterAttack = 0;
        // 충격파 공격의 차징 시간동안 기다린 후 다음 행 너머 부터 실행
        yield return shockWaveDelay;

        if (falseKnightAni.GetBool("IsStun") == true)
        {
            yield break;
        }

        StartCoroutine(AttackRemain());
        if (lookRight == true)
        {
            shockWave = Instantiate(shockWavePrefab, new Vector3(transform.position.x + 2f, transform.position.y - 2f, 0), transform.rotation);
        }
        else if (lookRight == false)
        {
            shockWave = Instantiate(shockWavePrefab, new Vector3(transform.position.x - 2f, transform.position.y - 2f, 0), transform.rotation);
        }
        // 여기서 코루틴 종료
        yield break;
    }

    // 기사가 후방점프후 충격파 공격을 하도록 하는 BackShockWave 코루틴
    IEnumerator BackShockWave()
    {
        falseKnightAni.SetBool("IsIdle", false);
        falseKnightAni.SetBool("IsBackShockWave", true);
        // 공격 시작 후의 시간을 0으로 초기화
        timeAfterAttack = 0;
        // 달리기와 점프 준비 애니메이션이 실행되는 시간 만큼 기다린 후 다음 행 너머 부터 실행
        yield return runAndJumpDelay;
        // 플레이어가 오른쪽에 있다면
        if (playerOnRight == true)
        {
            // 왼쪽으로 조금 뜀
            falseKnightRigidbody.velocity = Vector2.zero;
            falseKnightRigidbody.AddForce(new Vector2(-1 * 160, JUMP_FORCE));
        }
        // 플레이어가 오른쪽에 있지않다면
        else if (playerOnRight == false)
        {
            // 오른쪽을 조금 뜀
            falseKnightRigidbody.velocity = Vector2.zero;
            falseKnightRigidbody.AddForce(new Vector2(160, JUMP_FORCE));
        }

        while (falseKnightAni.GetBool("IsBackShockWave") == true)
        {
            if (falseKnightAni.GetCurrentAnimatorStateInfo(0).IsName("FalseKnightBackJump_Land") == true)
            {
                if (falseKnightAni.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.96f)
                {
                    // 공격 시작 후의 시간을 0으로 초기화
                    timeAfterAttack = 0;
                    // 충격파 공격의 차징 시간동안 기다린 후 다음 행 너머 부터 실행
                    yield return shockWaveDelay;
                    if (falseKnightAni.GetBool("IsStun") == true)
                    {
                        yield break;
                    }

                    StartCoroutine(AttackRemain());
                    if (lookRight == true)
                    {
                        shockWave = Instantiate(shockWavePrefab, new Vector3(transform.position.x + 2, transform.position.y - 2, 0), transform.rotation);
                    }
                    else if (lookRight == false)
                    {
                        shockWave = Instantiate(shockWavePrefab, new Vector3(transform.position.x - 2, transform.position.y - 2, 0), transform.rotation);
                    }
                    yield break;
                    // 여기서 코루틴 종료
                }
            }
            yield return null;
        }
    }

    // 기사의 모닝스타 공격 유지시간을 관리하는 AttackRemain 코루틴
    IEnumerator AttackRemain()
    {
        if(falseKnightAni.GetBool("IsStun") == true)
        {
            yield break;
        }
        // 모닝스타 공격 범위 콜라이더를 활성화 하고 
        attackRange.enabled = true;
        attackRange.transform.localPosition += new Vector3(0.00001f, 0, 0);
        // 공격 유지시간만큼 기다린 후 다음 행 너머 부터 실행
        yield return attackRemain;
        attackRange.transform.localPosition -= new Vector3(0.00001f, 0, 0);
        // 모닝스타 공격 범위 콜라이더를 비활성화 하고
        attackRange.enabled = false;
        // 충격파 공격 중이 true라면
        if (falseKnightAni.GetBool("IsShockWave") == true)
        {
            // 충격파 공격 중을 false로 초기화하고 기본 상태를 true로 초기화
            falseKnightAni.SetBool("IsShockWave", false);
            falseKnightAni.SetBool("IsIdle", true);
        }
        // 내려찍기 공격중이 true라면
        else if (falseKnightAni.GetBool("IsTakeDown") == true)
        {
            // 내려찍기 공격 중을 false로 초기화하고 기본 상태를 true로 초기화
            falseKnightAni.SetBool("IsTakeDown", false);
            falseKnightAni.SetBool("IsIdle", true);
        }
        else if (falseKnightAni.GetBool("IsBackShockWave") == true)
        {
            falseKnightAni.SetBool("IsBackShockWave", false);
            falseKnightAni.SetBool("IsIdle", true);
        }
        yield break;
    }

    // 기사의 투구를 여는 코루틴
    IEnumerator HeadOpen()
    {
        // 1프레임 후 다음 행 너머 부터 실행
        yield return null;
        // 기사의 콜라이더를 끄고 리지드바디를 못움직이게함
        falseKnightCollider.enabled = false;
        falseKnightRigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
        // 땅 착지 후 머리 열리기 까지 대기시간
        yield return beforeHeadOpen;
        falseKnightAni.SetBool("HeadOpen", true);
        //falseKnightRigidbody.gravityScale = 0;
        // 투구열리는 애니메이션 시간만큼 기다린 후 다음 행 너머 부터 실행
        yield return headOpenDelay;
        falseKnightAni.SetBool("HeadOpen", false);
        falseKnightAni.SetBool("IsHeadOpen", true);
        // 여기서 코루틴 종료
        yield break;
    }

    // 기사의 사망 코루틴
    IEnumerator Dead()
    {
        // 1프레임 후 다음 행 너머 부터 실행
        yield return null;
        // 기사의 콜라이더를 끄고 리지드바디를 못움직이게 함
        falseKnightCollider.enabled = false;
        falseKnightRigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
        yield break;
    }

    // 콜라이더 접촉시
    private void OnCollisionEnter2D(Collision2D other)
    {
        // 접촉한 콜라이더가 땅이라면
        if (other.collider.CompareTag("Platform"))
        {
            // IsGround를 true로 초기화, 이동 값을 0으로 초기화, 중력을 1로 초기화
            falseKnightAni.SetBool("IsGround", true);
            falseKnightRigidbody.velocity = Vector2.zero;
            falseKnightRigidbody.gravityScale = 1;
            // 사망이 true이면
            if (falseKnightAni.GetBool("IsDead") == true)
            {
                StartCoroutine(Dead());
            }
            // 스턴이 false 이고 내려찍기 공격중이 true라면
            if (falseKnightAni.GetBool("IsStun") == false)
            {
                if (falseKnightAni.GetBool("IsTakeDown") == true)
                {
                    // 모닝스타 공격 유지시간 관리 코루틴 실행
                    StartCoroutine(AttackRemain());
                }
            }
            // 사망이 false 이고 스턴이 true라면
            if (falseKnightAni.GetBool("IsDead") == false && falseKnightAni.GetBool("IsStun") == true)
            {
                StartCoroutine(HeadOpen());
            }
        }
    }

    // 콜라이더 접촉 해제시
    private void OnCollisionExit2D(Collision2D other)
    {
        // 접촉 해제한 콜라이더가 땅이라면
        if (other.collider.CompareTag("Platform"))
        {
            // IsGround를 false로 초기화
            falseKnightAni.SetBool("IsGround", false);
        }
    }

    // 트리거 접촉 시
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 트리거로 설정된 detectRange의 콜라이더에 플레이어의 콜라이더 감지 시
        if (other.CompareTag("Player"))
        {
            // 플레이어의 게임 오브젝트를 가져오고 detectRange 콜라이더를 끔
            // 1회 발각 시 죽을 때 까지 쫓아옴
            player = other.gameObject;
            detectRange.enabled = false;
        }

        // 기사가 트리거로 설정된 플레이어의 공격을 맞을 시
        if (other.CompareTag("PlayerAttack") && hp > 0)
        {
            hp -= 1;
        }
    }
}