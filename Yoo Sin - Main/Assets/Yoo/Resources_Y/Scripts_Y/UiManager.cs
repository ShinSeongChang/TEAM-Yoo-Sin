using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.ComponentModel;

public class UiManager : MonoBehaviour
{
    private const int HP = 0;

    private PlayerBehavior player;
    private TMP_Text hp;

    private int currentHp;
    // Start is called before the first frame update
    void Start()
    {
        hp = transform.GetChild(HP).GetComponent<TMP_Text>();
        player = transform.parent.GetComponent<PlayerBehavior>();
    }

    // Update is called once per frame
    void Update()
    {
        currentHp = player.Get_Hp();
        hp.text = "Hp:" + currentHp.ToString();
    }
}
