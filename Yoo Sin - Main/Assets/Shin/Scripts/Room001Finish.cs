using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Room001Finish : MonoBehaviour
{
    public GameObject player = default;
    public Transform room2Start = default;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            Vector3 moveRoom = new Vector3(room2Start.position.x + 5.0f, room2Start.position.y, room2Start.position.z);
            player.transform.position = moveRoom;
        }
    }
}
