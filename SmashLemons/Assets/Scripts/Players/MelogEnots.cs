using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MelogEnots : Character
{
    private void Awake() {
        this.speed = 6f;
        this.jumpForce = 0.5f;
        this.dashForce = 6f;
        this.weight = 100f;
        this.resistance = 0f;
        this.attackDamage = 40f;
        this.attackSpeed = 1f;
        this.attackDelay = 0.5f;
        this.attackSize = 0.8f;
        this.specialSpeed = 1f;
        this.ultimateProgression = 0f;
        this.isBlocking = false;
        this.isGrounded = false;
        this.maxDashes = 2;
        this.remainingLives = 3;
        this.points = 0;
    }
}
