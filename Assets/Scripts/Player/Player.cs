using System;
using System.Collections;
using Cinemachine;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour
{
    [SerializeField] private float minDistance;
    [SerializeField] private float closeEnoughDistance;
    [SerializeField] private float playerMovementSpeed;
    [SerializeField] private Transform tileCheckPoint;
    [SerializeField] private CinemachineVirtualCamera vCamera;
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private PlayerUIDisplay playerUiPrefab;
    [SerializeField] private CharacterController cc;

    [SerializeField] private TextMeshPro playerInWorldName;

    private PlayerUIDisplay playerUiDisplay;

    private Transform targetPoint;
    private bool currentState;
    [SerializeField] private Vector3 gravity;

    public CinemachineVirtualCamera PlayerView => vCamera;
    public int CurrentHealth { get; private set; }
    public int MaxHealth { get; private set; }
    public int SoulCount { get; private set; }
    public string DisplayName { get; set; }
    public PathTile Position { get; set; }
    public PathTile PrevPosition { get; set; }

    public static event Action<Player> OnPlayerPositionReached;
    public static event Action<int> OnPlayerChoiceChanged;
    public static event Action<int> OnPlayerTileSelected;


    private void Start()
    {
        MaxHealth = DataManager.PlayerStartingHealth;
        CurrentHealth = MaxHealth;
        SoulCount = 0;
        playerUiDisplay = Instantiate(playerUiPrefab, PlayerManager.Instance.PlayerUIParent);
    }

    private void UpdateUI()
    {
        playerUiDisplay.UpdateUI(this);
        playerInWorldName.text = "P" + DisplayName.Split(" ")[1];
    }

    public void MoveToTile(int roll)
    {
        StartCoroutine(MoveCurrentPlayerBy(roll));
    }

    private IEnumerator MoveCurrentPlayerBy(int diceRoll)
    {
        for (int i = 0; i < diceRoll; i++)
        {
            PathManager.Instance.SearchForNextTile(this);
            yield return new WaitUntil(() => PathManager.Instance.IsSelected);

            var pathTile = PathManager.Instance.GetNextTileForPlayer();
            targetPoint = pathTile.GetNextPoint();
            playerAnimator.SetBool("running", true);
            do
            {
                var dir = -(transform.position - targetPoint.position);
                dir.Normalize();
                cc.Move(dir * (playerMovementSpeed * Time.deltaTime) + gravity * Time.deltaTime);
                dir.y = 0;
                transform.forward = Vector3.Lerp(transform.forward, dir, 0.15f);
                if (!pathTile.HasChoices(this) && CloseToCurrentTarget() && i + 1 < diceRoll)
                {
                    break;
                }

                yield return null;
            } while (!CheckIfReached());

            PathManager.Instance.PlayerReachedNextTile(this);
            if (pathTile.HasChoices(this))
            {
                playerAnimator.SetBool("running", false);
            }

            print("Tile reached");
        }

        playerAnimator.SetBool("running", false);
        OnPlayerPositionReached?.Invoke(this);
    }

    private bool CloseToCurrentTarget()
    {
        return Vector3.Distance(tileCheckPoint.position, targetPoint.position) <= closeEnoughDistance;
    }

    private bool CheckIfReached()
    {
        return Vector3.Distance(tileCheckPoint.position, targetPoint.position) <= minDistance;
    }


    private void OnDrawGizmos()
    {
        if (targetPoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(targetPoint.position, 0.3f);
        }
    }

    public void UpdateStateOfPlayer(bool state)
    {
        if (playerUiDisplay)
        {
            currentState = state;
            playerUiDisplay.ToggleState(state);
        }
        else
        {
            StartCoroutine(TryToUpdateState(state));
        }
    }

    private IEnumerator TryToUpdateState(bool state)
    {
        yield return new WaitForSeconds(0.5f);
        UpdateStateOfPlayer(state);
    }

    public void UpdateSoulCount(int value)
    {
        SoulCount += value;
        UpdateUI();
    }

    public void UpdateHealth(int value)
    {
        CurrentHealth += value;
        CurrentHealth = Mathf.Clamp(CurrentHealth, 0, MaxHealth);
        UpdateUI();
    }

    private void OnDiceRoll()
    {
        if (!PlayerManager.Instance.GameStarted) return;
        print($"Dice roll input: {currentState}");
        if (currentState)
        {
            DiceRotationManager.Instance.RollDice();
        }
    }

    private void OnTileSelectionChanged(InputValue inputValue)
    {
        if (currentState)
        {
            OnPlayerChoiceChanged?.Invoke((int)inputValue.Get<float>());
        }
    }

    private void OnTileSelected()
    {
        if (currentState)
        {
            OnPlayerTileSelected?.Invoke(0);
        }
    }

    public void TeleportToPosition(Vector3 pos)
    {
        cc.enabled = false;
        transform.position = pos + (transform.position - tileCheckPoint.position);
        cc.enabled = true;
    }
}