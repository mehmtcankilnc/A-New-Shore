using TMPro;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    #region Singleton
    public static SelectionManager Instance { get; set; }

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

    public GameObject interactionInfoUI;
    public bool onTarget = false;
    public GameObject selectedObject;

    private TextMeshProUGUI interactionText;

    private void Start()
    {
        interactionText = interactionInfoUI.GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            var selectionTransform = hit.transform;
            InteractableObject interactable = selectionTransform.GetComponent<InteractableObject>();

            if (interactable && interactable.playerInRange)
            {
                onTarget = true;
                selectedObject = interactable.gameObject;

                interactionText.text = interactable.GetItemName();
                interactionInfoUI.SetActive(true);
            }
            else
            {
                onTarget = false;
                interactionInfoUI.SetActive(false);
            }

        }
        else
        {
            onTarget = false;
            interactionInfoUI.SetActive(false);
        }
    }
}