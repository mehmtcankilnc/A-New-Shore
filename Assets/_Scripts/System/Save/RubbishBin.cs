using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RubbishBin : MonoBehaviour
{
    public AudioSource campfireAudioSource;
    private QuickSlotSystem quickSlot;
    private Transform player;

    private void Start()
    {
        quickSlot = QuickSlotSystem.Instance;
        player = GameObject.FindWithTag("Player").transform;

        if (campfireAudioSource != null)
        {
            campfireAudioSource.loop = true;      
            campfireAudioSource.spatialBlend = 1; 
            campfireAudioSource.maxDistance = 10; 
            campfireAudioSource.rolloffMode = AudioRolloffMode.Logarithmic;

            campfireAudioSource.Play();
        }
    }

    private void Update()
    {
        if (campfireAudioSource != null && player != null)
        {
            float distance = Vector3.Distance(player.position, transform.position);

            campfireAudioSource.volume = distance < 15 ? 1 - (distance / 10) : 0;
        }

        if (quickSlot.slotList[quickSlot.slotCount].transform.childCount != 0) // we have an item in our slot system
        {
            float distance = Vector2.Distance(GameObject.FindWithTag("Player").transform.position, transform.position);
            if (Input.GetKeyDown(KeyCode.Q) && distance < 1.5f && SelectionManager.Instance.selectedObject == gameObject)
            {
                Transform item = quickSlot.slotList[quickSlot.slotCount].transform.GetChild(0);
                if (item.gameObject.CompareTag("ShopItem"))
                {
                    SoundEffectsManager.Instance.PlaySoundEffect("itemBurn");
                    Debug.Log("Item חצpe gitti." + item.name);
                    quickSlot.RemoveFromquickSlot();
                }
            }
        }
    }
}