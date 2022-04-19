using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public abstract class Character : MonoBehaviour, ICharacter, IDamageable
{ 
    /* Movement */
    [SerializeField]
    protected float speed;
    protected float jumpForce;
    protected float dashForce;
    protected bool isGrounded;
    protected float dashTimer;
    protected int remainingDashes;
    protected int maxDashes;



    /* Attacks */
    protected float attackDamage;
    protected float attackSpeed;
    protected float attackDelay;
    protected float attackSize;
    protected float specialSpeed;
    protected float ultimateProgression;
    protected float attackCooldown;
    protected long points;

    /* Defense */
    protected bool isBlocking;
    protected float weight;
    protected float resistance;
    protected float takenDamage;

    /* Life-Related */
    protected int remainingLives;
    public GameObject respawnParticle;
    protected bool isAlive = true;
    protected bool isTaunting = false;
    protected float tauntTimer;


    /* References to other objects */
    protected Rigidbody body;
    protected Transform groundChecker;
    protected Transform shield;
    protected Transform centerOfMass;
    protected Animator animator;
    protected Transform meleeRange;

    /* TODO Sort */
    protected Vector2 inputs = Vector2.zero;
    protected float GroundDistance = 0.2f;
    public LayerMask Ground;
    float turnSmoothVelocity;
    float turnSmoothTime = 0.05f;
    private Vector3 lastDirection;

    float dashDirection;

    public PlayerSounds playerSounds;

    private void Awake() {
        this.speed = 5f;
        this.jumpForce = 1f;
        this.dashForce = 4f;
        this.weight = 100f;
        this.resistance = 0f;
        this.attackDamage = 60f;
        this.attackSpeed = 2f;
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
    void Start() {
        body = GetComponent<Rigidbody>();
        groundChecker = transform.GetChild(0);
        animator = GetComponent<Animator>();
        meleeRange = transform.Find("MeleeRange");
        shield = transform.Find("Shield");
        centerOfMass = transform.Find("CenterOfMass");
    }

    virtual protected void Update() {
        if (!isAlive) { 
            animator.SetBool("isMoving", true);
            return;
        }
        if(isTaunting){
            Debug.Log("Taunt");
            if(tauntTimer>0f){
                tauntTimer-=Time.deltaTime;
            }
            else{
                isTaunting = false;
                animator.Play("Idle", 1, 0f);
                animator.Play("Idle", 0, 0f);
                
            }
        }
        
        isGrounded = Physics.CheckSphere(groundChecker.position, GroundDistance, Ground, QueryTriggerInteraction.Ignore);
        if (isGrounded) {
            if (dashTimer <= 0f) {
                remainingDashes = maxDashes;
            } else {
                dashTimer -= Time.deltaTime;
            }

        }

        inputs = inputs.normalized;
        if(isBlocking){
            inputs = new Vector3(0f, 0f, 0f);
        }

        if(true){
            Vector3 direction = new Vector3(inputs.x, 0f/*vertical*/, 0f).normalized;
            if (Mathf.Abs(inputs.x) >= 0.1f) {
                lastDirection = new Vector3(inputs.x, 0f, 0f);
            }

            // Lors d'un changement de sens lors d'un dash, la velocité est annulée
            if (inputs.x * dashDirection < 0f) {
                body.velocity = new Vector3(0f, body.velocity.y, body.velocity.z);
                body.angularVelocity = new Vector3(0f, body.angularVelocity.y, body.angularVelocity.z);
            }

            if (direction.magnitude >= 0.1f) {
                animator.SetBool("isMoving", true);
                float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
                transform.rotation = Quaternion.Euler(0f, angle, 0f);
            } else {
                animator.SetBool("isMoving", false);
            }
        }

        if (attackCooldown > 0f) {
            attackCooldown -= Time.deltaTime;
        }
        else{
            meleeRange.GetComponent<Renderer>().material.SetColor("_Color", Color.green);
        }

    }


    void FixedUpdate() {
        if (!isAlive || isTaunting) { return; }
        if (true) {
            Vector3 move = new Vector3(inputs.x, 0f, 0f);
            Move(move);
        }
    }

    public void OnMove(InputAction.CallbackContext ctx) => inputs = ctx.ReadValue<Vector2>();

    public void Move(Vector3 move) {
        if (!isAlive || isTaunting) { return; }
        body.MovePosition(body.position + move * speed * Time.fixedDeltaTime);
    }

    public void Jump() {

        if (!isAlive || isTaunting) { return; }
        if(isGrounded){
            body.AddForce(Vector3.up * Mathf.Sqrt(jumpForce * -2f * Physics.gravity.y), ForceMode.VelocityChange);
        }
        
    }

    public void Dash() {
        if (!isAlive || isTaunting) { return; }

        if (remainingDashes > 0) {
            dashTimer = 0.1f;
            dashDirection = inputs.x;
            remainingDashes--;
            
        
            Vector3 dashVelocity = new Vector3(lastDirection.x, 0f, 0f) * dashForce;
            if(inputs.magnitude > 0.1f){
                dashVelocity = inputs * dashForce;
            }
            body.velocity = Vector3.zero;
            body.AddForce(dashVelocity, ForceMode.VelocityChange);
        }
    }
    public void setAttack(InputAction.CallbackContext context){
        if (context.performed && !isBlocking) {
            if (attackCooldown <= 0f) {
                attackCooldown = 1f / attackSpeed;
                Attack(inputs);
            }
        }
    }

    private IEnumerator preAttack(Vector2 direction) {
        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSeconds(attackDelay);
        float Xdir = direction.x;
        if (Xdir > 0.7f) {
            Xdir = 1f;
        } else {
            if (Xdir < -0.7f) {
                Xdir = -1f;
            } else {
                Xdir = 0f;
            }
        }
        float Ydir = direction.y;
        if (Ydir > 0.7f) {
            Ydir = 1f;
        } else {
            if (Ydir < -0.7f) {
                Ydir = -1f;
            } else {
                Ydir = 0f;
            }
        }
        Vector3 projectionVector = new Vector3(Xdir, Ydir, 0f);

        meleeRange.GetComponent<Renderer>().material.SetColor("_Color", Color.red);
        Collider[] hitColliders = Physics.OverlapSphere(meleeRange.position, 0.3f);
        foreach (var hitCollider in hitColliders) {
            if (hitCollider.tag == "Player" && hitCollider.name != transform.name) {
                this.points += (long)attackDamage;
                hitCollider.transform.GetComponent<Character>().TakeDamage(meleeRange.position, projectionVector, attackDamage);
            }
        }

    }
    public void Attack(Vector2 direction) {
        if (!isAlive || isTaunting) { return; }
        animator.Play("BasicAttack", 1, 0f);
        playerSounds.PlayAttack();
        StartCoroutine("preAttack", direction);
        
    }

    public void SpecialAttack() {
        if (!isAlive || isTaunting) { return; }
        throw new System.NotImplementedException();
    }

    public void UltimateAttack() {
        if (!isAlive || isTaunting) { return; }
        throw new System.NotImplementedException();
    }

    public void Block(InputAction.CallbackContext context) {
        if (!isAlive || isTaunting) { return; }
        if (context.started) {
            isBlocking = true;
            animator.SetBool("isMoving", false);
            shield.GetComponent<Renderer>().material.SetColor("_Color", Color.blue);
        } else if (context.canceled) {
            isBlocking = false;
            shield.GetComponent<Renderer>().material.SetColor("_Color", Color.white);
        }
    }

    public void Taunt() {
        Debug.Log("Try Taunt");
        if (isAlive && !isTaunting) {
            Debug.Log("Start Taunt");
            isTaunting = true;
            tauntTimer = 1f;
            animator.Play("Taunt", 1, 0f);
            animator.Play("Taunt", 0, 0f);
            playerSounds.PlayTaunt();
            
            return;
        }
    }

    public float Heal(float amount){
        if (!isAlive) { return 0f; }
        takenDamage -= amount;
        if(takenDamage<0f){
            takenDamage = 0f;
        }
        return amount;
    }

    public float TakeDamage(Vector3 origin, Vector3 projection, float amount) {
        if (!isAlive) { return 0f; }
        // Attaque du côté du shield: (transform.position - origin).x * (shield.position - transform.position).x NEGATIF
        // Attaque de côté opposé au shield: (transform.position - origin).x * (shield.position - transform.position).x POSITIF
        if (!isBlocking || (transform.position - origin).x * (shield.position - transform.position).x > 0) {
            takenDamage += amount - amount * (resistance / 100);
            Vector3 direction = new Vector3(centerOfMass.position.x - origin.x, 0f, 0f).normalized;
            if(projection.y == 0f){
                direction = new Vector3(centerOfMass.position.x - origin.x, 0f, 0f).normalized;
            }
            else{
                direction = new Vector3(projection.x, projection.y, 0f).normalized;
            }
            direction *= (4 * (1 + takenDamage/100f)) / (weight / 100);
            body.AddForce(direction, ForceMode.VelocityChange);

            playerSounds.PlayHit();

            return amount;
        } else {
            return 0;
        }
    }

    public void Die() {
        if (!isAlive) { return; }
        //checkForLivesLeft
        remainingLives--;
        takenDamage = 0f;
        body.velocity = new Vector3(0f, body.velocity.y, body.velocity.z);
        body.angularVelocity = new Vector3(0f, body.angularVelocity.y, body.angularVelocity.z);
        transform.position = new Vector3(0f, 10f, 0f);
        if(remainingLives > 0){
            Respawn();
        }
        else{
            isAlive = false;
            body.useGravity = false;
            body.velocity = new Vector3(0f, 0f, 0f);
            body.angularVelocity = new Vector3(0f, 0f, 0f);
            transform.position = new Vector3(-100f, -100f, -100f);
        }
    }
    public void Spawn(){
        if (!isAlive) { return; }
        //Choose Respawn Position
        Vector3 respawnPosition;
        GameObject[] respawns = GameObject.FindGameObjectsWithTag("Respawn");
        if (respawns.Length == 0) {
            respawnPosition = new Vector3(0f, 10f, 0f);
        } else {
            int chosenRespawn = Random.Range(0, respawns.Length);
            respawnPosition = respawns[chosenRespawn].transform.position;
        }
        transform.position = respawnPosition;
    }
    public void Respawn(){
        if (!isAlive) { return; }
        //Choose Respawn Position
        Vector3 respawnPosition;
        
        GameObject[] respawns = GameObject.FindGameObjectsWithTag("Respawn");
        if (respawns.Length == 0) {
            respawnPosition = new Vector3(0f, 10f, 0f);
        } else {
            int chosenRespawn = Random.Range(0, respawns.Length);
            respawnPosition = respawns[chosenRespawn].transform.position;
        }
        //Respawn
        body.velocity = new Vector3(0f, 0f, 0f);
        body.angularVelocity = new Vector3(0f, 0f, 0f);
        transform.position = respawnPosition;
        Instantiate(respawnParticle, transform.position, Quaternion.identity).GetComponent<ParticleSystem>().Play();
    }
    public float getTakenDamage() {
        return takenDamage;
    }
    public int getRemainingLives() {
        return remainingLives;
    }

    public void setAlive(bool alive){
        isAlive = alive;
    }

    public long getPoints(){
        return points;
    }
}
