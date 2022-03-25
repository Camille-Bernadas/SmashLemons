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
        headAudioSource.PlayOneShot(taunt);
    }
    public void PlayHit() {
        headAudioSource.PlayOneShot(hit);
    }

    /*Middle/SFX*/
    public void PlayShield() {
        middleAudioSource.PlayOneShot(block);
    }
    
    public void PlayAttack() {
        middleAudioSource.PlayOneShot(basicAttack);
    }
    public void PlaySpecialAttack() {
        middleAudioSource.PlayOneShot(specialAttack);
    }
    public void PlayUltimateAttack() {
        middleAudioSource.PlayOneShot(ultimateAttack);
    }
    public void PlayDash() {
        middleAudioSource.PlayOneShot(dash);
    }
    public void PlayJump(){
        middleAudioSource.PlayOneShot(jump);
    }
}
