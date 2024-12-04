using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputController : MonoBehaviour
{
    public static InputController Instance { get; private set; }

    [Header("Input Actions")]
    [SerializeField] private InputActionAsset inputActions;
    private InputActionMap playerActionMap;
    private InputAction moveAction;
    private InputAction slideAction;
    private InputAction jumpAction;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        playerActionMap = inputActions.FindActionMap("Player");
        moveAction = playerActionMap.FindAction("Move");
        slideAction = playerActionMap.FindAction("Slide");
        jumpAction = playerActionMap.FindAction("Jump");
    }

    // Avoide all movement related input actions
    public void DisableInputActions()
    {
        moveAction.Disable();
        slideAction.Disable();
        jumpAction.Disable();
    }

    // Allow all movement related input actions
    public void EnableInputActions()
    {
        moveAction.Enable();
        slideAction.Enable();
        jumpAction.Enable();
    }
}
