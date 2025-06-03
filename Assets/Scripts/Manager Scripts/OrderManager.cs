using UnityEngine;
using System.Collections.Generic;

public class OrderManager : MonoBehaviour
{
    public static OrderManager Instance { get; private set; }

    public OrderData[] predefinedOrders;

    private List<int> unpickedIndices = new List<int>();
    private OrderData currentOrder;

    public OrderData GetCurrentOrder()
    {
        return currentOrder;
    }

    public OrderData AssignRandomOrder()
    {
        currentOrder = GetRandomOrder();
        return currentOrder;
    }

    private void Awake()
    {
        // Singleton setup
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("Multiple OrderManager instances detected. Destroying duplicate.");
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        ResetUnpickedIndices();
    }

    private void ResetUnpickedIndices()
    {
        unpickedIndices.Clear();
        for (int i = 0; i < predefinedOrders.Length; i++)
        {
            unpickedIndices.Add(i);
        }

        Debug.Log("[OrderManager] Order list has been reset.");
    }

    private OrderData GetRandomOrder()
    {
        if (predefinedOrders.Length == 0)
        {
            Debug.LogError("No predefined orders assigned in OrderManager!");
            return null;
        }

        if (unpickedIndices.Count == 0)
        {
            ResetUnpickedIndices();
        }

        int randomListIndex = Random.Range(0, unpickedIndices.Count);
        int orderIndex = unpickedIndices[randomListIndex];
        unpickedIndices.RemoveAt(randomListIndex);

        OrderData selectedOrder = predefinedOrders[orderIndex];

        Debug.Log($"[OrderManager] Selected Order #{orderIndex}: \"{selectedOrder.customerDialog}\"");
        for (int i = 0; i < selectedOrder.orderedFoods.Length; i++)
        {
            Debug.Log($"  Food {i + 1}: {selectedOrder.orderedFoods[i].foodName}");
        }

        return selectedOrder;
    }
}
