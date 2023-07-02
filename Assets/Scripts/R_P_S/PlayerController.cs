using UnityEngine;
using UnityEngine.InputSystem;

namespace R_P_S
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private Vector2[] inputMapping;
        [SerializeField] private GameObject playerAvatar;
        public int LastDecision { get; private set; }

        private void OnEnable()
        {
            playerAvatar.SetActive(false);
        }

        private void OnDisable()
        {
            playerAvatar.SetActive(true);
        }

        private void OnDecision(InputValue inputValue)
        {
            var decision = inputValue.Get<Vector2>();
            for (var index = 0; index < inputMapping.Length; index++)
            {
                var mapping = inputMapping[index];
                if (decision == mapping)
                {
                    LastDecision = index;
                    break;
                }
            }
        }
    }
}