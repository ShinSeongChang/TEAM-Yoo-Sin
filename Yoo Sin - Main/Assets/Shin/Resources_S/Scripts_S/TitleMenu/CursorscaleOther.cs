using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorscaleOther : MonoBehaviour
{
    // ���� Ŀ�� ��ġ �޾ƿ���
    [SerializeField] 
    RectTransform mainCursor = default;

    // �� Ŀ�� ��ġ �޾ƿ��� ( �� Ŀ�� = �θ� )
    private RectTransform myCursor = default;

    // �ڽ� Ŀ���� ������Ʈ �޾ƿ��� �迭�� �����صα�
    private Transform childCursor = default;
    private RectTransform[] childs = new RectTransform[2];

    private Vector2 minusScale = default;
    private Vector2 plusScale = default;

    // ������Ʈ �κп��� �� �ڷ�ƾ�� �������Ӹ��� ȣ���Ұ��̱⿡ ȣ�� ������ ���� �Ұ���
    private bool isEnlargement = false;
    private bool isRedution = false;
    private bool isLeave = false;

    // Start is called before the first frame update
    void Start()
    {
        // �� Ŀ�� ��ġ �޾ƿ���
        myCursor = GetComponent<RectTransform>();

        // �ڽ�Ŀ���� ������Ʈ �޾ƿ���
        childCursor = transform.GetComponentInChildren<RectTransform>();

        for(int i = 0; i < childCursor.childCount; i++)
        {
            // �޾ƿ� �ڽĵ� ������Ʈ �迭������ �����صΰ�, ������ �ٿ��α� (������ġ Ŀ�� ����� �����δ� ����)
            childs[i] = childCursor.GetChild(i).GetComponent<RectTransform>();
            childs[i].sizeDelta = new Vector2(0f, 0f);
        }

        // ���� �ڽ�Ŀ���� �̹����� ���������� Ȯ��, ��� ��ų ��
        plusScale = new Vector2(40f, 0f);
        minusScale = new Vector2(-40f, 0f);
    }

    // Update is called once per frame
    void Update()
    {
        // ����Ŀ�� ��ġ�� ��������
        if(isEnlargement == false && mainCursor.anchoredPosition.y == myCursor.anchoredPosition.y)
        {            
            // �ڽ�Ŀ���� �̹����� Ȯ���Ű�� �ڷ�ƾ ȣ��
            StartCoroutine(Enlargement());
        }
  

        // ���к��� ȣ�� �������� �ڱ��ڽ��� ����
        if(isLeave == false)
        {
            // ����Ŀ���� �� ��ġ�� ����ٸ�
            if(mainCursor.anchoredPosition.y != myCursor.anchoredPosition.y)
            {
                // ����Ŀ���� �����ٴ� ���ǰ� ����
                isLeave = true;
            }
        }
        

        // ����Ŀ���� �� ��ġ���� ����� ���¶��
        if(isRedution == false && isLeave == true)
        {
            // �ڽ�Ŀ���� �̹����� ��ҽ�Ű�� �ڷ�ƾ ȣ��
            StartCoroutine(Reduction());
        }

    }


    // �ڽ�Ŀ���� �̹��� Ȯ�� �ڷ�ƾ
    IEnumerator Enlargement()
    {
        // Update���� �����Ӹ��� �ڷ�ƾ�� ȣ���ϱ⿡ ���ǰ����� �����ش�.
        isEnlargement = true;

        // �ڷ�ƾ stop ���� == �̹��� width ���� 100�� �Ѿ�� �ϸ�
        if (childs[0].sizeDelta.x >= 100f)
        {
            // �ڽ�Ŀ�� ��� width, height ���� 100���� �ʱ�ȭ ( ���� ũ�� )
            for (int i = 0; i < childCursor.childCount; i++)
            {
                childs[i].sizeDelta = new Vector2(100f, 100f);
            }

            // Ȯ�� ����� �����ٸ� �ڽ��� ȣ�� ���ϰ� ���ǰ� ����
            isEnlargement = true;

            // Ŀ���� Ȯ��ɶ��� ����Ŀ���� �ڽ��� ��ġ�� �°��̴� isLeave�� false�� �ʱ�ȭ
            isLeave = false;

            // ����Ŀ���� ���� �� ��ҽ�Ű�� ���� ���ǰ� �ʱ�ȭ
            isRedution = false;

            yield break;
        }


        // �ڽ�Ŀ���� ��������ŭ widht Ű��� �ݺ� == 1�����Ӹ���
        for (int i = 0; i < childCursor.childCount; i++)
        {
            childs[i].sizeDelta += plusScale;

            yield return null;
        }

        // Ȯ���۾��� ������ Ż�� ���� �޼����� �ݺ�ȣ�� �ϱ����� ���ǰ� �ʱ�ȭ
        isEnlargement = false;

    }

    IEnumerator Reduction()
    {
        // Update���� �����Ӹ��� �ڷ�ƾ�� ȣ���ϱ⿡ ���ǰ����� �����ش�.
        isRedution = true;

        // �ڷ�ƾ stop ���� == �̹��� width ���� 0���� �۾������ϸ�
        if (childs[0].sizeDelta.x <= 0f)
        {
            // �ڽ�Ŀ�� ��� width���� 0���� �ʱ�ȭ ( �̹����� �Ⱥ��̰� )
            for (int i = 0; i < childCursor.childCount; i++)
            {
                childs[i].sizeDelta = new Vector2(0f, 100f);
            }

            // ��� ����� �����ٸ� �ڽ��� ȣ�� ���ϰ� ���ǰ� ����
            isRedution = true;

            // ����Ŀ���� ������ �� Ȯ���Ű�� ���� ���ǰ� �ʱ�ȭ
            isEnlargement = false;

            yield break;
        }

        // �ڽ�Ŀ���� ��������ŭ widht ���̱� �ݺ� == 1�����Ӹ���
        for (int i = 0; i < childCursor.childCount; i++)
        {
            childs[i].sizeDelta += minusScale;

            yield return null;
        }


        // ����۾��� ������ Ż�� ���� �޼����� �ݺ�ȣ�� �ϱ����� ���ǰ� �ʱ�ȭ
        isRedution = false;

    }
}
