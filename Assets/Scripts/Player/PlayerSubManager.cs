using System.Collections.Generic;
using R_P_S;
using RunicFloor;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSubManager : MonoBehaviour
{
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private GameObject boardPlayer;
    [SerializeField] private GameObject chaseGreenPlayer;
    [SerializeField] private ThirdPersonController player3rdPerson;
    [SerializeField] private GameObject customizationReference;
    [SerializeField] private PlayerController rpsPlayerController;
    [SerializeField] private Climb climbingPlayer;

    [Header("Color indicator")] [SerializeField]
    private MeshRenderer colorIndicator;

    [SerializeField] private Light lightIndicator;
    [SerializeField] private TextMeshPro inWorldName;
    [SerializeField] private Color[] playerColors;

    public static List<PlayerSubManager> PlayerRoots;

    public Player BoardPlayer { get; private set; }

    public ChaseGreen.PlayerController ChaseGreenPlayer { get; private set; }

    public ThirdPersonController ThirdPersonController => player3rdPerson;

    public PlayerController RpsPlayerController => rpsPlayerController;

    public Climb ClimbingPlayer => climbingPlayer;

    public int PlayerIndex { get; private set; }

    public string PlayerId { get; private set; }
    public Color ColorIndicator { get; private set; }

    private void Start()
    {
        PlayerRoots ??= new List<PlayerSubManager>();
        PlayerRoots.Add(this);
        BoardPlayer = boardPlayer.GetComponent<Player>();
        ChaseGreenPlayer = chaseGreenPlayer.GetComponent<ChaseGreen.PlayerController>();
        PlayerIndex = playerInput.playerIndex;
        PlayerId = "Player " + (playerInput.playerIndex + 1);
        ColorIndicator = playerColors[PlayerIndex];
        colorIndicator.material.color = ColorIndicator;
        lightIndicator.color = ColorIndicator;
        inWorldName.text = "P" + (PlayerIndex + 1);
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

    public void SwitchToRpsPlayer()
    {
        rpsPlayerController.gameObject.SetActive(true);
        playerInput.SwitchCurrentActionMap("R_P_S");
        boardPlayer.SetActive(false);
    }

    public void SwitchFromRpsPlayer()
    {
        playerInput.SwitchCurrentActionMap("BoardControl");
        boardPlayer.SetActive(true);
        rpsPlayerController.gameObject.SetActive(false);
    }

    public void SwitchToClimb()
    {
        climbingPlayer.gameObject.SetActive(true);
        playerInput.SwitchCurrentActionMap("Climbing");
        boardPlayer.SetActive(false);
    }

    public void SwitchFromClimb()
    {
        playerInput.SwitchCurrentActionMap("BoardControl");
        boardPlayer.SetActive(true);
        climbingPlayer.gameObject.SetActive(false);
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