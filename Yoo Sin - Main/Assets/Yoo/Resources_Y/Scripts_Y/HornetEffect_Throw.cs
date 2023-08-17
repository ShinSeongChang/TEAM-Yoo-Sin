using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HornetEffect_Throw : MonoBehaviour
{
    Animator throwEffect;
    // Start is called before the first frame update
    void Start()
    {
        throwEffect = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(throwEffect.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.98f)
        {
            Destroy(gameObject);
        }
    }
}
