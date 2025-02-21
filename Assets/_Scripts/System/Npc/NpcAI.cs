using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class NpcAI : MonoBehaviour
{
    public NpcData npcData;
    public ProductList productList;
    public int totalPrice = 0;
    public bool isInQueue = false;

    private NavMeshAgent agent;
    private Animator animator;
    private bool isWaiting = false;
    private bool orderGenerated = false;
    private bool isWandering;
    private Queue<GameObject> npcQueue = new Queue<GameObject>();
    private ShoppingBasket shoppingBasket;

    [SerializeField] private Transform[] queuePositions;
    [SerializeField] private List<string> orderNameList = new List<string>();
    [SerializeField] private List<int> orderQuantityList = new List<int>();

    void Start()
    {
        Initialize();
    }

    void Update()
    {
        HandleMovementAnimations();

        if (isInQueue && !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            isWaiting = true;
            ProcessQueue();
        }

        if (isWaiting && Input.GetKeyDown(KeyCode.E))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 20.0f) && hit.transform == transform)
            {
                SoundEffectsManager.Instance.PlaySoundEffect("takeOrder");
                if (!orderGenerated)
                    GenerateOrder();
                else
                    ShowOrderList();
            }
        }
    }

    public void Initialize()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        shoppingBasket = ShoppingBasket.Instance;

        agent.speed = npcData.moveSpeed;

        if (npcData.animatorController != null)
        {
            animator.runtimeAnimatorController = npcData.animatorController;
        }

        StartCoroutine(StateRoutine());
    }

    private void HandleMovementAnimations()
    {
        bool isWalking = agent.velocity.magnitude > 0.01f;
        if (animator.GetBool("isWalking") != isWalking)
        {
            animator.SetBool("isWalking", isWalking);
        }
    }

    private IEnumerator StateRoutine()
    {
        while (isDaytime() && !isInQueue)
        {
            float choice = Random.Range(0f, 1f);
            if (choice <= 0.2f)
            {
                Shopping();
                break;
            }
            else
            {
                Wandering();
            }

            while (agent.pathPending || agent.remainingDistance > 0.1f)
            {
                yield return null;
            }

            float restTime = Random.Range(5f, npcData.wanderTimer);
            yield return new WaitForSeconds(restTime);
        }
    }

    private void Wandering()
    {
        if (!isWandering)
        {
            isWandering = true;
            Vector3 newPos = RandomNavSphere(transform.position, npcData.wanderRadius, -1);
            agent.SetDestination(newPos);
            StartCoroutine(StopWanderingAfterTime(Random.Range(5f, npcData.wanderTimer)));
        }
    }

    private IEnumerator StopWanderingAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        isWandering = false;
    }

    private void Shopping()
    {
        for (int i = 0; i < queuePositions.Length; i++)
        {
            if (!queuePositions[i].GetComponent<WaitingPoint>().isOccupied && !isInQueue)
            {
                npcQueue.Enqueue(gameObject);
                agent.SetDestination(queuePositions[i].position);
                Debug.Log($"{gameObject.name} moving to position {queuePositions[i]}");
                isInQueue = true;
                queuePositions[i].GetComponent<WaitingPoint>().isOccupied = true;
                return;
            }
        }
    }

    private void ProcessQueue()
    {
        if (npcQueue.Count > 0)
        {
            GameObject firstNPC = npcQueue.Peek();
            if (firstNPC == gameObject && agent.velocity.magnitude < 0.1f)
            {
                transform.rotation = Quaternion.Euler(transform.rotation.x, -55f, transform.rotation.z);

                //if (Sepetteki ürünler doluysa)
                //{
                //    npcQueue.Dequeue();
                //    isWaiting = false;
                //    isInQueue = false;
                //    StartCoroutine(StateRoutine());

                //    for (int i = 0; i < npcQueue.Count; i++)
                //    {
                //        GameObject npc = npcQueue.ToArray()[i];
                //        npc.GetComponent<NavMeshAgent>().SetDestination(queuePositions[i].position);
                //    }
                //}
            }
        }
    }

    private void GenerateOrder()
    {
        if (productList == null || productList.products.Count == 0)
        {
            Debug.Log("Ürün listesi boþ!");
            return;
        }

        shoppingBasket.customer = gameObject;

        foreach (TextMeshProUGUI text in shoppingBasket.orderTexts)
        {
            text.fontStyle &= ~FontStyles.Strikethrough;
        }

        List<Product> shuffledProducts = new List<Product>(productList.products);
        for (int i = shuffledProducts.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            Product temp = shuffledProducts[i];
            shuffledProducts[i] = shuffledProducts[j];
            shuffledProducts[j] = temp;
        }

        int randomItemCount = Random.Range(1, 6);
        totalPrice = 0;

        for (int i = 0; i < randomItemCount; i++)
        {
            Product selectedProduct = shuffledProducts[i];
            int randomQuantity = Random.Range(1, selectedProduct.maxQuantity + 1);

            for (int j = 0; j < randomQuantity; j++)
            {
                shoppingBasket.AddToOrderList(selectedProduct.name.Replace("(Clone)", ""));
            }

            orderNameList.Add(selectedProduct.name/* + " - " + randomQuantity + " pieces " + selectedProduct.price * randomQuantity + " $ "*/);
            orderQuantityList.Add(randomQuantity);
            totalPrice += selectedProduct.price * randomQuantity;
        }

        orderGenerated = true;
        ShowOrderList();
    }

    private void ShowOrderList()
    {
        if (orderNameList == null || orderNameList.Count == 0 || orderQuantityList == null || orderQuantityList.Count == 0)
        {
            Debug.Log("Sipariþ boþ!");
            return;
        }

        OrderUIManager.Instance.ActivateOrderUI(orderNameList, orderQuantityList, totalPrice);
    }

    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randomDirection = Random.insideUnitSphere * dist;
        randomDirection += origin;

        NavMeshHit navHit;
        NavMesh.SamplePosition(randomDirection, out navHit, dist, layermask);

        return navHit.position;
    }

    public void WanderAfterShopping()
    {
        SoundEffectsManager.Instance.PlaySoundEffect("completeOrder");
        isWaiting = false;
        isInQueue = false;
        orderNameList.Clear();
        orderQuantityList.Clear();
        orderGenerated = false;

        Vector3 newPos = RandomNavSphere(transform.position, npcData.wanderRadius, -1);
        agent.SetDestination(newPos);

        StartCoroutine(AfterShoppingRoutine());
    }

    private IEnumerator AfterShoppingRoutine()
    {
        float wanderTime = Random.Range(5f, npcData.wanderTimer);
        yield return new WaitForSeconds(wanderTime);

        StartCoroutine(StateRoutine());
    }

    private bool isDaytime()
    {
        if (DayNightSystem.Instance.currentTimeOfDay > 0.34f && DayNightSystem.Instance.currentTimeOfDay < 0.95)
            return true;
        return false;
    }
}