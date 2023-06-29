using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopCheck : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private GameObject inventoryBox;

    private Animator animator;
    private DialogueManager dialogueManager;  
    public PlayerInfo playerInfo;
    public bool openShop;

    private void Start()
    {
        dialogueManager = dialogueBox.GetComponent<DialogueManager>();
        animator = inventoryBox.GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {           
            playerInfo = other.GetComponent<PlayerInfo>();
            animator.SetBool("IsOpen", false);
            openShop = true;           
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            openShop = false;
            dialogueManager.EndDialogue();
            animator.SetBool("IsOpen", true);
        }  
    }
}
