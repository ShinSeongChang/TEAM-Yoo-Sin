using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FalseKnightBehavior : MonoBehaviour
{
    // �����ѹ��� ��� ó��
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
    // �����ѹ��� ��� ó��

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
        // ��� ���°� false�̰�, ���� ���°� false �̸�
        if (falseKnightAni.GetBool("IsDead") == false && falseKnightAni.GetBool("IsStun") == false)
        {
            CheckStun();
            // �÷��̾ �����ƴٸ�
            if (player != null)
            {
                // ���� �پ������� false �̰�, ����ϴ� ���� 0�̸��̶��
                if (falseKnightAni.GetBool("IsGround") == false && falseKnightRigidbody.velocity.y < 0)
                {
                    // �߷°��� 5�� �ʱ�ȭ(���� ���������� ��)
                    falseKnightRigidbody.gravityScale = 5;
                }
                // ����, �޸��� ������ �ð��� ��ŸŸ�Ӹ�ŭ ����
                timeAfterAttack += Time.deltaTime;
                timeAfterRun += Time.deltaTime;
                PositionDiff();
                CheckTurn();
                Move();
                Attack();
            }
        }

        // ��� ���°�  false�̰�, ���� ���°� true �̸�
        if (falseKnightAni.GetBool("IsDead") == false && falseKnightAni.GetBool("IsStun") == true)
        {
            CheckWakeUp();
        }
    }

    // �÷��̾�� ��� ������ �Ÿ��� ��� �� �÷��̾ �����ʿ� �ִ��� Ȯ���ϴ� �Լ�
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
        //Debug.LogFormat(playerOnRight + ", �÷��̾��� x��ǥ : " + player.transform.position.x + "\n" + "����� x��ǥ : " + transform.position.x + "\n");
    }

    // ��簡 ������ȯ�� �ؾ� �ϴ��� üũ�ϴ� �Լ�
    private void CheckTurn()
    {
        // ��簡 ���� �پ��ְ�, �⺻ ������ ��
        if (falseKnightAni.GetBool("IsGround") == true && falseKnightAni.GetBool("IsIdle") == true)
        {
            // �÷��̾ �����ʿ� ���������� ��簡 �������� �ٶ󺸰� �ִٸ�
            if (playerOnRight == false && lookRight == true)
            {
                StartCoroutine(Turn());
            }
            // �÷��̾ �����ʿ� ������ ��簡 �������� �ٶ󺸰� �����ʴٸ�
            else if (playerOnRight == true && lookRight == false)
            {
                StartCoroutine(Turn());
            }
        }
    }

    // �÷��̾ ���κ��� �����Ÿ� �̻� �־����� �����ϴ� �Լ�
    private void Move()
    {
        // ��簡 �⺻ �����̰� �÷��̾���� �Ÿ��� 11�̻� �������ٸ�
        if (falseKnightAni.GetBool("IsIdle") == true && distance >= 11)
        {
            // �޸����� ���ð����� �޸� ���� �ð��� �� ũ�ٸ� (�޸����� ���ð��� �����ٸ�)
            if (timeAfterRun >= RUN_DELAY)
            {
                StartCoroutine(Run());
            }
        }
    }

    // �÷��̾ ����� �������� ���� ���� �� �����ϴ� ���� �Լ�
    private void Attack()
    {
        // ��簡 �⺻ �����̰� �ٸ� �ൿ�� �ϴ� ���� �ƴ϶��
        if (falseKnightAni.GetBool("IsIdle") == true && falseKnightAni.GetBool("IsRun") == false
            && falseKnightAni.GetBool("IsTakeDown") == false && falseKnightAni.GetBool("IsShockWave") == false)
        {
            // ���� �� ���� �ð��� ���� ���ð����� ũ�� (���� ���ð��� ������)
            if (timeAfterAttack >= ATTACK_DELAY)
            {
                // �Ÿ��� 0�̻� 5�̸��� ��� ����, �������, ������ ����� �� �ϳ� ���
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
                // �Ÿ��� 5�̻� 8�̸� �ϰ�� ����, �������, ����� �� �ϳ� ���
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
                // �÷��̾�� ����� �Ÿ��� 8�̻� 11�̸��̶�� �����
                else if (8 <= distance && distance < 11)
                {
                    StartCoroutine(ShockWave());
                }
            }
        }
    }

    // ��簡 ������ �Ǿ�� �ϴ��� üũ�ϴ� CheckStun�Լ�
    private void CheckStun()
    {
        // ����� ü���� 0���϶��
        if (hp <= 0)
        {
            // Stun�Լ� ����
            Stun();
        }
    }

    // ��簡 ���Ͽ��� ����� �ϴ��� üũ�ϴ� CheckWakeUp�Լ�
    private void CheckWakeUp()
    {
        // ����� ���� ������ �Ӹ��� ü���� 0���϶��
        if (head.hp <= 0)
        {
            // WakeUp�Լ� ����
            WakeUp();
        }
    }

    // ��簡 ���� ���·� �Ǵ� Stun�Լ� (���� Ƚ���� ���� Ƚ���� �Ǹ� ������� ����)
    private void Stun()
    {
        // ���� Ƚ���� 1 �߰��ϰ� ����� �ִϸ������� �⺻�������� false, ���ϻ������� true�� �ʱ�ȭ, StunStartƮ���� Ȱ��ȭ
        stunCount += 1;
        if (stunCount == 3)
        {
            falseKnightAni.SetBool("IsDead", true);
        }
        falseKnightAni.SetBool("IsIdle", false);
        falseKnightAni.SetBool("IsStun", true);
        falseKnightAni.SetTrigger("StunStart");
        // �������� ���� �ִٸ�
        if (lookRight == true)
        {
            // �������� �����Ÿ� ���ư�
            falseKnightRigidbody.velocity = Vector2.zero;
            falseKnightRigidbody.AddForce(new Vector2((-1 * 120), JUMP_FORCE / 2));
        }
        // �������� ���� ���� �ʴٸ�
        else if (playerOnRight == false)
        {
            // ���������� �����Ÿ� ���ư�
            falseKnightRigidbody.velocity = Vector2.zero;
            falseKnightRigidbody.AddForce(new Vector2(120, JUMP_FORCE / 2));
        }
    }

    // ��簡 ���� ���¿��� ����� WakeUp�Լ�
    private void WakeUp()
    {
        // ����� �ݶ��̴��� Ű�� ������ �ٵ��� ������ ���� �ʱ⿡ ������ ������ ������
        falseKnightCollider.enabled = true;
        falseKnightRigidbody.constraints = RigidbodyConstraints2D.None;
        falseKnightRigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
        //falseKnightRigidbody.gravityScale = 1;
        // ����� �ִϸ������� ���������� false, ���ϻ������� false, �⺻�������� true�� �ʱ�ȭ
        falseKnightAni.SetBool("IsHeadOpen", false);
        falseKnightAni.SetBool("IsStun", false);
        falseKnightAni.SetBool("IsIdle", true);
        // ����� ü�� �ٽ� ȸ��
        hp = 13;
        // ����� ���� �ð� 0���� �ٽ� ����
        timeAfterAttack = 0;
        timeAfterRun = 0;
    }

    // ��簡 ���� ��ȯ�� �ϵ��� ����� Turn �ڷ�ƾ
    IEnumerator Turn()
    {
        falseKnightAni.SetBool("IsIdle", false);
        falseKnightAni.SetBool("IsTurn", true);
        // Turn�ִϸ��̼��� ����Ǵ� �ð� ��ŭ ��ٸ� �� ���� �� �ʸ� ���� ����
        yield return turnDelay;
        // �÷��̾ ��� �����ʿ� ���������� ��簡 �������� ���� �ִٸ�
        if (playerOnRight == false && lookRight == true)
        {
            // ��簡 ������ �������ϰ�, ��簡 �������� ���������ʴٰ� ������
            transform.rotation = toLeft;
            lookRight = false;
        }
        // �÷��̾ ��� �����ʿ� ������ ��簡 �������� ���� �����ʴٸ�
        else if (playerOnRight == true && lookRight == false)
        {
            // ��簡 �������� �������ϰ�, ��簡 �������� �����ִٰ� ������
            transform.rotation = toRight;
            lookRight = true;
        }
        falseKnightAni.SetBool("IsIdle", true);
        falseKnightAni.SetBool("IsTurn", false);
        // ���⼭ �ڷ�ƾ ����
        yield break;
    }

    // ��簡 �޸������ϴ� Run �ڷ�ƾ
    IEnumerator Run()
    {
        falseKnightAni.SetBool("IsIdle", false);
        falseKnightAni.SetBool("IsRun", true);
        // �޸���� ���� �غ� �ִϸ��̼��� ����Ǵ� �ð� ��ŭ ��ٸ� �� ���� �� �ʸ� ���� ����
        yield return runAndJumpDelay;
        // �÷��̾ �����ʿ� �ִٸ�
        if (playerOnRight == true)
        {
            // ��簡 �Ÿ� ���� * 50 ��ŭ�� ���� ���� (���������� ���� ����)
            falseKnightRigidbody.velocity = Vector2.zero;
            falseKnightRigidbody.AddForce(new Vector2((distance * 50), 0));
        }
        // �÷��̾ �����ʿ� �����ʴٸ�
        else if (playerOnRight == false)
        {
            // ��簡 �Ÿ� ���� * 50 * -1 ��ŭ�� ���� ���� (�������� ���� ����)
            falseKnightRigidbody.velocity = Vector2.zero;
            falseKnightRigidbody.AddForce(new Vector2(-1 * (distance * 50), 0));
        }
        // �޸��� ���ӽð� ���� ��ٸ� �� ���� �� �ʸ� ���� ����
        yield return running;
        // ��簡 ���ߵ�����
        falseKnightRigidbody.velocity = Vector2.zero;
        falseKnightAni.SetBool("IsIdle", true);
        falseKnightAni.SetBool("IsRun", false);
        // �޸� ���� �ð��� 0���� �ʱ�ȭ
        timeAfterRun = 0;
        // ���⼭ �ڷ�ƾ ����
        yield break;
    }

    // ��簡 �������� �ϵ��� �ϴ� Jump �ڷ�ƾ
    IEnumerator Jump()
    {
        falseKnightAni.SetBool("IsIdle", false);
        falseKnightAni.SetBool("IsJump", true);
        // ���� ���� ���� �ð��� 0���� �ʱ�ȭ
        timeAfterAttack = 0;
        // �޸���� ���� �غ� �ִϸ��̼��� ����Ǵ� �ð� ��ŭ ��ٸ� �� ���� �� �ʸ� ���� ����
        yield return runAndJumpDelay;
        // �÷��̾ �����ʿ� �ִٸ�
        if (playerOnRight == true)
        {
            // ���������� �÷��̾�� �Ÿ����̿� ���� ���� �� (���� ���� ������ �÷��̾� ��ġ�� ���� ��)
            falseKnightRigidbody.velocity = Vector2.zero;
            falseKnightRigidbody.AddForce(new Vector2(distance * 40, JUMP_FORCE));
        }
        // �÷��̾ �����ʿ� �����ʴٸ�
        else if (playerOnRight == false)
        {
            // �������� �÷��̾�� �Ÿ����̿� ���� ���� �� (���� ���� ������ �÷��̾� ��ġ�� ���� ��)
            falseKnightRigidbody.velocity = Vector2.zero;
            falseKnightRigidbody.AddForce(new Vector2(-1 * distance * 40, JUMP_FORCE));
        }
        falseKnightAni.SetBool("IsIdle", true);
        falseKnightAni.SetBool("IsJump", false);
        // ���⼭ �ڷ�ƾ ����
        yield break;
    }

    // ��簡 ������� ������ �ϵ��� �ϴ� TakeDown �ڷ�ƾ
    IEnumerator TakeDown()
    {
        falseKnightAni.SetBool("IsIdle", false);
        falseKnightAni.SetBool("IsTakeDown", true);
        // ���� ���� ���� �ð��� 0���� �ʱ�ȭ
        timeAfterAttack = 0;
        // �޸���� ���� �غ� �ִϸ��̼��� ����Ǵ� �ð� ��ŭ ��ٸ� �� ���� �� �ʸ� ���� ����
        yield return runAndJumpDelay;
        // �÷��̾ �����ʿ� �ִٸ�
        if (playerOnRight == true)
        {
            // ���������� �÷��̾�� �Ÿ����̿� ���� ���� �� (���� ���� ������ �÷��̾� ��ġ�� ������ ���� ��)
            falseKnightRigidbody.velocity = Vector2.zero;
            falseKnightRigidbody.AddForce(new Vector2(distance * 40 - 100, JUMP_FORCE));
        }
        // �÷��̾ �����ʿ� �����ʴٸ�
        else if (playerOnRight == false)
        {
            // �������� �÷��̾�� �Ÿ����̿� ���� ���� �� (���� ���� ������ �÷��̾� ��ġ�� �������� ���� ��)
            falseKnightRigidbody.velocity = Vector2.zero;
            falseKnightRigidbody.AddForce(new Vector2(-1 * (distance * 40 - 100), JUMP_FORCE));
        }
        // ���⼭ �ڷ�ƾ ����
        yield break;
    }

    // ��簡 ����� ������ �ϵ��� �ϴ� ShockWave �ڷ�ƾ
    IEnumerator ShockWave()
    {
        falseKnightAni.SetBool("IsIdle", false);
        falseKnightAni.SetBool("IsShockWave", true);
        // ���� ���� ���� �ð��� 0���� �ʱ�ȭ
        timeAfterAttack = 0;
        // ����� ������ ��¡ �ð����� ��ٸ� �� ���� �� �ʸ� ���� ����
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
        // ���⼭ �ڷ�ƾ ����
        yield break;
    }

    // ��簡 �Ĺ������� ����� ������ �ϵ��� �ϴ� BackShockWave �ڷ�ƾ
    IEnumerator BackShockWave()
    {
        falseKnightAni.SetBool("IsIdle", false);
        falseKnightAni.SetBool("IsBackShockWave", true);
        // ���� ���� ���� �ð��� 0���� �ʱ�ȭ
        timeAfterAttack = 0;
        // �޸���� ���� �غ� �ִϸ��̼��� ����Ǵ� �ð� ��ŭ ��ٸ� �� ���� �� �ʸ� ���� ����
        yield return runAndJumpDelay;
        // �÷��̾ �����ʿ� �ִٸ�
        if (playerOnRight == true)
        {
            // �������� ���� ��
            falseKnightRigidbody.velocity = Vector2.zero;
            falseKnightRigidbody.AddForce(new Vector2(-1 * 160, JUMP_FORCE));
        }
        // �÷��̾ �����ʿ� �����ʴٸ�
        else if (playerOnRight == false)
        {
            // �������� ���� ��
            falseKnightRigidbody.velocity = Vector2.zero;
            falseKnightRigidbody.AddForce(new Vector2(160, JUMP_FORCE));
        }

        while (falseKnightAni.GetBool("IsBackShockWave") == true)
        {
            if (falseKnightAni.GetCurrentAnimatorStateInfo(0).IsName("FalseKnightBackJump_Land") == true)
            {
                if (falseKnightAni.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.96f)
                {
                    // ���� ���� ���� �ð��� 0���� �ʱ�ȭ
                    timeAfterAttack = 0;
                    // ����� ������ ��¡ �ð����� ��ٸ� �� ���� �� �ʸ� ���� ����
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
                    // ���⼭ �ڷ�ƾ ����
                }
            }
            yield return null;
        }
    }

    // ����� ��׽�Ÿ ���� �����ð��� �����ϴ� AttackRemain �ڷ�ƾ
    IEnumerator AttackRemain()
    {
        if(falseKnightAni.GetBool("IsStun") == true)
        {
            yield break;
        }
        // ��׽�Ÿ ���� ���� �ݶ��̴��� Ȱ��ȭ �ϰ� 
        attackRange.enabled = true;
        attackRange.transform.localPosition += new Vector3(0.00001f, 0, 0);
        // ���� �����ð���ŭ ��ٸ� �� ���� �� �ʸ� ���� ����
        yield return attackRemain;
        attackRange.transform.localPosition -= new Vector3(0.00001f, 0, 0);
        // ��׽�Ÿ ���� ���� �ݶ��̴��� ��Ȱ��ȭ �ϰ�
        attackRange.enabled = false;
        // ����� ���� ���� true���
        if (falseKnightAni.GetBool("IsShockWave") == true)
        {
            // ����� ���� ���� false�� �ʱ�ȭ�ϰ� �⺻ ���¸� true�� �ʱ�ȭ
            falseKnightAni.SetBool("IsShockWave", false);
            falseKnightAni.SetBool("IsIdle", true);
        }
        // ������� �������� true���
        else if (falseKnightAni.GetBool("IsTakeDown") == true)
        {
            // ������� ���� ���� false�� �ʱ�ȭ�ϰ� �⺻ ���¸� true�� �ʱ�ȭ
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

    // ����� ������ ���� �ڷ�ƾ
    IEnumerator HeadOpen()
    {
        // 1������ �� ���� �� �ʸ� ���� ����
        yield return null;
        // ����� �ݶ��̴��� ���� ������ٵ� �������̰���
        falseKnightCollider.enabled = false;
        falseKnightRigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
        // �� ���� �� �Ӹ� ������ ���� ���ð�
        yield return beforeHeadOpen;
        falseKnightAni.SetBool("HeadOpen", true);
        //falseKnightRigidbody.gravityScale = 0;
        // ���������� �ִϸ��̼� �ð���ŭ ��ٸ� �� ���� �� �ʸ� ���� ����
        yield return headOpenDelay;
        falseKnightAni.SetBool("HeadOpen", false);
        falseKnightAni.SetBool("IsHeadOpen", true);
        // ���⼭ �ڷ�ƾ ����
        yield break;
    }

    // ����� ��� �ڷ�ƾ
    IEnumerator Dead()
    {
        // 1������ �� ���� �� �ʸ� ���� ����
        yield return null;
        // ����� �ݶ��̴��� ���� ������ٵ� �������̰� ��
        falseKnightCollider.enabled = false;
        falseKnightRigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
        yield break;
    }

    // �ݶ��̴� ���˽�
    private void OnCollisionEnter2D(Collision2D other)
    {
        // ������ �ݶ��̴��� ���̶��
        if (other.collider.CompareTag("Platform"))
        {
            // IsGround�� true�� �ʱ�ȭ, �̵� ���� 0���� �ʱ�ȭ, �߷��� 1�� �ʱ�ȭ
            falseKnightAni.SetBool("IsGround", true);
            falseKnightRigidbody.velocity = Vector2.zero;
            falseKnightRigidbody.gravityScale = 1;
            // ����� true�̸�
            if (falseKnightAni.GetBool("IsDead") == true)
            {
                StartCoroutine(Dead());
            }
            // ������ false �̰� ������� �������� true���
            if (falseKnightAni.GetBool("IsStun") == false)
            {
                if (falseKnightAni.GetBool("IsTakeDown") == true)
                {
                    // ��׽�Ÿ ���� �����ð� ���� �ڷ�ƾ ����
                    StartCoroutine(AttackRemain());
                }
            }
            // ����� false �̰� ������ true���
            if (falseKnightAni.GetBool("IsDead") == false && falseKnightAni.GetBool("IsStun") == true)
            {
                StartCoroutine(HeadOpen());
            }
        }
    }

    // �ݶ��̴� ���� ������
    private void OnCollisionExit2D(Collision2D other)
    {
        // ���� ������ �ݶ��̴��� ���̶��
        if (other.collider.CompareTag("Platform"))
        {
            // IsGround�� false�� �ʱ�ȭ
            falseKnightAni.SetBool("IsGround", false);
        }
    }

    // Ʈ���� ���� ��
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Ʈ���ŷ� ������ detectRange�� �ݶ��̴��� �÷��̾��� �ݶ��̴� ���� ��
        if (other.CompareTag("Player"))
        {
            // �÷��̾��� ���� ������Ʈ�� �������� detectRange �ݶ��̴��� ��
            // 1ȸ �߰� �� ���� �� ���� �Ѿƿ�
            player = other.gameObject;
            detectRange.enabled = false;
        }

        // ��簡 Ʈ���ŷ� ������ �÷��̾��� ������ ���� ��
        if (other.CompareTag("PlayerAttack") && hp > 0)
        {
            hp -= 1;
        }
    }
}