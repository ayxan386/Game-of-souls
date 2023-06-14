using System.Collections;
using System.Linq;
using UnityEngine;

public class ChaseGreen_GameManager : MonoBehaviour
{
    [Header("Loading")] [SerializeField] private GameObject loadingScreen;

    [Header("Grid generation")] [SerializeField]
    private GameObject gridBlock;

    [SerializeField] private Vector2Int gridSize;
    [SerializeField] private Vector2 blockSize;
    [SerializeField] private Vector2 blockSpacing;

    [Header("Game phase")] [SerializeField]
    private Vector2Int safeBlockCountRange;

    [SerializeField] private Color safeBlockColor;
    [SerializeField] private Color otherBlocksColorStart;
    [SerializeField] private Color otherBlocksColorEnd;
    [SerializeField] private float duration;
    [SerializeField] [Range(0, 1f)] private float colorAnimationFactor;

    [Header("Post game phase")] [SerializeField]
    private int[] soulAwards;

    [SerializeField] private float waitDuration;
    [SerializeField] private GameObject awardScreen;
    [SerializeField] private PlayerAwardUI[] playerAwardUis;

    private MeshRenderer[,] gridMesh;
    private Vector2Int[] safeBlocks;
    private int eliminatedCount;


    private void Start()
    {
        loadingScreen.SetActive(true);
        GenerateGrid();
        StartCoroutine(GamePhase());
    }

    private IEnumerator GamePhase()
    {
        yield return new WaitUntil(() => ChaseGreen_PlayerManager.PlayersReady);
        loadingScreen.SetActive(false);
        while (true)
        {
            SelectSafeBlocks();
            colorAnimationFactor = 0.1f;
            for (var t = 0f; t < duration; t += duration * colorAnimationFactor)
            {
                ColorGrid(t / duration);
                yield return new WaitForSeconds(duration * colorAnimationFactor);
            }

            CheckPlayerPositions();

            if (eliminatedCount == PlayerSubManager.PlayerRoots.Count - 1)
            {
                AwardPlayers();
                yield return new WaitForSeconds(waitDuration);
                ResetPlayers();
                MiniGameManager.Instance.MinigameFinished();
                yield break;
            }
        }
    }

    private void ResetPlayers()
    {
        foreach (var playerRoot in PlayerSubManager.PlayerRoots)
        {
            playerRoot.ChaseGreenPlayer.Eliminated = false;
            playerRoot.UnLoadedChaseTheGreen();
        }
    }

    private void AwardPlayers()
    {
        awardScreen.SetActive(true);

        foreach (var t in playerAwardUis)
        {
            t.gameObject.SetActive(false);
        }

        foreach (var playerRoot in PlayerSubManager.PlayerRoots)
        {
            var player = playerRoot.ChaseGreenPlayer;
            var soulAward = soulAwards[player.EliminationIndex];
            playerAwardUis[player.EliminationIndex].gameObject.SetActive(true);
            playerAwardUis[player.EliminationIndex]
                .UpdateUI(player.EliminationIndex + 1, playerRoot.PlayerId, soulAward);
            PlayerManager.Instance.AwardPlayerWithSouls(playerRoot.PlayerId, soulAward);
        }
    }


    private void SelectSafeBlocks()
    {
        var safeBlockCount = Random.Range(safeBlockCountRange.x, safeBlockCountRange.y);
        safeBlocks = new Vector2Int[safeBlockCount];
        for (var i = 0; i < safeBlockCount; i++)
        {
            safeBlocks[i] = new Vector2Int(Random.Range(0, gridSize.x), Random.Range(0, gridSize.y));
        }
    }


    private void ColorGrid(float phase)
    {
        for (var x = 0; x < gridSize.x; x++)
        {
            for (var y = 0; y < gridSize.y; y++)
            {
                gridMesh[x, y].material.color = IsSafeBlock(x, y)
                    ? safeBlockColor
                    : Color.Lerp(otherBlocksColorStart, otherBlocksColorEnd, phase);
            }
        }
    }

    private bool IsSafeBlock(int x, int y)
    {
        return safeBlocks.Any(safeBlock => x == safeBlock.x && y == safeBlock.y);
    }

    private void GenerateGrid()
    {
        gridMesh = new MeshRenderer[gridSize.x, gridSize.y];
        for (var x = 0; x < gridSize.x; x++)
        {
            for (var y = 0; y < gridSize.y; y++)
            {
                var position = transform.position
                               + new Vector3(blockSize.x * (x - gridSize.x / 2), 0, blockSize.y * (y - gridSize.y / 2))
                               + new Vector3(blockSpacing.x * x, 0, blockSpacing.y * y);
                var gm = Instantiate(gridBlock, position, Quaternion.identity, transform);
                gm.name = $"grid block ({x}, {y})";
                gridMesh[x, y] = gm
                    .GetComponent<MeshRenderer>();
            }
        }
    }


    private void CheckPlayerPositions()
    {
        foreach (var playerRoot in PlayerSubManager.PlayerRoots)
        {
            var player = playerRoot.ChaseGreenPlayer;
            if (player.Eliminated) continue;

            var playerPos = player.transform.position;
            var x = Mathf.RoundToInt((playerPos.x + blockSize.x * gridSize.x / 2) /
                                     (blockSize.x + blockSpacing.x));
            var y = Mathf.RoundToInt((playerPos.z + blockSize.y * gridSize.y / 2) /
                                     (blockSize.y + blockSpacing.y));

            if (IsSafeBlock(x, y))
            {
                print($"Player {player.name} is safe");
            }
            else
            {
                player.Eliminated = true;
                player.EliminationIndex = PlayerSubManager.PlayerRoots.Count - eliminatedCount - 1;
                print($"Player {player.name} eliminated {player.EliminationIndex}");
                eliminatedCount++;
                player.gameObject.SetActive(false);
            }
        }
    }
}