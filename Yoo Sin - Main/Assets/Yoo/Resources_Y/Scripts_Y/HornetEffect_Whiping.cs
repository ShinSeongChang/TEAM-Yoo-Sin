using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HornetEffect_Whiping : MonoBehaviour
{
    private bool isEnd = false;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(AttackRemain());
    }

    // Update is called once per frame
    void Update()
    {
        if(isEnd == true)
        {
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
