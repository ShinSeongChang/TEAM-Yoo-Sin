using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FalseKnightIntro_Y : MonoBehaviour
{
    private BoxCollider2D myCollider = default;
    private bool isIntro = false;
    private CinemachineVirtualCamera falseCamera;

    // Start is called before the first frame update
    void Start()
    {
        myCollider = GetComponent<BoxCollider2D>();
        falseCamera = GameObject.FindGameObjectWithTag("FalseCam").GetComponent<CinemachineVirtualCamera>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player" && isIntro == false)
        {
            isIntro = true;
            GameManager.instance.FalseKngiht_Intro();
            falseCamera.enabled = true;
        }

        if(collision.tag == "Player" && GameManager.instance.boss1Die == true)
        {
            falseCamera.enabled = false;
        }
    }
}
