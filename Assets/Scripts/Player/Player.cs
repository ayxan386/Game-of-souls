using System;
using System.Collections;
using Cinemachine;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour
{
    [SerializeField] private float minDistance;
    [SerializeField] private float playerMovementSpeed;
    [SerializeField] private Transform tileCheckPoint;
    [SerializeField] private CinemachineVirtualCamera vCamera;
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private PlayerUIDisplay playerUiPrefab;

    private PlayerUIDisplay playerUiDisplay;

    private CharacterController cc;
    private Transform targetPoint;

    public CinemachineVirtualCamera PlayerView => vCamera;
    public int CurrentHealth { get; private set; }
    public int MaxHealth { get; private set; }
    public int SoulCount { get; private set; }
    public string DisplayName { get; private set; }

    public static event Action<string> OnPlayerPositionReached;

    private void Awake()
    {
        cc = GetComponent<CharacterController>();
    }

    private void Start()
    {
        MaxHealth = DataManager.PlayerStartingHealth;
        CurrentHealth = MaxHealth;
        SoulCount = 0;
        DisplayName = "Player " + Random.Range(1, 5);
        playerUiDisplay = Instantiate(playerUiPrefab, PlayerManager.Instance.PlayerUIParent);
        UpdateUI();
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
            PathManager.Instance.SearchForNextTile(name);
            yield return new WaitUntil(() => PathManager.Instance.IsSelected);

            var pathTile = PathManager.Instance.GetNextTileForPlayer();
            targetPoint = pathTile.GetNextPoint();
            playerAnimator.SetBool("running", true);
            do
            {
                var dir = -(transform.position - targetPoint.position);
                dir.Normalize();
                cc.Move(dir * (playerMovementSpeed * Time.deltaTime));
                dir.y = 0;
                transform.forward = Vector3.Lerp(transform.forward, dir, 0.15f);
                yield return null;
            } while (!CheckIfReached());

            playerAnimator.SetBool("running", false);
            PathManager.Instance.PlayerReachedNextTile(name);
            print("Tile reached");
        }

        OnPlayerPositionReached?.Invoke(name);
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
        playerUiDisplay.ToggleState(state);
    }
}