using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HornetIntro : MonoBehaviour
{
    private BoxCollider2D myCollider = default;
    private bool isIntro = false;

    // Start is called before the first frame update
    void Start()
    {
        myCollider = GetComponent<BoxCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" && isIntro == false)
        {
            isIntro = true;
            GameManager.instance.HornetIntro();
        }
    }
}
