using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Catherine : Character {
    private void Awake() {
        this.speed = 8f;
        this.jumpForce = 1f;
        this.dashForce = 3f;
        this.weight = 50f;
        this.resistance = 0f;
        this.attackDamage = 15f;
        this.attackSpeed = 10f;
        this.attackDelay = 0.08f;
        this.attackSize = 0.3f;
        this.specialSpeed = 1f;
        this.ultimateProgression = 0f;
        this.isBlocking = false;
        this.isGrounded = false;
        this.maxDashes = 2;
        this.remainingLives = 3;
        this.points = 0;
    }
}
