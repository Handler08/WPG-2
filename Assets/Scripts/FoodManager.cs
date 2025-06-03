using UnityEngine;

public class FoodManager : MonoBehaviour
{
    public static FoodManager Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void OrderFood(FoodItem item)
    {
        Debug.Log($"{item.foodName} ordered!");

        // TODO: Extend to queue, UI update, play sound, etc.
    }
}
