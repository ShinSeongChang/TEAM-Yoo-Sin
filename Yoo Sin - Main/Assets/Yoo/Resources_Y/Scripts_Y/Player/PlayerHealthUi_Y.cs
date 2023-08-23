using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUi_Y : MonoBehaviour
{
    private Transform childHealth = default;
    private Animator[] childAnimator = default;
    private WaitForSeconds blinkTerm;
    private int lifeCount = 5;
    
    // Start is called before the first frame update
    void Awake()
    {
        childAnimator = new Animator[lifeCount];
        childHealth = transform.GetComponentInChildren<Transform>();
        blinkTerm = new WaitForSeconds(2.5f);
        for(int i = 0; i < childAnimator.Length; i++)
        {
            childAnimator[i] = childHealth.GetChild(i).GetComponentInChildren<Animator>();
        }
        StartCoroutine(Blink());
    }

    public void HealtDown()
    {
        lifeCount -= 1;

        childAnimator[lifeCount].SetTrigger("isHit");
    }

    public void HealtUp()
    {
        childAnimator[lifeCount].SetTrigger("isHeal");

        lifeCount += 1;
    }

    public int GetHp() 
    {
        return lifeCount;
    }

    IEnumerator Blink()
    {
        while (true)
        {
            yield return blinkTerm;
            for(int i = childAnimator.Length - 1; i > -1; i--)
            {
                if (childAnimator[i].GetCurrentAnimatorStateInfo(0).IsName("PlayerHealth") == true)
                {
                    childAnimator[i].SetTrigger("isBlink");
                }
            }
            yield return null;
        }
    }
}
