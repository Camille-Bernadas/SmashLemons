using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestTarget : Character
{
    private Vector3 position;
    private float edgeMargin = 1.0f;
    private int state = 0;

    private float jumpCooldown = 0.0f;

    override protected void Update() {
        

        position = this.transform.position;


        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        GameObject[] platforms = GameObject.FindGameObjectsWithTag("Platform");

        GameObject targetPlayer = null;
        float targetPlayerDistance = 0.0f;

        foreach (var p in players) {
            float tmpDist = Vector3.Distance(position, p.transform.position);
            if(p.name != this.name && (targetPlayer==null || tmpDist < targetPlayerDistance)){
                targetPlayerDistance = tmpDist;
                targetPlayer = p;
            }
        }

        //0 idle    : does nothing
        //1 recovery: try to get back on a plateform
        //2 getCloser to the center
        //3 attack  : try to attack the targetPlayer
        //4 survive : try to survive to the player target

        
        PlatformData bestRecoveryPlatform = platforms[0].GetComponent<PlatformData>();
        PlatformData thePlatformIamOn    = platforms[0].GetComponent<PlatformData>();

        string bestRecoveryPlatformName = platforms[0].name;
        string thePlatformIamOnName = platforms[0].name;

        bool overTheVoid = !this.isGrounded;

        foreach (var p in platforms) {
            Vector3 platPosition = p.GetComponent<PlatformData>().position;
            Vector3 platSize = p.GetComponent<PlatformData>().size;

            if(betterRecoveryPlatform(p.GetComponent<PlatformData>(),bestRecoveryPlatform)){
                bestRecoveryPlatform = p.GetComponent<PlatformData>();
                bestRecoveryPlatformName = p.name;
            }

            if(position.y > platPosition.y && position.x > platPosition.x-platSize.x/2.0f && position.x < platPosition.x + platSize.x/2.0f){
                if(position.y > platPosition.y && platPosition.y>thePlatformIamOn.position.y){
                    thePlatformIamOn = p.GetComponent<PlatformData>();
                    thePlatformIamOnName = p.name;
                    overTheVoid = false;
                }
            }
        }

        
        if(overTheVoid){
            state = 1;
            //print("Too close to the edge");
            //print("Best Recovery platform : "+bestRecoveryPlatformName);
        }else if(position.x < thePlatformIamOn.position.x-thePlatformIamOn.size.x/2.0f +edgeMargin || position.x > thePlatformIamOn.position.x + thePlatformIamOn.size.x/2.0f - edgeMargin){
            state = 2;
        }else if(Vector3.Distance(position,thePlatformIamOn.position + new Vector3(0.0f, thePlatformIamOn.size.y/2, 0.0f)) < edgeMargin){
            state = 0;
        }


        if(state == 0 && targetPlayerDistance < 5.0f){
            state = 3;
        }
        
        //print("thePlatformIamOnName : "+thePlatformIamOnName);
        
        //isBlocking = false;
        switch (state)
        {
            case 0:
            //print("idle");
                inputs = new Vector2(0.0f, 0.0f);
                
                break;
            case 1:
                //print("recovery");
                inputs.x = 0.0f;
                inputs.y = 1.0f;
                Dash();

                Vector3 platDir = platformEdgeDirection(bestRecoveryPlatform);
                inputs.x = platDir.x;
                //inputs.y = 0;
                break;
            case 2:
                //print("Get closerToTheCenter");
                platDir = platformCenterDirection(thePlatformIamOn);
                inputs.x = platDir.x;
                inputs.y = platDir.y;
                break;
            
            case 3:
                //print("attack");
                Vector2 attackDirection = targetPlayer.transform.position - position;
                //Debug.Log(targetPlayer.name);
                if(targetPlayerDistance<1.0f){
                    if (attackCooldown <= 0f) {
                        attackCooldown = 1f / attackSpeed;
                        Attack(new Vector2(attackDirection[0],attackDirection[1]));
                    }else{
                        //isBlocking = true;
                    }
                }else{
                    inputs.x = attackDirection[0];
                    inputs.y = attackDirection[1];
                    if(targetPlayer.transform.position[1]-position[1]>1){
                        if (jumpCooldown <= 0f) {
                            jumpCooldown = 1f;
                            Jump();
                        }
                        
                    }
                }
                break;
        }


        if (jumpCooldown > 0f) {
            jumpCooldown -= Time.deltaTime;
        }


        base.Update();
    }
   
    bool betterRecoveryPlatform(PlatformData a, PlatformData b){
        return(position.x<a.position.x+a.size.x/2.0 && position.x>a.position.x-a.size.x/2.0)?
            (position.y - a.position.y > a.size.y/2 ? 
                (position.y - b.position.y > b.size.y/2 ?
                    position.y - a.position.y < position.y - b.position.y :
                    true):
                (position.y - b.position.y > b.size.y/2 ?
                    false :
                    position.y - a.position.y > position.y - b.position.y)):

        ((position.x>a.position.x+a.size.x/2.0)?
            Vector3.Distance(position, a.position + new Vector3( a.size.x/2.0f, 0.0f, 0.0f)):
            Vector3.Distance(position, a.position + new Vector3(-a.size.x/2.0f, 0.0f, 0.0f)))<
        ((position.x>b.position.x+b.size.x/2.0)?
            Vector3.Distance(position, b.position + new Vector3( b.size.x/2.0f, 0.0f, 0.0f)):
            Vector3.Distance(position, b.position + new Vector3(-b.size.x/2.0f, 0.0f, 0.0f)));
    }

    Vector3 platformEdgeDirection(PlatformData p){
        return (position.x>p.position.x+p.size.x/2.0)?
            p.position + new Vector3( p.size.x/2.0f, p.size.y, 0.0f) - position:
            p.position + new Vector3(-p.size.x/2.0f, p.size.y, 0.0f) - position;
    }

    Vector3 platformCenterDirection(PlatformData p){
        return p.position + new Vector3( 0.0f , p.size.y, 0.0f) - position;
    }
}
