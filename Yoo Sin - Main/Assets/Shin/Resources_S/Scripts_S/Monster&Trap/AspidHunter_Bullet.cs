using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AspidHunter_Bullet : MonoBehaviour
{
    private Rigidbody2D bulletRigid = default;
    private CircleCollider2D bulletCollider = default;
    private Animator bulletAnimator = default;
    private SpriteRenderer bulletSprite = default;
    private Transform player = default;

    private float bulletSpeed = 8.0f;

    void Start()
    {
        bulletRigid = GetComponent<Rigidbody2D>();
        bulletCollider = GetComponent<CircleCollider2D>();
        bulletAnimator = GetComponent<Animator>();
        bulletSprite = GetComponent<SpriteRenderer>();
        player = GameObject.FindWithTag("Player").GetComponent<Transform>();

        Vector2 target = player.position - transform.position;

        bulletRigid.velocity = target.normalized * bulletSpeed;

        Debug.Log(bulletRigid.velocity);
        
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        Vector2 myPos = new Vector2(transform.position.x, transform.position.y);

        Vector2 offset = new Vector2(collision.transform.position.x - myPos.x, collision.transform.position.y - myPos.y);


        if (collision.tag.Equals("Player"))
        {
            //Debug.LogFormat("����ü ������ : {0}", myPos);
            Debug.LogFormat("���� : {0}", offset.normalized);

            bulletRigid.velocity = Vector2.zero;

            if (offset.normalized.x < 0f && (offset.normalized.y > -0.8f && offset.normalized.y < 0.8f))
            {
                bulletAnimator.SetTrigger("isLeft");
            }

            if (offset.normalized.x > 0f && (offset.normalized.y > -0.8f && offset.normalized.y < 0.8f))
            {
                bulletSprite.flipX = true;
                bulletAnimator.SetTrigger("isLeft");
            }

            if (offset.normalized.y < 0f && (offset.normalized.x > -0.2f && offset.normalized.x < 0.2f))
            {
                bulletAnimator.SetTrigger("isDown");
            }

            if (offset.normalized.y > 0f && (offset.normalized.x > -0.2f && offset.normalized.x < 0.2f))
            {
                bulletSprite.flipY = true;
                bulletAnimator.SetTrigger("isDown");
            }

            StartCoroutine(BulletDestroy());
        }
        else if (collision.tag.Equals("Platform"))
        {
            /*
                ====== Legacy : �÷����� �浹�Ͽ����� �浹������ �׸��� ��ǥ�� ���... �浹 �ش������� �޾ƿ����� �ݶ��̴��� �ٲ�� �ϳ�? ===========

            Debug.LogFormat("����ü ������ : {0}", myPos);
            Debug.Log(collision.transform.position);
            Debug.LogFormat("���� : {0}", offset.normalized);
            bulletRigid.velocity = Vector2.zero;

                ====== Legacy : �÷����� �浹�Ͽ����� �浹������ �׸��� ��ǥ�� ���... �浹 �ش������� �޾ƿ����� �ݶ��̴��� �ٲ�� �ϳ�? ===========
            */

            if (offset.normalized.y > 0f)
            {
                bulletAnimator.SetTrigger("isDown");
            }

            if (offset.normalized.y < 0f)
            {
                bulletSprite.flipY = true;
                bulletAnimator.SetTrigger("isDown");
            }

            StartCoroutine(BulletDestroy());
        }
        else if (collision.tag.Equals("Wall"))
        {
            bulletRigid.velocity = Vector2.zero;

            if (offset.normalized.x < 0f)
            {
                bulletAnimator.SetTrigger("isLeft");
            }

            if (offset.normalized.x > 0f)
            {
                bulletSprite.flipX = true;
                bulletAnimator.SetTrigger("isLeft");
            }

            StartCoroutine(BulletDestroy());
        }

    }

    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    Vector2 myPos = new Vector2(transform.position.x, transform.position.y);

    //    Vector2 offset = new Vector2(collision.transform.position.x - myPos.x, collision.transform.position.y - myPos.y);


    //    if (collision.collider.tag.Equals("Player"))
    //    {
    //        //Debug.LogFormat("����ü ������ : {0}", myPos);
    //        Debug.LogFormat("���� : {0}", offset.normalized);

    //        bulletRigid.velocity = Vector2.zero;

    //        if (offset.normalized.x < 0f && (offset.normalized.y > -0.8f && offset.normalized.y < 0.8f))
    //        {
    //            bulletAnimator.SetTrigger("isLeft");
    //        }

    //        if (offset.normalized.x > 0f && (offset.normalized.y > -0.8f && offset.normalized.y < 0.8f))
    //        {
    //            bulletSprite.flipX = true;
    //            bulletAnimator.SetTrigger("isLeft");
    //        }

    //        if (offset.normalized.y < 0f && (offset.normalized.x > -0.2f && offset.normalized.x < 0.2f))
    //        {
    //            bulletAnimator.SetTrigger("isDown");
    //        }

    //        if (offset.normalized.y > 0f && (offset.normalized.x > -0.2f && offset.normalized.x < 0.2f))
    //        {
    //            bulletSprite.flipY = true;
    //            bulletAnimator.SetTrigger("isDown");
    //        }

    //        StartCoroutine(BulletDestroy());
    //    }
    //    else if (collision.collider.tag.Equals("Platform"))
    //    {
    //        /*
    //            ====== Legacy : �÷����� �浹�Ͽ����� �浹������ �׸��� ��ǥ�� ���... �浹 �ش������� �޾ƿ����� �ݶ��̴��� �ٲ�� �ϳ�? ===========

    //        Debug.LogFormat("����ü ������ : {0}", myPos);
    //        Debug.Log(collision.transform.position);
    //        Debug.LogFormat("���� : {0}", offset.normalized);
    //        bulletRigid.velocity = Vector2.zero;

    //            ====== Legacy : �÷����� �浹�Ͽ����� �浹������ �׸��� ��ǥ�� ���... �浹 �ش������� �޾ƿ����� �ݶ��̴��� �ٲ�� �ϳ�? ===========
    //        */

    //        if (offset.normalized.y > 0f)
    //        {
    //            bulletAnimator.SetTrigger("isDown");
    //        }

    //        if (offset.normalized.y < 0f)
    //        {
    //            bulletSprite.flipY = true;
    //            bulletAnimator.SetTrigger("isDown");
    //        }

    //        StartCoroutine(BulletDestroy());
    //    }
    //    else if (collision.collider.tag.Equals("Wall"))
    //    {
    //        bulletRigid.velocity = Vector2.zero;

    //        if (offset.normalized.x < 0f)
    //        {
    //            bulletAnimator.SetTrigger("isLeft");
    //        }

    //        if (offset.normalized.x > 0f)
    //        {
    //            bulletSprite.flipX = true;
    //            bulletAnimator.SetTrigger("isLeft");
    //        }

    //        StartCoroutine(BulletDestroy());
    //    }
    //}

    IEnumerator BulletDestroy()
    {
        
        yield return new WaitForSeconds(0.15f);

        Destroy(this.gameObject);

    }
}
