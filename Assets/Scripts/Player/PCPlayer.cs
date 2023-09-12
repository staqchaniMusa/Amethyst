using KinematicCharacterController.Examples;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PCPlayer : MonoBehaviourPun
{

    [Tooltip("Maximum slope the character can jump on")]
    [Range(5f, 60f)]
    public float slopeLimit = 45f;
    [Tooltip("Move speed in meters/second")]
    public float moveSpeed = 5f;
    [Tooltip("Turn speed in degrees/second, left (+) or right (-)")]
    public float turnSpeed = 300;
    [Tooltip("Whether the character can jump")]
    public bool allowJump = false;
    [Tooltip("Upward speed to apply when jumping in meters/second")]
    public float jumpSpeed = 4f;
    public bool IsGrounded { get; private set; }
    public float ForwardInput { get; set; }
    public float TurnInput { get; set; }
    public bool JumpInput { get; set; }

    [Range(0.0f, 0.3f)]
    public float RotationSmoothTime = 0.12f;
    private float _targetRotation = 0.0f;
    private float _rotationVelocity;

    public float lookSenesitivity = 0.1f;
    ExampleCharacterCamera CharacterCamera;
    PhotonView PV;
    private new Rigidbody rigidbody;
    private CapsuleCollider capsuleCollider;
    private void Start()
    {
        PV = GetComponent<PhotonView>();
        
        rigidbody = GetComponent<Rigidbody>();
        capsuleCollider = GetComponentInChildren<CapsuleCollider>();
        if (PV.IsMine)
        {
            CharacterCamera = Instantiate(Resources.Load<ExampleCharacterCamera>("OrbitCamera"), Vector3.zero, Quaternion.identity);
            CharacterCamera.SetFollowTransform(transform);

            Vector3 pos;
            if((int)PV.Owner.CustomProperties["team"] == 0)
            {
                int spawnPicker = Random.Range(1, GameSetUp.GS.spawnPointsAlpha.Length);
                pos = GameSetUp.GS.spawnPointsAlpha[spawnPicker].position;
            }else
            {
                int spawnPicker = Random.Range(1, GameSetUp.GS.spawnPointsAlpha.Length);
                pos = GameSetUp.GS.spawnPointsBeta[spawnPicker].position;
            }
            transform.position = pos;
        }
        else rigidbody.isKinematic = true;
    }

    private void FixedUpdate()
    {
        if (PV.IsMine)
        {
            CheckGrounded();
            ProcessActions();
            HandleCameraInput();
        }
    }
    
    /// <summary>
    /// Processes input actions and converts them into movement
    /// </summary>
    private void ProcessActions()
    {
        ForwardInput = Mathf.RoundToInt(Input.GetAxisRaw("Vertical"));
        TurnInput = Mathf.RoundToInt(Input.GetAxisRaw("Horizontal"));
        bool jump = Input.GetKey(KeyCode.Space);
        var _input = new Vector2(TurnInput, ForwardInput);
        /* // Process Turning
         if (TurnInput != 0f)
         {
             float angle = Mathf.Clamp(TurnInput, -1f, 1f) * turnSpeed;
             transform.Rotate(Vector3.up, Time.fixedDeltaTime * angle);
         }*/

        // normalise input direction
        Vector3 inputDirection = new Vector3(_input.x, 0.0f, _input.y).normalized;

        // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
        // if there is a move input rotate player when the player is moving
        if (_input != Vector2.zero)
        {
            _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                              CharacterCamera.transform.eulerAngles.y;
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
                RotationSmoothTime);

            // rotate to face input direction relative to camera position
            transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
        }
        Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;
        //Debug.Log(targetDirection);
        // Process Movement/Jumping
        if (IsGrounded)
        {
            // Reset the velocity
            rigidbody.velocity = Vector3.zero;
            // Check if trying to jump
            if (JumpInput && allowJump)
            {
                // Apply an upward velocity to jump
                rigidbody.velocity += Vector3.up * jumpSpeed;
            }

            
            rigidbody.velocity += targetDirection.normalized * moveSpeed;
        }
        else
        {
            // Check if player is trying to change forward/backward movement while jumping/falling
            if (!Mathf.Approximately(ForwardInput, 0f))
            {
                
                // Override just the forward velocity with player input at half speed
                Vector3 verticalVelocity = Vector3.Project(rigidbody.velocity, Vector3.up);
                rigidbody.velocity = verticalVelocity + targetDirection.normalized * moveSpeed / 2f;
            }
        }
    }

    /// <summary>
    /// Checks whether the character is on the ground and updates <see cref="IsGrounded"/>
    /// </summary>
    private void CheckGrounded()
    {
        IsGrounded = false;
        float capsuleHeight = Mathf.Max(capsuleCollider.radius * 2f, capsuleCollider.height);
        Vector3 capsuleBottom = transform.TransformPoint(capsuleCollider.center - Vector3.up * capsuleHeight / 2f);
        float radius = transform.TransformVector(capsuleCollider.radius, 0f, 0f).magnitude;
        Ray ray = new Ray(capsuleBottom + transform.up * .01f, -transform.up);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, radius * 5f))
        {
            float normalAngle = Vector3.Angle(hit.normal, transform.up);
            if (normalAngle < slopeLimit)
            {
                float maxDist = radius / Mathf.Cos(Mathf.Deg2Rad * normalAngle) - radius + .02f;
                if (hit.distance < maxDist)
                    IsGrounded = true;
            }
        }
    }
    private void HandleCameraInput()
    {
        // Create the look input vector for the camera
        float mouseLookAxisUp = Input.GetAxis("Mouse Y");
        float mouseLookAxisRight = Input.GetAxis("Mouse X");
        Vector3 lookInputVector = new Vector3(mouseLookAxisRight, mouseLookAxisUp, 0f);
        if (Input.GetMouseButtonDown(1))
        {
            Cursor.lockState = CursorLockMode.Confined;
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
        }
        // Prevent moving the camera while the cursor isn't locked
        if (Cursor.lockState != CursorLockMode.Confined)
        {
            lookInputVector = Vector3.zero;
        }

        // Input for zooming the camera (disabled in WebGL because it can cause problems)
        var MouseScrollInput = Input.mouseScrollDelta.y;
        float scrollInput = -MouseScrollInput * lookSenesitivity;
#if UNITY_WEBGL
        scrollInput = 0f;
#endif

        // Apply inputs to the camera
        CharacterCamera.UpdateWithInput(Time.deltaTime, scrollInput, lookInputVector);


    }

   
}
