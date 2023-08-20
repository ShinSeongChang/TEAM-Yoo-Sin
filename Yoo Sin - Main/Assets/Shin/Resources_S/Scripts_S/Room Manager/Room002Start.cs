using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room002Start : MonoBehaviour
{

    public GameObject player = default;
    public Transform room001Finish = default;

    // Room002 �������� ���޽� �÷��̾ Room001 ���������� �̵�
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            Vector3 moveRoom = new Vector3(room001Finish.position.x - 5.0f, 29.45f, room001Finish.position.z);
            player.transform.position = moveRoom;
        }
    }

}
