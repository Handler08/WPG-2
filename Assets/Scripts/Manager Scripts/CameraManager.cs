using UnityEngine;
using UnityEngine.UI;
using Unity.Cinemachine;
using System.Collections;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private CinemachineCamera servingCamera; // Camera for serving food (looking down)
    [SerializeField] private CinemachineCamera customerCamera; // Camera for looking at customer (looking up)
    [SerializeField] private Button arrowUpButton; // UI button for looking up
    [SerializeField] private Button arrowDownButton; // UI button for looking down
    [SerializeField] private CustomerBehaviour customer; // Reference to CustomerBehaviour to control dialog
    [SerializeField] private CinemachineBrain cinemachineBrain; // Reference to CinemachineBrain for blend detection

    private enum CameraState { ReceivingOrder, PreparingFood }
    private CameraState currentState;

    private void Start()
    {
        Debug.Log("[CameraManager] Initializing in ReceivingOrder state");
        currentState = CameraState.ReceivingOrder;
        SetCameraState(CameraState.ReceivingOrder, force: true);

        // Ensure button listeners are added
        if (arrowUpButton != null)
        {
            arrowUpButton.onClick.RemoveAllListeners(); // Clear any existing listeners
            arrowUpButton.onClick.AddListener(LookUp);
            arrowUpButton.interactable = true;
            Debug.Log("[CameraManager] Up button listener added, interactable: " + arrowUpButton.interactable);
        }
        else
        {
            Debug.LogError("[CameraManager] arrowUpButton is not assigned");
        }

        if (arrowDownButton != null)
        {
            arrowDownButton.onClick.RemoveAllListeners(); // Clear any existing listeners
            arrowDownButton.onClick.AddListener(LookDown);
            arrowDownButton.interactable = true;
            Debug.Log("[CameraManager] Down button listener added, interactable: " + arrowDownButton.interactable);
        }
        else
        {
            Debug.LogError("[CameraManager] arrowDownButton is not assigned");
        }
    }

    private void Update()
    {
        // Check if customer just entered Waiting state
        if (currentState == CameraState.ReceivingOrder && customer != null && customer.IsWaiting() && !customer.IsDialogShown())
        {
            StartCoroutine(ShowDialogAfterBlend());
        }
    }

    private void LookUp()
    {
        Debug.Log("[CameraManager] LookUp called");
        SetCameraState(CameraState.ReceivingOrder);
    }

    private void LookDown()
    {
        Debug.Log("[CameraManager] LookDown called");
        SetCameraState(CameraState.PreparingFood);
    }

    private void SetCameraState(CameraState newState, bool force = false)
    {
        if (currentState == newState && !force)
        {
            Debug.Log($"[CameraManager] Already in {newState} state, skipping");
            return;
        }

        Debug.Log($"[CameraManager] Switching to {newState} state");
        currentState = newState;

        if (newState == CameraState.ReceivingOrder)
        {
            SetCameraPriority(servingCamera, 0);
            SetCameraPriority(customerCamera, 10);
            if (arrowUpButton != null) arrowUpButton.gameObject.SetActive(false);
            if (arrowDownButton != null) arrowDownButton.gameObject.SetActive(true);
            if (customer != null)
            {
                customer.HideDialog();
                StartCoroutine(ShowDialogAfterBlend());
            }
        }
        else // PreparingFood
        {
            SetCameraPriority(servingCamera, 10);
            SetCameraPriority(customerCamera, 0);
            if (customer != null)
            {
                customer.HideDialog();
            }
            if (arrowUpButton != null) arrowUpButton.gameObject.SetActive(true);
            if (arrowDownButton != null) arrowDownButton.gameObject.SetActive(false);
        }
    }

    private IEnumerator ShowDialogAfterBlend()
    {
        Debug.Log("[CameraManager] Waiting for camera blend to complete");
        while (cinemachineBrain.IsBlending || cinemachineBrain.ActiveVirtualCamera != (ICinemachineCamera)customerCamera)
        {
            Debug.Log("[CameraManager] Still blending, ActiveCamera: " + (cinemachineBrain.ActiveVirtualCamera?.Name ?? "None"));
            yield return null;
        }
        Debug.Log("[CameraManager] Camera blend complete, active camera: " + cinemachineBrain.ActiveVirtualCamera?.Name);
        if (currentState == CameraState.ReceivingOrder && customer != null && customer.IsWaiting())
        {
            string dialogText = customer.GetCurrentDialog();
            if (!string.IsNullOrEmpty(dialogText))
            {
                Debug.Log("[CameraManager] Showing dialog: " + dialogText);
                customer.ShowDialog(dialogText);
            }
            else
            {
                Debug.LogWarning("[CameraManager] Dialog text is empty");
            }
        }
        else
        {
            Debug.Log("[CameraManager] Not showing dialog: " +
                $"State={currentState}, CustomerWaiting={customer != null && customer.IsWaiting()}");
        }
    }

    private void SetCameraPriority(CinemachineCamera camera, int priority)
    {
        if (camera != null)
        {
            camera.Priority = priority;
            Debug.Log($"[CameraManager] Set {camera.name} priority to {priority}");
        }
        else
        {
            Debug.LogWarning("[CameraManager] Camera is null when setting priority");
        }
    }

    public bool IsInReceivingOrder()
    {
        return currentState == CameraState.ReceivingOrder;
    }
}