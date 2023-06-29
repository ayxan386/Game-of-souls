using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryManager : MonoBehaviour
{
    private int[] numberOfItems = { 0, 0, 0, 0, 0 };
    public GameObject[] inventoryItems;

    public void ItemStore(ShopItem item)
    {
        for (int i = 0; i < inventoryItems.Length; i++)
        {
            if (item.ID == i)
            {
                var image = inventoryItems[i].transform.GetChild(0).GetComponent<Image>();
                var textMesh = inventoryItems[i].GetComponentInChildren<TextMeshProUGUI>();

                for (int n = 0; n < numberOfItems.Length; n++)
                {
                    if (item.ID == n)
                    {
                        numberOfItems[n] += 1;
                        textMesh.text = numberOfItems[n].ToString();
                    }
                }
                
                var tempColor = image.color;
                tempColor.a = 1f;
                image.color = tempColor;
            }
        }                  
    }
}
