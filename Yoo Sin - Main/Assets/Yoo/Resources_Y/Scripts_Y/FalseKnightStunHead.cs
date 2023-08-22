using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FalseKnightStunHead : MonoBehaviour
{
    private const int BODY = 1;

    public int hp = 8;
    private SkillGauge_Y skillGauge;
    private Animator headAni;
    private Animator bodyAni;
    // Start is called before the first frame update
    void Start()
    {
        skillGauge = GameObject.Find("GaugeImg").GetComponent<SkillGauge_Y>();
        headAni = GetComponent<Animator>();
        bodyAni = transform.parent.transform.GetChild(BODY).GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        // ����� ü���� 0�̰�, ���� ���� ������Ʈ�� �������� 0�̶��
        if(hp == 0 && transform.parent.transform.localScale == Vector3.zero) 
        {
            // ����� ü�� 8�� �ʱ�ȭ, ���ִϸ����� ��Ȱ��ȭ
            hp = 8;
            headAni.enabled = false;
        }
        // ����� �ִϸ����Ͱ� ��Ȱ��ȭ �����̰�, ���ϰ��� ������Ʈ�� �������� 1�̶��
        if (headAni.enabled == false && transform.parent.transform.localScale == Vector3.one)
        {
            // ����� �ִϸ����� Ȱ��ȭ
            headAni.enabled = true;
        }
    }

    // Ʈ���� ���˽�
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // ����� ü���� 0���� ũ��, ������ Ʈ���Ű� �÷��̾��� �����±׸� ������ �ִٸ�
        if(hp > 0 && collision.CompareTag("PlayerAttack")) 
        {
            StartCoroutine(Hit());
        }
    }

    // �ǰݽ� ����� ü���� �����ϵ����ϰ�, �ִϸ����͸� �����ϴ� Hit�ڷ�ƾ
    IEnumerator Hit()
    {
        // ����� �ִϸ����Ϳ� �ٵ��� �ִϸ������� IsHit�� true�� �ʱ�ȭ��
        headAni.SetBool("IsHit", true);
        bodyAni.SetBool("IsHit", true);
        // Hit�ִϸ��̼� ���ӽð���ŭ ��ٸ� ��
        yield return new WaitForSeconds(0.22f);
        // ����� ü���� 1 ���ҽ�Ű�� ����� �ִϸ����Ϳ� �ٵ��� �ִϸ������� IsHit�� false�� �ʱ�ȭ��
        hp -= 1;
        headAni.SetBool("IsHit", false);
        bodyAni.SetBool("IsHit", false);
        skillGauge.GaugePlus();
    }
}
