using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ChaseGreen_PlayerController : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private Animator animator;
    [SerializeField] private CharacterController cc;
    [SerializeField] [Range(0, 1f)] private float rotationFactor;

    private Vector3 movementVector;

    public static List<Transform> Players;

    private void Awake()
    {
        if (Players == null)
        {
            Players = new List<Transform>();
        }

        Players.Add(transform);
    }

    void Update()
    {
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
}