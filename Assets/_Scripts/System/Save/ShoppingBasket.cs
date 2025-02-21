using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class ShoppingBasket : MonoBehaviour
{
    #region Singleton
    public static ShoppingBasket Instance {  get; set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }
    #endregion

    private QuickSlotSystem quickSlot;
    private List<string> tempItemList = new List<string>();

    [SerializeField] private List<Transform> itemPlaces = new List<Transform> ();
    [SerializeField] private List<string> itemList = new List<string>();
    [SerializeField] private List<string> orderList = new List<string>();

    public List<TextMeshProUGUI> orderTexts = new List<TextMeshProUGUI>();
    public GameObject customer { get; set; }

    private void Start()
    {
        quickSlot = QuickSlotSystem.Instance;
    }

    private void Update()
    {
        if (quickSlot.slotList[quickSlot.slotCount].transform.childCount != 0) // we have an item in our slot system
        {
            float distance = Vector2.Distance(GameObject.FindWithTag("Player").transform.position, transform.position);
            if (Input.GetKeyDown(KeyCode.Q) && distance < 4f && SelectionManager.Instance.selectedObject == gameObject) // we are close enough to basket and trying to drop item
            {
                Transform item = quickSlot.slotList[quickSlot.slotCount].transform.GetChild(0);
                if (item.gameObject.CompareTag("ShopItem")) // do item we have in the quick slot has ShopItem tag
                {
                    SoundEffectsManager.Instance.PlaySoundEffect("grabItem");
                    string itemName = item.name.Replace("(Clone)", "");
                    string modelName = $"{itemName}_Model";
                    InstantiateModels(modelName);
                    AddToCart(itemName);
                    CheckOrderCompletion();
                    quickSlot.RemoveFromquickSlot();
                }
            }
        }
    }

    public void InstantiateModels(string modelToIns) // instantiate 3d models at basket
    {
        Transform placeToIns = FindNextEmptyPlace();
        if (placeToIns != null)
        {
            GameObject item = Instantiate(Resources.Load<GameObject>(modelToIns), placeToIns);
            item.transform.SetParent(placeToIns);
        }
    }

    private Transform FindNextEmptyPlace() // find empty item places
    {
        foreach (Transform place in itemPlaces)
        {
            if (place.childCount == 0)
                return place;
        }
        return null;
    }

    private void ClearItemPlaceChilds() // after order completion clear 3d models
    {
        foreach (Transform place in itemPlaces)
        {
            if (place.childCount != 0)
            {
                DestroyImmediate(place.GetChild(0).gameObject);
            }
        }
    }

    public void AddToOrderList(string itemToAdd) // add item to orderList via NpcAI
    {
        orderList.Add(itemToAdd);
    }

    public void AddToCart(string itemToAdd) // add item to itemList to check order
    {
        tempItemList.Add(itemToAdd);
        itemList.Add(itemToAdd);

        foreach (TextMeshProUGUI text in orderTexts)
        {
            string item = text.text;
            int quantity;
            if (tempItemList.Contains(item))
            {
                Transform quantityObject = text.transform.GetChild(0);
                quantity = int.Parse(quantityObject.GetComponent<TextMeshProUGUI>().text);
                if (quantity > 1)
                {
                    quantity--;
                    quantityObject.GetComponent<TextMeshProUGUI>().text = quantity.ToString();
                }
                else
                {
                    quantity = 0;
                    text.fontStyle |= FontStyles.Strikethrough;
                    quantityObject.GetComponent<TextMeshProUGUI>().text = quantity.ToString();
                }

                tempItemList.Remove(item);
            }
        }
    }

    public void RemoveFromItemList(string itemName) // specific item deletion from basket
    {
        tempItemList.Remove(itemName);
        itemList.Remove(itemName);
    }

    public void ClearCartAndOrder() // when the order is completed
    {
        tempItemList.Clear();
        itemList.Clear();
        orderList.Clear();
    }

    public void CheckOrderCompletion() 
    {
        if (IsOrderCompleted())
        {
            Debug.Log("Sipariþ tamamlandý!");

            customer.GetComponent<NpcAI>().WanderAfterShopping();
            OrderUIManager.Instance.ResetOrder();
            OrderUIManager.Instance.IncreaseCoin(customer.GetComponent<NpcAI>().totalPrice);
            PlayerLevel.Instance.CompleteOrder(CalculateExp());
            ClearCartAndOrder();
            ClearItemPlaceChilds();
        }
        else
        {
            Debug.Log("Sipariþ tam deðil!");
        }
    }

    private int CalculateExp()
    {
        int exp = 0;
        foreach (var item in orderList)
        {
            exp++;
        }
        return exp;
    }

    private bool IsOrderCompleted()
    {
        if (itemList.Count != orderList.Count)
            return false;

        List<string> sortedList1 = itemList.OrderBy(x => x).ToList();
        List<string> sortedList2 = orderList.OrderBy(x => x).ToList();

        for (int i = 0; i < sortedList1.Count; i++)
        {
            if (sortedList1[i] != sortedList2[i])
                return false;
        }

        return true;
    }
}