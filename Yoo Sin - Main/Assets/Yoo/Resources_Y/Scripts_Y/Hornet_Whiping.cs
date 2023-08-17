using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hornet_Whiping : MonoBehaviour
{
    private bool isEnd = false;
    private Animator hornetAni;
    // Start is called before the first frame update
    void Start()
    {
        hornetAni = GameObject.Find("Hornet").GetComponent<Animator>();
        StartCoroutine(AttackRemain());
    }

    // Update is called once per frame
    void Update()
    {
        if(isEnd == true)
        {
            hornetAni.SetTrigger("WhipEnd");
            Destroy(gameObject);
        }
    }

    IEnumerator AttackRemain()
    {
        yield return new WaitForSeconds(1.5f);
        isEnd = true;
        yield break;
    }
}
