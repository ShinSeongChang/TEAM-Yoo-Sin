using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goam : MonoBehaviour
{
    private CapsuleCollider2D goamCollider = default;
    private Animator goamAnimator = default;
    private SpriteRenderer goamSprite = default;


    // Start is called before the first frame update
    void Start()
    {
        goamCollider = GetComponent<CapsuleCollider2D>();
        goamAnimator = GetComponent<Animator>();
        goamSprite = GetComponent<SpriteRenderer>();


        goamCollider.enabled = false;
        goamSprite.enabled = false;


        goamAnimator.SetTrigger("isAttack");
        StartCoroutine(GoamAttack());
    }   

    IEnumerator GoamAttack()
    {
        goamCollider.enabled = true;
        goamSprite.enabled = true;
        StartCoroutine(OffsetUp());

        yield return new WaitForSeconds(0.45f);

        goamAnimator.SetTrigger("isIdle");

        StartCoroutine(GoamReturn());
        
    }

    IEnumerator GoamReturn()
    {
        yield return new WaitForSeconds(1.5f);

        goamCollider.offset = new Vector2(goamCollider.offset.x, -3f);
        goamAnimator.SetTrigger("isReturn");        
        StartCoroutine(GoamStay());
    }

    IEnumerator GoamStay()
    {
        goamCollider.enabled = false;
        yield return new WaitForSeconds(0.45f);

        goamSprite.enabled = false;
        
        yield return new WaitForSeconds(1.5f);

        goamAnimator.SetTrigger("isAttack");

        StartCoroutine(GoamAttack());

    }

    IEnumerator OffsetUp()
    {
        yield return new WaitForSeconds(0.1f);

        goamCollider.offset = new Vector2(goamCollider.offset.x, -1.3f);
        goamCollider.size = new Vector2(goamCollider.size.x, 3f);


        yield return new WaitForSeconds(0.1f);

        goamCollider.offset = new Vector2(goamCollider.offset.x, 0.5f);

        yield return new WaitForSeconds(0.1f);

        goamCollider.offset = new Vector2(goamCollider.offset.x, 0.8f);
        goamCollider.size = new Vector2(goamCollider.size.x, 3.5f);

        yield break;


    }

}
