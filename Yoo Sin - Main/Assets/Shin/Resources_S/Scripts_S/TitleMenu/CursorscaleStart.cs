using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorscaleStart : MonoBehaviour
{
    // ���� Ŀ�� ��ġ �޾ƿ���
    [SerializeField]
    RectTransform mainCursor = default;

    // �� Ŀ�� ��ġ �޾ƿ���
    private RectTransform myCursor = default;

    // �ڽ� Ŀ���� ������Ʈ �޾ƿ��� �迭�� �����صα�
    private Transform childCursor = default;
    private RectTransform[] childs = new RectTransform[2];

    private Vector2 maxSize = default;
    private Vector2 plusScale = default;
    private Vector2 minusScale = default;
    private Vector2 minSize = default;

    private bool isEnlargement = false;
    private bool isRedution = false;
    private bool isLeave = false;    

    // Start is called before the first frame update
    void Start()
    {
        myCursor = GetComponent<RectTransform>();
        childCursor = transform.GetComponentInChildren<RectTransform>();

        maxSize = new Vector2(100f, 100f);
        plusScale = new Vector2(2000f, 0f);
        minSize = new Vector2(0f, 100f);
        minusScale = new Vector2(-2000f, 0f);

        for (int i = 0; i < childCursor.childCount; i++)
        {
            childs[i] = childCursor.GetChild(i).GetComponent<RectTransform>();
            childs[i].sizeDelta = maxSize;
        }

        isEnlargement = true;
    }

    // Update is called once per frame
    void Update()
    {
        // ����Ŀ�� ��ġ�� ��������
        if (isEnlargement == false && mainCursor.anchoredPosition.y == myCursor.anchoredPosition.y)
        {
            StartCoroutine(Enlargement());
        }


        if (mainCursor.anchoredPosition.y == myCursor.anchoredPosition.y)
        {
            isLeave = false;
        }


        if (isLeave == false)
        {
            if (mainCursor.anchoredPosition.y != myCursor.anchoredPosition.y)
            {
                isLeave = true;
            }
        }

        // ����Ŀ���� �� ��ġ���� ����� ���¶��
        if (isLeave == true)
        {
            StartCoroutine(Reduction());
        }

    }

    IEnumerator Enlargement()
    {
        isEnlargement = true;

        if (childs[1].sizeDelta.x < 100f)
        {
            for (int i = 0; i < childCursor.childCount; i++)
            {
                childs[i].sizeDelta += plusScale * Time.unscaledDeltaTime;

                yield return null;
            }
        }

        if (childs[1].sizeDelta.x >= 100f)
        {
            for (int i = 0; i < childCursor.childCount; i++)
            {
                childs[i].sizeDelta = maxSize;
            }
        }

        //isLeave = false;
        //isRedution = false;

        // Ȯ���۾��� ������ Ż�� ���� �޼����� �ݺ�ȣ�� �ϱ����� ���ǰ� �ʱ�ȭ
        isEnlargement = false;

        yield break;

    }

    IEnumerator Reduction()
    {
        // Update���� �����Ӹ��� �ڷ�ƾ�� ȣ���ϱ⿡ ���ǰ����� �����ش�.
        if (childs[1].sizeDelta.x >= 0f)
        {
            for (int i = 0; i < childCursor.childCount; i++)
            {
                childs[i].sizeDelta += minusScale * Time.unscaledDeltaTime;

                yield return null;
            }
        }

        if (childs[1].sizeDelta.x <= 0f)
        {
            for (int i = 0; i < childCursor.childCount; i++)
            {
                childs[i].sizeDelta = minSize;
            }
        }

        isEnlargement = false;

        yield break;

    }
}
