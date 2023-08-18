using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUi : MonoBehaviour
{
    private Transform childHealth = default;
    private Animator[] childAnimator = default;

    private int lifeCount = 5;
    
    // Start is called before the first frame update
    void Awake()
    {
        childAnimator = new Animator[lifeCount];
        childHealth = transform.GetComponentInChildren<Transform>();

        for(int i = 0; i < childAnimator.Length; i++)
        {
            childAnimator[i] = childHealth.GetChild(i).GetComponentInChildren<Animator>();
        }

    }

    public void HealtDown()
    {
        lifeCount -= 1;

        childAnimator[lifeCount].SetTrigger("isHit");
    }
}
