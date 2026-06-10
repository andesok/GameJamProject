using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    private InputAction moveAction;

    [SerializeField] private InputActionAsset InputActions;
    [SerializeField] private CharacterMovement characterMovement;
    [SerializeField] private Inventory inventory;

    private void OnEnable()
    {
        InputActions.FindActionMap("Player").Enable();
    }
    private void OnDisable()
    {
        InputActions.FindActionMap("Player").Disable();
    }
    private void Awake()
    {
        moveAction = InputSystem.actions.FindAction("Move");
    }
    private void Update()
    {
        characterMovement.moveVals = moveAction.ReadValue<Vector2>();
        if (Input.GetKeyDown(KeyCode.E))
        {
            inventory.TryPickUpItem();
        }
        if (Input.GetMouseButtonDown(0))
        {
            inventory.FirstUse();
        }

        if (Input.GetMouseButtonDown(1))
        {
            inventory.SecondUse();
        }
    }
}