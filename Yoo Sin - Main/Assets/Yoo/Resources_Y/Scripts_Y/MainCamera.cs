using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    private Camera main;
    public GameObject player;
    private Animator playerAni;
    // Start is called before the first frame update
    void Start()
    {
        playerAni = player.GetComponent<Animator>();
        main = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null)
        {
            if (playerAni.GetBool("IsDead") == true)
            {
                transform.position = player.transform.position + new Vector3(0, 1, -10);
                main.enabled = true;
            }
        }
    }
}
