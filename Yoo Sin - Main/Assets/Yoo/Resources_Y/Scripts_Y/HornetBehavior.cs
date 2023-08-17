using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HornetBehavior : MonoBehaviour
{
    private GameObject player;
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
    private float timeAfterRun;
    private Collider2D detectRange;
    private int hp = 10;
    private int randomNumber;
    private int stunCount;
    private RaycastHit2D hit;

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
        lookLeft = true;
        timeAfterRun = 0;
        isConer = false;
        //StartCoroutine(ThrowNeedle());
        //StartCoroutine(WhipingGround());
        StartCoroutine(Run());
        //StartCoroutine(Jump());
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null)
        {
            PositionDiff();
            CheckTurn();
            if(isConer == true && hornetAni.GetBool("IsIdle") == true)
            {
                StartCoroutine(Jump());
            }
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
        //Debug.LogFormat(playerOnLeft + ", 플레이어의 x좌표 : " + player.transform.position.x + "\n" + "기사의 x좌표 : " + transform.position.x + "\n");
    }

    private void CheckTurn()
    {
        // 호넷이 땅에 붙어있고, 기본 상태일 때
        if (hornetAni.GetBool("IsGround") == true && hornetAni.GetBool("IsIdle") == true)
        {
            // 플레이어가 왼쪽에 있지않으나 호넷이 왼쪽을 바라보고 있다면
            if (playerOnLeft == false && lookLeft == true)
            {
                Turn();
            }
            // 플레이어가 왼쪽에 있으나 호넷이 왼쪽을 바라보고 있지않다면
            else if (playerOnLeft == true && lookLeft == false)
            {
                Turn();
            }
        }
    }

    private void Turn()
    {
        // 호넷이 왼쪽을 보고 있다면
        if (lookLeft == true)
        {
            // 호넷이 오른쪽을 보도록하고, 호넷이 왼쪽을 보고있지않다고 저장함
            transform.rotation = toRight;
            lookLeft = false;
        }
        // 호넷이 왼쪽을 보고 있지않다면
        else if (lookLeft == false)
        {
            // 호넷이 왼쪽을 보도록하고, 호넷이 왼쪽을 보고있다고 저장함
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

    IEnumerator Run()
    {
        //yield return new WaitForSeconds(1);
        timeAfterRun = 0;
        hornetAni.SetBool("IsIdle", false);
        hornetAni.SetBool("IsRun", true);
        // 왼쪽을 보고있지않다면
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
        // 왼쪽을 보고있다면
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
                    // 오른쪽을 보고있다면
                    if (isConer == true && lookLeft == false)
                    {
                        // 오른쪽으로 일정거리 점프함
                        hornetRigidbody.velocity = Vector2.zero;
                        hornetRigidbody.AddForce(new Vector2(600, JUMP_FORCE));
                        isConer = false;
                        Debug.Log("몇번실행함?");
                        break;
                    }
                    // 왼쪽을 보고있다면
                    else if (isConer == true && lookLeft == true)
                    {
                        // 왼쪽으로 일정거리 점프함
                        hornetRigidbody.velocity = Vector2.zero;
                        hornetRigidbody.AddForce(new Vector2(-1 * 600, JUMP_FORCE));
                        isConer = false;
                        Debug.Log("몇번실행함?");
                        break;
                    }
                    // 오른쪽을 보고있다면
                    else if (lookLeft == false)
                    {
                        // 오른쪽으로 일정거리 점프함
                        hornetRigidbody.velocity = Vector2.zero;
                        hornetRigidbody.AddForce(new Vector2(150, JUMP_FORCE));
                        Debug.Log("몇번실행함?");
                        break;
                    }
                    // 왼쪽을 보고있다면
                    else if (lookLeft == true)
                    {
                        // 왼쪽으로 일정거리 점프함
                        hornetRigidbody.velocity = Vector2.zero;
                        hornetRigidbody.AddForce(new Vector2(-1 * 150, JUMP_FORCE));
                        Debug.Log("몇번실행함?");
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
                    Debug.Log("여기 실행함?");
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
            //Debug.Log("플레이어를 감지함");
        }
    }
}
