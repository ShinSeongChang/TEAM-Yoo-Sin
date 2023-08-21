using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator : MonoBehaviour
{
    private BoxCollider2D myCollider = default;
    private Rigidbody2D myRigid = default;

    // Start is called before the first frame update
    void Start()
    {
        myCollider = GetComponent<BoxCollider2D>();
        myRigid = GetComponent<Rigidbody2D>();

    }

    private void Update()
    {
        // �÷��̾� ���Ͻ� ������� ���������� �и�����
        if(GameManager.instance.boss1Die == true)
        {
            // ������ ������ ���������� �浹�� ���� �÷��̾� �ִϸ��̼� ����, ���� �ִϸ��̼� ����, ��� ����� �� �߻�
            // => ���������� �浹�ۿ��� ���ֱ����� �ݶ��̴� offset���� �ٸ���ġ�� �����ξ��ٰ� ������ ������ �������� �����ϵ��� ����
            myCollider.offset = new Vector2(0f, -0.1f);
        }

        // ��ǥ���̱��� �ö���� ������ ����
        if(transform.position.y >= 28.7f)
        {
            myRigid.constraints = RigidbodyConstraints2D.FreezeAll;
        }

        // �÷��̾ ���������Ϳ��� ��� ���� ���������Ͱ� ������ġ���� �������� ���߱�
        if (transform.position.y < 2.65f)
        {
            myRigid.velocity = Vector2.zero;
            transform.position = new Vector2(transform.position.x, 2.66f);
        }
    }

    // ���������� ���
    private void OnCollisionStay2D(Collision2D collision)
    {
        // ���������Ͱ� ��ǥ ��ġ���� ����
        if (transform.position.y < 28.7f)
        {            
            // ������ ���� ���¿��� �÷��̾ �����ߴٸ�
            if (GameManager.instance.boss1Die == true && collision.collider.tag == "Player")
            {
                // ���������ʹ� ���� ����ϴ� �ӵ��� ������.
                if (transform.position.y < 28.7f)
                {
                    myRigid.velocity = Vector2.up * 4f;            
                }
            }            
        }
    }

    // ���������� �ϰ�
    private void OnCollisionExit2D(Collision2D collision)
    {
        // ������ �������¿��� �÷��̾ ���������͸� �����
        if (GameManager.instance.boss1Die == true && collision.collider.tag == "Player")
        {
            // ���������ʹ� �ϰ��Ѵ�.
            if (transform.position.y < 28.7f)
            {
                myRigid.velocity = Vector2.down * 4f;
            }
        }
    }

}
