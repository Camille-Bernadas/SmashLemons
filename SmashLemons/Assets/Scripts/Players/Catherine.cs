using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Catherine : Character {
    private void Awake() {
        this.speed = 8f;
        this.jumpForce = 1f;
        this.dashForce = 3f;
        this.weight = 50f;
        this.resistance = 50f;
        this.attackDamage = 5f;
        this.attackSpeed = 10f;
        this.specialSpeed = 1f;
        this.ultimateProgression = 0f;
        this.isBlocking = false;
        this.isGrounded = false;
        this.maxDashes = 5;
        this.remainingLives = 3;
    }
}
