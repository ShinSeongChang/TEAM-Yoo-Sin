using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackDown : MonoBehaviour
{
    private Rigidbody2D playerRigidbody;
    private WaitForSeconds attackRemainTime;
    private PlayerBehavior playerAct;
    private float repulsForce = 300f;
    // Start is called before the first frame update
    void Start()
    {
        playerRigidbody = transform.GetComponentInParent<Rigidbody2D>();
        playerAct = transform.GetComponentInParent<PlayerBehavior>();
        attackRemainTime = new WaitForSeconds(0.3f);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Monster"))
        {
            StartCoroutine(Hit());
        }
    }

    IEnumerator Hit()
    {
        playerAct.isHitDown = true;
        playerRigidbody.gravityScale = 1;
        playerRigidbody.velocity = Vector2.zero;
        playerRigidbody.AddForce(new Vector2(0 ,repulsForce));
        yield return attackRemainTime;
        playerAct.isHitDown = false;
        yield break;
    }
}
