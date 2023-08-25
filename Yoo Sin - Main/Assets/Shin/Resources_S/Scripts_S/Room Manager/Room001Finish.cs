using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Room001Finish : MonoBehaviour
{
    //[SerializeField] GameObject RoomMove;
    public Transform player = default;
    public Transform room2Start = default;

    // Room001 ������ ���޽� �÷��̾ Room002 ������������ �̵�
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            Vector3 moveRoom = new Vector3(room2Start.position.x + 2.0f, 31.65f, room2Start.position.z);
            player.transform.position = moveRoom;

            //RoomMove.SetActive(true);

            //StartCoroutine(MoveComplete());
        }
    }

    //IEnumerator MoveComplete()
    //{
    //    yield return new WaitForSeconds(3.0f);

    //    //RoomMove.SetActive(false);
    //}
}
