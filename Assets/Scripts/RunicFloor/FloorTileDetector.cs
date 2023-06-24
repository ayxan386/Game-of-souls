using System;
using UnityEngine;

namespace RunicFloor
{
    public class FloorTileDetector : MonoBehaviour
    {
        [SerializeField] private float radius;
        [SerializeField] private LayerMask detectionLayer;

        void FixedUpdate()
        {
            var detectedColliders = Physics.OverlapSphere(transform.position, radius, detectionLayer);
            foreach (var detectedCollider in detectedColliders)
            {
                if (detectedCollider.TryGetComponent(out FloorTile floorTile))
                {
                    floorTile.StartCounter();
                }
            }
        }


        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(transform.position, radius);
        }
    }
}