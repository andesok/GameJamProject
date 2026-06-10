using UnityEngine;
using UnityEngine.InputSystem.XR;

[RequireComponent(typeof(CharacterController))]
public class CharacterMovement : MonoBehaviour
{
    [HideInInspector] public Vector2 moveVals;
    private Vector3 moveDirection;
    private float smoothRotation = 0.05f;
    private float currentRotationVelocity;
    private Vector3 velocity;
    private float gravity = -9.81f;
    private float pushForce = 10f;
    private CharacterController controller;
    
    [SerializeField]
    private float speed = 5.0f;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Walk()
    {
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
            var angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetangle, ref currentRotationVelocity, smoothRotation);
            transform.localRotation = Quaternion.Euler(0, angle, 0);
        }
    }
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody body = hit.collider.attachedRigidbody;

        if (body == null || body.isKinematic) return;

        Vector3 pushDir = hit.gameObject.transform.position - transform.position;
        pushDir.y = 0;
        pushDir.Normalize();

        body.AddForceAtPosition(pushDir * pushForce, transform.position, ForceMode.Impulse);

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

    private void FixedUpdate()
    {
        Walk();
        Rotation();
        ForceGravity();
    }
}
