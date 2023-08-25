using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager audioInstance = default;

    [SerializeField] AudioMixer audioMixer;

    public float maxValue = default;
    public float minValue = default;

    public float upValue = 3f;
    public float downValue = -3f;

    public float masterValue = default;
    public float bgmValue = default;
    public float effectValue = default;

    private void Awake()
    {
        if (audioInstance == null)
        {
            audioInstance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        maxValue = -5f;
        minValue = -35f;

        masterValue = maxValue;
        bgmValue = maxValue;
        effectValue = maxValue;

        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        audioMixer.SetFloat("Master", masterValue);
        audioMixer.SetFloat("Bgm", bgmValue);
        audioMixer.SetFloat("Effect", effectValue);
    }


}
