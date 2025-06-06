using UnityEngine;

public class FoodButton : MonoBehaviour
{
    public FoodItem foodData;

    [SerializeField] private Renderer buttonRenderer;
    [SerializeField] private Color highlightColor = Color.yellow;

    private Color originalColor;

    private void Start()
    {
        if (buttonRenderer == null)
            buttonRenderer = GetComponent<Renderer>();

        if (buttonRenderer != null)
            originalColor = buttonRenderer.material.color;
    }

    private void OnMouseDown()
    {
        if (foodData != null)
        {
            FoodManager.Instance.OnFoodSelected(foodData);
        }
    }

    private void OnMouseEnter()
    {
        if (buttonRenderer != null)
            buttonRenderer.material.color = highlightColor;
    }

    private void OnMouseExit()
    {
        if (buttonRenderer != null)
            buttonRenderer.material.color = originalColor;
    }
}
