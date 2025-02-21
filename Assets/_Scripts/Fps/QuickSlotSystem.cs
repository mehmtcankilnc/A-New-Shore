using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuickSlotSystem : MonoBehaviour
{
    #region Singleton
    public static QuickSlotSystem Instance { get; set; }
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

    public GameObject quickSlotUI;
    public Sprite standartSlotImg;
    public Sprite activeSlotImg;
    public GameObject toolHolder;

    public List<GameObject> slotList = new List<GameObject>();
    public List<string> itemList = new List<string>();
    public int slotCount = 0;

    private int slotListLength = 0;
    private bool isEquipped = false;

    private GameObject itemToAdd;
    private GameObject itemModel;

    void Start()
    {
        GatherSlots();
        slotList[0].GetComponent<Image>().sprite = activeSlotImg;
        if (slotList[slotCount].transform.childCount != 0)
            SetEquippedModel(slotList[slotCount].transform.GetChild(0).name.Replace("(Clone)", ""));
    }

    private void GatherSlots()
    {
        foreach (Transform child in quickSlotUI.transform)
        {
            if (child.CompareTag("Slot"))
            {
                slotListLength++;
                slotList.Add(child.gameObject);
            }
        }
    }

    void Update()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll > 0)
        {
            UnSetEquippedModel();
            slotList[slotCount].GetComponent<Image>().sprite = standartSlotImg;

            if (slotCount > 0)
                slotCount--;
            else
                slotCount = slotListLength - 1;

            slotList[slotCount].GetComponent<Image>().sprite = activeSlotImg;

            if (slotList[slotCount].transform.childCount != 0 && slotList[slotCount].transform.GetChild(0).CompareTag("Equipable"))
                SetEquippedModel(slotList[slotCount].transform.GetChild(0).name.Replace("(Clone)", ""));
        }
        else if (scroll < 0)
        {
            UnSetEquippedModel();
            slotList[slotCount].GetComponent<Image>().sprite = standartSlotImg;

            if (slotCount < slotListLength - 1)
                slotCount++;
            else
                slotCount = 0;

            slotList[slotCount].GetComponent<Image>().sprite = activeSlotImg;
            if (slotList[slotCount].transform.childCount != 0 && slotList[slotCount].transform.GetChild(0).CompareTag("Equipable"))
                SetEquippedModel(slotList[slotCount].transform.GetChild(0).name.Replace("(Clone)", ""));
        }
    }

    public void SetEquippedModel(string itemName)
    {
        if (itemName != null && !isEquipped)
        {
            itemModel = Instantiate(Resources.Load<GameObject>(itemName + "_Model"), new Vector3(0.35f, 0.85f, 1.2f), Quaternion.Euler(-102f, -15f, 90f));

            itemModel.transform.SetParent(toolHolder.transform, false);
            isEquipped = true;
        }
    }

    public void UnSetEquippedModel()
    {
        if (itemModel != null)
        {
            DestroyImmediate(itemModel.gameObject);
            itemModel = null;
            isEquipped = false;
        }
    }

    public void AddToQuickSlot(string itemName)
    {
        string cleanName;
        cleanName = itemName.Replace("(Clone)", "");

        if (slotList[slotCount].transform.childCount != 0)
        {
            GameObject availableSlot = FindNextEmptySlot();

            if (availableSlot != null)
            {
                itemToAdd = Instantiate(Resources.Load<GameObject>(itemName), availableSlot.transform.position, availableSlot.transform.rotation);
                itemToAdd.transform.SetParent(availableSlot.transform);
                
                itemList.Add(cleanName);
            }
        }
        else
        {
            itemToAdd = Instantiate(Resources.Load<GameObject>(itemName), slotList[slotCount].transform.position, slotList[slotCount].transform.rotation);
            itemToAdd.transform.SetParent(slotList[slotCount].transform);

            itemList.Add(cleanName);
        }
    }

    public void RemoveFromquickSlot()
    {
        if (slotList[slotCount].transform.childCount != 0)
        {
            DestroyImmediate(slotList[slotCount].transform.GetChild(0).gameObject);
        }
    }

    private GameObject FindNextEmptySlot()
    {
        foreach (GameObject slot in slotList)
        {
            if (slot.transform.childCount == 0)
                return slot;
        }
        return null;
    }

    public bool CheckIfFull()
    {
        int counter = 0;

        foreach (GameObject slot in slotList)
        {
            if (slot.transform.childCount > 0)
                counter++;
        }
        if (counter == 5)
            return true;
        else
            return false;
    }
}
