using UnityEngine;

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
    protected float specialSpeed;
    protected float ultimateProgression;
    protected float attackCooldown;

    /* Defense */
    protected bool isBlocking;
    protected float weight;
    protected float resistance;
    protected float takenDamage;

    /* Life-Related */
    protected int remainingLives;
    public GameObject respawnParticle;
    protected bool isAlive = true;


    /* References to other objects */
    protected Rigidbody body;
    protected Transform groundChecker;
    protected Transform shield;
    protected Transform centerOfMass;
    protected Animator animator;
    protected Transform meleeRange;

    /* TODO Sort */
    protected Vector3 inputs = Vector3.zero;
    protected float GroundDistance = 0.2f;
    public LayerMask Ground;
    float turnSmoothVelocity;
    float turnSmoothTime = 0.05f;
    private Vector3 lastDirection;

    float dashDirection;
<<<<<<< Updated upstream:SmashLemons/Assets/Scripts/Character.cs

=======
    public PlayerSounds playerSounds;
    
>>>>>>> Stashed changes:SmashLemons/Assets/Scripts/Players/Character.cs

    private void Awake() {
        this.speed = 5f;
        this.jumpForce = 2f;
        this.dashForce = 10f;
        this.weight = 100f;
        this.resistance = 0f;
<<<<<<< Updated upstream:SmashLemons/Assets/Scripts/Character.cs
        this.attackDamage = 1f;
=======
        this.attackDamage = 60f;
>>>>>>> Stashed changes:SmashLemons/Assets/Scripts/Players/Character.cs
        this.attackSpeed = 2f;
        this.specialSpeed = 1f;
        this.ultimateProgression = 0f;
        this.isBlocking = false;
        this.isGrounded = false;
        this.maxDashes = 2;
        this.remainingLives = 3;
    } 
    void Start() {
        body = GetComponent<Rigidbody>();
        groundChecker = transform.GetChild(0);
        animator = GetComponent<Animator>();
        meleeRange = transform.Find("MeleeRange");
        shield = transform.Find("Shield");
        centerOfMass = transform.Find("CenterOfMass");
    }

    void Update() {
        if (!isAlive) { return; }
        isGrounded = Physics.CheckSphere(groundChecker.position, GroundDistance, Ground, QueryTriggerInteraction.Ignore);
        if (isGrounded) {
            if (dashTimer <= 0f) {
                remainingDashes = maxDashes;
            } else {
                dashTimer -= Time.deltaTime;
            }

        }

        

        inputs = Vector3.zero;
        inputs.x = Input.GetAxisRaw("Horizontal");
        inputs.y = Input.GetAxis("Vertical");
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

        if (Input.GetButtonDown("Jump") && isGrounded) {
            Jump();
        }
        if (Input.GetButtonDown("Dash") && remainingDashes > 0) {
            dashTimer = 0.1f;
            dashDirection = inputs.x;
            remainingDashes--;
            Dash();
        }


        if(Input.GetButtonDown("Attack")){
            
            if(attackCooldown<= 0f){
                attackCooldown = 1f / attackSpeed;
                Attack(inputs);
            }
        }
        if (attackCooldown > 0f) {
            attackCooldown -= Time.deltaTime;
        }
        else{
            meleeRange.GetComponent<Renderer>().material.SetColor("_Color", Color.green);
        }

        if (Input.GetButtonDown("Block")) {
            animator.SetBool("isMoving", false);
            Block();
            shield.GetComponent<Renderer>().material.SetColor("_Color", Color.blue);
        }
        if (Input.GetButtonUp("Block")) {
            isBlocking = false;
            shield.GetComponent<Renderer>().material.SetColor("_Color", Color.white);
        }
    }


    void FixedUpdate() {
        if (!isAlive) { return; }
        if (true) {
            Vector3 move = new Vector3(inputs.x, 0f, 0f);
            Move(move);
        }
    }



    public void Move(Vector3 move) {
        if (!isAlive) { return; }
        body.MovePosition(body.position + move * speed * Time.fixedDeltaTime);
    }


    public void Jump() {
<<<<<<< Updated upstream:SmashLemons/Assets/Scripts/Character.cs
        body.AddForce(Vector3.up * Mathf.Sqrt(jumpForce * -2f * Physics.gravity.y), ForceMode.VelocityChange);
    }

    public void Dash() {
        Vector3 dashVelocity = new Vector3(lastDirection.x, 0f, 0f) * dashForce;
        if(inputs.magnitude > 0.1f){
            dashVelocity = inputs * dashForce;
=======
        if (!isAlive) { return; }
        if(isGrounded){
            body.AddForce(Vector3.up * Mathf.Sqrt(jumpForce * -2f * Physics.gravity.y), ForceMode.VelocityChange);
        }
        
    }

    public void Dash() {
        if (!isAlive) { return; }
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
            Debug.Log("Dash");
>>>>>>> Stashed changes:SmashLemons/Assets/Scripts/Players/Character.cs
        }
        body.velocity = Vector3.zero;
        body.AddForce(dashVelocity, ForceMode.VelocityChange);
        Debug.Log("Dash");
    }

