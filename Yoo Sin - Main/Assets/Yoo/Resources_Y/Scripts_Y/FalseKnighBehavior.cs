using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FalseKnighBehavior : MonoBehaviour
{
    // 매직넘버들 상수 처리
    private const int ATTACK_RANGE = 0;

    private const float SHOCKWAVE_CHARGE_TIME = 1.37f;
    private const float TURN_TIME = 0.167f;
    private const float RUN_JUMP_TIME = 0.25f;
    private const float RUNNING = 0.7f;
    private const float ATTACK_REMAIN_TIME = 0.3f;

    private const float RUN_DELAY = 5f;
    private const float ATTACK_DELAY = 2.5f;
    private const float JUMP_FORCE = 400f;
    // 매직넘버들 상수 처리

    private Rigidbody2D falseKnightRigidbody;
    private Animator falseKnightAni;
    private BoxCollider2D detectRange;
    private GameObject player;
    private Collider2D attackRange;

    private Quaternion toLeft = Quaternion.Euler(0, 180, 0);
    private Quaternion toRight = Quaternion.Euler(0, 0, 0);

    private int randomNumber;
    private Skill whatToDo;

    private float distance;
    private float timeAfterAttack;
    private float timeAfterRun;

    private WaitForSeconds shockWaveDelay = new WaitForSeconds(SHOCKWAVE_CHARGE_TIME);
    private WaitForSeconds turnDelay = new WaitForSeconds(TURN_TIME);
    private WaitForSeconds runAndJumpDelay = new WaitForSeconds(RUN_JUMP_TIME);
    private WaitForSeconds running = new WaitForSeconds(RUNNING);
    private WaitForSeconds attackRemain = new WaitForSeconds(ATTACK_REMAIN_TIME);

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
        falseKnightRigidbody = GetComponent<Rigidbody2D>();
        falseKnightAni = GetComponent<Animator>();
        detectRange = gameObject.GetComponent<BoxCollider2D>();
        attackRange = transform.GetChild(ATTACK_RANGE).gameObject.GetComponent<Collider2D>();
        player = null;
        distance = 0;
        lookRight = true;
        timeAfterAttack = 0;
        timeAfterRun = 0;
        whatToDo = 0;
        randomNumber = Random.Range(0, 3);
    }

    void Update()
    {
        // 플레이어가 감지됐다면
        if(player != null)
        {
            // 땅에 붙어있음이 false 이고, 상승하는 힘이 0미만이라면
            if(falseKnightAni.GetBool("IsGround") == false && falseKnightRigidbody.velocity.y < 0)
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

    // 트리거로 설정된 detectRange의 콜라이더에 플레이어의 콜라이더 감지시
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            // 플레이어의 게임 오브젝트를 가져오고 detectRange 콜라이더를 끔
            // 1회 발각 시 죽을 때 까지 쫓아옴
            player = other.gameObject;
            detectRange.enabled = false;
        }
    }

    // 플레이어와 기사 사이의 거리를 계산 및 플레이어가 오른쪽에 있는지 확인하는 함수
    private void PositionDiff()
    {
        if(transform.position.x < player.transform.position.x)
        {
            distance = -1 * (transform.position.x - player.transform.position.x);
            playerOnRight = true;
        }
        else if(transform.position.x >= player.transform.position.x)
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
            else if(playerOnRight == true && lookRight == false)
            {
                StartCoroutine(Turn());
            }
        }
    }

    // 플레이어가 기사로부터 일정거리 이상 멀어지면 실행하는 함수
    private void Move()
    {
        // 기사가 기본 상태이고 플레이어와의 거리가 11이상 떨어졌다면
        if(falseKnightAni.GetBool("IsIdle") == true && distance >= 11)
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
            // 공격 한 후의 시간이 공격 대기시간보다 크고 (공격 대기시간이 지났고) 플레이어와 기사의 거리가 11미만이라면
            if(timeAfterAttack >= ATTACK_DELAY && distance < 11)
            {
                // 3분의 1의 확률로 점프 공격, 충격파 공격, 내려찍기 공격을 함
                if (whatToDo == Skill.jump)
                {
                    StartCoroutine(Jump());
                }
                else if (whatToDo == Skill.shockWave)
                {
                    StartCoroutine(ShockWave());
                }
                else if (whatToDo == Skill.takeDown)
                {
                    StartCoroutine(TakeDown());
                }

                randomNumber = Random.Range(0, 3);

                if(randomNumber == 0)
                {
                    whatToDo = Skill.jump;
                }
                else if(randomNumber == 1)
                {
                    whatToDo = Skill.shockWave;
                }
                else if(randomNumber == 2)
                {
                    whatToDo = Skill.takeDown;
                }
            }
        }
    }

    // 기사가 방향 전환을 하도록 만드는 Turn 코루틴
    IEnumerator Turn()
    {
        falseKnightAni.SetBool("IsIdle", false);
        falseKnightAni.SetBool("IsTurn", true);
        // Turn애니메이션이 실행되는 시간 만큼 기다린 후 198행 너머 부터 실행
        yield return turnDelay;
        // 플레이어가 기사 오른쪽에 있지않으나 기사가 오른쪽을 보고 있다면
        if(playerOnRight == false && lookRight == true)
        {
            // 기사가 왼쪽을 보도록하고, 기사가 오른쪽을 보고있지않다고 저장함
            transform.rotation = toLeft;
            lookRight = false;
        }
        // 플레이어가 기사 오른쪽에 있으나 기사가 오른쪽을 보고 있지않다면
        else if(playerOnRight == true && lookRight == false)
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
        // 달리기와 점프 준비 애니메이션이 실행되는 시간 만큼 기다린 후 225행 너머 부터 실행
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
        // 달리기 지속시간 동안 기다린 후 241행 너머 부터 실행
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
        // 달리기와 점프 준비 애니메이션이 실행되는 시간 만큼 기다린 후 260행 너머 부터 실행
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
        // 달리기와 점프 준비 애니메이션이 실행되는 시간 만큼 기다린 후 288행 너머 부터 실행
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
        // 충격파 공격의 차징 시간동안 기다린 후 316행 너머 부터 실행
        yield return shockWaveDelay;
        //// 여기에 지진파 생성 넣기 ////
        StartCoroutine(AttackRemain());
        // 여기서 코루틴 종료
        yield break;
    }

    // 기사의 모닝스타 공격 유지시간을 관리하는 AttackRemain 코루틴
    IEnumerator AttackRemain()
    {
        // 모닝스타 공격 범위 콜라이더를 활성화 하고 
        attackRange.enabled = true;
        // 공격 유지시간만큼 기다린 후 329행 너머 부터 실행
        yield return attackRemain;
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
            falseKnightAni.SetBool("IsIdle", true);
            falseKnightAni.SetBool("IsTakeDown", false);
        }
        yield break;
    }

    // 콜라이더 접촉시
    private void OnCollisionEnter2D(Collision2D other)
    {
        // 접촉한 콜라이더가 땅이라면
        if(other.collider.CompareTag("Ground"))
        {
            // IsGround를 true로 초기화, 이동 값을 0으로 초기화, 중력을 1로 초기화
            falseKnightAni.SetBool("IsGround", true);
            falseKnightRigidbody.velocity = Vector2.zero;
            falseKnightRigidbody.gravityScale = 1;
            // 내려찍기 공격중이 true라면
            if(falseKnightAni.GetBool("IsTakeDown") == true)
            {
                // 모닝스타 공격 유지시간 관리 코루틴 실행
                StartCoroutine(AttackRemain());
            }
        }
    }

    // 콜라이더 접촉 해제시
    private void OnCollisionExit2D(Collision2D other)
    {
        // 접촉 해제한 콜라이더가 땅이라면
        if (other.collider.CompareTag("Ground"))
        {
            // IsGround를 false로 초기화
            falseKnightAni.SetBool("IsGround", false);
        }
    }
}