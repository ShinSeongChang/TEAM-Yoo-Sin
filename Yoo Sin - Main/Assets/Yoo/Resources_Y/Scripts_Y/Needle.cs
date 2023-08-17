using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Needle : MonoBehaviour
{
    private const float SPEED_CHANGE_TERM = 0.005f;
    private WaitForSeconds speedChangeTime = new WaitForSeconds(SPEED_CHANGE_TERM);
    private const float RECALL_TERM = 0.3f;
    private WaitForSeconds recallTime = new WaitForSeconds(RECALL_TERM);

    public float speed = 28f;
    private Vector3 haveToGo;
    private Vector3 startPosition;
    private Vector3 dir;
    private bool isReturn;
    private bool isEnd;
    private bool callSpeedDown;
    private bool callSpeedUp;

    // Start is called before the first frame update
    void Start()
    {
        isReturn = false;
        isEnd = false;
        startPosition = transform.position;
        if (transform.rotation == Quaternion.Euler(0, 0, 0))
        {
            haveToGo = transform.position + Vector3.left * 10;
            dir = Vector3.left;
        }
        else
        {
            haveToGo = transform.position + Vector3.right * 10;
            dir = Vector3.right;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.rotation == Quaternion.Euler(0, 0, 0))
        {
            if (transform.position.x <= haveToGo.x && isReturn == false)
            {
                isReturn = true;
            }

            if (transform.position.x >= startPosition.x && isReturn == true)
            {
                isEnd = true;
                Destroy(gameObject);
            }

            if (isReturn == false)
            {
                transform.position += speed * Time.deltaTime * dir;
                if(callSpeedDown == false)
                {
                    StartCoroutine(speedDown());
                    callSpeedDown = true;
                }
            }
            else if(isReturn == true && isEnd == false)
            {
                transform.position -= speed * Time.deltaTime * dir;
                if(callSpeedUp == false)
                {
                    StartCoroutine(speedUp());
                    callSpeedUp = true;
                }
            }
        }
        else
        {
            if (transform.position.x >= haveToGo.x && isReturn == false)
            {
                isReturn = true;
            }

            if (transform.position.x <= startPosition.x && isReturn == true)
            {
                isEnd = true;
                Destroy(gameObject);
            }

            if (isReturn == false)
            {
                transform.position += speed * Time.deltaTime * dir;
                if (callSpeedDown == false)
                {
                    StartCoroutine(speedDown());
                    callSpeedDown = true;
                }
            }
            else if (isReturn == true && isEnd == false)
            {
                transform.position -= speed * Time.deltaTime * dir;
                if (callSpeedUp == false)
                {
                    StartCoroutine(speedUp());
                    callSpeedUp = true;
                }
            }
        }
    }

    IEnumerator speedDown()
    {
        while (speed > 1)
        {
            if (isReturn == true)
            {
                yield break;
            }
            speed -= 1f;
            yield return speedChangeTime;
        }
        yield break;
    }

    IEnumerator speedUp()
    {
        yield return recallTime;
        while (speed < 28)
        {
            if (isEnd == true)
            {
                yield break;
            }
            speed += 1f;
            yield return speedChangeTime;
        }
        yield break;
    }

    public bool Get_IsReturn()
    {
        return isReturn;
    }
}
