using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class Crawlid : MonoBehaviour
{
    public Rigidbody2D crawlidRigid = default;
    public SpriteRenderer crawlidSprite = default;
    public Animator crawlidAnimator = default;

    private float speed = 1.5f;

    public int vector = default;

    public bool isTurn = false;
    public bool isGround = false;
    public bool isCliff = false;
    public bool isCrash = false;

    // Start is called before the first frame update
    void Start()
    {
        crawlidRigid = GetComponent<Rigidbody2D>();
        crawlidSprite = GetComponent<SpriteRenderer>();
        crawlidAnimator = GetComponent<Animator>();

        // Crawlid �� ��������, �⺻ ��������Ʈ�� ������ �ٶ󺸴� �����̱⿡ -1 ���� �⺻���� �־���.
        vector = -1;
    }

    private void FixedUpdate()
    {
        // �÷��� ���� �����鼭 ������ȯ�� �ϰ� �ִ°��� �ƴ϶�� �ش� �������� �����Ѵ�.
        if(isGround.Equals(true) && isTurn.Equals(false) )
        {
            Vector2 crawlidPos = new Vector2(speed * vector, crawlidRigid.velocity.y);
            crawlidRigid.velocity = crawlidPos;            
        }

        // ���� �� ���� ������ġ �����ϱ�
        Vector2 front = new Vector2(transform.position.x + 0.5f * vector, transform.position.y);
        Vector2 frontDown = new Vector3(0f, -0.6f);
        // ���� �� ���� ������ġ �����ϱ�


        // ���� �� ���� ����
        Debug.DrawRay(front, frontDown, Color.yellow);

        // �÷��� ���� �������� (���߿����� ���̸� ���� �ʴ´�.) �ڽ��� �������� �Ʒ��� ���̷� �������� üũ�Ѵ�.    
        if (isGround.Equals(true))
        {
            RaycastHit2D hit = Physics2D.Raycast(front, frontDown, 1, LayerMask.GetMask("Platform"));

            // ���̰� ������ ������ �ݶ��̴��� ���ٸ� (�����̶��) ������ ���̴�.
            if(hit.collider == null)
            {
                isCliff = true;
                
            }
            else if(hit.collider != null)
            {
                isCliff = false;
            }

        }

        // �����̰ų� �浹�� �Ͼ�ٸ� ������ȯ�� ������ �ٲ��ش�.
        if (isCliff.Equals(true) || isCrash.Equals(true))
        {            
            // ���� ������ȯ�� �������� �浹�� �� �� �ʱ�ȭ, ������ȯ���� crawlid�� �̵����� �������� isTurn = true.
            isCrash = false;
            isTurn = true;

            crawlidAnimator.SetTrigger("isTurn");

            // ���� ��ȯ�ִϸ��̼� Ŭ���� ����Ǵ� �ð���ŭ crawlid�� ����д�.
            crawlidRigid.velocity = Vector2.zero;

            // crawlid�� ����� �ð�, ��������Ʈ ������ �Լ� CrawlidFlip ȣ��
            Invoke("CrawlidFlip", 0.5f);

            // �������� ��ȯ�� ���� -1�� ���Ͽ� ������ �ٽ� ����ش�.
            vector *= -1;
        }

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // crawlid���� �浹�� �߻��� ������ ��ǥ�� �޾Ƴ��� contact
        ContactPoint2D contact = collision.contacts[0];

        // crawlid�� �ڱ� ������ ��������
        Vector2 myPos = new Vector2(transform.position.x, transform.position.y);

        // �浹ü���� �Ÿ� ���
        Vector2 offset = collision.contacts[0].point - myPos;

        // Legacy : ��ǥ��, ���Ⱚ Ȯ�ο� Log
        // Debug.LogFormat("�浹ü �� : {0}", collision.contacts[0].point.normalized);
        // Debug.LogFormat("�浹ü���� ���Ⱚ : {0}", offset.normalized);


        // �浹 ������ �������� �̻��̶�� ( ���濡�� �浹�� �Ͼ�ٸ� ) �浹���� ������ �ٲ��ش�.
        if(offset.normalized.y >= -0.2f)
        {
            isCrash = true;
        }

        // ���߿��� �������� Ž���� �������� isGround
        if (collision.collider.tag == "Platform")
        {
            isGround = true;
        }

    }


    private void OnCollisionExit2D(Collision2D collision)
    {
        // ���߿��� �������� Ž���� �������� isGround
        if (collision.collider.tag == "Platform")
        {
            isGround = false;
        }
    }


    public void CrawlidFlip()
    {
        // �⺻ ��������Ʈ�� ���ʹ��� (-1 ����) �� �ٶ󺸱⿡ vector ���� ������ �ø� false, ����� �ø� true.
        if (vector <= 0)
        {
            crawlidSprite.flipX = false;
        }
        else
        {
            crawlidSprite.flipX = true;
        }

        isTurn = false;
    }

}
