using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerBehavior_F : MonoBehaviour
{
    public static bool playermove = false;
    public float moveSpeed = 7f;

    private const float BLINK_TERM = 0.1f;
    private const float INVINCIBLE_TIME = 1f;
    private const float KNOCKBACK_TIME = 0.4f;
    private WaitForSeconds blinkTerm = new WaitForSeconds(BLINK_TERM);
    private WaitForSeconds invinclbleTime = new WaitForSeconds(INVINCIBLE_TIME);
    private WaitForSeconds knockBackTime = new WaitForSeconds(KNOCKBACK_TIME);
    private PlayerHealthUi_Y playerHealthUi;
    private SkillGauge_Y playerSkillGauge;

    private SpriteRenderer playerSprite;
    private Rigidbody2D playerRigidbody;
    private Collider2D playerCollider;
    private Animator playerAni;
    private WaitForSeconds attackRemainTime;
    private SpriteRenderer attackSprite;
    private Collider2D attackCollider;
    private Animator attackAni;
    private int jumpCount;
    private int hp;

    private Vector3 toLeft = new Vector3(0, 180, 0);
    private Vector3 toRight = new Vector3(0, 0, 0);
    private float attackDelay = 0.46f;
    private float timeAfterAttack = 0f;
    private float jumpForce = 500f;
    private float horizontalKnockBackForce = 300f;
    private float verticalKnockBackForce = 300f;

    private Vector3 monsterPosition;
    private float yDiff = 0f;

    public bool isHitFront;
    public bool isHitLeft;
    public bool isHitDown;

    private bool isWall;
    private bool isRight;
    private bool isUp;
    private bool isDown;
    private bool isJumping;
    private bool isGround;
    private bool isInvincible;
    private bool isDead;
    private bool isKnockBack;
    enum attackDirection
    {
        UP_ATTACK = 0,
        RIGHT_ATTACK = 1,
        DOWN_ATTACK = 2
    }

    // Start is called before the first frame update
    void Start()
    {
        playerAni = transform.GetChild(0).GetComponent<Animator>();
        playerSprite = transform.GetChild(0).GetComponent<SpriteRenderer>();
        playerRigidbody = transform.GetComponent<Rigidbody2D>();
        playerCollider = transform.GetComponent<Collider2D>();
        attackRemainTime = new WaitForSeconds(0.2f);
        jumpCount = 1;
        isRight = true;
        isUp = false;
        isDown = false;
        isJumping = false;
        isGround = true;
        isHitFront = false;
        isHitDown = false;
        isInvincible = false;
        isDead = false;
        isKnockBack = false;
        isWall = false;
        playerHealthUi = GameObject.Find("Healts").GetComponent<PlayerHealthUi_Y>();
        playerSkillGauge = GameObject.Find("GaugeImg").GetComponent<SkillGauge_Y>();
        hp = playerHealthUi.GetHp();
        playermove = true;

        //Debug.LogFormat("체력 : " + hp + "\n");
    }

    // Update is called once per frame
    void Update()
    {
        if (playermove == true && playerAni.GetBool("IsDead") == false)
        {
            if (isDead == true)
            {
                playerAni.SetBool("IsDead", true);
                StartCoroutine(Dead());
            }

            if (isRight == false)
            {
                transform.GetChild(0).eulerAngles = toLeft;
            }

            if (isRight == true)
            {
                transform.GetChild(0).eulerAngles = toRight;
            }

            if (hp <= 0)
            {
                isDead = true;
            }

            // 공격 딜레이를 위해 델타타임 추가
            timeAfterAttack += Time.deltaTime;

            // 점프 관련
            #region
            if (isKnockBack == false)
            {
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

            if (isKnockBack == true)
            {
                playerRigidbody.gravityScale = 1;
            }

            if (isKnockBack == false)
            {
                // 하단 공격이 false 이고 점프 중이 false이고 땅에 붙어있음이 false이거나 하단 공격이 false 이고 플레이어의 리지드바디의 벨로시티의 y값이 0 미만이라면(위로 올라가는 힘이 0미만이라면)
                if (isHitDown == false && isJumping == false && isGround == false || isHitDown == false && isGround == false && playerRigidbody.velocity.y < 0)
                {
                    // 땅으로 더 빨리 떨어지게함, 떨어지는 중임을 true로 초기화
                    if (playerRigidbody.gravityScale == 1)
                    {
                        playerRigidbody.gravityScale = 5;
                        playerAni.SetBool("IsFall", true);
                    }
                }
            }
            #endregion

            // 방향키(이동) 관련
            #region
            // 걷는 중을 false로 초기화
            playerAni.SetBool("IsWalk", false);

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

            if (isKnockBack == false)
            {
                // 전방 공격성공이 아니고, 우측 방향키를 누르는 중이고 좌측 방향키를 누르는 중이 아닐 때
                if (isHitFront == false && Input.GetKey(KeyCode.RightArrow) && !Input.GetKey(KeyCode.LeftArrow))
                {
                    if (isRight == true && isWall == true)
                    {
                        return;
                    }
                    playerRigidbody.velocity = new Vector2(0, playerRigidbody.velocity.y);
                    // 우측으로 이동속도에 비례하게 이동, 왼쪽을 봄을 false로, 오른쪽을 봄을 true로 초기화
                    transform.position += moveSpeed * Time.deltaTime * transform.right;
                    isRight = true;
                    playerAni.SetBool("IsRight", true);
                    playerAni.SetBool("IsWalk", true);
                }
                // 전방 공격성공이 아니고, 좌측 방향키를 누르는 중이고 우측 방향키를 누르는 중이 아닐 때
                else if (isHitFront == false && Input.GetKey(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.RightArrow))
                {
                    if (isRight == false && isWall == true)
                    {
                        return;
                    }
                    playerRigidbody.velocity = new Vector2(0, playerRigidbody.velocity.y);
                    // 좌측으로 이동속도에 비례하게 이동, 오른쪽을 봄을 false로, 왼쪽을 봄을 true로 초기화
                    transform.position -= moveSpeed * Time.deltaTime * transform.right;
                    isRight = false;
                    playerAni.SetBool("IsRight", false);
                    playerAni.SetBool("IsWalk", true);
                }
            }
            #endregion

            // x키(공격) 관련
            if (Input.GetKeyDown(KeyCode.X))
            {
                //if (isKnockBack == true)
                //{
                //    return;
                //}
                // 위를 봄이 true일 때
                if (isUp == true)
                {
                    // 공격 후 지난 시간이 공격 딜레이보다 크다면
                    if (timeAfterAttack > attackDelay)
                    {
                        // 위쪽 방향의 스프라이트와 콜라이더를 켰다가 공격지속시간후에 끔 (왼쪽에서 오른쪽으로 공격)
                        attackSprite = playerAni.gameObject.transform.GetChild((int)attackDirection.UP_ATTACK).GetComponent<SpriteRenderer>();
                        attackCollider = playerAni.gameObject.transform.GetChild((int)attackDirection.UP_ATTACK).GetComponent<Collider2D>();
                        attackAni = playerAni.gameObject.transform.GetChild((int)attackDirection.UP_ATTACK).GetComponent<Animator>();
                        timeAfterAttack = 0;
                        StartCoroutine(Attack());
                    }
                }

                // 아래를 봄이 true이고, 땅에 붙어있음이 false일 때
                if (isGround == false && isDown == true)
                {
                    // 공격 후 지난 시간이 공격 딜레이보다 크다면
                    if (timeAfterAttack > attackDelay)
                    {
                        // 아래쪽 방향의 스프라이트와 콜라이더를 켰다가 공격지속시간후에 끔 (오른쪽에서 왼쪽으로 공격)
                        attackSprite = playerAni.gameObject.transform.GetChild((int)attackDirection.DOWN_ATTACK).GetComponent<SpriteRenderer>();
                        attackCollider = playerAni.gameObject.transform.GetChild((int)attackDirection.DOWN_ATTACK).GetComponent<Collider2D>();
                        attackAni = playerAni.gameObject.transform.GetChild((int)attackDirection.DOWN_ATTACK).GetComponent<Animator>();
                        timeAfterAttack = 0;
                        StartCoroutine(Attack());
                    }
                }

                // 위를 봄이 false일 때
                if (isUp == false)
                {
                    // 공격 후 지난 시간이 공격 딜레이보다 크다면
                    if (timeAfterAttack > attackDelay)
                    {
                        // 오른쪽 방향의 스프라이트와 콜라이더, 애니메이터를 가져옴
                        attackSprite = playerAni.gameObject.transform.GetChild((int)attackDirection.RIGHT_ATTACK).GetComponent<SpriteRenderer>();
                        attackCollider = playerAni.gameObject.transform.GetChild((int)attackDirection.RIGHT_ATTACK).GetComponent<Collider2D>();
                        attackAni = playerAni.gameObject.transform.GetChild((int)attackDirection.RIGHT_ATTACK).GetComponent<Animator>();
                        timeAfterAttack = 0;
                        StartCoroutine(Attack());
                    }
                }
            }

            // a키(스킬) 관련
            if (Input.GetKeyDown(KeyCode.A))
            {
                if (playerHealthUi.GetHp() >= 5)
                {
                    return;
                }

                if (playerSkillGauge.GetGauge() >= 4)
                {
                    playerHealthUi.HealtUp();
                    playerSkillGauge.UseHeal();
                    hp = playerHealthUi.GetHp();
                }
            }
        }
    }

    public bool GetDead()
    {
        return isDead;
    }

    private void Hit()
    {
        StartCoroutine(GracePeriod());
        playerAni.SetTrigger("Hurt");
        StartCoroutine(KnockBack());
        StartCoroutine(Blink());
    }

    IEnumerator Dead()
    {
        // 1프레임 후 오브젝트 파괴
        yield return null;
        playermove = false;
        playerRigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
        playerCollider.enabled = false;
        Destroy(transform.GetChild(0).gameObject);
        yield break;
    }

    // 플레이어 공격 코루틴
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

    // 플레이어 무적시간 관리 코루틴
    IEnumerator GracePeriod()
    {
        isInvincible = true;
        yield return invinclbleTime;
        isInvincible = false;
        yield break;
    }

    // 플레이어 무적인동안 깜빡거리게 만드는 코루틴
    IEnumerator Blink()
    {
        Color tempColor = playerSprite.color;
        yield return null;
        while (true)
        {
            //Debug.Log("블링크와일 들어옴");
            if (isInvincible == false)
            {
                tempColor.a = 1;
                playerSprite.color = tempColor;
                gameObject.layer = 0;
                yield break;
            }

            if (isInvincible == true)
            {
                //Debug.Log("블링크깜박이는부분 들어옴");
                if (playerSprite == null)
                {
                    yield break;
                }

                if (playerSprite.color.a == 1)
                {
                    //Debug.Log("블링크깜박이는부분 들어옴");
                    tempColor.a = 0;
                    playerSprite.color = tempColor;
                }
                else if (playerSprite.color.a == 0)
                {
                    //Debug.Log("블링크깜박이는부분 들어옴");
                    tempColor.a = 1;
                    playerSprite.color = tempColor;
                }
                yield return blinkTerm;
            }
            yield return null;
        }
    }

    // 플레이어 피격시 밀려나는 코루틴
    IEnumerator KnockBack()
    {
        isKnockBack = true;
        if (monsterPosition.x >= transform.position.x)
        {
            playerRigidbody.velocity = Vector2.zero;
            if (yDiff < -1.4f)
            {
                playerRigidbody.AddForce(new Vector2(-horizontalKnockBackForce * 1.3f, -verticalKnockBackForce));
            }
            else if (yDiff > 0.5f)
            {
                playerRigidbody.AddForce(new Vector2(-horizontalKnockBackForce * 1.3f, verticalKnockBackForce));
            }
            else
            {
                playerRigidbody.AddForce(new Vector2(-horizontalKnockBackForce, 0));
            }
        }
        if (monsterPosition.x < transform.position.x)
        {
            playerRigidbody.velocity = Vector2.zero;
            if (yDiff < -1.4f)
            {
                playerRigidbody.AddForce(new Vector2(horizontalKnockBackForce * 1.3f, -verticalKnockBackForce));
            }
            else if (yDiff > 0.5f)
            {
                playerRigidbody.AddForce(new Vector2(horizontalKnockBackForce * 1.3f, verticalKnockBackForce));
            }
            else
            {
                playerRigidbody.AddForce(new Vector2(horizontalKnockBackForce, 0));
            }
        }
        yield return knockBackTime;
        isKnockBack = false;
        yield break;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Platform") || collision.collider.CompareTag("StunBody"))
        {
            jumpCount = 1;
            playerRigidbody.velocity = new Vector2(playerRigidbody.velocity.x, 0);
            isGround = true;
            playerRigidbody.gravityScale = 1;
            playerAni.SetBool("IsGround", true);
            playerAni.SetBool("IsFall", false);
        }

        if (isInvincible == false && collision.collider.CompareTag("Monster"))
        {
            gameObject.layer = 10;
            Hit();
            monsterPosition = collision.transform.position;
            yDiff = transform.position.y - monsterPosition.y;
            //hp -= 1;
            Debug.LogFormat("체력 : " + hp + "\n");

            playerHealthUi.HealtDown();
            hp = playerHealthUi.GetHp();
            //StartCoroutine(KnockBack());
            //StartCoroutine(GracePeriod());
            //StartCoroutine(Blink());
        }

        if (collision.collider.CompareTag("Wall") && isInvincible == false)
        {
            isWall = true;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Platform") || collision.collider.CompareTag("StunBody"))
        {
            //jumpCount = 1;
            playerRigidbody.gravityScale = 1;
            //isGround = true;
            playerAni.SetBool("IsGround", true);
            //playerAni.SetBool("IsFall", false);
        }

        if (isInvincible == false && collision.collider.CompareTag("Monster"))
        {
            gameObject.layer = 10;
            Hit();
            monsterPosition = collision.transform.position;
            yDiff = transform.position.y - monsterPosition.y;
            //hp -= 1;
            Debug.LogFormat("체력 : " + hp + "\n");

            playerHealthUi.HealtDown();
            hp = playerHealthUi.GetHp();
            //StartCoroutine(KnockBack());
            //StartCoroutine(GracePeriod());
            //StartCoroutine(Blink());
        }

        if (collision.collider.CompareTag("Wall") && isInvincible == false)
        {
            isWall = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Platform") || collision.collider.CompareTag("StunBody"))
        {
            isGround = false;
            playerAni.SetBool("IsGround", false);
        }

        if (collision.collider.CompareTag("Wall") && isInvincible == false)
        {
            isWall = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isInvincible == false && collision.CompareTag("MonsterAttack"))
        {
            gameObject.layer = 10;
            Hit();
            monsterPosition = collision.transform.position;
            yDiff = transform.position.y - monsterPosition.y;
            //hp -= 1;
            Debug.LogFormat("체력 : " + hp + "\n");

            playerHealthUi.HealtDown();
            hp = playerHealthUi.GetHp();
            //StartCoroutine(KnockBack());
            //StartCoroutine(GracePeriod());
            //StartCoroutine(Blink());
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (isInvincible == false && collision.CompareTag("MonsterAttack"))
        {
            gameObject.layer = 10;
            Hit();
            monsterPosition = collision.transform.position;
            yDiff = transform.position.y - monsterPosition.y;
            //hp -= 1;
            Debug.LogFormat("체력 : " + hp + "\n");

            playerHealthUi.HealtDown();
            hp = playerHealthUi.GetHp();
            //StartCoroutine(KnockBack());
            //StartCoroutine(GracePeriod());
            //StartCoroutine(Blink());
        }
    }
}
