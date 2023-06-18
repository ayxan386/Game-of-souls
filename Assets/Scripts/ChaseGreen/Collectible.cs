using UnityEngine;

namespace ChaseGreen
{
    public class Collectible : MonoBehaviour
    {
        [SerializeField] private int awardAmount;

        public int AwardAmount => awardAmount;
    }
}