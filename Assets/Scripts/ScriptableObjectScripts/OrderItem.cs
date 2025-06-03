using UnityEngine;

[CreateAssetMenu(fileName = "New Order", menuName = "Canteen/Order")]
public class OrderData : ScriptableObject
{
    public string customerDialog;
    public FoodItem[] orderedFoods = new FoodItem[4];
}
