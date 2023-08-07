using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackRight : MonoBehaviour
{
    private Rigidbody2D playerRigidbody;
    private WaitForSeconds attackRemainTime;
    private PlayerBehavior playerAct;
    private float repulsForce = -230f;
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
        playerAct.isHitRight = true;
        playerRigidbody.velocity = Vector2.zero;
        playerRigidbody.AddForce(new Vector2(repulsForce, 0));
        yield return attackRemainTime;
        playerAct.isHitRight = false;
        yield break;
    }
}
