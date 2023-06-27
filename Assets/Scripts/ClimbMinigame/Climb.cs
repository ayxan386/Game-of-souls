using ClimbMinigame;
using UnityEngine;

public class Climb : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private Animator animator;
    [SerializeField] private PlayerSubManager subManager;

    private bool win;

    private void OnEnable()
    {
        animator.transform.SetParent(transform, false);
        animator.SetBool("running", false);
        animator.SetBool("climbing", true);
        win = false;
    }

    private void OnDisable()
    {
        animator.SetBool("climbing", false);
    }

    private void OnClimb()
    {
        if (win) return;

        Debug.Log("Estoy subiendo");
        transform.Translate(transform.up * speed * Time.deltaTime, Space.World);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!win)
        {
            Win();
        }
    }

    private void Win()
    {
        Debug.Log("He ganado");
        win = true;
        animator.SetBool("climbing", false);
        GameManager.Instance.OnWin(subManager);
    }
}