using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HornetBehavior : MonoBehaviour
{
    private GameObject player;
    private const float MOVE_SPEED = 7f;
    private const float RUNNING_TIME = 0.7f;
    private const float EVADE_SPEED = 14f;
    private const float EVADE_TIME = 0.3f;
    private const float JUMP_FORCE = 1200f;
    private const float ACT_TERM = 0.4f;
    private const float EVADE_COOLDOWN = 5f;
    private const float DASH_DISTANCE = 7f;

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
    private float dashDistanceX;
    private float dashDistanceY;
    private float timeAfterRun;
    private float timeAfterEvade;
    private float timeAfterAct;
    private float timeAfterEvaded;
    private Collider2D detectRange;
    private int hp = 10;
    private int stunHp = 1;
    private int randomNumber;
    private int stunCount;
    private float xDiff;
    private float yDiff;
    //private RaycastHit2D hit;
    //private int dashCount = 1;

    private Vector2 dashAirDir;
    private Vector2 dashGroundDir;
    private Vector3 playerPosition;
    private float xPositionToDash;
    private Rigidbody2D hornetRigidbody;

    private Quaternion toLeft = Quaternion.Euler(0, 0, 0);
    private Quaternion toRight = Quaternion.Euler(0, 180, 0);

    public bool isConer;
    public bool hitPlayer;
    private bool playerOnLeft;
    private bool lookLeft;
    // Start is called before the first frame update
    void Start()
    {
        detectRange = GetComponent<Collider2D>();
        hornetAni = GetComponent<Animator>();
        hornetRigidbody = GetComponent<Rigidbody2D>();
        timeAfterRun = 0;
        timeAfterEvade = 0;
        timeAfterAct = 0;
        distance = 0;
        lookLeft = true;
        isConer = false;
        //StartCoroutine(DashAir());
        //StartCoroutine(ThrowNeedle());
        //StartCoroutine(WhipingGround());
        //StartCoroutine(Run());
        //StartCoroutine(Jump());
        //StartCoroutine(WhipingAir());
        //StartCoroutine(Evade());
    }

    // Update is called once per frame
    void Update()
    {
        if (hornetAni.GetBool("IsDead") == false && player != null)
        {
            if (hornetAni.GetBool("IsStun") == false)
            {
                CheckStun();
                PositionDiff();
                CheckTurn();
                timeAfterEvaded += Time.deltaTime;

                if (hornetAni.GetCurrentAnimatorStateInfo(0).IsName("Hornet_idle") == true && hornetAni.GetBool("IsGround") == true)
                {
                    timeAfterAct += Time.deltaTime;
                }

                if (timeAfterAct >= ACT_TERM)
                {
                    Act();
                    timeAfterAct = 0;
                }
            }

            if (hornetAni.GetBool("IsStun") == true)
            {
                CheckWakeUp();
            }
            //if(hornetAni.GetBool("IsIdle") == true && dashCount == 1)
            //{
            //    StartCoroutine(DashGround());
            //}
            //if (isConer == true && hornetAni.GetBool("IsIdle") == true)
            //{
            //    StartCoroutine(DashAir());
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
        //Debug.LogFormat(playerOnLeft + ", 플레이어의 x좌표 : " + player.transform.position.x + "\n" + "기사의 x좌표 : " + transform.position.x + "\n");
    }

    private void CheckTurn()
    {
        // 호넷이 땅에 붙어있고, 기본 상태일 때 혹은 공중에서 대쉬하려할때
        if ((hornetAni.GetBool("IsGround") == true && hornetAni.GetCurrentAnimatorStateInfo(0).IsName("Hornet_idle") == true)
            || hornetAni.GetBool("IsDashA") == true)
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

    private void Act()
    {
        if (isConer == true)
        {
            randomNumber = Random.Range(0, 5);
            switch (randomNumber)
            {
                case 0:
                    StartCoroutine(Jump());
                    break;
                case 1:
                    StartCoroutine(DashGround());
                    break;
                case 2:
                    StartCoroutine(WhipingAir());
                    break;
                case 3:
                    StartCoroutine(DashAir());
                    break;
                case 4:
                    StartCoroutine(ThrowNeedle());
                    break;
            }
            return;
        }

        if (0 <= distance && distance < 4)
        {
            if (timeAfterEvaded >= EVADE_COOLDOWN)
            {
                randomNumber = Random.Range(0, 7);
            }
            else
            {
                randomNumber = Random.Range(0, 6);
            }

            switch (randomNumber)
            {
                case 0:
                    StartCoroutine(Run());
                    break;
                case 1:
                    StartCoroutine(Jump());
                    break;
                case 2:
                    StartCoroutine(ThrowNeedle());
                    break;
                case 3:
                    StartCoroutine(WhipingGround());
                    break;
                case 4:
                    StartCoroutine(DashGround());
                    break;
                case 5:
                    StartCoroutine(WhipingAir());
                    break;
                case 6:
                    StartCoroutine(Evade());
                    break;
            }
        }
        else if (4 <= distance && distance < 8)
        {
            randomNumber = Random.Range(0, 7);
            switch (randomNumber)
            {
                case 0:
                    StartCoroutine(Run());
                    break;
                case 1:
                    StartCoroutine(Jump());
                    break;
                case 2:
                    StartCoroutine(ThrowNeedle());
                    break;
                case 3:
                    StartCoroutine(WhipingGround());
                    break;
                case 4:
                    StartCoroutine(DashGround());
                    break;
                case 5:
                    StartCoroutine(WhipingAir());
                    break;
                case 6:
                    StartCoroutine(DashAir());
                    break;
            }
        }
        else if (8 <= distance)
        {
            randomNumber = Random.Range(0, 6);
            switch (randomNumber)
            {
                case 0:
                    StartCoroutine(Run());
                    break;
                case 1:
                    StartCoroutine(Jump());
                    break;
                case 2:
                    StartCoroutine(DashGround());
                    break;
                case 3:
                    StartCoroutine(WhipingAir());
                    break;
                case 4:
                    StartCoroutine(DashAir());
                    break;
                case 5:
                    StartCoroutine(WhipingAir());
                    break;
            }
        }
    }

    IEnumerator Stun()
    {
        for(int i = 0; i < hornetAni.parameters.Length; i ++)
        {
            hornetAni.parameters[i].defaultBool = false;
        }
        hornetAni.SetTrigger("StunStart");
        hornetAni.SetBool("IsGround", true);
        hornetAni.SetBool("IsStun", true);
        hornetAni.SetBool("IsIdle", true);
        stunCount += 1;

        if (lookLeft == true)
        {
            hornetRigidbody.gravityScale = 5;
            hornetRigidbody.velocity = Vector2.zero;
            hornetRigidbody.AddForce(new Vector2(500, 600));
        }
        else if(lookLeft == false)
        {
            hornetRigidbody.gravityScale = 5;
            hornetRigidbody.velocity = Vector2.zero;
            hornetRigidbody.AddForce(new Vector2(-500, 600));
        }

        while (true)
        {
            if (hornetAni.GetCurrentAnimatorStateInfo(0).IsName("HornetStunPart1"))
            {
                if (hornetAni.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95f && hornetAni.GetBool("IsGround") == true)
                {
                    hornetAni.SetBool("IsIdle", false);
                    hornetRigidbody.velocity = Vector2.zero;
                    hornetRigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
                    yield break;
                }
            }
            yield return null;
        }
    }

    private void CheckStun()
    {
        if (hp <= 0)
        {
            StopAllCoroutines();
            StartCoroutine(Stun());
        }
    }

    private void WakeUp()
    {
        hornetAni.SetTrigger("WakeUp");
        hornetAni.SetBool("IsStun", false);
        hornetAni.SetBool("IsIdle", true);
        hornetRigidbody.constraints = RigidbodyConstraints2D.None;
        hornetRigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
        timeAfterAct = 0;
        hp = 10;
        stunHp = 1;
    }

    private void CheckWakeUp()
    {
        if (stunHp <= 0)
        {
            WakeUp();
        }
    }

    public void HpDown()
    {
        hp -= 1;
    }

    public void StunHpDown()
    {
        stunHp -= 1;
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
                if (timeAfterRun >= RUNNING_TIME || isConer == true || hitPlayer == true)
                {
                    hornetAni.SetBool("IsRun", false);
                    hornetAni.SetBool("IsIdle", true);
                    if (hitPlayer == true)
                    {
                        hitPlayer = false;
                    }
                    yield break;
                }
                transform.position -= Time.deltaTime * MOVE_SPEED * transform.right;
                yield return null;
            }
        }
        // 왼쪽을 보고있다면
        if (lookLeft == true)
        {
            while (true)
            {
                timeAfterRun += Time.deltaTime;
                if (timeAfterRun >= RUNNING_TIME || isConer == true || hitPlayer == true)
                {
                    hornetAni.SetBool("IsRun", false);
                    hornetAni.SetBool("IsIdle", true);
                    if (hitPlayer == true)
                    {
                        hitPlayer = false;
                    }
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
                        //Debug.Log("몇번실행함?");
                        break;
                    }
                    // 왼쪽을 보고있다면
                    else if (isConer == true && lookLeft == true)
                    {
                        // 왼쪽으로 일정거리 점프함
                        hornetRigidbody.velocity = Vector2.zero;
                        hornetRigidbody.AddForce(new Vector2(-1 * 600, JUMP_FORCE));
                        isConer = false;
                        //Debug.Log("몇번실행함?");
                        break;
                    }
                    // 오른쪽을 보고있다면
                    else if (lookLeft == false)
                    {
                        // 오른쪽으로 일정거리 점프함
                        hornetRigidbody.velocity = Vector2.zero;
                        hornetRigidbody.AddForce(new Vector2(150, JUMP_FORCE));
                        //Debug.Log("몇번실행함?");
                        break;
                    }
                    // 왼쪽을 보고있다면
                    else if (lookLeft == true)
                    {
                        // 왼쪽으로 일정거리 점프함
                        hornetRigidbody.velocity = Vector2.zero;
                        hornetRigidbody.AddForce(new Vector2(-1 * 150, JUMP_FORCE));
                        //Debug.Log("몇번실행함?");
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
                if (hornetAni.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95f)
                {
                    hornetAni.SetBool("IsIdle", true);
                    hornetAni.SetBool("IsJump", false);
                    if (hitPlayer == true)
                    {
                        hitPlayer = false;
                    }
                    //Debug.Log("여기 실행함?");
                    yield break;
                }
            }
            yield return null;
        }
    }

    IEnumerator Evade()
    {
        yield return new WaitForSeconds(1);
        timeAfterEvade = 0;
        hornetAni.SetBool("IsIdle", false);
        hornetAni.SetBool("IsEvade", true);

        while (true)
        {
            if (hornetAni.GetCurrentAnimatorStateInfo(0).IsName("HornetEvade_Ready"))
            {
                if (hornetAni.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95f)
                {
                    break;
                }
            }
            yield return null;
        }

        // 플레이어가 왼쪽에 없다면
        if (playerOnLeft == false)
        {
            while (true)
            {
                timeAfterEvade += Time.deltaTime;
                if (timeAfterEvade >= EVADE_TIME || isConer == true)
                {
                    hornetAni.SetBool("IsEvade", false);
                    break;
                }
                transform.position -= Time.deltaTime * EVADE_SPEED * transform.right;
                yield return null;
            }
        }
        // 플레이어가 왼쪽에 있다면
        else if (playerOnLeft == true)
        {
            while (true)
            {
                timeAfterEvade += Time.deltaTime;
                if (timeAfterEvade >= EVADE_TIME || isConer == true)
                {
                    hornetAni.SetBool("IsEvade", false);
                    break;
                }
                transform.position += Time.deltaTime * EVADE_SPEED * transform.right;
                yield return null;
            }
        }

        while (true)
        {
            if (hornetAni.GetCurrentAnimatorStateInfo(0).IsName("HornetEvade_End"))
            {
                if (hornetAni.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95f)
                {
                    hornetAni.SetBool("IsIdle", true);
                    timeAfterEvaded = 0;
                    yield break;
                }
            }
            yield return null;
        }
    }

    IEnumerator ThrowNeedle()
    {
        //yield return new WaitForSeconds(1);
        hornetAni.SetBool("IsIdle", false);
        hornetAni.SetBool("IsThrow", true);

        if (isConer == true)
        {
            isConer = false;
        }

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

    IEnumerator DashGround()
    {
        //yield return new WaitForSeconds(1);
        //dashCount -= 1;

        hornetAni.SetBool("IsIdle", false);
        hornetAni.SetTrigger("DashStart");

        if (isConer == true)
        {
            isConer = false;
        }

        while (true)
        {
            if (hornetAni.GetCurrentAnimatorStateInfo(0).IsName("HornetDashG_Ready"))
            {
                if (hornetAni.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95f)
                {
                    effect = Instantiate(groundDashEffectPrefab, transform.position, transform.rotation);
                    // 플레이어가 왼쪽에 있지않다면
                    if (playerOnLeft == false)
                    {
                        xPositionToDash = transform.position.x + DASH_DISTANCE;
                        // 오른쪽으로 플레이어와 거리차이에 따라 힘을 줌 (대쉬 시작 시점의 플레이어 위치를 향한 힘)
                        hornetRigidbody.velocity = Vector2.zero;
                        hornetRigidbody.AddForce(new Vector2(1000f, 0));
                    }
                    // 플레이어가 왼쪽에 있다면
                    else if (playerOnLeft == true)
                    {
                        xPositionToDash = transform.position.x - DASH_DISTANCE;
                        // 왼쪽으로 플레이어와 거리차이에 따라 힘을 줌 (대쉬 시작 시점의 플레이어 위치를 향한 힘)
                        hornetRigidbody.velocity = Vector2.zero;
                        hornetRigidbody.AddForce(new Vector2(-1000f, 0));
                    }

                    break;
                }
            }
            yield return null;
        }

        while (true)
        {
            if (lookLeft == true)
            {
                if (transform.position.x <= xPositionToDash || hitPlayer == true || isConer == true)
                {
                    hornetRigidbody.velocity = Vector2.zero;
                    hornetAni.SetTrigger("DashEnd");
                    Debug.Log("여기 실행함?1");
                    break;
                }
            }
            else if (lookLeft == false)
            {
                if (transform.position.x >= xPositionToDash || hitPlayer == true || isConer == true)
                {
                    hornetRigidbody.velocity = Vector2.zero;
                    hornetAni.SetTrigger("DashEnd");
                    Debug.Log("여기 실행함?2");
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
                    if (hitPlayer == true)
                    {
                        hitPlayer = false;
                    }
                    yield break;
                }
            }
            yield return null;
        }
    }

    IEnumerator WhipingAir()
    {
        hornetAni.SetBool("IsIdle", false);
        hornetAni.SetBool("IsWhipA", true);

        while (true)
        {
            if (hornetAni.GetCurrentAnimatorStateInfo(0).IsName("HornetWhipingJump_ReadyA"))
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
                        //Debug.Log("몇번실행함?");
                        break;
                    }
                    // 왼쪽을 보고있다면
                    else if (isConer == true && lookLeft == true)
                    {
                        // 왼쪽으로 일정거리 점프함
                        hornetRigidbody.velocity = Vector2.zero;
                        hornetRigidbody.AddForce(new Vector2(-1 * 600, JUMP_FORCE));
                        isConer = false;
                        //Debug.Log("몇번실행함?");
                        break;
                    }
                    // 오른쪽을 보고있다면
                    else if (lookLeft == false)
                    {
                        // 오른쪽으로 일정거리 점프함
                        hornetRigidbody.velocity = Vector2.zero;
                        hornetRigidbody.AddForce(new Vector2(150, JUMP_FORCE));
                        //Debug.Log("몇번실행함?");
                        break;
                    }
                    // 왼쪽을 보고있다면
                    else if (lookLeft == true)
                    {
                        // 왼쪽으로 일정거리 점프함
                        hornetRigidbody.velocity = Vector2.zero;
                        hornetRigidbody.AddForce(new Vector2(-1 * 150, JUMP_FORCE));
                        //Debug.Log("몇번실행함?");
                        break;
                    }
                    //break;
                }
            }
            yield return null;
        }

        while (true)
        {
            if (hornetRigidbody.velocity.y <= -18)
            {
                hornetRigidbody.gravityScale = 0;
                hornetRigidbody.velocity = Vector2.zero;
                hornetAni.SetBool("IsWhipA", false);
                hornetAni.SetTrigger("WhipStart");
                break;
            }
            yield return null;
        }

        while (true)
        {
            if (hornetAni.GetCurrentAnimatorStateInfo(0).IsName("HornetWhiping_ReadyA"))
            {
                if (hornetAni.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95f)
                {

                    effect = Instantiate(startWhipEffectPrefab, transform.position, transform.rotation);
                    whip = Instantiate(whipingPrefab, transform.position, transform.rotation);
                    break;
                }
            }
            yield return null;
        }

        while (true)
        {
            if (hornetAni.GetCurrentAnimatorStateInfo(0).IsName("HornetWhiping_EndA"))
            {
                if (hornetAni.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95f)
                {
                    hornetRigidbody.gravityScale = 5;
                    hornetAni.SetBool("IsIdle", true);
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
                    // 오른쪽을 보고있다면
                    if (isConer == true && lookLeft == false)
                    {
                        // 오른쪽으로 일정거리 점프함
                        hornetRigidbody.velocity = Vector2.zero;
                        hornetRigidbody.AddForce(new Vector2(600, JUMP_FORCE));
                        isConer = false;
                        //Debug.Log("몇번실행함?");
                        break;
                    }
                    // 왼쪽을 보고있다면
                    else if (isConer == true && lookLeft == true)
                    {
                        // 왼쪽으로 일정거리 점프함
                        hornetRigidbody.velocity = Vector2.zero;
                        hornetRigidbody.AddForce(new Vector2(-1 * 600, JUMP_FORCE));
                        isConer = false;
                        //Debug.Log("몇번실행함?");
                        break;
                    }
                    // 오른쪽을 보고있다면
                    else if (lookLeft == false)
                    {
                        // 오른쪽으로 일정거리 점프함
                        hornetRigidbody.velocity = Vector2.zero;
                        hornetRigidbody.AddForce(new Vector2(150, JUMP_FORCE));
                        //Debug.Log("몇번실행함?");
                        break;
                    }
                    // 왼쪽을 보고있다면
                    else if (lookLeft == true)
                    {
                        // 왼쪽으로 일정거리 점프함
                        hornetRigidbody.velocity = Vector2.zero;
                        hornetRigidbody.AddForce(new Vector2(-1 * 150, JUMP_FORCE));
                        //Debug.Log("몇번실행함?");
                        break;
                    }
                    //break;
                }
            }
            yield return null;
        }

        while (true)
        {
            if (hornetRigidbody.velocity.y <= -8)
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
                    dashDistanceX = playerPosition.x - transform.position.x;
                    dashDistanceY = playerPosition.y - transform.position.y;
                    if (dashDistanceX <= 0)
                    {
                        dashDistanceX *= -1;
                    }

                    if (dashDistanceY <= 0)
                    {
                        dashDistanceY *= -1;
                    }

                    effect = Instantiate(airDashEffectPrefab, transform.position, transform.rotation);
                    // 플레이어가 왼쪽에 있지않다면
                    if (playerOnLeft == false)
                    {
                        // 오른쪽으로 플레이어와 거리차이에 따라 힘을 줌 (대쉬 시작 시점의 플레이어 위치를 향한 힘)
                        //hornetRigidbody.gravityScale = 5;
                        hornetRigidbody.velocity = Vector2.zero;
                        dashAirDir = new Vector2(dashDistanceX, -1 * dashDistanceY).normalized;
                        //hornetRigidbody.AddForce(new Vector2(dashDistanceX * 125, -1 * dashDistanceY * 125));
                        hornetRigidbody.AddForce(dashAirDir * 1000);
                    }
                    // 플레이어가 왼쪽에 있다면
                    else if (playerOnLeft == true)
                    {
                        // 왼쪽으로 플레이어와 거리차이에 따라 힘을 줌 (대쉬 시작 시점의 플레이어 위치를 향한 힘)
                        //hornetRigidbody.gravityScale = 5;
                        hornetRigidbody.velocity = Vector2.zero;
                        dashAirDir = new Vector2(-1 * dashDistanceX, -1 * dashDistanceY).normalized;
                        //hornetRigidbody.AddForce(new Vector2(dashDistanceX * 125, -1 * dashDistanceY * 125));
                        hornetRigidbody.AddForce(dashAirDir * 1000);
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
                    if (hitPlayer == true)
                    {
                        hitPlayer = false;
                    }
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
            StartCoroutine(Encount());
            Debug.Log("플레이어를 감지함");
        }
    }
}
