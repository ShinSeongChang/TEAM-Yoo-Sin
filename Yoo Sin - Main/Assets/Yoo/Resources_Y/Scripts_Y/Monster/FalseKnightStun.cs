using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FalseKnightStun : MonoBehaviour
{
    public FalseKnightStunHead head;
    private Animator falseKnightAni;
    private SpriteRenderer falseKnightSprite;
    // Start is called before the first frame update
    void Start()
    {
        falseKnightAni = transform.parent.GetComponent<Animator>();
        falseKnightSprite = transform.parent.GetComponent<SpriteRenderer>();
        head = transform.GetComponentInChildren<FalseKnightStunHead>();
    }

    // Update is called once per frame
    void Update()
    {
        // ���� ���� �ϴ� ������Ʈ�� �������� 0�̰�, ����� �ִϸ������� IsHeadOpen�� true���
        if(transform.localScale == Vector3.zero && falseKnightAni.GetBool("IsHeadOpen") == true)
        {
            // ����� ��������Ʈ�� ����, ���� ���� ������Ʈ�� �������� 1�� �ʱ�ȭ��
            falseKnightSprite.enabled = false;
            transform.localScale = Vector3.one;
        }

        // ���� ���� �ϴ� ������Ʈ�� �������� 1�̰�, ����� �ִϸ������� IsHeadOpen�� false���
        if(transform.localScale == Vector3.one && falseKnightAni.GetBool("IsHeadOpen") == false)
        {
            // ����� ��������Ʈ�� Ű��, ���� ���� ������Ʈ�� �������� 0���� �ʱ�ȭ��
            falseKnightSprite.enabled = true;
            transform.localScale = Vector3.zero;
        }
    }
}
