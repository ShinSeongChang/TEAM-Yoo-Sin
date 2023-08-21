using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvisibleDoor : MonoBehaviour
{
    private BoxCollider2D myCollider = default;
    private SpriteRenderer mySprite = default;
    private Animator myAni = default;


    // Start is called before the first frame update
    void Start()
    {
        myCollider = GetComponent<BoxCollider2D>();
        mySprite = GetComponent<SpriteRenderer>();
        myAni = GetComponent<Animator>();        
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {           
            myAni.SetTrigger("InvisibleClear");
            myCollider.isTrigger = false;
        }
    }

}
