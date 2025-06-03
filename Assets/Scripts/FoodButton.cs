using UnityEngine;

public class FoodObject : MonoBehaviour
{
    public FoodItem foodItem;

    private void OnMouseDown()
    {
        // Debug.Log("FoodObject clicked: " + foodItem.foodName);
        FoodManager.Instance.OrderFood(foodItem);
    }
}
