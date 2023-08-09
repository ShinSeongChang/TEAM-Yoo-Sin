using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackLeft : MonoBehaviour
{
    private Rigidbody2D playerRigidbody;
    private WaitForSeconds attackRemainTime;
    private PlayerBehavior playerAct;
    private float repulsForce = 230f;
    private bool isAttacking = false;
    // Start is called before the first frame update
    void Start()
    {
        playerRigidbody = transform.GetComponentInParent<Rigidbody2D>();
        playerAct = transform.GetComponentInParent<PlayerBehavior>();
        attackRemainTime = new WaitForSeconds(0.2f);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Monster") && isAttacking == false)
        {
            StartCoroutine(Hit());
        }
    }

    IEnumerator Hit()
    {
        isAttacking = true;
        playerAct.isHitLeft = true;
        playerRigidbody.velocity = Vector2.zero;
        playerRigidbody.AddForce(new Vector2(repulsForce, 0));
        yield return attackRemainTime;
        isAttacking = false;
        playerAct.isHitLeft = false;
        yield break;
    }
}
