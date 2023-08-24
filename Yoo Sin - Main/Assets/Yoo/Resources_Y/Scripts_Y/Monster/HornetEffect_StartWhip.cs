using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HornetEffect_StartWhip : MonoBehaviour
{
    Animator startWhipEffect;
    // Start is called before the first frame update
    void Start()
    {
        startWhipEffect = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (startWhipEffect.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.98f)
        {
            Destroy(gameObject);
        }
    }
}
