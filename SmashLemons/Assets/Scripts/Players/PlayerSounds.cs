using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSounds : MonoBehaviour
{
    public AudioSource headAudioSource, middleAudioSource;

    [SerializeField]
    public AudioClip hit, jump, taunt, basicAttack, specialAttack, ultimateAttack, block, dash;


    /*Head/Voices*/
    public void PlayTaunt(){
        float pitch = 1f;
        headAudioSource.PlayOneShot(taunt);
    }
    public void PlayHit() {
        float pitch = Random.Range(0.7f, 1.3f);
        headAudioSource.pitch = pitch;
        headAudioSource.PlayOneShot(hit);
    }

    /*Middle/SFX*/
    public void PlayShield() {
        middleAudioSource.PlayOneShot(block);
    }
    
    public void PlayAttack() {
        float pitch = Random.Range(0.7f, 1.3f);
        middleAudioSource.pitch = pitch;
        middleAudioSource.PlayOneShot(basicAttack);
    }
    public void PlaySpecialAttack() {
        middleAudioSource.PlayOneShot(specialAttack);
    }
    public void PlayUltimateAttack() {
        middleAudioSource.PlayOneShot(ultimateAttack);
    }
    public void PlayDash() {
        float pitch = Random.Range(0.7f, 1.3f);
        middleAudioSource.pitch = pitch;
        middleAudioSource.PlayOneShot(dash);
    }
    public void PlayJump(){
        float pitch = Random.Range(0.7f, 1.3f);
        middleAudioSource.pitch = pitch;
        middleAudioSource.PlayOneShot(jump);
    }
}
