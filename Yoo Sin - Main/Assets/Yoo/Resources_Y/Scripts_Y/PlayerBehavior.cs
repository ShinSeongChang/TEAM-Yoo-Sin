using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerBehavior : MonoBehaviour
{
    public float moveSpeed = 7f;

    private Rigidbody2D playerRigidbody;
    private Animator playerAni;
    private WaitForSeconds attackRemainTime;
    private SpriteRenderer attackSprite;
    private Collider2D attackCollider;
    private Animator attackAni;
    private int jumpCount;

    private float attackDelay = 0.45f;
    private float timeAfterAttack = 0f;
    private float jumpForce = 500f;

    public bool isHitRight;
    public bool isHitLeft;
    public bool isHitDown;

    private bool isRight;
    private bool isLeft;
    private bool isUp;
    private bool isDown;
    private bool isJumping;
    private bool isGround;
    enum attackDirection
    {
        UP_ATTACK_L = 1,
        UP_ATTACK_R = 2,
        LEFT_ATTACK = 3,
        RIGHT_ATTACK = 4,
        DOWN_ATTACK_L = 5,
        DOWN_ATTACK_R = 6
    }
    // Start is called before the first frame update
    void Start()
    {
        playerAni = GetComponent<Animator>();
        playerRigidbody = GetComponent<Rigidbody2D>();
        attackRemainTime = new WaitForSeconds(0.2f);
        jumpCount = 1;
        isRight = true;
        isLeft = false;
        isUp = false;
        isDown = false;
        isJumping = false;
        isGround = true;
        isHitRight = false;
        isHitLeft = false;
        isHitDown = false;
    }

    // Update is called once per frame
    void Update()
    {
        // 공격 딜레이를 위해 델타타임 추가
        timeAfterAttack += Time.deltaTime;

        // 점프 관련
        #region
        // 점프 가능 횟수가 1 이고, z키(점프키)를 누르는 순간
        if (jumpCount == 1 && Input.GetKeyDown(KeyCode.Z))
        {
            // 점프 카운트 0으로 초기화, 땅에 있음, 떨어지는 중을 false으로 초기화
            jumpCount = 0;
            isGround = false;
            playerAni.SetBool("IsGround", false);
            playerAni.SetBool("IsFall", false);
            // velocity 0으로 초기화, velocity에 점프 힘 추가
            playerRigidbody.velocity = Vector2.zero;
            playerRigidbody.AddForce(new Vector2(0, jumpForce));
        }

        // 땅에 있음이 false이고 z키(점프키)를 누르는 중이고 점프 카운트가 0 일 때 
        if (isGround == false && jumpCount == 0 && Input.GetKey(KeyCode.Z))
        {
            // 점프 중을 true로 초기화
            isJumping = true;
        }

        // 땅에 붙어있지 않고 z키(점프키)를 떼는 순간
        if (isGround == false && Input.GetKeyUp(KeyCode.Z))
        {
            // 점프 중임을 false로 초기화, 떨어지는 중임을 true로 초기화
            isJumping = false;
            playerAni.SetBool("IsFall", true);
        }

        // 하단 공격이 false 이고 점프 중이 false이고 땅에 붙어있음이 false이거나 하단 공격이 false 이고 플레이어의 리지드바디의 벨로시티의 y값이 0 미만이라면(위로 올라가는 힘이 0미만이라면)
        if (isHitDown == false && isJumping == false && isGround == false || isHitDown == false && isGround == false &&  playerRigidbody.velocity.y < 0)
        {
            // 땅으로 더 빨리 떨어지게함, 떨어지는 중임을 true로 초기화
            if (playerRigidbody.gravityScale == 1)
            {
                playerRigidbody.gravityScale = 5;
                playerAni.SetBool("IsFall", true);
            }
        }
        #endregion

        // 방향키(이동) 관련
        #region
        // 위 방향키를 누르는 중일 때
        if (Input.GetKey(KeyCode.UpArrow))
        {
            // 위를 봄을 true로, 아래를 봄을 false로 초기화
            isUp = true;
            playerAni.SetBool("IsUp", true);
            isDown = false;
        }

        // 위 방향키를 떼는 순간
        if (Input.GetKeyUp(KeyCode.UpArrow))
        {
            // 위를 봄을 false로 초기화
            isUp = false;
            playerAni.SetBool("IsUp", false);
        }

        // 아래 방향키를 누르는중이고 위 방향키를 누르는중이 아닐 때
        if (Input.GetKey(KeyCode.DownArrow) && !Input.GetKey(KeyCode.UpArrow))
        {
            // 아래를 봄을 true로 초기화
            isDown = true;
            playerAni.SetBool("IsDown", true);
        }

        // 아래 방향키를 떼는 순간
        if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            // 아래를 봄을 false로 초기화
            isDown = false;
            playerAni.SetBool("IsDown", false);
        }

        // 우측 공격성공이 아니고, 우측 방향키를 누르는 중이고 좌측 방향키를 누르는 중이 아닐 때
        if (isHitRight == false && Input.GetKey(KeyCode.RightArrow) && !Input.GetKey(KeyCode.LeftArrow))
        {
            // 우측으로 이동속도에 비례하게 이동, 왼쪽을 봄을 false로, 오른쪽을 봄을 true로 초기화
            transform.position += moveSpeed * Time.deltaTime * transform.right;
            isLeft = false;
            isRight = true;
            playerAni.SetBool("IsLeft", false);
            playerAni.SetBool("IsRight", true);
            playerAni.SetBool("IsWalk", true);
        }
        // 좌측 공격성공이 아니고, 좌측 방향키를 누르는 중이고 우측 방향키를 누르는 중이 아닐 때
        else if (isHitLeft == false && Input.GetKey(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.RightArrow))
        {
            // 좌측으로 이동속도에 비례하게 이동, 오른쪽을 봄을 false로, 왼쪽을 봄을 true로 초기화
            transform.position -= moveSpeed * Time.deltaTime * transform.right;
            isLeft = true;
            isRight = false;
            playerAni.SetBool("IsLeft", true);
            playerAni.SetBool("IsRight", false);
            playerAni.SetBool("IsWalk", true);
        }
        // 그 외 라면
        else
        {
            // 걷는 중을 false로 초기화
            playerAni.SetBool("IsWalk", false);
        }
        #endregion

        // x키(공격) 관련
        if (Input.GetKeyDown(KeyCode.X))
        {
            // 위를 봄이 true일 때
            if (isUp == true)
            {
                // 공격 후 지난 시간이 공격 딜레이보다 크다면
                if (timeAfterAttack > attackDelay)
                {
                    if (isLeft == true)
                    {
                        // 위쪽 방향의 스프라이트와 콜라이더를 켰다가 공격지속시간후에 끔 (왼쪽에서 오른쪽으로 공격)
                        attackSprite = transform.GetChild((int)attackDirection.UP_ATTACK_L).GetComponent<SpriteRenderer>();
                        attackCollider = transform.GetChild((int)attackDirection.UP_ATTACK_L).GetComponent<Collider2D>();
                        attackAni = transform.GetChild((int)attackDirection.UP_ATTACK_L).GetComponent<Animator>();
                        timeAfterAttack = 0;
                        StartCoroutine(Attack());
                    }

                    if (isRight == true)
                    {
                        // 위쪽 방향의 스프라이트와 콜라이더를 켰다가 공격지속시간후에 끔 (오른쪽에서 왼쪽으로 공격)
                        attackSprite = transform.GetChild((int)attackDirection.UP_ATTACK_R).GetComponent<SpriteRenderer>();
                        attackCollider = transform.GetChild((int)attackDirection.UP_ATTACK_R).GetComponent<Collider2D>();
                        attackAni = transform.GetChild((int)attackDirection.UP_ATTACK_R).GetComponent<Animator>();
                        timeAfterAttack = 0;
                        StartCoroutine(Attack());
                    }
                }
            }

            // 아래를 봄이 true이고, 땅에 붙어있음이 false일 때
            if (isGround == false && isDown == true)
            {
                // 공격 후 지난 시간이 공격 딜레이보다 크다면
                if (timeAfterAttack > attackDelay)
                {
                    if(isLeft == true)
                    {
                        // 아래쪽 방향의 스프라이트와 콜라이더를 켰다가 공격지속시간후에 끔 (왼쪽에서 오른쪽으로 공격)
                        attackSprite = transform.GetChild((int)attackDirection.DOWN_ATTACK_L).GetComponent<SpriteRenderer>();
                        attackCollider = transform.GetChild((int)attackDirection.DOWN_ATTACK_L).GetComponent<Collider2D>();
                        attackAni = transform.GetChild((int)attackDirection.DOWN_ATTACK_L).GetComponent<Animator>();
                        timeAfterAttack = 0;
                        StartCoroutine(Attack());
                    }

                    if(isRight == true)
                    {
                        // 아래쪽 방향의 스프라이트와 콜라이더를 켰다가 공격지속시간후에 끔 (오른쪽에서 왼쪽으로 공격)
                        attackSprite = transform.GetChild((int)attackDirection.DOWN_ATTACK_R).GetComponent<SpriteRenderer>();
                        attackCollider = transform.GetChild((int)attackDirection.DOWN_ATTACK_R).GetComponent<Collider2D>();
                        attackAni = transform.GetChild((int)attackDirection.DOWN_ATTACK_R).GetComponent<Animator>();
                        timeAfterAttack = 0;
                        StartCoroutine(Attack());
                    }
                }
            }

            // 왼쪽을 봄이 true이고 위를 봄이 false일 때
            if (isLeft == true && isUp == false)
            {
                // 공격 후 지난 시간이 공격 딜레이보다 크다면
                if (timeAfterAttack > attackDelay)
                {
                    // 왼쪽 방향의 스프라이트와 콜라이더, 애니메이터를 가져옴
                    attackSprite = transform.GetChild((int)attackDirection.LEFT_ATTACK).GetComponent<SpriteRenderer>();
                    attackCollider = transform.GetChild((int)attackDirection.LEFT_ATTACK).GetComponent<Collider2D>();
                    attackAni = transform.GetChild((int)attackDirection.LEFT_ATTACK).GetComponent<Animator>();
                    timeAfterAttack = 0;
                    StartCoroutine(Attack());
                }
            }

            // 오른쪽을 봄이 true이고 위를 봄이 false일 때
            if (isRight == true && isUp == false)
            {
                // 공격 후 지난 시간이 공격 딜레이보다 크다면
                if (timeAfterAttack > attackDelay)
                {
                    // 오른쪽 방향의 스프라이트와 콜라이더, 애니메이터를 가져옴
                    attackSprite = transform.GetChild((int)attackDirection.RIGHT_ATTACK).GetComponent<SpriteRenderer>();
                    attackCollider = transform.GetChild((int)attackDirection.RIGHT_ATTACK).GetComponent<Collider2D>();
                    attackAni = transform.GetChild((int)attackDirection.RIGHT_ATTACK).GetComponent<Animator>();
                    timeAfterAttack = 0;
                    StartCoroutine(Attack());
                }
            }
        }
    }

    IEnumerator Attack()
    {
        // 플레이어 애니메이터의 어택트리거 활성화
        playerAni.SetTrigger("Attack");
        // 공격 스프라이트와 공격 콜라이더, 공격 애니메이터를 킴
        attackAni.enabled = true;
        attackAni.SetTrigger("Awake");
        attackSprite.enabled = true;
        attackCollider.enabled = true;
        // 공격 지속 시간이 지난 후 아래 줄을 실행함
        yield return attackRemainTime;
        // 공격 스프라이트와 공격 콜라이더, 공격 애니메이터를 끈 후 코루틴 종료
        attackSprite.enabled = false;
        attackAni.SetTrigger("Sleep");
        attackAni.enabled = false;
        attackCollider.enabled = false;
        yield break;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Platform"))
        {
            jumpCount = 1;
            playerRigidbody.velocity = Vector2.zero;
            isGround = true;
            //playerRigidbody.gravityScale = 1;
            playerAni.SetBool("IsGround", true);
            playerAni.SetBool("IsFall", false);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if(collision.collider.CompareTag("Platform"))
        {
            playerRigidbody.gravityScale = 1;
            playerAni.SetBool("IsGround", true);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Platform"))
        {
            isGround = false;
            playerAni.SetBool("IsGround", false);
        }
    }
}