<<<<<<< Updated upstream:SmashLemons/Assets/Scripts/Character.cs
    public void Attack(Vector3 direction) {
=======
    public void setAttack(InputAction.CallbackContext context){
        if (context.performed && !isBlocking) {
            Debug.Log("Attack");
            if (attackCooldown <= 0f) {
                attackCooldown = 1f / attackSpeed;
                Attack(inputs);
            }
        }
    }
    public void Attack(Vector2 direction) {
        if (!isAlive) { return; }
        animator.Play("BasicAttack", 1, 0.3f);
        playerSounds.PlayAttack();
>>>>>>> Stashed changes:SmashLemons/Assets/Scripts/Players/Character.cs
        float Xdir = direction.x;
        if(Xdir > 0.7f){
            Xdir = 1f;
        }
        else{
            if (Xdir < -0.7f) {
                Xdir = -1f;
            }
            else{
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
            if(hitCollider.tag == "Player" && hitCollider.name != transform.name){
                hitCollider.transform.GetComponent<Character>().TakeDamage(meleeRange.position, projectionVector, attackDamage);
            }
        }
    }

    public void SpecialAttack() {
        if (!isAlive) { return; }
        throw new System.NotImplementedException();
    }

    public void UltimateAttack() {
        if (!isAlive) { return; }
        throw new System.NotImplementedException();
    }
<<<<<<< Updated upstream:SmashLemons/Assets/Scripts/Character.cs
=======
    public void Block(InputAction.CallbackContext context) {
        if (!isAlive) { return; }
        if (context.started) {
            Debug.Log("startBlock");
            isBlocking = true;
            animator.SetBool("isMoving", false);
            shield.GetComponent<Renderer>().material.SetColor("_Color", Color.blue);
        } else if (context.canceled) {
            Debug.Log("stopBlock");
            isBlocking = false;
            shield.GetComponent<Renderer>().material.SetColor("_Color", Color.white);
        }
>>>>>>> Stashed changes:SmashLemons/Assets/Scripts/Players/Character.cs

    public void Block() {
        isBlocking = true;
    }

    public void Taunt() {
        if (!isAlive) { return; }
        throw new System.NotImplementedException();
    }

    public float Heal(float amount){
<<<<<<< Updated upstream:SmashLemons/Assets/Scripts/Character.cs
=======
        if (!isAlive) { return 0f; }
        takenDamage -= amount;
        if(takenDamage<0f){
            takenDamage = 0f;
        }
>>>>>>> Stashed changes:SmashLemons/Assets/Scripts/Players/Character.cs
        return amount;
    }

    public float TakeDamage(Vector3 origin, Vector3 projection, float amount) {
        if (!isAlive) { return 0f; }
        // Attaque du côté du shield: (transform.position - origin).x * (shield.position - transform.position).x NEGATIF
        // Attaque de côté opposé au shield: (transform.position - origin).x * (shield.position - transform.position).x POSITIF
        if (!isBlocking || (transform.position - origin).x * (shield.position - transform.position).x > 0) {
            takenDamage += amount - amount * (resistance / 100);

            /***/
            Vector3 direction = new Vector3(centerOfMass.position.x - origin.x, 0f, 0f).normalized;
            if(projection.y == 0f){
                direction = new Vector3(centerOfMass.position.x - origin.x, 0f, 0f).normalized;
            }
            else{
                direction = new Vector3(projection.x, projection.y, 0f).normalized;
            }
            Debug.Log(direction);

            direction *= (1 + takenDamage) * (weight / 100);
            Debug.Log(takenDamage);
            // Add pushForce;
            body.AddForce(direction, ForceMode.VelocityChange);
<<<<<<< Updated upstream:SmashLemons/Assets/Scripts/Character.cs
            //(1+takenDamage) * weight/100

            /***/
            Debug.Log("Target takes a " + amount + " hit.");
=======
            playerSounds.PlayHit();

>>>>>>> Stashed changes:SmashLemons/Assets/Scripts/Players/Character.cs
            return amount;
        } else {
            Debug.Log("Target blocks a " + amount + " hit.");
            return 0;
        }
    }

    public void Die() {
        if (!isAlive) { return; }
        //checkForLivesLeft
        remainingLives--;
        takenDamage = 0f;
<<<<<<< Updated upstream:SmashLemons/Assets/Scripts/Character.cs
        body.velocity = new Vector3(0f, body.velocity.y, body.velocity.z);
        body.angularVelocity = new Vector3(0f, body.angularVelocity.y, body.angularVelocity.z);
        transform.position = new Vector3(0f, 10f, 0f);
=======
        if(remainingLives > 0){
            Respawn();
        }
        else{
            Debug.Log(transform.name + " is dead.");
            isAlive = false;
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
>>>>>>> Stashed changes:SmashLemons/Assets/Scripts/Players/Character.cs

    public void setAlive(bool alive){
        isAlive = alive;
    }
}
