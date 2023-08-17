using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HornetBehavior : MonoBehaviour
{
    public GameObject player;
    private Vector3 dir;
    private const float MOVE_SPEED = 7f;
    private const float RUNNING_TIME = 2f;
    private const float JUMP_FORCE = 1200f;

    public GameObject airDashEffectPrefab;
    public GameObject groundDashEffectPrefab;
    public GameObject startWhipEffectPrefab;
    public GameObject throwNeedleEffectPrefab;
    public GameObject needlePrefab;
    public GameObject whipingPrefab;

    private GameObject needle;
    private GameObject whip;
    private GameObject effect;
    private Animator hornetAni;
    private float distance;
    private float dashDistance;
    private float timeAfterRun;
    private Collider2D detectRange;
    private int hp = 10;
    private int randomNumber;
    private int stunCount;
    private float xDiff;
    private float yDiff;
    //private RaycastHit2D hit;
    //private int dashCount = 1;

    private Vector3 playerPosition;
    private Rigidbody2D hornetRigidbody;

    private Quaternion toLeft = Quaternion.Euler(0, 0, 0);
    private Quaternion toRight = Quaternion.Euler(0, 180, 0);

    public bool isConer;
    private bool playerOnLeft;
    private bool lookLeft;
    // Start is called before the first frame update
    void Start()
    {
        detectRange = GetComponent<Collider2D>();
        hornetAni = GetComponent<Animator>();
        hornetRigidbody = GetComponent<Rigidbody2D>();
        timeAfterRun = 0;
        distance = 0;
        lookLeft = true;
        isConer = false;

        StartCoroutine(DashAir());
        //StartCoroutine(ThrowNeedle());
        //StartCoroutine(WhipingGround());
        //StartCoroutine(Run());
        //StartCoroutine(Jump());
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null)
        {
            PositionDiff();
            CheckTurn();

            //if(hornetAni.GetBool("IsIdle") == true && dashCount == 1)
            //{
            //    StartCoroutine(DashGround());
            //}
            //if(isConer == true && hornetAni.GetBool("IsIdle") == true)
            //{
            //    StartCoroutine(Jump());
            //}
        }
    }

    public bool Get_LookLeft()
    {
        return lookLeft;
    }

    private void PositionDiff()
    {
        if (transform.position.x < player.transform.position.x)
        {
            distance = -1 * (transform.position.x - player.transform.position.x);
            playerOnLeft = false;
        }
        else if (transform.position.x >= player.transform.position.x)
        {
            distance = transform.position.x - player.transform.position.x;
            playerOnLeft = true;
        }
        //Debug.LogFormat(playerOnLeft + ", �÷��̾��� x��ǥ : " + player.transform.position.x + "\n" + "����� x��ǥ : " + transform.position.x + "\n");
    }

    private void CheckTurn()
    {
        // ȣ���� ���� �پ��ְ�, �⺻ ������ �� Ȥ�� ȣ���� ����ä�������϶� Ȥ�� ���߿��� �뽬�Ϸ��Ҷ�
        if ((hornetAni.GetBool("IsGround") == true && hornetAni.GetBool("IsIdle") == true) 
            || hornetAni.GetBool("IsWhipA") == true || hornetAni.GetBool("IsDashA") == true)
        {
            // �÷��̾ ���ʿ� ���������� ȣ���� ������ �ٶ󺸰� �ִٸ�
            if (playerOnLeft == false && lookLeft == true)
            {
                Turn();
            }
            // �÷��̾ ���ʿ� ������ ȣ���� ������ �ٶ󺸰� �����ʴٸ�
            else if (playerOnLeft == true && lookLeft == false)
            {
                Turn();
            }
        }
    }

    private void Turn()
    {
        // ȣ���� ������ ���� �ִٸ�
        if (lookLeft == true)
        {
            // ȣ���� �������� �������ϰ�, ȣ���� ������ ���������ʴٰ� ������
            transform.rotation = toRight;
            lookLeft = false;
        }
        // ȣ���� ������ ���� �����ʴٸ�
        else if (lookLeft == false)
        {
            // ȣ���� ������ �������ϰ�, ȣ���� ������ �����ִٰ� ������
            transform.rotation = toLeft;
            lookLeft = true;
        }
    }

    IEnumerator ThrowNeedle()
    {
        //yield return new WaitForSeconds(1);
        hornetAni.SetBool("IsIdle", false);
        hornetAni.SetBool("IsThrow", true);
        while (true)
        {
            if (hornetAni.GetCurrentAnimatorStateInfo(0).IsName("HornetThrow_Ready"))
            {
                if (hornetAni.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95f)
                {
                    needle = Instantiate(needlePrefab, transform.position, transform.rotation);
                    effect = Instantiate(throwNeedleEffectPrefab, transform.position, transform.rotation);
                    break;
                }
            }
            yield return null;
        }

        while (true)
        {
            if (hornetAni.GetCurrentAnimatorStateInfo(0).IsName("HornetThrow_End"))
            {
                if (hornetAni.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95f)
                {
                    hornetAni.SetBool("IsIdle", true);
                    yield break;
                }
            }
            yield return null;
        }
    }

    IEnumerator WhipingGround()
    {
        //yield return new WaitForSeconds(1);
        hornetAni.SetBool("IsIdle", false);
        hornetAni.SetTrigger("WhipStart");
        while (true)
        {
            if (hornetAni.GetCurrentAnimatorStateInfo(0).IsName("HornetWhiping_ReadyG"))
            {
                if (hornetAni.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95f)
                {
                    whip = Instantiate(whipingPrefab, transform.position, transform.rotation);
                    effect = Instantiate(startWhipEffectPrefab, transform.position, transform.rotation);
                    break;
                }
            }
            yield return null;
        }

        while (true)
        {
            if (hornetAni.GetCurrentAnimatorStateInfo(0).IsName("HornetWhiping_EndG"))
            {
                if (hornetAni.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95f)
                {
                    hornetAni.SetBool("IsIdle", true);
                    yield break;
                }
            }
            yield return null;
        }
    }

    IEnumerator Encount()
    {
        hornetAni.SetBool("IsIdle", false);
        hornetAni.SetTrigger("Encount");
        while (true)
        {
            if (hornetAni.GetCurrentAnimatorStateInfo(0).IsName("HornetEncount"))
            {
                if (hornetAni.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95f)
                {
                    hornetAni.SetBool("IsIdle", true);
                    yield break;
                }
            }
            yield return null;
        }
    }

    IEnumerator DashGround()
    {
        //yield return new WaitForSeconds(1);
        //dashCount -= 1;

        playerPosition = player.transform.position;
        dashDistance = playerPosition.x - transform.position.x;
        if(dashDistance <= 0) 
        {
            dashDistance *= -1;
        }
        hornetAni.SetBool("IsIdle", false);
        hornetAni.SetTrigger("DashStart");
        while (true)
        {
            if (hornetAni.GetCurrentAnimatorStateInfo(0).IsName("HornetDashG_Ready"))
            {
                if (hornetAni.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95f)
                {
                    effect = Instantiate(groundDashEffectPrefab, transform.position, transform.rotation);
                    // �÷��̾ ���ʿ� �����ʴٸ�
                    if (playerOnLeft == false)
                    {
                        // ���������� �÷��̾�� �Ÿ����̿� ���� ���� �� (�뽬 ���� ������ �÷��̾� ��ġ�� ���� ��)
                        hornetRigidbody.velocity = Vector2.zero;
                        hornetRigidbody.AddForce(new Vector2(dashDistance * 125, 0));
                    }
                    // �÷��̾ ���ʿ� �ִٸ�
                    else if (playerOnLeft == true)
                    {
                        // �������� �÷��̾�� �Ÿ����̿� ���� ���� �� (�뽬 ���� ������ �÷��̾� ��ġ�� ���� ��)
                        hornetRigidbody.velocity = Vector2.zero;
                        hornetRigidbody.AddForce(new Vector2(-1 * dashDistance * 125, 0));
                    }

                    break;
                }
            }
            yield return null;
        }

        while (true)
        {
            if(lookLeft == true) 
            {
                if (transform.position.x <= playerPosition.x)
                {
                    hornetRigidbody.velocity = Vector2.zero;
                    hornetAni.SetTrigger("DashEnd");
                    Debug.Log("���� ������?1");
                    break;
                }
            }
            else if(lookLeft == false)
            {
                if (transform.position.x >= playerPosition.x)
                {
                    hornetRigidbody.velocity = Vector2.zero;
                    hornetAni.SetTrigger("DashEnd");
                    Debug.Log("���� ������?2");
                    break;
                }
            }
            yield return null;
        }

        while (true)
        {
            if (hornetAni.GetCurrentAnimatorStateInfo(0).IsName("HornetDashG_Land"))
            {
                if (hornetAni.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95f)
                {
                    hornetAni.SetBool("IsIdle", true);
                    yield break;
                }
            }
            yield return null;
        }
    }

    IEnumerator Run()
    {
        //yield return new WaitForSeconds(1);
        timeAfterRun = 0;
        hornetAni.SetBool("IsIdle", false);
        hornetAni.SetBool("IsRun", true);
        // ������ ���������ʴٸ�
        if (lookLeft == false)
        {
            while (true)
            {
                timeAfterRun += Time.deltaTime;
                if (timeAfterRun >= RUNNING_TIME || isConer == true)
                {
                    hornetAni.SetBool("IsIdle", true);
                    hornetAni.SetBool("IsRun", false);
                    yield break;
                }
                transform.position += Time.deltaTime * MOVE_SPEED * transform.right;
                yield return null;
            }
        }
        // ������ �����ִٸ�
        else if (lookLeft == true)
        {
            while (true)
            {
                timeAfterRun += Time.deltaTime;
                if (timeAfterRun >= RUNNING_TIME || isConer == true)
                {
                    hornetAni.SetBool("IsIdle", true);
                    hornetAni.SetBool("IsRun", false);
                    yield break;
                }
                transform.position -= Time.deltaTime * MOVE_SPEED * transform.right;
                yield return null;
            }
        }
    }

    IEnumerator Jump()
    {
        //yield return new WaitForSeconds(1);

        hornetAni.SetBool("IsIdle", false);
        hornetAni.SetBool("IsJump", true);

        while (true)
        {
            if (hornetAni.GetCurrentAnimatorStateInfo(0).IsName("HornetJump_Ready"))
            {
                if (hornetAni.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95f)
                {
                    // �������� �����ִٸ�
                    if (isConer == true && lookLeft == false)
                    {
                        // ���������� �����Ÿ� ������
                        hornetRigidbody.velocity = Vector2.zero;
                        hornetRigidbody.AddForce(new Vector2(600, JUMP_FORCE));
                        isConer = false;
                        //Debug.Log("���������?");
                        break;
                    }
                    // ������ �����ִٸ�
                    else if (isConer == true && lookLeft == true)
                    {
                        // �������� �����Ÿ� ������
                        hornetRigidbody.velocity = Vector2.zero;
                        hornetRigidbody.AddForce(new Vector2(-1 * 600, JUMP_FORCE));
                        isConer = false;
                        //Debug.Log("���������?");
                        break;
                    }
                    // �������� �����ִٸ�
                    else if (lookLeft == false)
                    {
                        // ���������� �����Ÿ� ������
                        hornetRigidbody.velocity = Vector2.zero;
                        hornetRigidbody.AddForce(new Vector2(150, JUMP_FORCE));
                        //Debug.Log("���������?");
                        break;
                    }
                    // ������ �����ִٸ�
                    else if (lookLeft == true)
                    {
                        // �������� �����Ÿ� ������
                        hornetRigidbody.velocity = Vector2.zero;
                        hornetRigidbody.AddForce(new Vector2(-1 * 150, JUMP_FORCE));
                        //Debug.Log("���������?");
                        break;
                    }
                    //break;
                }
            }
            yield return null;
        }

        while (true)
        {
            if (hornetAni.GetCurrentAnimatorStateInfo(0).IsName("HornetJump_Land"))
            {
                if (hornetAni.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9f)
                {
                    hornetAni.SetBool("IsIdle", true);
                    hornetAni.SetBool("IsJump", false);
                    //Debug.Log("���� ������?");
                    yield break;
                }
            }
            yield return null;
        }
    }

    IEnumerator DashAir()
    {
        hornetAni.SetBool("IsIdle", false);
        hornetAni.SetBool("IsDashA", true);

        while (true)
        {
            if (hornetAni.GetCurrentAnimatorStateInfo(0).IsName("HornetDashJump_ReadyA"))
            {
                if (hornetAni.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95f)
                {
                    // �������� �����ִٸ�
                    if (isConer == true && lookLeft == false)
                    {
                        // ���������� �����Ÿ� ������
                        hornetRigidbody.velocity = Vector2.zero;
                        hornetRigidbody.AddForce(new Vector2(600, JUMP_FORCE));
                        isConer = false;
                        //Debug.Log("���������?");
                        break;
                    }
                    // ������ �����ִٸ�
                    else if (isConer == true && lookLeft == true)
                    {
                        // �������� �����Ÿ� ������
                        hornetRigidbody.velocity = Vector2.zero;
                        hornetRigidbody.AddForce(new Vector2(-1 * 600, JUMP_FORCE));
                        isConer = false;
                        //Debug.Log("���������?");
                        break;
                    }
                    // �������� �����ִٸ�
                    else if (lookLeft == false)
                    {
                        // ���������� �����Ÿ� ������
                        hornetRigidbody.velocity = Vector2.zero;
                        hornetRigidbody.AddForce(new Vector2(150, JUMP_FORCE));
                        //Debug.Log("���������?");
                        break;
                    }
                    // ������ �����ִٸ�
                    else if (lookLeft == true)
                    {
                        // �������� �����Ÿ� ������
                        hornetRigidbody.velocity = Vector2.zero;
                        hornetRigidbody.AddForce(new Vector2(-1 * 150, JUMP_FORCE));
                        //Debug.Log("���������?");
                        break;
                    }
                    //break;
                }
            }
            yield return null;
        }

        while(true)
        {
            if(hornetRigidbody.velocity.y <= -10)
            {
                hornetRigidbody.gravityScale = 0;
                hornetRigidbody.velocity = Vector2.zero;

                playerPosition = player.transform.position;
                yDiff = transform.position.y - playerPosition.y;
                xDiff = transform.position.x - playerPosition.x;
                hornetAni.SetBool("IsDashA", false);
                if (lookLeft == false)
                {
                    transform.eulerAngles -= Vector3.forward * (Mathf.Atan2(yDiff, xDiff) * Mathf.Rad2Deg - 180);
                }
                else if (lookLeft == true)
                {
                    transform.eulerAngles += Vector3.forward * (Mathf.Atan2(yDiff, xDiff) * Mathf.Rad2Deg);
                }
                hornetAni.SetTrigger("DashStart");
                break;
            }
            yield return null;
        }

        while (true)
        {
            if (hornetAni.GetCurrentAnimatorStateInfo(0).IsName("HornetDashA_Ready"))
            {
                if (hornetAni.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95f)
                {
                    dashDistance = playerPosition.x - transform.position.x;
                    if (dashDistance <= 0)
                    {
                        dashDistance *= -1;
                    }
                    effect = Instantiate(airDashEffectPrefab, transform.position, transform.rotation);
                    // �÷��̾ ���ʿ� �����ʴٸ�
                    if (playerOnLeft == false)
                    {
                        // ���������� �÷��̾�� �Ÿ����̿� ���� ���� �� (�뽬 ���� ������ �÷��̾� ��ġ�� ���� ��)
                        hornetRigidbody.velocity = Vector2.zero;
                        hornetRigidbody.AddForce(new Vector2(dashDistance * 80, -1 * JUMP_FORCE / 3));
                    }
                    // �÷��̾ ���ʿ� �ִٸ�
                    else if (playerOnLeft == true)
                    {
                        // �������� �÷��̾�� �Ÿ����̿� ���� ���� �� (�뽬 ���� ������ �÷��̾� ��ġ�� ���� ��)
                        hornetRigidbody.velocity = Vector2.zero;
                        hornetRigidbody.AddForce(new Vector2(-1 * dashDistance * 80, -1 * JUMP_FORCE / 3));
                    }
                    break;
                }
            }
            yield return null;
        }

        while (true)
        {
            if (hornetAni.GetCurrentAnimatorStateInfo(0).IsName("HornetDashA_Land"))
            {
                if (hornetAni.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95f)
                {
                    hornetAni.SetBool("IsIdle", true);
                    yield break;
                }
            }
            yield return null;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") == true)
        {
            player = collision.gameObject;
            detectRange.enabled = false;
            //StartCoroutine(Encount());
            //Debug.Log("�÷��̾ ������");
        }
    }
}
