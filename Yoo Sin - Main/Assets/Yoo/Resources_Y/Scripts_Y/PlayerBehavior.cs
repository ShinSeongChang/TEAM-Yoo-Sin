using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerBehavior : MonoBehaviour
{
    public float moveSpeed = 7f;

    private Rigidbody2D playerRigidbody;
    private Animator playerAni;
    private WaitForSeconds attackRemainTime;
    private SpriteRenderer attackSprite;
    private Collider2D attackCollider;
    private Animator attackAni;

    private int jumpCount = 1;

    private float attackDelay = 0.6f;
    private float timeAfterAttack = 0f;
    private float jumpForce = 420f;
    private float gravityForce = 3f;

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
        // ���� �����̸� ���� ��ŸŸ�� �߰�
        timeAfterAttack += Time.deltaTime;

        // ���� ����
        #region
        // ���� ������ true�̰� zŰ(����Ű)�� ������ ����
        if (Input.GetKeyDown(KeyCode.Z) && isGround == true)
        {
            // ���� ī��Ʈ 0���� �ʱ�ȭ, ���� ������ false���� �ʱ�ȭ
            jumpCount = 0;
            isGround = false;
            playerAni.SetBool("IsGround", false);
            // ���� �� �߰�
            playerRigidbody.AddForce(new Vector2(0, jumpForce));
        }

        // ���� ī��Ʈ�� 0 �̰� ���� ������ false�̰� zŰ(����Ű)�� ������ ���� ��
        if (Input.GetKey(KeyCode.Z) && jumpCount == 0 && isGround == false)
        {
            // ���� ���� true�� �ʱ�ȭ
            isJumping = true;
        }

        // ���� �پ����� �ʰ� zŰ(����Ű)�� ���� ����
        if (Input.GetKeyUp(KeyCode.Z) && isGround == false)
        {
            // ���� ������ false�� �ʱ�ȭ, �������� ������ true�� �ʱ�ȭ
            isJumping = false;
            playerAni.SetBool("IsFall", true);
        }

        // ���� ���� false�̰� ���� �پ������� false�̰ų� �÷��̾��� ������ٵ��� ���ν�Ƽ�� y���� 0 �̸��̶��(���� �ö󰡴� ���� 0�̸��̶��)
        if (isJumping == false && isGround == false || playerRigidbody.velocity.y < 0)
        {
            // ������ �� ���� ����������, �������� ������ true�� �ʱ�ȭ
            playerRigidbody.AddForce(new Vector2(0, -gravityForce));
            playerAni.SetBool("IsFall", true);
        }
        #endregion

        // ����Ű(�̵�) ����
        #region
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

        // ���� ����Ű�� ������ ���̰� ���� ����Ű�� ������ ���� �ƴ� ��
        if (Input.GetKey(KeyCode.RightArrow) && !Input.GetKey(KeyCode.LeftArrow))
        {
            // �������� �̵��ӵ��� ����ϰ� �̵�, ������ ���� false��, �������� ���� true�� �ʱ�ȭ
            transform.position += moveSpeed * Time.deltaTime * transform.right;
            isLeft = false;
            isRight = true;
            playerAni.SetBool("IsLeft", false);
            playerAni.SetBool("IsRight", true);
            playerAni.SetBool("IsWalk", true);
        }
        // ���� ����Ű�� ������ ���̰� ���� ����Ű�� ������ ���� �ƴ� ��
        else if (Input.GetKey(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.RightArrow))
        {
            // �������� �̵��ӵ��� ����ϰ� �̵�, �������� ���� false��, ������ ���� true�� �ʱ�ȭ
            transform.position += -moveSpeed * Time.deltaTime * transform.right;
            isLeft = true;
            isRight = false;
            playerAni.SetBool("IsLeft", true);
            playerAni.SetBool("IsRight", false);
            playerAni.SetBool("IsWalk", true);
        }
        // �� �� ���
        else
        {
            // �ȴ� ���� false�� �ʱ�ȭ
            playerAni.SetBool("IsWalk", false);
        }
        #endregion

        // xŰ(����) ����
        if (Input.GetKeyDown(KeyCode.X))
        {
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
                    if(isLeft == true)
                    {
                        // �Ʒ��� ������ ��������Ʈ�� �ݶ��̴��� �״ٰ� �������ӽð��Ŀ� �� (���ʿ��� ���������� ����)
                        attackSprite = transform.GetChild((int)attackDirection.DOWN_ATTACK_L).GetComponent<SpriteRenderer>();
                        attackCollider = transform.GetChild((int)attackDirection.DOWN_ATTACK_L).GetComponent<Collider2D>();
                        attackAni = transform.GetChild((int)attackDirection.DOWN_ATTACK_L).GetComponent<Animator>();
                        timeAfterAttack = 0;
                        StartCoroutine(Attack());
                    }

                    if(isRight == true)
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        isGround = true;
        playerAni.SetBool("IsGround", true);
        playerAni.SetBool("IsFall", false);
        jumpCount = 1;
    }
}
