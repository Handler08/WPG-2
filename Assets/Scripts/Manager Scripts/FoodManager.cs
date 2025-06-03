using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class FoodManager : MonoBehaviour
{
    public static FoodManager Instance { get; private set; }

    private List<FoodItem> selectedFoods = new List<FoodItem>();
    public CustomerBehaviour currentCustomer;
    public bool canSelectFood = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void OnFoodSelected(FoodItem food)
    {
        if (!canSelectFood)
        {
            Debug.Log("[FoodManager] Cannot select food yet. No active order.");
            return;
        }

        if (selectedFoods.Count >= 4)
        {
            Debug.Log("[FoodManager] Already selected 4 foods. Resetting...");
            selectedFoods.Clear();
        }

        selectedFoods.Add(food);
        Debug.Log($"[FoodManager] Selected: {food.foodName}");

        if (selectedFoods.Count == 4)
        {
            CheckOrderMatch();
        }
    }

    private void CheckOrderMatch()
    {
        OrderData currentOrder = OrderManager.Instance.GetCurrentOrder(); // ← Add this method if needed

        if (currentOrder == null)
        {
            Debug.LogWarning("[FoodManager] No current order to compare with.");
            return;
        }

        var selectedSet = new HashSet<FoodItem>(selectedFoods);
        var orderSet = new HashSet<FoodItem>(currentOrder.orderedFoods);

        if (selectedSet.SetEquals(orderSet))
        {
            Debug.Log("✅ Order MATCHES! Customer is happy!");
            currentCustomer?.NotifyOrderServed();
            canSelectFood = false; // Prevent further selection
        }
        else
        {
            Debug.Log("❌ Order does NOT match.");
            currentCustomer?.NotifyOrderServed();
        }

        selectedFoods.Clear();
    }
}
