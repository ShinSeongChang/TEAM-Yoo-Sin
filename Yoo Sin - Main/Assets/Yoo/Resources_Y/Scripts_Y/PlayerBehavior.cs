using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerBehavior : MonoBehaviour
{
    public float moveSpeed = 7f;

    private const float INVINCIBLE_TIME = 2f;
    private const float KNOCKBACK_TIME = 0.5f;
    private WaitForSeconds invinclbleTime = new WaitForSeconds(INVINCIBLE_TIME);
    private WaitForSeconds knockBackTime = new WaitForSeconds(KNOCKBACK_TIME);

    private Rigidbody2D playerRigidbody;
    private Animator playerAni;
    private WaitForSeconds attackRemainTime;
    private SpriteRenderer attackSprite;
    private Collider2D attackCollider;
    private Animator attackAni;
    private int jumpCount;
    private int hp = 5;

    private float attackDelay = 0.46f;
    private float timeAfterAttack = 0f;
    private float jumpForce = 500f;
    private float horizontalKnockBackForce = 400f;
    private float verticalKnockBackForce = 300f;

    private Vector3 monsterPosition;
    private float yDiff = 0f;

    public bool isHitRight;
    public bool isHitLeft;
    public bool isHitDown;

    private bool isRight;
    private bool isLeft;
    private bool isUp;
    private bool isDown;
    private bool isJumping;
    private bool isGround;
    private bool isInvincible;
    private bool isDead;
    private bool isKnockBack;
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
        isInvincible = false;
        isDead = false;
        isKnockBack = false;
        Debug.LogFormat("ü�� : " + hp + "\n");
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead == true)
        {
            playerAni.SetBool("IsDead", true);
            StartCoroutine(Dead());
        }

        if (hp <= 0)
        {
            isDead = true;
        }

        // ���� �����̸� ���� ��ŸŸ�� �߰�
        timeAfterAttack += Time.deltaTime;

        // ���� ����
        #region
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
            if (isHitRight == false && Input.GetKey(KeyCode.RightArrow) && !Input.GetKey(KeyCode.LeftArrow))
            {
                // �������� �̵��ӵ��� ����ϰ� �̵�, ������ ���� false��, �������� ���� true�� �ʱ�ȭ
                transform.position += moveSpeed * Time.deltaTime * transform.right;
                isLeft = false;
                isRight = true;
                playerAni.SetBool("IsLeft", false);
                playerAni.SetBool("IsRight", true);
                playerAni.SetBool("IsWalk", true);
            }
            // ���� ���ݼ����� �ƴϰ�, ���� ����Ű�� ������ ���̰� ���� ����Ű�� ������ ���� �ƴ� ��
            else if (isHitLeft == false && Input.GetKey(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.RightArrow))
            {
                // �������� �̵��ӵ��� ����ϰ� �̵�, �������� ���� false��, ������ ���� true�� �ʱ�ȭ
                transform.position -= moveSpeed * Time.deltaTime * transform.right;
                isLeft = true;
                isRight = false;
                playerAni.SetBool("IsLeft", true);
                playerAni.SetBool("IsRight", false);
                playerAni.SetBool("IsWalk", true);
            }
        }
        #endregion

        // xŰ(����) ����
        if (Input.GetKeyDown(KeyCode.X))
        {
            if (isKnockBack == true)
            {
                return;
            }
            // ���� ���� true�� ��
            if (isUp == true)
            {
                // ���� �� ���� �ð��� ���� �����̺��� ũ�ٸ�
                if (timeAfterAttack > attackDelay)
                {
                    if (isLeft == true)
                    {
                        // ���� ������ ��������Ʈ�� �ݶ��̴��� �״ٰ� �������ӽð��Ŀ� �� (���ʿ��� ���������� ����)
                        attackSprite = transform.GetChild((int)attackDirection.UP_ATTACK_L).GetComponent<SpriteRenderer>();
                        attackCollider = transform.GetChild((int)attackDirection.UP_ATTACK_L).GetComponent<Collider2D>();
                        attackAni = transform.GetChild((int)attackDirection.UP_ATTACK_L).GetComponent<Animator>();
                        timeAfterAttack = 0;
                        StartCoroutine(Attack());
                    }

                    if (isRight == true)
                    {
                        // ���� ������ ��������Ʈ�� �ݶ��̴��� �״ٰ� �������ӽð��Ŀ� �� (�����ʿ��� �������� ����)
                        attackSprite = transform.GetChild((int)attackDirection.UP_ATTACK_R).GetComponent<SpriteRenderer>();
                        attackCollider = transform.GetChild((int)attackDirection.UP_ATTACK_R).GetComponent<Collider2D>();
                        attackAni = transform.GetChild((int)attackDirection.UP_ATTACK_R).GetComponent<Animator>();
                        timeAfterAttack = 0;
                        StartCoroutine(Attack());
                    }
                }
            }

            // �Ʒ��� ���� true�̰�, ���� �پ������� false�� ��
            if (isGround == false && isDown == true)
            {
                // ���� �� ���� �ð��� ���� �����̺��� ũ�ٸ�
                if (timeAfterAttack > attackDelay)
                {
                    if (isLeft == true)
                    {
                        // �Ʒ��� ������ ��������Ʈ�� �ݶ��̴��� �״ٰ� �������ӽð��Ŀ� �� (���ʿ��� ���������� ����)
                        attackSprite = transform.GetChild((int)attackDirection.DOWN_ATTACK_L).GetComponent<SpriteRenderer>();
                        attackCollider = transform.GetChild((int)attackDirection.DOWN_ATTACK_L).GetComponent<Collider2D>();
                        attackAni = transform.GetChild((int)attackDirection.DOWN_ATTACK_L).GetComponent<Animator>();
                        timeAfterAttack = 0;
                        StartCoroutine(Attack());
                    }

                    if (isRight == true)
                    {
                        // �Ʒ��� ������ ��������Ʈ�� �ݶ��̴��� �״ٰ� �������ӽð��Ŀ� �� (�����ʿ��� �������� ����)
                        attackSprite = transform.GetChild((int)attackDirection.DOWN_ATTACK_R).GetComponent<SpriteRenderer>();
                        attackCollider = transform.GetChild((int)attackDirection.DOWN_ATTACK_R).GetComponent<Collider2D>();
                        attackAni = transform.GetChild((int)attackDirection.DOWN_ATTACK_R).GetComponent<Animator>();
                        timeAfterAttack = 0;
                        StartCoroutine(Attack());
                    }
                }
            }

            // ������ ���� true�̰� ���� ���� false�� ��
            if (isLeft == true && isUp == false)
            {
                // ���� �� ���� �ð��� ���� �����̺��� ũ�ٸ�
                if (timeAfterAttack > attackDelay)
                {
                    // ���� ������ ��������Ʈ�� �ݶ��̴�, �ִϸ����͸� ������
                    attackSprite = transform.GetChild((int)attackDirection.LEFT_ATTACK).GetComponent<SpriteRenderer>();
                    attackCollider = transform.GetChild((int)attackDirection.LEFT_ATTACK).GetComponent<Collider2D>();
                    attackAni = transform.GetChild((int)attackDirection.LEFT_ATTACK).GetComponent<Animator>();
                    timeAfterAttack = 0;
                    StartCoroutine(Attack());
                }
            }

            // �������� ���� true�̰� ���� ���� false�� ��
            if (isRight == true && isUp == false)
            {
                // ���� �� ���� �ð��� ���� �����̺��� ũ�ٸ�
                if (timeAfterAttack > attackDelay)
                {
                    // ������ ������ ��������Ʈ�� �ݶ��̴�, �ִϸ����͸� ������
                    attackSprite = transform.GetChild((int)attackDirection.RIGHT_ATTACK).GetComponent<SpriteRenderer>();
                    attackCollider = transform.GetChild((int)attackDirection.RIGHT_ATTACK).GetComponent<Collider2D>();
                    attackAni = transform.GetChild((int)attackDirection.RIGHT_ATTACK).GetComponent<Animator>();
                    timeAfterAttack = 0;
                    StartCoroutine(Attack());
                }
            }
        }
    }

    public int Get_Hp()
    {
        return hp;
    }

    IEnumerator Dead()
    {
        // 1������ �� ������Ʈ �ı�
        yield return null;
        Destroy(gameObject);
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
            //playerRigidbody.gravityScale = 1;
            playerAni.SetBool("IsGround", true);
            //playerAni.SetBool("IsFall", false);
        }

        if (isInvincible == false && collision.collider.CompareTag("Monster"))
        {
            monsterPosition = collision.transform.position;
            yDiff = transform.position.y - monsterPosition.y;
            hp -= 1;
            Debug.LogFormat("ü�� : " + hp + "\n");
            StartCoroutine(KnockBack());
            StartCoroutine(GracePeriod());
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Platform") || collision.collider.CompareTag("StunBody"))
        {
            playerRigidbody.gravityScale = 1;
            playerAni.SetBool("IsGround", true);
            //playerAni.SetBool("IsFall", false);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Platform") || collision.collider.CompareTag("StunBody"))
        {
            isGround = false;
            playerAni.SetBool("IsGround", false);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (isInvincible == false && collision.CompareTag("MonsterAttack"))
        {
            monsterPosition = collision.transform.position;
            yDiff = transform.position.y - monsterPosition.y;
            hp -= 1;
            Debug.LogFormat("ü�� : " + hp + "\n");
            StartCoroutine(KnockBack());
            StartCoroutine(GracePeriod());
        }
    }
}
