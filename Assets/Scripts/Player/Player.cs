using System;
using System.Collections;
using Cinemachine;
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
    [SerializeField] private Vector3 gravity;
    [SerializeField] private Transform arrowBasePoint;
    [Header("Soul drop")] [SerializeField] private float dropFraction;
    [SerializeField] private SoulDropCollectible dropPrefab;
    [Header("Custom")] [SerializeField] private GameObject customizationMenu;

    private PlayerUIDisplay playerUiDisplay;

    private Transform targetPoint;
    private bool isCustomized;
    private bool currentState;

    public Player(string displayName)
    {
        DisplayName = displayName;
    }

    public CinemachineVirtualCamera PlayerView => vCamera;
    public int CurrentHealth { get; private set; }
    public int MaxHealth { get; private set; }
    public int SoulCount { get; private set; }
    public string DisplayName { get; set; }
    public PathTile Position { get; set; }
    public PathTile PrevPosition { get; set; }
    public Transform ArrowBasePoint => arrowBasePoint;
    public Transform FootPoint => tileCheckPoint;

    public bool IsCustomized => isCustomized;

    public static event Action<Player> OnPlayerPositionReached;
    public static event Action<int> OnPlayerChoiceChanged;
    public static event Action<int> OnPlayerTileSelected;

    private void OnEnable()
    {
        playerAnimator.transform.SetParent(transform, false);
        playerAnimator.SetBool("running", false);
    }

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
                transform.forward = Vector3.Lerp(transform.forward, dir, 0.09f);
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
        if (!isCustomized)
        {
            StartCoroutine(CustomizePlayer(state));
            return;
        }

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

    private IEnumerator CustomizePlayer(bool state)
    {
        customizationMenu.SetActive(true);
        yield return new WaitUntil(() => isCustomized);
        customizationMenu.SetActive(false);
        UpdateStateOfPlayer(state);
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

        if (Mathf.Approximately(CurrentHealth, 0))
        {
            var dropCollectible = Instantiate(dropPrefab);
            dropCollectible.SoulAmount = Mathf.RoundToInt(SoulCount * dropFraction);
            SoulCount -= dropCollectible.SoulAmount;
            dropCollectible.SetPosition(Position);
            PlayerManager.Instance.SetPlayerToStartingPosition(this);
        }

        UpdateUI();
    }

    public void ResetPlayerHealth()
    {
        CurrentHealth = MaxHealth;
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
        var value = Mathf.RoundToInt(inputValue.Get<float>());
        if (currentState && value != 0)
        {
            OnPlayerChoiceChanged?.Invoke(value);
        }
    }

    private void OnTileSelected()
    {
        if (currentState)
        {
            OnPlayerTileSelected?.Invoke(0);
        }
    }

    public void TeleportToTile(PathTile tile)
    {
        TeleportToPosition(tile.GetNextPoint().position);
        Position = tile;
        PrevPosition = tile;
        Invoke(nameof(DelayedReachTile), 1.5f);
    }

    public void TeleportToPosition(Vector3 pos)
    {
        cc.enabled = false;
        transform.position = pos + (transform.position - tileCheckPoint.position);
        transform.rotation = Quaternion.identity;
        cc.enabled = true;
    }

    private void DelayedReachTile()
    {
        OnPlayerPositionReached?.Invoke(this);
    }

    public void CompleteCustomization()
    {
        if (PlayerManager.Instance != null)
            PlayerManager.Instance.NextSelectable();
        isCustomized = true;
    }
}