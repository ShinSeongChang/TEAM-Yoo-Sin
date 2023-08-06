using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room002Finish : MonoBehaviour
{

    public GameObject player = default;
    public Transform room003Start = default;

    // Room002 끝지점 도달시 플레이어를 Room003 시작지점으로 이동
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            Vector3 moveRoom = new Vector3(room003Start.position.x + 5.0f, room003Start.position.y, room003Start.position.z);
            player.transform.position = moveRoom;
        }
    }

}
