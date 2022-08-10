using System;
using UnityEngine;
using Photon.Pun;
public class PlayerMovementNew : MonoBehaviour
{
    private float playerHeight = 2f;

    [SerializeField] Transform orientation;
    [SerializeField] Transform ui;

    [Header("Movement")]
    [SerializeField] float moveSpeed;
    [SerializeField] float airMovementMultiplier;

    private float movementMultiplier = 10f;

    [Header("Sprinting")]
    [SerializeField] float walkSpeed;
    [SerializeField] float sprintSpeed;
    [SerializeField] float acceleration;

    [Header("Jumping")]
    public float jumpForce = 15f;

    [Header("Keybinds")]
    [SerializeField] KeyCode jumpKey;
    [SerializeField] KeyCode sprintKey;

    [Header("Drag")]
    [SerializeField] float groundDrag;
    [SerializeField] float airDrag;

    private float horizontalMovement;
    private float verticalMovement;

    [Header("Ground Detection")]
    [SerializeField] Transform groundCheck;
    [SerializeField] LayerMask groundMask;
    public bool isGrounded;
    public bool wasGrounded;
    private float groundDistance = 0.4f;

    private PhotonView PV;

    private Vector3 moveDirection;
    private Vector3 slopeMoveDirection;

    private Rigidbody rb;

    private RaycastHit slopeHit;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        PV = GetComponent<PhotonView>();
    }

    private void Start()
    {
        if (!PV.IsMine)
        {
            //Setting other player's layers to 9(Enemy)...
            foreach (var kid in this.gameObject.GetComponentsInChildren(typeof(Transform), true))
            {
                kid.gameObject.layer = 9;
            }

            //..tags to Enemy 
            this.gameObject.tag = "Enemy";
            this.transform.GetChild(0).gameObject.tag = "Enemy";

            //..turn on isKinematic on their rigidbodies
            rb.isKinematic = true;

            //and destroy their ui object(everything is done on the local machine)
            Destroy(ui.gameObject);
        }
        else
        {
            //Set the current's player layer to 6(Player)...
            foreach (var kid in this.gameObject.GetComponentsInChildren(typeof(Transform), true))
            {
                kid.gameObject.layer = 6;
            }

            //..tag to LocalPlayer
            this.gameObject.tag = "LocalPlayer";

            //..and freezeRotation for the current rigidbody
            rb.freezeRotation = true;
        }
    }

    private void Update()
    {
        if (!PV.IsMine)
        {
            return;
        }
        if ((Convert.ToBoolean(PhotonNetwork.CurrentRoom.CustomProperties["GameStarting"])) == true) return;

        wasGrounded = isGrounded;

        //Verifing the players position using a vertical raycast 
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        MyInput();
        ControlDrag();
        ControlSpeed();

        if (Input.GetKeyDown(jumpKey) && isGrounded)
        {
            Jump();
        }

        slopeMoveDirection = Vector3.ProjectOnPlane(moveDirection, slopeHit.normal);
    }


    private void MyInput()
    {
        horizontalMovement = Input.GetAxisRaw("Horizontal");
        verticalMovement = Input.GetAxisRaw("Vertical");

        moveDirection = orientation.forward * verticalMovement + orientation.right * horizontalMovement;
    }

    private void Jump()
    {
        //Resets the y velocity of the player(Bug fix for the jump)
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ControlDrag()
    {
        if (isGrounded)
        {
            rb.drag = groundDrag;
        }
        else
        {
            rb.drag = airDrag;
        }
    }

    private void ControlSpeed()
    {
        if (Input.GetKey(sprintKey) && isGrounded)
        {
            moveSpeed = Mathf.Lerp(moveSpeed, sprintSpeed, acceleration * Time.deltaTime);
        }
        else
        {
            moveSpeed = Mathf.Lerp(moveSpeed, walkSpeed, acceleration * Time.deltaTime);

        }
    }


    //Fixed update is used because it has the frequency of the physics system and since we use a rigidbody component the movemnt will look smoother
    private void FixedUpdate()
    {
        if (!PV.IsMine)
        {
            return;
        }
        MovePlayer();
    }


    private void MovePlayer()
    {
        if (isGrounded && !onSlope())
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * movementMultiplier, ForceMode.Acceleration);
        }
        else if (isGrounded && onSlope())
        {
            rb.AddForce(slopeMoveDirection.normalized * moveSpeed * movementMultiplier, ForceMode.Acceleration);
        }
        else
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * movementMultiplier * airMovementMultiplier, ForceMode.Acceleration);
        }
    }


    private bool onSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.5f))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

}
