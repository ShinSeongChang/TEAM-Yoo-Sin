using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HornetEffect_DashA : MonoBehaviour
{
    Animator dashAEffect;
    // Start is called before the first frame update
    void Start()
    {
        dashAEffect = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (dashAEffect.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.98f)
        {
            Destroy(gameObject);
        }
    }
}
