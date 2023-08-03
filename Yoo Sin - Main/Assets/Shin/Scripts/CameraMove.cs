using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public GameObject player = default;
    // Start is called before the first frame update
    void Start()
    {
        //Vector3 offset = transform.position - 
    }

    // Update is called once per frame
    void Update()
    {

        transform.position = new Vector2(player.transform.position.x, player.transform.position.y);
    }
}
