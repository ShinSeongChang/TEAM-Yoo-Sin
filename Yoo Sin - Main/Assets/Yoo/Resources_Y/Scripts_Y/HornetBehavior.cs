using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HornetBehavior : MonoBehaviour
{
    public GameObject player;
    private Vector3 dir;
    private float xdiff;

    public GameObject AirDashEffectPrefab;
    public GameObject GroundDashEffectPrefab;
    public GameObject StartWhipEffectPrefab;
    public GameObject ThrowNeedleEffectPrefab;
    public GameObject NeedlePrefab;
    public GameObject WhipingPrefab;

    private int hp = 10;
    private int randomNumber;
    private int stunCount;

    private Quaternion toLeft = Quaternion.Euler(0, 0, 0);
    private Quaternion toRight = Quaternion.Euler(0, 180, 0);

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
}
