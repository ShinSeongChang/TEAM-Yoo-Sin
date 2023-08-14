using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goam_Cross : MonoBehaviour
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

        StartCoroutine(FirstAttack());
    }

    IEnumerator FirstAttack()
    {       
        yield return new WaitForSeconds(1.95f);

        goamAnimator.SetTrigger("isAttack");
        StartCoroutine(GoamAttack());

    }
    IEnumerator GoamAttack()
    {
        goamCollider.enabled = true;
        goamSprite.enabled = true;

        yield return new WaitForSeconds(0.45f);

        goamAnimator.SetTrigger("isIdle");

        StartCoroutine(GoamReturn());
        
    }

    IEnumerator GoamReturn()
    {
        yield return new WaitForSeconds(1.5f);

        goamAnimator.SetTrigger("isReturn");

        StartCoroutine(GoamStay());
    }

    IEnumerator GoamStay()
    {
        goamCollider.enabled = false;
        yield return new WaitForSeconds(0.45f);

        goamSprite.enabled = false;
        
        yield return new WaitForSeconds(1.5f);

        goamCollider.enabled = true;
        goamSprite.enabled = true;

        goamAnimator.SetTrigger("isAttack");

        StartCoroutine(GoamAttack());

    }
}
