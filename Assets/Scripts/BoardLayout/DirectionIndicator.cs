using UnityEngine;

namespace BoardLayout
{
    public class DirectionIndicator : MonoBehaviour
    {
        [SerializeField] private Color selectedColor;
        [SerializeField] private Color defaultColor;
        [SerializeField] private MeshRenderer[] colorChangedParts;
        
        public PathTile RelatedTile { get; set; }

        public void Select()
        {
            foreach (var colorChangedPart in colorChangedParts)
            {
                colorChangedPart.material.color = selectedColor;
            }
        }

        public void UnSelect()
        {
            foreach (var colorChangedPart in colorChangedParts)
            {
                colorChangedPart.material.color = defaultColor;
            }
        }
    }
}