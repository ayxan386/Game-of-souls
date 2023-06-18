using System.Collections;
using UnityEngine;

namespace ChaseGreen
{
    public class CollectibleManager : MonoBehaviour
    {
        [SerializeField] private Collectible[] collectibles;
        [SerializeField] private float spawnRate;
        [SerializeField] private int maxSpawnCount;

        private Vector3[] gridCornerPositions;

        private IEnumerator Start()
        {
            yield return new WaitUntil(() => PlayerManager.PlayersReady);
            yield return new WaitForSeconds(spawnRate / 2);
            gridCornerPositions = GameManager.Instance.GetGridCornerPositions();

            while (true)
            {
                yield return new WaitForSeconds(spawnRate);
                yield return new WaitUntil(() => transform.childCount <= maxSpawnCount);

                var randomCollectible = collectibles[Random.Range(0, collectibles.Length)];

                var randomPos =
                    Vector3.Lerp(gridCornerPositions[1], gridCornerPositions[0], Random.value) // random point 
                    + gridCornerPositions[2] -
                    Vector3.Lerp(gridCornerPositions[2], gridCornerPositions[0], Random.value); // random offset
                randomPos.y += 1;
                Instantiate(randomCollectible, randomPos, Quaternion.identity, transform);
            }
        }
    }
}