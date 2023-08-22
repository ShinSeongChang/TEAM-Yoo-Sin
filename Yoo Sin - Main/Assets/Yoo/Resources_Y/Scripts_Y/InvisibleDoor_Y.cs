using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvisibleDoor_Y : MonoBehaviour
{
    private BoxCollider2D myCollider = default;
    private SpriteRenderer mySprite = default;
    private Animator myAni = default;
    private CinemachineVirtualCamera hornetCamera;

    // Start is called before the first frame update
    void Start()
    {
        myCollider = GetComponent<BoxCollider2D>();
        mySprite = GetComponent<SpriteRenderer>();
        myAni = GetComponent<Animator>();        
        hornetCamera = GameObject.FindGameObjectWithTag("HornetCam").GetComponent<CinemachineVirtualCamera>();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {           
            myAni.SetTrigger("InvisibleClear");
            myCollider.isTrigger = false;
            hornetCamera.enabled = true;
        }
    }

}
