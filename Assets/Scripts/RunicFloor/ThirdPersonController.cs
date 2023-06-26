using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RunicFloor
{
    public class ThirdPersonController : MonoBehaviour
    {
        [SerializeField] private CharacterController cc;
        [SerializeField] private float speed;
        [SerializeField] private float rotationSpeed;
        [SerializeField] private float jumpForce;
        [SerializeField] private float gravityFactor;
        [SerializeField] private Animator animator;

        [Header("In world indicators")] [SerializeField]
        private TextMeshPro playerInWorldName;

        [SerializeField] private MeshRenderer colorIndicator;
        [SerializeField] private Light colorLightIndicator;

        private Vector3 movementVector;
        private Vector2 inputVector;
        public PlayerRoundData RoundData { get; set; }


        private void OnEnable()
        {
            animator.transform.SetParent(transform, false);
            animator.SetBool("running", false);
        }

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
            if (!GameManager.Instance.GameRunning) return;
            inputVector = val.Get<Vector2>();
            animator.SetBool("running", inputVector.sqrMagnitude > 0);
        }

        private void OnJump()
        {
            if (!cc.isGrounded || !GameManager.Instance.GameRunning) return;

            animator.SetBool("jumping", true);
            movementVector.y = jumpForce;
        }

        public void TeleportToPosition(Vector3 pos)
        {
            cc.enabled = false;
            transform.position = pos;
            cc.enabled = true;
        }

        public void UpdateIndicator(string fullName, Color color)
        {
            playerInWorldName.text = "P" + fullName.Split(" ")[1];
            colorIndicator.material.color = color;
            colorLightIndicator.color = color;
        }
    }
}