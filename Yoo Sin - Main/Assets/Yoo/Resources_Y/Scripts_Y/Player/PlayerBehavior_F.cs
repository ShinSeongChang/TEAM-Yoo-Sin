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

        //Debug.LogFormat("ü�� : " + hp + "\n");
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

            // ���� �����̸� ���� ��ŸŸ�� �߰�
            timeAfterAttack += Time.deltaTime;

            // ���� ����
            #region
            if (isKnockBack == false)
            {
                // ���� ���� Ƚ���� 1 �̰�, zŰ(����Ű)�� ������ ����
                if (jumpCount == 1 && Input.GetKeyDown(KeyCode.Z))
                {
                    // ���� ī��Ʈ 0���� �ʱ�ȭ, ���� ����, �������� ���� false���� �ʱ�ȭ
                    jumpCount = 0;
                    isGround = false;
                    playerAni.SetBool("IsGround", false);
                    playerAni.SetBool("IsFall", false);
                    // velocity 0���� �ʱ�ȭ, velocity�� ���� �� �߰�
                    playerRigidbody.velocity = Vector2.zero;
                    playerRigidbody.AddForce(new Vector2(0, jumpForce));
                }
            }

            // ���� ������ false�̰� zŰ(����Ű)�� ������ ���̰� ���� ī��Ʈ�� 0 �� �� 
            if (isGround == false && jumpCount == 0 && Input.GetKey(KeyCode.Z))
            {
                // ���� ���� true�� �ʱ�ȭ
                isJumping = true;
            }

            // ���� �پ����� �ʰ� zŰ(����Ű)�� ���� ����
            if (isGround == false && Input.GetKeyUp(KeyCode.Z))
            {
                // ���� ������ false�� �ʱ�ȭ, �������� ������ true�� �ʱ�ȭ
                isJumping = false;
                playerAni.SetBool("IsFall", true);
            }

            if (isKnockBack == true)
            {
                playerRigidbody.gravityScale = 1;
            }

            if (isKnockBack == false)
            {
                // �ϴ� ������ false �̰� ���� ���� false�̰� ���� �پ������� false�̰ų� �ϴ� ������ false �̰� �÷��̾��� ������ٵ��� ���ν�Ƽ�� y���� 0 �̸��̶��(���� �ö󰡴� ���� 0�̸��̶��)
                if (isHitDown == false && isJumping == false && isGround == false || isHitDown == false && isGround == false && playerRigidbody.velocity.y < 0)
                {
                    // ������ �� ���� ����������, �������� ������ true�� �ʱ�ȭ
                    if (playerRigidbody.gravityScale == 1)
                    {
                        playerRigidbody.gravityScale = 5;
                        playerAni.SetBool("IsFall", true);
                    }
                }
            }
            #endregion

            // ����Ű(�̵�) ����
            #region
            // �ȴ� ���� false�� �ʱ�ȭ
            playerAni.SetBool("IsWalk", false);

            // �� ����Ű�� ������ ���� ��
            if (Input.GetKey(KeyCode.UpArrow))
            {
                // ���� ���� true��, �Ʒ��� ���� false�� �ʱ�ȭ
                isUp = true;
                playerAni.SetBool("IsUp", true);
                isDown = false;
            }

            // �� ����Ű�� ���� ����
            if (Input.GetKeyUp(KeyCode.UpArrow))
            {
                // ���� ���� false�� �ʱ�ȭ
                isUp = false;
                playerAni.SetBool("IsUp", false);
            }

            // �Ʒ� ����Ű�� ���������̰� �� ����Ű�� ���������� �ƴ� ��
            if (Input.GetKey(KeyCode.DownArrow) && !Input.GetKey(KeyCode.UpArrow))
            {
                // �Ʒ��� ���� true�� �ʱ�ȭ
                isDown = true;
                playerAni.SetBool("IsDown", true);
            }

            // �Ʒ� ����Ű�� ���� ����
            if (Input.GetKeyUp(KeyCode.DownArrow))
            {
                // �Ʒ��� ���� false�� �ʱ�ȭ
                isDown = false;
                playerAni.SetBool("IsDown", false);
            }

            if (isKnockBack == false)
            {
                // ���� ���ݼ����� �ƴϰ�, ���� ����Ű�� ������ ���̰� ���� ����Ű�� ������ ���� �ƴ� ��
                if (isHitFront == false && Input.GetKey(KeyCode.RightArrow) && !Input.GetKey(KeyCode.LeftArrow))
                {
                    if (isRight == true && isWall == true)
                    {
                        return;
                    }
                    playerRigidbody.velocity = new Vector2(0, playerRigidbody.velocity.y);
                    // �������� �̵��ӵ��� ����ϰ� �̵�, ������ ���� false��, �������� ���� true�� �ʱ�ȭ
                    transform.position += moveSpeed * Time.deltaTime * transform.right;
                    isRight = true;
                    playerAni.SetBool("IsRight", true);
                    playerAni.SetBool("IsWalk", true);
                }
                // ���� ���ݼ����� �ƴϰ�, ���� ����Ű�� ������ ���̰� ���� ����Ű�� ������ ���� �ƴ� ��
                else if (isHitFront == false && Input.GetKey(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.RightArrow))
                {
                    if (isRight == false && isWall == true)
                    {
                        return;
                    }
                    playerRigidbody.velocity = new Vector2(0, playerRigidbody.velocity.y);
                    // �������� �̵��ӵ��� ����ϰ� �̵�, �������� ���� false��, ������ ���� true�� �ʱ�ȭ
                    transform.position -= moveSpeed * Time.deltaTime * transform.right;
                    isRight = false;
                    playerAni.SetBool("IsRight", false);
                    playerAni.SetBool("IsWalk", true);
                }
            }
            #endregion

            // xŰ(����) ����
            if (Input.GetKeyDown(KeyCode.X))
            {
                //if (isKnockBack == true)
                //{
                //    return;
                //}
                // ���� ���� true�� ��
                if (isUp == true)
                {
                    // ���� �� ���� �ð��� ���� �����̺��� ũ�ٸ�
                    if (timeAfterAttack > attackDelay)
                    {
                        // ���� ������ ��������Ʈ�� �ݶ��̴��� �״ٰ� �������ӽð��Ŀ� �� (���ʿ��� ���������� ����)
                        attackSprite = playerAni.gameObject.transform.GetChild((int)attackDirection.UP_ATTACK).GetComponent<SpriteRenderer>();
                        attackCollider = playerAni.gameObject.transform.GetChild((int)attackDirection.UP_ATTACK).GetComponent<Collider2D>();
                        attackAni = playerAni.gameObject.transform.GetChild((int)attackDirection.UP_ATTACK).GetComponent<Animator>();
                        timeAfterAttack = 0;
                        StartCoroutine(Attack());
                    }
                }

                // �Ʒ��� ���� true�̰�, ���� �پ������� false�� ��
                if (isGround == false && isDown == true)
                {
                    // ���� �� ���� �ð��� ���� �����̺��� ũ�ٸ�
                    if (timeAfterAttack > attackDelay)
                    {
                        // �Ʒ��� ������ ��������Ʈ�� �ݶ��̴��� �״ٰ� �������ӽð��Ŀ� �� (�����ʿ��� �������� ����)
                        attackSprite = playerAni.gameObject.transform.GetChild((int)attackDirection.DOWN_ATTACK).GetComponent<SpriteRenderer>();
                        attackCollider = playerAni.gameObject.transform.GetChild((int)attackDirection.DOWN_ATTACK).GetComponent<Collider2D>();
                        attackAni = playerAni.gameObject.transform.GetChild((int)attackDirection.DOWN_ATTACK).GetComponent<Animator>();
                        timeAfterAttack = 0;
                        StartCoroutine(Attack());
                    }
                }

                // ���� ���� false�� ��
                if (isUp == false)
                {
                    // ���� �� ���� �ð��� ���� �����̺��� ũ�ٸ�
                    if (timeAfterAttack > attackDelay)
                    {
                        // ������ ������ ��������Ʈ�� �ݶ��̴�, �ִϸ����͸� ������
                        attackSprite = playerAni.gameObject.transform.GetChild((int)attackDirection.RIGHT_ATTACK).GetComponent<SpriteRenderer>();
                        attackCollider = playerAni.gameObject.transform.GetChild((int)attackDirection.RIGHT_ATTACK).GetComponent<Collider2D>();
                        attackAni = playerAni.gameObject.transform.GetChild((int)attackDirection.RIGHT_ATTACK).GetComponent<Animator>();
                        timeAfterAttack = 0;
                        StartCoroutine(Attack());
                    }
                }
            }

            // aŰ(��ų) ����
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
        // 1������ �� ������Ʈ �ı�
        yield return null;
        playermove = false;
        playerRigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
        playerCollider.enabled = false;
        Destroy(transform.GetChild(0).gameObject);
        yield break;
    }

    // �÷��̾� ���� �ڷ�ƾ
    IEnumerator Attack()
    {
        // �÷��̾� �ִϸ������� ����Ʈ���� Ȱ��ȭ
        playerAni.SetTrigger("Attack");
        // ���� ��������Ʈ�� ���� �ݶ��̴�, ���� �ִϸ����͸� Ŵ
        attackAni.enabled = true;
        attackAni.SetTrigger("Awake");
        attackSprite.enabled = true;
        attackCollider.enabled = true;
        // ���� ���� �ð��� ���� �� �Ʒ� ���� ������
        yield return attackRemainTime;
        // ���� ��������Ʈ�� ���� �ݶ��̴�, ���� �ִϸ����͸� �� �� �ڷ�ƾ ����
        attackSprite.enabled = false;
        attackAni.SetTrigger("Sleep");
        attackAni.enabled = false;
        attackCollider.enabled = false;
        yield break;
    }

    // �÷��̾� �����ð� ���� �ڷ�ƾ
    IEnumerator GracePeriod()
    {
        isInvincible = true;
        yield return invinclbleTime;
        isInvincible = false;
        yield break;
    }

    // �÷��̾� �����ε��� �����Ÿ��� ����� �ڷ�ƾ
    IEnumerator Blink()
    {
        Color tempColor = playerSprite.color;
        yield return null;
        while (true)
        {
            //Debug.Log("��ũ���� ����");
            if (isInvincible == false)
            {
                tempColor.a = 1;
                playerSprite.color = tempColor;
                gameObject.layer = 0;
                yield break;
            }

            if (isInvincible == true)
            {
                //Debug.Log("��ũ�����̴ºκ� ����");
                if (playerSprite == null)
                {
                    yield break;
                }

                if (playerSprite.color.a == 1)
                {
                    //Debug.Log("��ũ�����̴ºκ� ����");
                    tempColor.a = 0;
                    playerSprite.color = tempColor;
                }
                else if (playerSprite.color.a == 0)
                {
                    //Debug.Log("��ũ�����̴ºκ� ����");
                    tempColor.a = 1;
                    playerSprite.color = tempColor;
                }
                yield return blinkTerm;
            }
            yield return null;
        }
    }

    // �÷��̾� �ǰݽ� �з����� �ڷ�ƾ
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
            Debug.LogFormat("ü�� : " + hp + "\n");

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
            Debug.LogFormat("ü�� : " + hp + "\n");

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
            Debug.LogFormat("ü�� : " + hp + "\n");

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
            Debug.LogFormat("ü�� : " + hp + "\n");

            playerHealthUi.HealtDown();
            hp = playerHealthUi.GetHp();
            //StartCoroutine(KnockBack());
            //StartCoroutine(GracePeriod());
            //StartCoroutine(Blink());
        }
    }
}
