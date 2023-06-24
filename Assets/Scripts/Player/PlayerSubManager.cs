using System.Collections.Generic;
using ChaseGreen;
using RunicFloor;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSubManager : MonoBehaviour
{
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private GameObject boardPlayer;
    [SerializeField] private GameObject chaseGreenPlayer;
    [SerializeField] private ThirdPersonController player3rdPerson;

    public static List<PlayerSubManager> PlayerRoots;

    public Player BoardPlayer { get; private set; }

    public PlayerController ChaseGreenPlayer { get; private set; }

    public ThirdPersonController ThirdPersonController => player3rdPerson;

    public string PlayerId { get; private set; }
    public Color ColorIndicator { get; set; }

    private void Start()
    {
        PlayerRoots ??= new List<PlayerSubManager>();
        PlayerRoots.Add(this);
        BoardPlayer = boardPlayer.GetComponent<Player>();
        ChaseGreenPlayer = chaseGreenPlayer.GetComponent<PlayerController>();
        PlayerId = "Player " + (playerInput.playerIndex + 1);
    }

    public void LoadedChaseTheGreen()
    {
        playerInput.SwitchCurrentActionMap("ChaseTheGreen");
        boardPlayer.SetActive(false);
        chaseGreenPlayer.SetActive(true);
    }

    public void UnLoadedChaseTheGreen()
    {
        playerInput.SwitchCurrentActionMap("BoardControl");
        boardPlayer.SetActive(true);
        chaseGreenPlayer.SetActive(false);
    }

    public void SwitchTo3rdPerson()
    {
        player3rdPerson.UpdateIndicator(PlayerId, ColorIndicator);
        playerInput.SwitchCurrentActionMap("3rdPerson");
        boardPlayer.SetActive(false);
        player3rdPerson.gameObject.SetActive(true);
    }

    public void SwitchFrom3rdPerson()
    {
        playerInput.SwitchCurrentActionMap("BoardControl");
        boardPlayer.SetActive(true);
        player3rdPerson.gameObject.SetActive(false);
    }


    private void Update()
    {
        if (Input.GetKey(KeyCode.E))
        {
            SwitchTo3rdPerson();
        }
        else if (Input.GetKey(KeyCode.Q))
        {
            SwitchFrom3rdPerson();
        }
    }
}