using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackRight : MonoBehaviour
{
    private Rigidbody2D playerRigidbody;
    private WaitForSeconds attackRemainTime;
    private PlayerBehavior_F playerAct;
    public Animator playerAni;
    private float repulsForce = -300f;
    private bool isAttacking = false;
    // Start is called before the first frame update
    void Start()
    {
        playerRigidbody = GetComponentInParent<SpriteRenderer>().gameObject.GetComponentInParent<Rigidbody2D>();
        playerAct = GetComponentInParent<SpriteRenderer>().gameObject.GetComponentInParent<PlayerBehavior_F>();
        attackRemainTime = new WaitForSeconds(0.2f);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isAttacking == false)
        {
            if (other.CompareTag("Monster") || other.CompareTag("StunBody"))
            {
                if (playerAni.GetBool("IsRight") == false && repulsForce == -300f)
                {
                    repulsForce = 300f;
                }

                if (playerAni.GetBool("IsRight") == true && repulsForce == 300f)
                {
                    repulsForce = -300f;
                }
                StartCoroutine(Hit());
            }
        }
    }

    IEnumerator Hit()
    {
        isAttacking = true;
        playerAct.isHitFront = true;
        playerRigidbody.velocity = Vector2.zero;
        playerRigidbody.AddForce(new Vector2(repulsForce, 0));
        yield return attackRemainTime;
        isAttacking = false;
        playerAct.isHitFront = false;
        yield break;
    }
}
