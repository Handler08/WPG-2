using UnityEngine;

public class TempButton : MonoBehaviour
{
    private void OnMouseDown()
    {
        OrderManager.Instance.AssignRandomOrder();
        Debug.Log("Random order requested.");
    }
}
