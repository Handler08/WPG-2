using UnityEngine;
using UnityEngine.AI;

public class CustomerBehaviour : MonoBehaviour
{
    private enum State { WalkingToBuy, Waiting, WalkingAway }
    private State currentState;

    private NavMeshAgent agent;
    private Transform pointStart, pointBuy, pointEnd;
    public Transform StandPoint;

    private bool orderServed = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        pointStart = GameObject.FindGameObjectWithTag("Point Start").transform;
        pointBuy = GameObject.FindGameObjectWithTag("Point Buy").transform;
        pointEnd = GameObject.FindGameObjectWithTag("Point End").transform;

        transform.position = pointStart.position;
        GoToBuy();
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
                }
                break;

            case State.Waiting:
                FaceTarget(StandPoint.position);
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
                }
                break;
        }
    }

    void GoToBuy()
    {
        currentState = State.WalkingToBuy;
        agent.SetDestination(pointBuy.position);
        agent.isStopped = false;
        Debug.Log("[Customer] Walking to buy point...");
    }

    void CallOrder()
    {
        OrderData order = OrderManager.Instance.AssignRandomOrder();
        Debug.Log($"[Customer] Says: {order.customerDialog}");

        FoodManager.Instance.canSelectFood = true; // Enable food selection
        orderServed = false;
    }


    void GoToEnd()
    {
        currentState = State.WalkingAway;
        agent.SetDestination(pointEnd.position);
        agent.isStopped = false;
        Debug.Log("[Customer] Walking away...");
        FoodManager.Instance.canSelectFood = false;
    }

    void RestartCycle()
    {
        transform.position = pointStart.position;
        GoToBuy();
    }

    // Call this from FoodManager when order is successfully served
    public void NotifyOrderServed()
    {
        orderServed = true;
    }
    
    void FaceTarget(Vector3 targetPos)
    {
        Vector3 direction = (targetPos - transform.position).normalized;
        direction.y = 0f; // Prevent tilting up/down

        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f); // Smooth rotate
        }
    }

}
