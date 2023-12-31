using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hornet_Thread : MonoBehaviour
{
    private Hornet_Needle needle;
    private Animator threadAni;
    // Start is called before the first frame update
    void Start()
    {
        needle = transform.GetComponentInParent<Hornet_Needle>();
        threadAni = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(needle.Get_IsReturn() == true && transform.localScale == Vector3.zero)
        {
            transform.localScale = Vector3.one;
            StartCoroutine(threadAniOn());
        }
    }

    IEnumerator threadAniOn()
    {
        threadAni.enabled = true;
        while(true)
        {
            if(threadAni.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.98f)
            {
                transform.localScale = Vector3.zero;
                threadAni.enabled = false;
                yield break;
            }
            yield return null;
        }
    }
}
