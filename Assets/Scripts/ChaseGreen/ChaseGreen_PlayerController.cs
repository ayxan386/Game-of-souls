using UnityEngine;
using UnityEngine.InputSystem;

public class ChaseGreen_PlayerController : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private Animator animator;
    [SerializeField] private CharacterController cc;
    [SerializeField] [Range(0, 1f)] private float rotationFactor;

    private Vector3 movementVector;

    public int EliminationIndex { get; set; }
    public bool Eliminated { get; set; }

    void Update()
    {
        if (!ChaseGreen_PlayerManager.PlayersReady) return;

        animator.SetBool("running", movementVector.sqrMagnitude > 0);
        cc.SimpleMove(movementVector);

        transform.forward = Vector3.Lerp(transform.forward, movementVector.normalized, rotationFactor);
    }

    private void OnMove(InputValue inputValue)
    {
        var vec = inputValue.Get<Vector2>();
        vec.Normalize();
        movementVector = new Vector3(vec.x, 0, vec.y) * speed;
    }

    public void TeleportToPosition(Vector3 pos)
    {
        cc.enabled = false;
        transform.position = pos;
        cc.enabled = true;
    }
}