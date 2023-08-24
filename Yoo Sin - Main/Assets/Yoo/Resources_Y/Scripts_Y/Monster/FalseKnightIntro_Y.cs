using Cinemachine;
using System;
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
            falseCamera.enabled = true;
            GameManager.instance.FalseKngiht_Intro();
            BgmMainController.instance.sources[0].clip = BgmMainController.instance.bgms[1];
            BgmMainController.instance.sources[0].Play();
            BgmMainController.instance.sources[1].enabled = false;
        }

        if((collision.tag == "Player" && GameManager.instance.boss1Die == true) && falseCamera.enabled == true)
        {
            falseCamera.enabled = false;
            BgmMainController.instance.sources[0].clip = BgmMainController.instance.bgms[0];
            BgmMainController.instance.sources[0].Play();
            BgmMainController.instance.sources[1].enabled = true;
            BgmMainController.instance.sources[1].Play();
        }
    }
}
