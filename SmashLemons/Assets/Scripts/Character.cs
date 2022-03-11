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


    private void Awake() {
        this.speed = 5f;
        this.jumpForce = 2f;
        this.dashForce = 10f;
        this.weight = 100f;
        this.resistance = 0f;
        this.attackDamage = 1f;
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
        centerOfMass = transform.Find("CenterOfMass");
    }

    void Update() {
        isGrounded = Physics.CheckSphere(groundChecker.position, GroundDistance, Ground, QueryTriggerInteraction.Ignore);
        if(isGrounded){
            if(dashTimer <= 0f){
                remainingDashes = maxDashes;
            }
            else{
                dashTimer -= Time.deltaTime;
            }
            
        }


        inputs = Vector3.zero;
        inputs.x = Input.GetAxisRaw("Horizontal");
        inputs.y = Input.GetAxis("Vertical");
        inputs = inputs.normalized;
        Vector3 direction = new Vector3(inputs.x, 0f/*vertical*/, 0f).normalized;
        if(Mathf.Abs(inputs.x) >= 0.1f){
            lastDirection = new Vector3(inputs.x, 0f, 0f);
        }

        // Lors d'un changement de sens lors d'un dash, la velocité est annulée
        if(inputs.x * dashDirection < 0f){
            body.velocity = new Vector3(0f, body.velocity.y, body.velocity.z);
            body.angularVelocity = new Vector3(0f, body.angularVelocity.y, body.angularVelocity.z);
        }

        if (direction.magnitude >= 0.1f) {
            animator.SetBool("isMoving", true);
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
        }
        else{
            animator.SetBool("isMoving", false);
        }


        if (Input.GetButtonDown("Jump") && isGrounded) {
            Jump();
        }
        if (Input.GetButtonDown("Dash") && remainingDashes>0) {
            dashTimer = 0.1f;
            dashDirection = inputs.x;
            remainingDashes--;
            Dash();
        }

        if(Input.GetButtonDown("Attack")){
            
            if(attackCooldown<= 0f){
                attackCooldown = 1f / attackSpeed;
                Attack();
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
        Vector3 move = new Vector3(inputs.x, 0f, 0f);
        Move(move);
    }



    public void Move(Vector3 move) {
        body.MovePosition(body.position + move * speed * Time.fixedDeltaTime);
    }


    public void Jump() {
        body.AddForce(Vector3.up * Mathf.Sqrt(jumpForce * -2f * Physics.gravity.y), ForceMode.VelocityChange);
    }

    public void Dash() {
        Vector3 dashVelocity = new Vector3(lastDirection.x, 0f, 0f) * dashForce;
        if(inputs.magnitude > 0.1f){
            dashVelocity = inputs * dashForce;
        }
        body.velocity = Vector3.zero;
        body.AddForce(dashVelocity, ForceMode.VelocityChange);
        Debug.Log("Dash");
    }

    public void Attack() {
        meleeRange.GetComponent<Renderer>().material.SetColor("_Color", Color.red);
        Collider[] hitColliders = Physics.OverlapSphere(meleeRange.position, 0.3f);
        foreach (var hitCollider in hitColliders) {
            if(hitCollider.tag == "Player" && hitCollider.name != transform.name){
                hitCollider.transform.GetComponent<Character>().TakeDamage(meleeRange.position, attackDamage);
            }
            
        }
    }

    public void SpecialAttack() {
        throw new System.NotImplementedException();
    }

    public void UltimateAttack() {
        throw new System.NotImplementedException();
    }

    public void Block() {
        isBlocking = true;
    }

    public void Taunt() {
        throw new System.NotImplementedException();
    }

    public float Heal(float amount){
        return amount;
    }

    public float TakeDamage(Vector3 origin, float amount) {
        Debug.Log(centerOfMass.name);
        // Attaque du côté du shield: (transform.position - origin).x * (shield.position - transform.position).x NEGATIF
        // Attaque de côté opposé au shield: (transform.position - origin).x * (shield.position - transform.position).x POSITIF
        if (!isBlocking || (transform.position - origin).x * (shield.position - transform.position).x > 0) {
            takenDamage += amount - amount * (resistance / 100);
            Vector3 direction = (centerOfMass.position - origin).normalized;
            direction *= (1 + takenDamage) * (weight / 100);
            Debug.Log(takenDamage);
            // Add pushForce;
            body.AddForce(direction, ForceMode.VelocityChange);
            //(1+takenDamage) * weight/100
            Debug.Log("Target takes a " + amount + " hit.");
            return amount;
        } else {
            Debug.Log("Target blocks a " + amount + " hit.");
            return 0;
        }
    }

    public void Die() {
        //checkForLivesLeft
        //Respawn
        body.velocity = new Vector3(0f, body.velocity.y, body.velocity.z);
        body.angularVelocity = new Vector3(0f, body.angularVelocity.y, body.angularVelocity.z);
        transform.position = new Vector3(0f, 10f, 0f);

    }
}
