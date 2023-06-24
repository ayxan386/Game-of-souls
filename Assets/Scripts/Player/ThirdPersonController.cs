using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonController : MonoBehaviour
{
    [SerializeField] private CharacterController cc;
    [SerializeField] private float speed;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float gravityFactor;
    [SerializeField] private Animator animator;

    private Vector3 movementVector;
    private Vector2 inputVector;

    void Update()
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime * inputVector.x);

        var prevY = movementVector.y;
        movementVector = transform.forward * (inputVector.y * speed);
        movementVector.y = prevY;

        if (cc.isGrounded)
        {
            animator.SetBool("jumping", false);
        }
        else
        {
            movementVector.y -= gravityFactor * Time.deltaTime;
        }

        cc.Move(movementVector * Time.deltaTime);
    }

    private void OnMove(InputValue val)
    {
        inputVector = val.Get<Vector2>();
        animator.SetBool("running", inputVector.sqrMagnitude > 0);
    }

    private void OnJump()
    {
        if (!cc.isGrounded) return;

        animator.SetBool("jumping", true);
        movementVector.y = jumpForce;
    }
}