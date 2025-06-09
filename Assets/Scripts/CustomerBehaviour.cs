using UnityEngine;
using UnityEngine.AI;
using TMPro;

public class CustomerBehaviour : MonoBehaviour
{
    private enum State { WalkingToBuy, Waiting, WalkingAway }
    private State currentState;

    private NavMeshAgent agent;
    private Transform pointStart, pointBuy, pointEnd;
    public Transform standPoint;

    [SerializeField] private TextMeshProUGUI dialogText;
    [SerializeField] private GameObject dialogBubble; // Add this for the image bubble parent

    private bool orderServed = false;
    private bool isFacingTarget = false;
    private bool hasShownDialog = false;
    private string pendingDialogText = "";

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        pointStart = GameObject.FindGameObjectWithTag("Point Start").transform;
        pointBuy = GameObject.FindGameObjectWithTag("Point Buy").transform;
        pointEnd = GameObject.FindGameObjectWithTag("Point End").transform;

        transform.position = pointStart.position;
        GoToBuy();
        HideDialog();
        Debug.Log("[CustomerBehaviour] Initialized at Start point");
    }

    void Update()
    {
        switch (currentState)
        {
            case State.WalkingToBuy:
                if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
                {
                    currentState = State.Waiting;
                    agent.isStopped = true;
                    CallOrder();
                    Debug.Log("[CustomerBehaviour] Reached buy point, entering Waiting state");
                }
                break;

            case State.Waiting:
                FaceTarget(standPoint.position);
                if (orderServed)
                {
                    GoToEnd();
                }
                break;

            case State.WalkingAway:
                if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
                {
                    // Restart loop
                    RestartCycle();
                    Debug.Log("[CustomerBehaviour] Reached end point, restarting cycle");
                }
                break;
        }
    }

    void GoToBuy()
    {
        currentState = State.WalkingToBuy;
        agent.SetDestination(pointBuy.position);
        agent.isStopped = false;
        HideDialog();
        Debug.Log("[CustomerBehaviour] Walking to buy point...");
    }

    void CallOrder()
    {
        OrderData order = OrderManager.Instance.AssignRandomOrder();
        pendingDialogText = order.customerDialog;
        Debug.Log($"[CustomerBehaviour] Order assigned, dialog text: {pendingDialogText}");
        HideDialog(); // Ensure dialog is hidden until CameraManager shows it
        isFacingTarget = false;
        hasShownDialog = false;
        FoodManager.Instance.canSelectFood = true;
        orderServed = false;
    }

    void GoToEnd()
    {
        HideDialog();
        currentState = State.WalkingAway;
        agent.SetDestination(pointEnd.position);
        agent.isStopped = false;
        Debug.Log("[CustomerBehaviour] Walking away...");
    }

    void RestartCycle()
    {
        transform.position = pointStart.position;
        GoToBuy();
    }

    public void NotifyOrderServed()
    {
        orderServed = true;
        Debug.Log("[CustomerBehaviour] Order served, will walk away");
    }

    void FaceTarget(Vector3 targetPos)
    {
        Vector3 direction = (targetPos - transform.position).normalized;
        direction.y = 0f;

        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);

            float angle = Quaternion.Angle(transform.rotation, lookRotation);
            if (angle < 5f)
            {
                isFacingTarget = true;
            }
        }
    }

    public void ShowDialog(string text)
    {
        if (dialogText != null)
        {
            dialogText.text = text;
            dialogText.gameObject.SetActive(true);
        }
        if (dialogBubble != null)
        {
            dialogBubble.SetActive(true);
        }
        hasShownDialog = true;
        Debug.Log("[CustomerBehaviour] Showing dialog: " + text);
    }

    public void HideDialog()
    {
        if (dialogText != null)
        {
            dialogText.gameObject.SetActive(false);
        }
        if (dialogBubble != null)
        {
            dialogBubble.SetActive(false);
        }
        hasShownDialog = false;
        Debug.Log("[CustomerBehaviour] Hiding dialog");
    }

    public string GetCurrentDialog()
    {
        return pendingDialogText;
    }

    public bool IsWaiting()
    {
        return currentState == State.Waiting;
    }

    public bool IsDialogShown()
    {
        return hasShownDialog;
    }
}