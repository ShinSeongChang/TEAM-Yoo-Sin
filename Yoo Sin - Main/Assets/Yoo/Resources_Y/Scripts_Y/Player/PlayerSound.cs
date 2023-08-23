using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerSound : MonoBehaviour
{
    public List<AudioClip> sounds;
    public List<AudioClip> attackSounds;
    private AudioSource playerAudio;
    private Animator playerAni;
    private string tempString;
    private string tempMoveString;
    private int randomSound;
    enum sound
    {

    }
    // Start is called before the first frame update
    void Start()
    {
        playerAudio = GetComponent<AudioSource>();
        playerAni = transform.GetChild(0).GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        SelectSound();
    }

    private void SelectSound()
    {
        if(playerAni.GetCurrentAnimatorStateInfo(0).IsName("Player_idle_R") && tempMoveString != "Player_idle_R")
        { 
            if(tempMoveString == "PlayerFall")
            {
                return;
            }
            playerAudio.Stop();
            tempMoveString = "Player_idle_R";
            return;
        }

        if(playerAni.GetCurrentAnimatorStateInfo(0).IsName("PlayerWalk") && tempMoveString != "PlayerWalk")
        {
            playerAudio.clip = sounds[0];
            playerAudio.Play();
            //playerAudio.PlayOneShot(sounds[0]);
            tempMoveString = "PlayerWalk";
            return;
        }

        if (playerAni.GetCurrentAnimatorStateInfo(0).IsName("PlayerFall") && tempMoveString != "PlayerFall")
        {
            if(tempMoveString == "PlayerWalk")
            {
                playerAudio.Stop();
            }
            playerAudio.clip = sounds[2];
            playerAudio.Play();
            //playerAudio.PlayOneShot(sounds[0]);
            tempMoveString = "PlayerFall";
            return;
        }

        if (playerAni.GetCurrentAnimatorStateInfo(0).IsName("PlayerJump") && tempMoveString != "PlayerJump")
        {
            if (tempMoveString == "PlayerWalk")
            {
                playerAudio.Stop();
            }
            //playerAudio.clip = sounds[2];
            //playerAudio.Play();
            playerAudio.PlayOneShot(sounds[1]);
            tempMoveString = "PlayerJump";
            return;
        }

        if (playerAni.GetCurrentAnimatorStateInfo(0).IsName("PlayerHurt") && tempMoveString != "PlayerHurt")
        {
            playerAudio.Stop();
            //playerAudio.clip = sounds[2];
            //playerAudio.Play();
            playerAudio.PlayOneShot(sounds[4]);
            tempMoveString = "PlayerHurt";
            return;
        }

        if (playerAni.GetCurrentAnimatorStateInfo(0).IsName("PlayerAttack") && tempString != "PlayerAttack")
        {
            randomSound = Random.Range(0, attackSounds.Count);
            //playerAudio.clip = sounds[2];
            //playerAudio.Play();
            tempString = "PlayerAttack";
            playerAudio.PlayOneShot(attackSounds[randomSound]);
            StartCoroutine(CheckAttackEnd());
            return;
        }

        if (playerAni.GetCurrentAnimatorStateInfo(0).IsName("PlayerAttack_D") && tempString != "PlayerAttack_D")
        {
            randomSound = Random.Range(0, attackSounds.Count);
            //playerAudio.clip = sounds[2];
            //playerAudio.Play();
            tempString = "PlayerAttack_D";
            playerAudio.PlayOneShot(attackSounds[randomSound]);
            StartCoroutine(CheckAttackDEnd());
            return;
        }

        if (playerAni.GetCurrentAnimatorStateInfo(0).IsName("PlayerAttack_U") && tempString != "PlayerAttack_U")
        {
            randomSound = Random.Range(0, attackSounds.Count);
            //playerAudio.clip = sounds[2];
            //playerAudio.Play();
            tempString = "PlayerAttack_U";
            playerAudio.PlayOneShot(attackSounds[randomSound]);
            StartCoroutine(CheckAttackUEnd());
            return;
        }
    }

    IEnumerator CheckAttackEnd()
    {
        while (true)
        {
            if (playerAni.GetCurrentAnimatorStateInfo(0).IsName("PlayerAttack"))
            {
                if (playerAni.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95f)
                {
                    tempString = null;
                    yield break;
                }
            }
            else
            {
                tempString = null;
                yield break;
            }
            yield return null;
        }
    }

    IEnumerator CheckAttackUEnd()
    {
        while (true)
        {
            if (playerAni.GetCurrentAnimatorStateInfo(0).IsName("PlayerAttack_U"))
            {
                if (playerAni.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95f)
                {
                    tempString = null;
                    yield break;
                }
            }
            else
            {
                tempString = null;
                yield break;
            }
            yield return null;
        }
    }

    IEnumerator CheckAttackDEnd()
    {
        while (true)
        {
            if (playerAni.GetCurrentAnimatorStateInfo(0).IsName("PlayerAttack_D"))
            {
                if (playerAni.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95f)
                {
                    tempString = null;
                    yield break;
                }
            }
            else
            {
                tempString = null;
                yield break;
            }
            yield return null;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Platform") || collision.collider.CompareTag("StunBody"))
        {
            playerAudio.Stop();
            playerAudio.PlayOneShot(sounds[3]);
        }
    }
}
