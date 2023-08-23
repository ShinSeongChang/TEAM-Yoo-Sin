using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BgmMainController : MonoBehaviour
{
    public static BgmMainController instance = null;

    public List<AudioClip> bgms;
    public List<AudioSource> sources;

    private void Awake()
    {
        // ╫л╠шео фпео
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != null)
        {
            Destroy(gameObject);
        }
        // ╫л╠шео фпео
    }

    public enum Bgm
    {
        normal, falseKnight, hornet
    }

    public enum Source
    {
        changable, fix
    }

    // Start is called before the first frame update
    void Start()
    {
        sources[Convert.ToInt32(Source.changable)].clip = bgms[Convert.ToInt32(Bgm.normal)];
        sources[Convert.ToInt32(Source.changable)].Play();
        sources[Convert.ToInt32(Source.fix)].Play();
    }

    // Update is called once per frame
    void Update()
    {
        if(sources[Convert.ToInt32(Source.fix)].enabled == false && GameManager.instance.boss2Die == true)
        {
            sources[Convert.ToInt32(Source.changable)].clip = bgms[Convert.ToInt32(Bgm.normal)];
            sources[Convert.ToInt32(Source.fix)].enabled = true;
        }
    }
}
