using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	
	//origional
    [Header("Assighnables")]
    public Rigidbody rb;

    [SerializeField] private Transform Camera;
    [SerializeField] private Transform head;
    [SerializeField] private Transform orientation;

    [Header("Mouse Settings")]
    [SerializeField] private Vector2 sensitivity = new Vector2(5, 5);
    [SerializeField] private float sensitvityMultiplier = 0.01f;    

    [Header("Movement Settings")]
    [SerializeField] private float speed;
    [SerializeField] private float drag;
    [SerializeField] private float airdrag;
    [SerializeField] private float speedMultiplier = 1f;
    [SerializeField] private float sprintMultiplier = 2f;
    [SerializeField] private float aimMultiplier = 0.4f;
    [SerializeField] private float airMultiplier = 0.2f;

    [Header("Sliding and Crouching")]
    [SerializeField] private Vector3 crouchScale = new Vector3(1, 0.5f, 1);
    [SerializeField] private Vector3 playerScale;
    [SerializeField] private float slideCounterMovement = 0.2f;
    [SerializeField] private float slideStartThreshold = 5f;
    [SerializeField] private float slideSpeed = 4f;
    [SerializeField] private float slideDuration = 1f;
    [SerializeField] private float crouchMovementMultiplier = 0.5f;

    [Header("JumpSettings")]
    [SerializeField] private float groundCheckRadious;
    [SerializeField] private Vector3 groundCheckPos;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float jumpForce;
    [SerializeField] private int maxNumJump = 2;

    [Header("Double Jump")]
    [SerializeField] private float doubleJumpForce;

    
    public bool _grounded, _readytojump, _walking, _sprinting, _crouching, _sliding, _grappling;

    bool isSliding;
    public static float grapplemultiplier = 0.05f;
    public static float grappleDrag = 0.05f;

    float X;
    float Y;
    public static Vector3 moveDir;
    float _multiplier;
    float _drag;
    int _numJumps;

    public Vector3 velocityOnHorizontalPlane;


    void Awake()
    {
	    Cursor.lockState = CursorLockMode.Locked;
	_numJumps = maxNumJump;
	Inputs.readytojump = true;
    }

    void UpdateCamPos()
    {
        Camera.position = head.position;
    }

    float xRotation;
    float yRotation;
    void Look()
    {
	    float mouseX = Input.GetAxis("Mouse X");
	    float mouseY = Input.GetAxis("Mouse Y");        

	    yRotation += mouseX * sensitivity.x * sensitvityMultiplier;
	    xRotation -= mouseY * sensitivity.y * sensitvityMultiplier;

	    xRotation = Mathf.Clamp(xRotation, -90, 90);
    }	
    
    void SetSpeed()
    {
        if(Inputs.grappling == true && Inputs.grounded == false)
        {
            _multiplier = grapplemultiplier;
            _drag = grappleDrag;
            return;
        }

        if(Inputs.grounded == false && Inputs.grappling == false)
        {
            _multiplier = airMultiplier;
            _drag = airdrag;
            return;
        }

        if (Inputs.walking)
        {
            _multiplier = speedMultiplier;
            _drag = drag;
        }
        else if (Inputs.Sprinting)
        {
            _multiplier = sprintMultiplier;
            _drag = drag;
        }
        else if (Inputs.crouching == true)
        {
            _multiplier = crouchMovementMultiplier;
            _drag = drag;
            if(Inputs.sliding == true)
            {
                _drag = slideCounterMovement;
            }
        }
    }
    

    void StartCrouch()
    {
        transform.localScale = crouchScale;
        _groundCheckPos = groundCheckPos - new Vector3(0, 0.5f, 0);

        if (Inputs.crouching == true && rb.velocity.magnitude >= slideStartThreshold && isSliding == true && Inputs.grounded)
        {
            Slide();
            isSliding = false;
        }
    }
    void StopCrouch()
    {
        transform.localScale = playerScale;
        _groundCheckPos = groundCheckPos;
        Inputs.crouching = false;
        Inputs.sliding = false;
        isSliding = true;
    }

    void Slide()
    {
        Inputs.sliding = true;
        rb.AddForce(orientation.transform.forward * slideSpeed, ForceMode.Impulse);

        StartCoroutine(StopProjectedSlide(slideDuration));
        //StopProjectedSlide(velocityOnHorizontalPlane.magnitude);
    }

    private IEnumerator StopProjectedSlide(float duration)
    {
        yield return new WaitForSeconds(duration);

        Inputs.sliding = false;
        isSliding = false;
    }

    void Jump(float jumpForce)
    {
        Inputs.crouching = false;
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    void Movement()
    {
        //basic movement
	    X = Input.GetAxis("Horizontal");
	    Y = Input.GetAxis("Vertical");

	    moveDir = orientation.right * X + orientation.forward * Y;


        //friction
        Drag();

        //jumping
        if (GroundCheck())
        {
            _numJumps = maxNumJump;
        }

	if (Input.GetKey(KeyCode.Space) && _numJumps > 0 && Inputs.readytojump)
        {
	    _numJumps--;
            Jump(jumpForce);
	    //Inputs.readytojump = false;
        }//else if (Input.GetKeyUp(KeyCode.Space)) {Inputs.readytojump = true;}
        
        //crouching and sliding
        if (Inputs.crouching)
            StartCrouch();
        else
            StopCrouch();

        if (Inputs.sliding)
            return;

        //adding force
        rb.AddForce(moveDir * speed * _multiplier);
    }

    Vector3 _groundCheckPos;
    bool GroundCheck()
    {
        bool _grounded = Physics.CheckSphere(transform.position - _groundCheckPos, groundCheckRadious, groundMask);

        return _grounded;
    }
    

    Vector3 counterMovement = Vector3.zero;
    void Drag()
    {
	    velocityOnHorizontalPlane = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        if (Inputs.sliding == true)
        {
            counterMovement = -velocityOnHorizontalPlane * slideCounterMovement * slideSpeed;
        }

        counterMovement = -velocityOnHorizontalPlane * _drag * speed;

	    rb.AddForce(counterMovement);
    }


    void Update()
    {
        Inputs.cameraLook = Camera.transform;
        
        //looking around
	    Look();
	    Camera.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);
	    orientation.localRotation = Quaternion.Euler(0, yRotation, 0f);

        //grounded bool
        Inputs.grounded = GroundCheck();

        //movement State
        Inputs.State();
        SetSpeed();
        


        _grounded = Inputs.grounded;
	_readytojump = Inputs.readytojump;
        _walking = Inputs.walking;
        _sprinting = Inputs.Sprinting;
        _crouching = Inputs.crouching;
        _sliding = Inputs.sliding;
        _grappling = Inputs.grappling;

        velocityOnHorizontalPlane = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
    }

    void LateUpdate()
    {
	    UpdateCamPos();
    }    

    void FixedUpdate()
    {	
	    Movement();
    }


    //Debugging

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position - _groundCheckPos, groundCheckRadious);
    }
}
