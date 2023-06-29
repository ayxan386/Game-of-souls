using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Shop : MonoBehaviour
{
    private int r;
    private bool openOnce;
    private ShopCheck shopCheck;
    private InventoryManager inventoryManager;
    private TextMeshProUGUI souls;
    private GameObject[] itemsInStore;
    private List<ShopItem> tempList = new List<ShopItem>();

    [Header("List of items sold")]
    [SerializeField] List<ShopItem> shopItem = new List<ShopItem>();

    [Header("References")]
    [SerializeField] private GameObject inventory;
    [SerializeField] private GameObject soulText;
    [SerializeField] private Transform shopContainer;
    [SerializeField] private Transform shopInterface;
    [SerializeField] private GameObject shopItemPrefab;

    private void Start()
    {
        shopCheck = GetComponentInChildren<ShopCheck>();
        souls = soulText.GetComponent<TextMeshProUGUI>();
        inventoryManager = inventory.GetComponent<InventoryManager>();
    }

    private void Update()
    {       
        if (shopCheck.openShop == false && openOnce == true)
        {
            openOnce = false;
            DestroyShop();
        }
    }

    public void ActiveShop()
    {
        shopInterface.gameObject.SetActive(true);

        PopulateShop();

        souls.text = "Souls: " + shopCheck.playerInfo.souls;

        openOnce = true;
    }
 
    private void PopulateShop()
    {
        for (int i = 0; i < 3; i++)
        {
            r = Random.Range(0, shopItem.Count);

            ShopItem si = shopItem[r];

            tempList.Add(si);
            
            GameObject itemObject = Instantiate(shopItemPrefab, shopContainer);

            itemObject.tag = "ShopItem";
            itemObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = si.cost.ToString();
            itemObject.transform.GetChild(1).GetComponent<Image>().sprite = si.sprite;
            itemObject.transform.GetChild(2).GetComponent<Button>().onClick.AddListener(() => Buy(si));

            shopItem.Remove(si);

            itemsInStore = GameObject.FindGameObjectsWithTag("ShopItem");
        }

        foreach(ShopItem items in tempList)
        {
            shopItem.Add(items);
        }

        tempList.Clear();
    }

    public void DestroyShop()
    {
        shopInterface.gameObject.SetActive(false);

        for (int i = 0; i < itemsInStore.Length; i++)
        {
            Destroy(itemsInStore[i]);
        }
    }

    private void Buy(ShopItem item) 
    {
        if (item.cost <= shopCheck.playerInfo.souls)
        {
            inventoryManager.ItemStore(item);           
            shopCheck.playerInfo.souls -= item.cost;
            souls.text = "Souls: " + shopCheck.playerInfo.souls;
            DestroyShop();
        }
    }
}
