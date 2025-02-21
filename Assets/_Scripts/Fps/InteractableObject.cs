using System;
using System.Collections;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    public string ItemName;
    public bool playerInRange;

    public string GetItemName()
    {
        return ItemName;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && playerInRange && SelectionManager.Instance.onTarget && SelectionManager.Instance.selectedObject == gameObject)
        {
            if (gameObject.CompareTag("Equipable"))
            {
                HandleEquipSystem();
            }
            else if (gameObject.CompareTag("ShopItem"))
            {
                HandleShopSystem();
            }
            else if (gameObject.CompareTag("TableItem"))
            {
                HandlePickUpFromTable();
            }
            else if (gameObject.CompareTag("Door"))
            {
                StartCoroutine(HandleOpeningDoor(gameObject));
            }
            else if (gameObject.CompareTag("Bed"))
            {
                HandleSleeping();
            }
            else if (gameObject.CompareTag("TalkingNpc") || gameObject.CompareTag("Head-Man"))
            {
                HandleNpcQuest(gameObject);
            }
            else if (gameObject.CompareTag("Elevator"))
            {
                HandleElevatorSystem();
            }
        }
    }

    private void HandleElevatorSystem()
    {
        SoundEffectsManager.Instance.PlaySoundEffect("leverPull");
        ElevatorSystem.Instance.StartElevator();
    }

    private void HandleNpcQuest(GameObject gameObject)
    {
        gameObject.GetComponent<TalkingNpc>().InteractWithNpc();

        if (DialogSystem.Instance.dialogUIActive)
        {
            SelectionManager.Instance.interactionInfoUI.SetActive(false);
        }
    }

    private void HandleSleeping()
    {
        DayNightSystem dayNightSystem = DayNightSystem.Instance;
        GameObject[] npcs = GameObject.FindGameObjectsWithTag("Customer");

        bool isAnyCustomerWaiting = false;

        foreach (GameObject npc in npcs)
        {
            if (npc.GetComponent<NpcAI>().isInQueue == true)
            {
                isAnyCustomerWaiting = true;
                Debug.Log("You can't sleep if any customer is in queue!");
            }
        }

        if (dayNightSystem.currentHour == 22 && !isAnyCustomerWaiting)
        {
            SoundEffectsManager.Instance.PlaySoundEffect("yawn");
            dayNightSystem.currentTimeOfDay = 0.35f;
            dayNightSystem.dayCompleted = false;
            TimeManager.Instance.TriggerNextDay();

            foreach (GameObject npc in npcs)
            {
                npc.GetComponent<NpcAI>().Initialize();
            }
        }

        if (dayNightSystem.currentHour != 22)
            Debug.Log("You can't sleep at daytime!");
    }

    private IEnumerator HandleOpeningDoor(GameObject gameObject)
    {
        Animator animator = gameObject.GetComponent<Animator>();
        animator.SetTrigger("Open");
        SoundEffectsManager.Instance.PlaySoundEffect("doorOpen");

        yield return new WaitForSeconds(3f);
        animator.SetTrigger("Close");
        SoundEffectsManager.Instance.PlaySoundEffect("doorOpen");
    }

    private void HandlePickUpFromTable()
    {
        if (!QuickSlotSystem.Instance.CheckIfFull())
        {
            QuickSlotSystem.Instance.AddToQuickSlot(ItemName);
            SoundEffectsManager.Instance.PlaySoundEffect("grabItem");
            DestroyImmediate(gameObject);
            ShoppingBasket.Instance.RemoveFromItemList(ItemName);
            ShoppingBasket.Instance.CheckOrderCompletion();
        }
    }

    private void HandleShopSystem()
    {
        if (!QuickSlotSystem.Instance.CheckIfFull())
        {
            QuickSlotSystem.Instance.AddToQuickSlot(ItemName);
            SoundEffectsManager.Instance.PlaySoundEffect("grabItem");
        }
        else
        {
            Debug.Log("Slot is Full");
        }
    }

    private void HandleEquipSystem()
    {
        if (!QuickSlotSystem.Instance.CheckIfFull())
        {
            QuickSlotSystem.Instance.AddToQuickSlot(ItemName);
            QuickSlotSystem.Instance.SetEquippedModel(ItemName);
            SoundEffectsManager.Instance.PlaySoundEffect("grabItem");
            Destroy(gameObject);
        }
        else
        {
            Debug.Log("Slot is Full");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}