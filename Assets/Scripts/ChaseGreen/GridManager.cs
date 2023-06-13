using System.Collections;
using UnityEngine;

public class GridManager : MonoBehaviour
{
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

    private MeshRenderer[,] gridMesh;
    private Vector2Int[] safeBlocks;
    private int eliminatedCount;


    private void Start()
    {
        GenerateGrid();
        StartCoroutine(GamePhase());
    }

    private IEnumerator GamePhase()
    {
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

            if (eliminatedCount == ChaseGreen_PlayerController.Players.Count)
            {
                yield break;
            }
        }
    }


    private void SelectSafeBlocks()
    {
        var safeBlockCount = Random.Range(safeBlockCountRange.x, safeBlockCountRange.y);
        safeBlocks = new Vector2Int[safeBlockCount];
        for (int i = 0; i < safeBlockCount; i++)
        {
            safeBlocks[i] = new Vector2Int(Random.Range(0, gridSize.x), Random.Range(0, gridSize.y));
        }
    }


    private void ColorGrid(float phase)
    {
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                if (IsSafeBlock(x, y))
                {
                    gridMesh[x, y].material.color = safeBlockColor;
                }
                else
                {
                    gridMesh[x, y].material.color =
                        Color.Lerp(otherBlocksColorStart, otherBlocksColorEnd, phase);
                }
            }
        }
    }

    private bool IsSafeBlock(int x, int y)
    {
        foreach (var safeBlock in safeBlocks)
        {
            if (x == safeBlock.x && y == safeBlock.y)
            {
                return true;
            }
        }

        return false;
    }

    private void GenerateGrid()
    {
        gridMesh = new MeshRenderer[gridSize.x, gridSize.y];
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
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
        foreach (var player in ChaseGreen_PlayerController.Players)
        {
            var x = Mathf.RoundToInt((player.position.x + blockSize.x * gridSize.x / 2) /
                                     (blockSize.x + blockSpacing.x));
            var y = Mathf.RoundToInt((player.position.z + blockSize.y * gridSize.y / 2) /
                                     (blockSize.y + blockSpacing.y));

            if (IsSafeBlock(x, y))
            {
                print($"Player {player.name} is safe");
            }
            else
            {
                player.gameObject.SetActive(false);
                eliminatedCount++;
            }
        }
    }
}