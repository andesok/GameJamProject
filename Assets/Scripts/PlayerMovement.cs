using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public InputActionAsset InputActions;
    [SerializeField] CharacterController controller;
    public GameObject PlayerMesh;

    private InputAction moveAction;

    public Vector2 moveVals;
    private Vector3 moveDirection;

    public float speed = 5.0f;
    public float pushForce = 10f;
    private float smoothRotation = 0.05f;
    public float gravity = -9.81f;
    public Vector3 velocity;
    private float currentRotationVelocity;

    private void OnEnable()
    {
        InputActions.FindActionMap("Player").Enable();
    }
    private void OnDisable()
    {
        InputActions.FindActionMap("Player").Disable();
    }

    private void Awake()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        Debug.Log(PlayerMesh);
    }

    void Update()
    {

    }

    private void Walking()
    {
        moveVals = moveAction.ReadValue<Vector2>();
        if (moveVals.magnitude > 0.1f)
        {
            moveDirection = new Vector3(moveVals.x, 0, moveVals.y).normalized;
            Vector3 targetPosition = moveDirection * speed * Time.fixedDeltaTime;
            controller.Move(targetPosition);
        }
        else
        {
            moveDirection = Vector3.zero;
        }
    }

    private void Rotation()
    {
        if (moveDirection.magnitude > 0)
        {
            var targetangle = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg;
            var angle = Mathf.SmoothDampAngle(PlayerMesh.transform.eulerAngles.y, targetangle, ref currentRotationVelocity,smoothRotation);
            PlayerMesh.transform.localRotation = Quaternion.Euler(0, angle, 0);
        }
    }

    private void FixedUpdate()
    {
        Walking();
        Rotation();
        ForceGravity();
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody body = hit.collider.attachedRigidbody;

        if (body == null || body.isKinematic) return;

        Vector3 pushDir = hit.gameObject.transform.position - transform.position;
        pushDir.y = 0;
        pushDir.Normalize();

        body.AddForceAtPosition(pushDir*pushForce,transform.position,ForceMode.Impulse);

    }

    private void ForceGravity()
    {
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;

        }
        velocity.y += gravity * Time.fixedDeltaTime;

        controller.Move(velocity * Time.fixedDeltaTime);
    }
}