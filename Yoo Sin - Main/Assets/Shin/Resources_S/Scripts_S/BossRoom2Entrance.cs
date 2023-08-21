using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRoom2Entrance : MonoBehaviour
{
    private BoxCollider2D myCollider = default;

    // Start is called before the first frame update
    void Start()
    {
        myCollider = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if(GameManager.instance.boss1Die == true)
        {
            gameObject.SetActive(false);
        }
    }
}
