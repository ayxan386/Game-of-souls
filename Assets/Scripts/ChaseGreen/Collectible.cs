using UnityEngine;

namespace ChaseGreen
{
    public class Collectible : MonoBehaviour
    {
        [SerializeField] private int awardAmount;
        [SerializeField] private AudioClip pickUpSound;

        public int AwardAmount => awardAmount;

        public void PickedUp()
        {
            GameManager.Instance.AudioSource.PlayOneShot(pickUpSound);
        }
    }
}