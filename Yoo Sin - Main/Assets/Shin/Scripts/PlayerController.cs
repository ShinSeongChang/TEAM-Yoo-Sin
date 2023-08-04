using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 7f;

    private WaitForSeconds attackRemainTime;
    private Rigidbody2D playerRigidbody;
    private SpriteRenderer attackSprite;
    private Collider2D attackCollider;

    private int jumpCount = 1;

    private float attackDelay = 0.7f;
    private float timeAfterAttack = 0f;
    private float jumpForce = 420f;
    private float gravityForce = 5f;

    private bool isRight;
    private bool isLeft;
    private bool isUp;
    private bool isDown;
    private bool isJumping;
    private bool isGround;
    enum attackDirection
    {
        UP_ATTACK = 1,
        LEFT_ATTACK = 2,
        RIGHT_ATTACK = 3,
        DOWN_ATTACK = 4
    }
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);

        playerRigidbody = GetComponent<Rigidbody2D>();
        attackRemainTime = new WaitForSeconds(0.3f);
        isRight = true;
        isLeft = false;
        isUp = false;
        isDown = false;
        isJumping = false;
        isGround = true;
    }

    // Update is called once per frame
    void Update()
    {
        // 공격 딜레이를 위해 델타타임 추가
        timeAfterAttack += Time.deltaTime;

        // 땅에 있음이 true이고 z키(점프키)를 누르는 순간
        if (Input.GetKeyDown(KeyCode.Z) && isGround == true)
        {
            // 점프 카운트 0으로 초기화, 땅에 있음을 false으로 초기화
            jumpCount = 0;
            isGround = false;
            // 점프 힘 추가
            playerRigidbody.AddForce(new Vector2(0, jumpForce));
        }

        // 점프 카운트가 0 이고 땅에 있음이 false이고 z키(점프키)를 누르는 중일 때
        if (Input.GetKey(KeyCode.Z) && jumpCount == 0 && isGround == false)
        {
            // 점프 중을 true로 초기화
            isJumping = true;
        }

        // 땅에 붙어있지 않고 z키(점프키)를 떼는 순간
        if (Input.GetKeyUp(KeyCode.Z) && isGround == false)
        {
            // 점프 중임을 false로 초기화
            isJumping = false;
        }

        // 점프 중이 false이고, 땅에 붙어있음이 false이면
        if (isJumping == false && isGround == false)
        {
            // 땅으로 더 빨리 떨어지게함
            playerRigidbody.AddForce(new Vector2(0, -gravityForce));
        }

        // 위 방향키를 누르는 중일 때
        if (Input.GetKey(KeyCode.UpArrow))
        {
            // 위를 봄을 true로, 아래를 봄을 false로 초기화
            isUp = true;
            isDown = false;
        }

        // 위 방향키를 떼는 순간
        if (Input.GetKeyUp(KeyCode.UpArrow))
        {
            // 위를 봄을 false로 초기화
            isUp = false;
        }

        // 아래 방향키를 누르는중이고 위 방향키를 누르는중이 아닐 때
        if (Input.GetKey(KeyCode.DownArrow) && !Input.GetKey(KeyCode.UpArrow))
        {
            // 아래를 봄을 true로 초기화
            isDown = true;
        }

        // 아래 방향키를 떼는 순간
        if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            // 아래를 봄을 false로 초기화
            isDown = false;
        }

        // 우측 방향키를 누르는 중이고 좌측 방향키를 누르는 중이 아닐 때
        if (Input.GetKey(KeyCode.RightArrow) && !Input.GetKey(KeyCode.LeftArrow))
        {
            // 우측으로 이동속도에 비례하게 이동, 왼쪽을 봄을 false로, 오른쪽을 봄을 true로 초기화
            transform.position += moveSpeed * Time.deltaTime * transform.right;
            isLeft = false;
            isRight = true;
        }

        // 좌측 방향키를 누르는 중이고 우측 방향키를 누르는 중이 아닐 때
        if (Input.GetKey(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.RightArrow))
        {
            // 좌측으로 이동속도에 비례하게 이동, 오른쪽을 봄을 false로, 왼쪽을 봄을 true로 초기화
            transform.position += -moveSpeed * Time.deltaTime * transform.right;
            isRight = false;
            isLeft = true;
        }

        // x키(공격키)를 누르는 순간
        if (Input.GetKeyDown(KeyCode.X))
        {
            // 위를 봄이 true일 때
            if (isUp == true)
            {
                // 공격 후 지난 시간이 공격 딜레이보다 크다면
                if (timeAfterAttack > attackDelay)
                {
                    // 위쪽 방향의 스프라이트와 콜라이더를 켰다가 공격지속시간후에 끔
                    attackSprite = transform.GetChild((int)attackDirection.UP_ATTACK).GetComponent<SpriteRenderer>();
                    attackCollider = transform.GetChild((int)attackDirection.UP_ATTACK).GetComponent<Collider2D>();
                    attackSprite.enabled = true;
                    attackCollider.enabled = true;
                    timeAfterAttack = 0;
                    StartCoroutine(Attack());
                }
            }

            // 아래를 봄이 true이고, 땅에 붙어있음이 false일 때
            if (isDown == true && isGround == false)
            {
                // 공격 후 지난 시간이 공격 딜레이보다 크다면
                if (timeAfterAttack > attackDelay)
                {
                    // 아래쪽 방향의 스프라이트와 콜라이더를 켰다가 공격지속시간후에 끔
                    attackSprite = transform.GetChild((int)attackDirection.DOWN_ATTACK).GetComponent<SpriteRenderer>();
                    attackCollider = transform.GetChild((int)attackDirection.DOWN_ATTACK).GetComponent<Collider2D>();
                    attackSprite.enabled = true;
                    attackCollider.enabled = true;
                    timeAfterAttack = 0;
                    StartCoroutine(Attack());
                }
            }

            // 왼쪽을 봄이 true이고 위를 봄이 false일 때
            if (isLeft == true && isUp == false)
            {
                // 공격 후 지난 시간이 공격 딜레이보다 크다면
                if (timeAfterAttack > attackDelay)
                {
                    // 왼쪽 방향의 스프라이트와 콜라이더를 켰다가 공격지속시간후에 끔
                    attackSprite = transform.GetChild((int)attackDirection.LEFT_ATTACK).GetComponent<SpriteRenderer>();
                    attackCollider = transform.GetChild((int)attackDirection.LEFT_ATTACK).GetComponent<Collider2D>();
                    attackSprite.enabled = true;
                    attackCollider.enabled = true;
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
                    // 오른쪽 방향의 스프라이트와 콜라이더를 켰다가 공격지속시간후에 끔
                    attackSprite = transform.GetChild((int)attackDirection.RIGHT_ATTACK).GetComponent<SpriteRenderer>();
                    attackCollider = transform.GetChild((int)attackDirection.RIGHT_ATTACK).GetComponent<Collider2D>();
                    attackSprite.enabled = true;
                    attackCollider.enabled = true;
                    timeAfterAttack = 0;
                    StartCoroutine(Attack());
                }
            }
        }
    }

    IEnumerator Attack()
    {
        // 공격 지속 시간이 지난 후 아래 줄을 실행함
        yield return attackRemainTime;
        // 공격 스프라이트와 공격 콜라이더를 끈 후 코루틴 종료
        attackSprite.enabled = false;
        attackCollider.enabled = false;
        yield break;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        isGround = true;
        jumpCount = 1;
    }
}
