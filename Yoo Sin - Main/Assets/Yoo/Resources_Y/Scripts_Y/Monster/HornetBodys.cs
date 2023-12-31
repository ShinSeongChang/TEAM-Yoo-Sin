using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HornetBodys : MonoBehaviour
{
    private Animator hornetAni;
    private SkillGauge_Y skillGauge;
    private Rigidbody2D hornetRigidbody;
    private float hornetRotationZ;
    private HornetBehavior hornetBehavior;
    private AudioSource hornetAudio;


    private Quaternion toLeft = Quaternion.Euler(0, 0, 0);
    private Quaternion toRight = Quaternion.Euler(0, 180, 0);
    // Start is called before the first frame update
    void Start()
    {
        hornetAni = transform.GetComponentInParent<Animator>();
        hornetRigidbody = transform.GetComponentInParent<Rigidbody2D>();
        hornetRotationZ = transform.parent.eulerAngles.z;
        hornetBehavior = transform.GetComponentInParent<HornetBehavior>();
        hornetAudio = transform.GetComponentInParent<AudioSource>();
        skillGauge = GameObject.Find("GaugeImg").GetComponent<SkillGauge_Y>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerAttack") == true)
        {
            hornetAudio.PlayOneShot(hornetBehavior.hitSound);
            if (hornetAni.GetBool("IsStun") == false)
            {
                hornetBehavior.HpDown();
                skillGauge.GaugePlus();
            }
            if (hornetAni.GetBool("IsStun") == true && hornetAni.GetBool("IsIdle") == false)
            {
                hornetBehavior.StunHpDown();
                skillGauge.GaugePlus();
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (hornetRigidbody.gravityScale != 5)
        {
            hornetRigidbody.gravityScale = 5;
        }

        if (collision.collider.CompareTag("Platform"))
        {
            hornetRigidbody.velocity = Vector2.zero;
            hornetAni.SetBool("IsGround", true);

            if (hornetAni.GetBool("IsIdle") == false)
            {
                //Debug.Log("여기들어옴?");
                if (hornetBehavior.Get_LookLeft() == true)
                {
                    transform.parent.transform.rotation = toLeft;
                    //Debug.Log("왼쪽으로");
                }
                else if (hornetBehavior.Get_LookLeft() == false)
                {
                    transform.parent.transform.rotation = toRight;
                    //Debug.Log("오른쪽으로");
                }
            }

            //if (hornetRigidbody.gravityScale != 5)
            //{
            //    hornetRigidbody.gravityScale = 5;
            //}
        }

        if (collision.collider.CompareTag("Wall"))
        {
            hornetBehavior.isConer = true;

            //if (hornetRigidbody.gravityScale != 5)
            //{
            //    hornetRigidbody.gravityScale = 5;
            //}
        }

        if (collision.collider.CompareTag("Player"))
        {
            hornetBehavior.hitPlayer = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Platform"))
        {
            hornetAni.SetBool("IsGround", false);
        }

        if (collision.collider.CompareTag("Wall"))
        {
            hornetBehavior.isConer = false;

            //if (hornetRigidbody.gravityScale != 5)
            //{
            //    hornetRigidbody.gravityScale = 5;
            //}
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Wall"))
        {
            hornetBehavior.isConer = true;

            //if (hornetRigidbody.gravityScale != 5)
            //{
            //    hornetRigidbody.gravityScale = 5;
            //}
        }
    }
}
