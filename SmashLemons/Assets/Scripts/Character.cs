using UnityEngine;

public abstract class Character : MonoBehaviour, ICharacter, IDamageable
{ 
    /* Movement */
    [SerializeField]
    private float speed;
    private float jumpForce;
    private float dashForce;
    private bool isGrounded;
    private float dashTimer;
    private int remainingDashes;
    private int maxDashes;



    /* Attacks */
    private float attackDamage;
    private float attackSpeed;
    private float specialSpeed;
    private float ultimateProgression;

    /* Defense */
    private bool isBlocking;
    private float weight;
    private float resistance;
    private float takenDamage;


    /* References to other objects */
    private Rigidbody body;
    private Transform groundChecker;
    private Transform shield;

    /* TODO Sort */
    private Vector3 inputs = Vector3.zero;
    private float GroundDistance = 0.2f;
    public LayerMask Ground;
    float turnSmoothVelocity;
    float turnSmoothTime = 0.05f;
    private Vector3 lastDirection;

    float dashDirection;


    private void Awake() {
        this.speed = 5f;
        this.jumpForce = 2f;
        this.dashForce = 10f;
        this.weight = 1f;
        this.resistance = 1f;
        this.attackDamage = 1f;
        this.attackSpeed = 1f;
        this.specialSpeed = 1f;
        this.ultimateProgression = 0f;
        this.isBlocking = false;
        this.isGrounded = false;
        this.maxDashes = 2;
    } 
    void Start() {
        body = GetComponent<Rigidbody>();
        groundChecker = transform.GetChild(0);
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
            //animator.SetBool("isMoving", true);
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
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
        throw new System.NotImplementedException();
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
        // Attaque du côté du shield: (transform.position - origin).x * (shield.position - transform.position).x NEGATIF
        // Attaque de côté opposé au shield: (transform.position - origin).x * (shield.position - transform.position).x POSITIF
        if(!isBlocking || (transform.position - origin).x * (shield.position - transform.position).x > 0){
            takenDamage += amount - amount*(resistance/100);
            // Add pushForce;
            //(1+takenDamage) * weight/100
            return amount;
        }
        else{
            return 0;
        }
    }

    void Die(){
        //checkForLivesLeft
        //Respawn
    }
}
