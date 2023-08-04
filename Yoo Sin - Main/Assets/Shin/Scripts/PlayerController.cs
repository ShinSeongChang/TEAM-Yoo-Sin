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
        // ���� �����̸� ���� ��ŸŸ�� �߰�
        timeAfterAttack += Time.deltaTime;

        // ���� ������ true�̰� zŰ(����Ű)�� ������ ����
        if (Input.GetKeyDown(KeyCode.Z) && isGround == true)
        {
            // ���� ī��Ʈ 0���� �ʱ�ȭ, ���� ������ false���� �ʱ�ȭ
            jumpCount = 0;
            isGround = false;
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
            // ���� ������ false�� �ʱ�ȭ
            isJumping = false;
        }

        // ���� ���� false�̰�, ���� �پ������� false�̸�
        if (isJumping == false && isGround == false)
        {
            // ������ �� ���� ����������
            playerRigidbody.AddForce(new Vector2(0, -gravityForce));
        }

        // �� ����Ű�� ������ ���� ��
        if (Input.GetKey(KeyCode.UpArrow))
        {
            // ���� ���� true��, �Ʒ��� ���� false�� �ʱ�ȭ
            isUp = true;
            isDown = false;
        }

        // �� ����Ű�� ���� ����
        if (Input.GetKeyUp(KeyCode.UpArrow))
        {
            // ���� ���� false�� �ʱ�ȭ
            isUp = false;
        }

        // �Ʒ� ����Ű�� ���������̰� �� ����Ű�� ���������� �ƴ� ��
        if (Input.GetKey(KeyCode.DownArrow) && !Input.GetKey(KeyCode.UpArrow))
        {
            // �Ʒ��� ���� true�� �ʱ�ȭ
            isDown = true;
        }

        // �Ʒ� ����Ű�� ���� ����
        if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            // �Ʒ��� ���� false�� �ʱ�ȭ
            isDown = false;
        }

        // ���� ����Ű�� ������ ���̰� ���� ����Ű�� ������ ���� �ƴ� ��
        if (Input.GetKey(KeyCode.RightArrow) && !Input.GetKey(KeyCode.LeftArrow))
        {
            // �������� �̵��ӵ��� ����ϰ� �̵�, ������ ���� false��, �������� ���� true�� �ʱ�ȭ
            transform.position += moveSpeed * Time.deltaTime * transform.right;
            isLeft = false;
            isRight = true;
        }

        // ���� ����Ű�� ������ ���̰� ���� ����Ű�� ������ ���� �ƴ� ��
        if (Input.GetKey(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.RightArrow))
        {
            // �������� �̵��ӵ��� ����ϰ� �̵�, �������� ���� false��, ������ ���� true�� �ʱ�ȭ
            transform.position += -moveSpeed * Time.deltaTime * transform.right;
            isRight = false;
            isLeft = true;
        }

        // xŰ(����Ű)�� ������ ����
        if (Input.GetKeyDown(KeyCode.X))
        {
            // ���� ���� true�� ��
            if (isUp == true)
            {
                // ���� �� ���� �ð��� ���� �����̺��� ũ�ٸ�
                if (timeAfterAttack > attackDelay)
                {
                    // ���� ������ ��������Ʈ�� �ݶ��̴��� �״ٰ� �������ӽð��Ŀ� ��
                    attackSprite = transform.GetChild((int)attackDirection.UP_ATTACK).GetComponent<SpriteRenderer>();
                    attackCollider = transform.GetChild((int)attackDirection.UP_ATTACK).GetComponent<Collider2D>();
                    attackSprite.enabled = true;
                    attackCollider.enabled = true;
                    timeAfterAttack = 0;
                    StartCoroutine(Attack());
                }
            }

            // �Ʒ��� ���� true�̰�, ���� �پ������� false�� ��
            if (isDown == true && isGround == false)
            {
                // ���� �� ���� �ð��� ���� �����̺��� ũ�ٸ�
                if (timeAfterAttack > attackDelay)
                {
                    // �Ʒ��� ������ ��������Ʈ�� �ݶ��̴��� �״ٰ� �������ӽð��Ŀ� ��
                    attackSprite = transform.GetChild((int)attackDirection.DOWN_ATTACK).GetComponent<SpriteRenderer>();
                    attackCollider = transform.GetChild((int)attackDirection.DOWN_ATTACK).GetComponent<Collider2D>();
                    attackSprite.enabled = true;
                    attackCollider.enabled = true;
                    timeAfterAttack = 0;
                    StartCoroutine(Attack());
                }
            }

            // ������ ���� true�̰� ���� ���� false�� ��
            if (isLeft == true && isUp == false)
            {
                // ���� �� ���� �ð��� ���� �����̺��� ũ�ٸ�
                if (timeAfterAttack > attackDelay)
                {
                    // ���� ������ ��������Ʈ�� �ݶ��̴��� �״ٰ� �������ӽð��Ŀ� ��
                    attackSprite = transform.GetChild((int)attackDirection.LEFT_ATTACK).GetComponent<SpriteRenderer>();
                    attackCollider = transform.GetChild((int)attackDirection.LEFT_ATTACK).GetComponent<Collider2D>();
                    attackSprite.enabled = true;
                    attackCollider.enabled = true;
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
                    // ������ ������ ��������Ʈ�� �ݶ��̴��� �״ٰ� �������ӽð��Ŀ� ��
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
        // ���� ���� �ð��� ���� �� �Ʒ� ���� ������
        yield return attackRemainTime;
        // ���� ��������Ʈ�� ���� �ݶ��̴��� �� �� �ڷ�ƾ ����
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
