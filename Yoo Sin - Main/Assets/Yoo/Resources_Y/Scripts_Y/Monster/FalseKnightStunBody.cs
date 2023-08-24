using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FalseKnightStunBody: MonoBehaviour
{
    private int hp = 13;
    private SkillGauge_Y skillGauge;
    private Animator bodyAni;
    private Collider2D bodyCollider;
    private AudioSource falseAudio;
    private FalseKnightBehavior falseBehavior;
    // Start is called before the first frame update
    void Start()
    {
        skillGauge = GameObject.Find("GaugeImg").GetComponent<SkillGauge_Y>();
        bodyAni = GetComponent<Animator>();
        bodyCollider = GetComponent<Collider2D>();
        falseAudio = transform.parent.transform.parent.GetComponent<AudioSource>();
        falseBehavior = transform.parent.transform.parent.GetComponent<FalseKnightBehavior>();
    }

    // Update is called once per frame
    void Update()
    {
        // �ٵ��� ü���� 0�̸�
        if (hp == 0) 
        {
            // �ٵ��ݶ��̴� ��Ȱ��ȭ
            bodyCollider.enabled = false;
        }
        // �ٵ��� ü���� 0�̰�, ���� ���� ������Ʈ�� �������� 0�̶��
        if (hp == 0 && transform.parent.transform.localScale == Vector3.zero)
        {
            // �ٵ��� ü�� 13���� �ʱ�ȭ, �ٵ�ִϸ����� ��Ȱ��ȭ, �ٵ��ݶ��̴� Ȱ��ȭ
            hp = 13;
            bodyAni.enabled = false;
            bodyCollider.enabled = true;
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
        // �� �´� �Ҹ���
        falseAudio.PlayOneShot(falseBehavior.hitSounds[0]);
        // �ٵ��� �ִϸ������� IsHit�� true�� �ʱ�ȭ��
        bodyAni.SetBool("IsHit", true);
        // Hit�ִϸ��̼� ���ӽð���ŭ ��ٸ� ��
        yield return new WaitForSeconds(0.22f);
        // �ٵ��� ü���� 1 ���ҽ�Ű�� �ٵ��� �ִϸ������� IsHit�� false�� �ʱ�ȭ��
        hp -= 1;
        bodyAni.SetBool("IsHit", false);
        skillGauge.GaugePlus();
        yield break;
    }
}
