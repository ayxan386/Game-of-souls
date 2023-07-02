using UnityEngine;
using UnityEngine.UI;

public class AutoSelectable : MonoBehaviour
{
    [SerializeField] private Selectable selectableTarget;

    private void OnEnable()
    {
        selectableTarget.Select();
    }
}