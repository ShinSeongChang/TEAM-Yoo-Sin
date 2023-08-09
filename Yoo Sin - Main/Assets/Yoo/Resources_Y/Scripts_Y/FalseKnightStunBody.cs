using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FalseKnightStunBody: MonoBehaviour
{
    private int hp = 13;
    private Animator bodyAni;
    private Collider2D bodyCollider;
    // Start is called before the first frame update
    void Start()
    {
        bodyAni = GetComponent<Animator>();
        bodyCollider = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        // �ٵ��� ü���� 0�̰�, �ٵ��ݶ��̴� Ʈ���Ű� ��Ȱ��ȭ �̸�
        if (hp == 0 && bodyCollider.isTrigger == false) 
        {
            // �ٵ��ݶ��̴� Ʈ���� Ȱ��ȭ
            bodyCollider.isTrigger = true;
        }
        // �ٵ��� ü���� 0�̰�, ���� ���� ������Ʈ�� �������� 0�̶��
        if (hp == 0 && transform.parent.transform.localScale == Vector3.zero)
        {
            // �ٵ��� ü�� 13���� �ʱ�ȭ, �ٵ�ִϸ�����, �ٵ��ݶ��̴� Ʈ���� ��Ȱ��ȭ
            hp = 13;
            bodyAni.enabled = false;
            bodyCollider.isTrigger = false;
        }
        // �ٵ��� �ִϸ����Ͱ� ��Ȱ��ȭ �����̰�, ���ϰ��� ������Ʈ�� �������� 1�̶��
        if (bodyAni.enabled == false && transform.parent.transform.localScale == Vector3.one)
        {
            // �ٵ��� �ִϸ����� Ȱ��ȭ
            bodyAni.enabled = true;
        }
    }

    // Ʈ���� ���˽�
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // �ٵ��� ü���� 0���� ũ��, ������ Ʈ���Ű� �÷��̾��� �����±׸� ������ �ִٸ�
        if (hp > 0 && collision.CompareTag("PlayerAttack"))
        {
            StartCoroutine(Hit());
        }
    }

    // �ǰݽ� �ٵ��� ü���� �����ϵ����ϰ�, �ִϸ����͸� �����ϴ� Hit�ڷ�ƾ
    IEnumerator Hit()
    {
        // �ٵ��� �ִϸ������� IsHit�� true�� �ʱ�ȭ��
        bodyAni.SetBool("IsHit", true);
        // Hit�ִϸ��̼� ���ӽð���ŭ ��ٸ� ��
        yield return new WaitForSeconds(0.22f);
        // �ٵ��� ü���� 1 ���ҽ�Ű�� �ٵ��� �ִϸ������� IsHit�� false�� �ʱ�ȭ��
        hp -= 1;
        bodyAni.SetBool("IsHit", false);
    }
}
