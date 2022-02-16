using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other) {
        Debug.Log("Collide");
        if(other.tag == "Player"){
            Debug.Log("isPlayer");
            other.GetComponent<PlayerController>().Respawn();
        }
    }
}
