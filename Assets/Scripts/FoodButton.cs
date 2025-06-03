using UnityEngine;

public class FoodButton : MonoBehaviour
{
    public FoodItem foodData;

    private void OnMouseDown()
    {
        if (foodData != null)
        {
            FoodManager.Instance.OnFoodSelected(foodData);
        }
    }
}
