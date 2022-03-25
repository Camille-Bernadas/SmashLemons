using UnityEngine;
using UnityEngine.InputSystem;

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
    public PlayerUIManager uiManager;

    private void Awake() {
        this.speed = 5f;
        this.jumpForce = 1f;
        this.dashForce = 4f;
        this.weight = 100f;
        this.resistance = 0f;
        this.attackDamage = 20f;
        this.attackSpeed = 2f;
        this.specialSpeed = 1f;
        this.ultimateProgression = 0f;
        this.isBlocking = false;
        this.isGrounded = false;
        this.maxDashes = 2;
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
        isGrounded = Physics.CheckSphere(groundChecker.position, GroundDistance, Ground, QueryTriggerInteraction.Ignore);
        if (isGrounded) {
            if (dashTimer <= 0f) {
                remainingDashes = maxDashes;
            } else {
                dashTimer -= Time.deltaTime;
            }

        }

        inputs = inputs.normalized;

        if(!isBlocking){
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
        if (!isBlocking) {
            Vector3 move = new Vector3(inputs.x, 0f, 0f);
            Move(move);
        }
    }

    public void OnMove(InputAction.CallbackContext ctx) => inputs = ctx.ReadValue<Vector2>();

    public void Move(Vector3 move) {
        body.MovePosition(body.position + move * speed * Time.fixedDeltaTime);
    }

    public void Jump() {
        if(isGrounded){
            body.AddForce(Vector3.up * Mathf.Sqrt(jumpForce * -2f * Physics.gravity.y), ForceMode.VelocityChange);
        }
        
    }

    public void Dash() {
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
        }
    }

    public void setAttack(InputAction.CallbackContext context){
        if (context.performed) {
            Debug.Log("Attack");
            if (attackCooldown <= 0f) {
                attackCooldown = 1f / attackSpeed;
                Attack(inputs);
            }
        }
    }
    public void Attack(Vector2 direction) {
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
        throw new System.NotImplementedException();
    }

    public void UltimateAttack() {
        throw new System.NotImplementedException();
    }
    public void Block(InputAction.CallbackContext context) {
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

    }

    public void Taunt() {
        throw new System.NotImplementedException();
    }

    public float Heal(float amount){
        takenDamage -= amount;
        if(takenDamage<0f){
            takenDamage = 0f;
        }
        uiManager.setDamage(takenDamage);
        return amount;
    }

    public float TakeDamage(Vector3 origin, Vector3 projection, float amount) {
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
            Debug.Log(direction);
            direction *= (4 * (1 + takenDamage/100f)) / (weight / 100);
            Debug.Log(takenDamage);
            body.AddForce(direction, ForceMode.VelocityChange);
            uiManager.setDamage(takenDamage);
            return amount;
        } else {
            return 0;
        }
    }

    public void Die() {
        //checkForLivesLeft
        //Respawn
        takenDamage = 0f;
        uiManager.setDamage(takenDamage);
        body.velocity = new Vector3(0f, body.velocity.y, body.velocity.z);
        body.angularVelocity = new Vector3(0f, body.angularVelocity.y, body.angularVelocity.z);
        transform.position = new Vector3(0f, 10f, 0f);

    }
}
