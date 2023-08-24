using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HornetEffect_DashG : MonoBehaviour
{
    Animator dashGEffect;
    // Start is called before the first frame update
    void Start()
    {
        dashGEffect = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (dashGEffect.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.98f)
        {
            Destroy(gameObject);
        }
    }
}
