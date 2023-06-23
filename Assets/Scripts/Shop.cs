using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Shop : MonoBehaviour
{
    public int itemID;
    public TextMeshProUGUI PriceText;
    public GameObject shopManager;
    
    private void Update()
    {
        PriceText.text = "Price: " + shopManager.GetComponent<ShopManager>().shopItems[2, itemID].ToString();
    }
}
