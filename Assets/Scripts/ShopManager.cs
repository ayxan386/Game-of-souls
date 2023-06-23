using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ShopManager : MonoBehaviour
{
    [SerializeField] private GameObject Shop;

    public int[,] shopItems = new int[4, 4];
    public float souls;
    public TextMeshProUGUI soulsText;

    // Start is called before the first frame update
    void Start()
    {
        soulsText.text = "Souls" + souls.ToString();

        // ID
        shopItems[1, 1] = 1;
        shopItems[1, 2] = 2;
        shopItems[1, 3] = 3;

        // Price
        shopItems[2, 1] = 20;
        shopItems[2, 2] = 10;
        shopItems[2, 3] = 15;
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Shop.SetActive(true);
        }
    }

    public void Buy()
    {
        GameObject buttonRef = GameObject.FindGameObjectWithTag("Event").GetComponent<EventSystem>().currentSelectedGameObject;

        if (souls >= shopItems[2, buttonRef.GetComponent<Shop>().itemID])
        {
            souls -= shopItems[2, buttonRef.GetComponent<Shop>().itemID];
            print("ItemBuy");
            soulsText.text = "Souls: " + souls.ToString();
        }
    }
}
