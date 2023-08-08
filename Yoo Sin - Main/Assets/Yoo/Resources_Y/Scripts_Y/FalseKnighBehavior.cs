using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FalseKnighBehavior : MonoBehaviour
{
    private const int ATTACK_RANGE = 0;

    private const float SHOCKWAVE_CHARGE_TIME = 1.37f;
    private const float TURN_TIME = 0.167f;
    private const float RUN_JUMP_TIME = 0.25f;
    private const float RUNNING = 0.7f;
    private const float ATTACK_REMAIN_TIME = 0.3f;

    private const float RUN_DELAY = 5f;
    private const float ATTACK_DELAY = 2.5f;
    private const float JUMP_FORCE = 400f;

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

    // Start is called before the first frame update
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

    // Update is called once per frame
    void Update()
    {
        if(player != null)
        {
            if(falseKnightAni.GetBool("IsGround") == false && falseKnightRigidbody.velocity.y < 0)
            {
                falseKnightRigidbody.gravityScale = 5;
            }

            timeAfterAttack += Time.deltaTime;
            timeAfterRun += Time.deltaTime;
            PositionDiff();
            CheckTurn();
            Move();
            Attack();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            player = other.gameObject;
            detectRange.enabled = false;
        }
    }

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

    private void CheckTurn()
    {
        if (falseKnightAni.GetBool("IsGround") == true && falseKnightAni.GetBool("IsIdle") == true)
        {
            if (playerOnRight == false && lookRight == true)
            {
                StartCoroutine(Turn());
            }

            else if(playerOnRight == true && lookRight == false)
            {
                StartCoroutine(Turn());
            }
        }
    }

    private void Move()
    {
        if(falseKnightAni.GetBool("IsIdle") == true && distance >= 11)
        {
            if (timeAfterRun >= RUN_DELAY)
            {
                StartCoroutine(Run());
            }
        }
    }

    private void Attack()
    {
        if (falseKnightAni.GetBool("IsIdle") == true && falseKnightAni.GetBool("IsRun") == false 
            && falseKnightAni.GetBool("IsTakeDown") == false && falseKnightAni.GetBool("IsShockWave") == false)
        {
            if(timeAfterAttack >= ATTACK_DELAY && distance < 11)
            {
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

    IEnumerator Turn()
    {
        falseKnightAni.SetBool("IsIdle", false);
        falseKnightAni.SetBool("IsTurn", true);

        yield return turnDelay;

        if(playerOnRight == false && lookRight == true)
        {
            transform.rotation = toLeft;
            lookRight = false;
        }
        else if(playerOnRight == true && lookRight == false)
        {
            transform.rotation = toRight;
            lookRight = true;
        }
        falseKnightAni.SetBool("IsIdle", true);
        falseKnightAni.SetBool("IsTurn", false);
        yield break;
    }

    IEnumerator Run()
    {
        falseKnightAni.SetBool("IsIdle", false);
        falseKnightAni.SetBool("IsRun", true);
        yield return runAndJumpDelay;
        if (playerOnRight == true)
        {
            falseKnightRigidbody.velocity = Vector2.zero;
            falseKnightRigidbody.AddForce(new Vector2((distance * 50), 0));
        }
        else if (playerOnRight == false)
        {
            falseKnightRigidbody.velocity = Vector2.zero;
            falseKnightRigidbody.AddForce(new Vector2(-1 * (distance * 50), 0));
        }
        yield return running;
        falseKnightRigidbody.velocity = Vector2.zero;
        falseKnightAni.SetBool("IsIdle", true);
        falseKnightAni.SetBool("IsRun", false);
        timeAfterRun = 0;
        yield break;
    }

    IEnumerator Jump()
    {
        falseKnightAni.SetBool("IsIdle", false);
        falseKnightAni.SetBool("IsJump", true);
        timeAfterAttack = 0;
        yield return runAndJumpDelay;
        if (playerOnRight == true)
        {
            falseKnightRigidbody.velocity = Vector2.zero;
            falseKnightRigidbody.AddForce(new Vector2(distance * 40, JUMP_FORCE));
        }
        else if (playerOnRight == false)
        {
            falseKnightRigidbody.velocity = Vector2.zero;
            falseKnightRigidbody.AddForce(new Vector2(-1 * distance * 40, JUMP_FORCE));
        }
        falseKnightAni.SetBool("IsIdle", true);
        falseKnightAni.SetBool("IsJump", false);
        yield break;
    }

    IEnumerator TakeDown()
    {
        falseKnightAni.SetBool("IsIdle", false);
        falseKnightAni.SetBool("IsTakeDown", true);
        timeAfterAttack = 0;
        yield return runAndJumpDelay;
        if (playerOnRight == true)
        {
            falseKnightRigidbody.velocity = Vector2.zero;
            falseKnightRigidbody.AddForce(new Vector2(distance * 40 - 100, JUMP_FORCE));
        }
        else if (playerOnRight == false)
        {
            falseKnightRigidbody.velocity = Vector2.zero;
            falseKnightRigidbody.AddForce(new Vector2(-1 * (distance * 40 - 100), JUMP_FORCE));
        }
        yield break;
    }

    IEnumerator ShockWave()
    {
        falseKnightAni.SetBool("IsIdle", false);
        falseKnightAni.SetBool("IsShockWave", true);
        timeAfterAttack = 0;
        yield return shockWaveDelay;
        // 여기에 지진파 넣기
        StartCoroutine(AttackRemain());
        yield break;
    }

    IEnumerator AttackRemain()
    {
        attackRange.enabled = true;
        yield return attackRemain;
        attackRange.enabled = false;
        falseKnightAni.SetBool("IsIdle", true);
        falseKnightAni.SetBool("IsShockWave", false);
        yield break;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if(other.collider.CompareTag("Ground"))
        {
            falseKnightAni.SetBool("IsGround", true);
            falseKnightRigidbody.velocity = Vector2.zero;
            falseKnightRigidbody.gravityScale = 1;

            if(falseKnightAni.GetBool("IsTakeDown") == true)
            {
                StartCoroutine(AttackRemain());
                falseKnightAni.SetBool("IsTakeDown",false);
            }
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.collider.CompareTag("Ground"))
        {
            falseKnightAni.SetBool("IsGround", false);
        }
    }
}
