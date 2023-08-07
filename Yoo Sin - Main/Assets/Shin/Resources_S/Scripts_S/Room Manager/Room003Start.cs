using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room003Start : MonoBehaviour
{
    public GameObject player = default;
    public Transform room002Finish = default;

    // Room003 시작지점 도달시 플레이어를 Room002 끝지점으로 이동
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            Vector3 moveRoom = new Vector3(room002Finish.position.x - 5.0f, room002Finish.position.y, room002Finish.position.z);
            player.transform.position = moveRoom;
        }
    }
}
