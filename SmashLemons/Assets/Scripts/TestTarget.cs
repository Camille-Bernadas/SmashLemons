using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestTarget : Character
{

    void Update() {
        isGrounded = Physics.CheckSphere(groundChecker.position, GroundDistance, Ground, QueryTriggerInteraction.Ignore);
    }


    void FixedUpdate() {
    }
}
