using UnityEngine;
using UnityEngine.InputSystem;

public class EnemyInput : MonoBehaviour
{
    [SerializeField] private CharacterMovement characterMovement;
    [SerializeField] private Inventory inventory;

    private void OnEnable()
    {

    }
    private void OnDisable()
    {

    }
    private void Awake()
    {

    }
    private void Update()
    {
        //characterMovement.moveVals = moveAction.ReadValue<Vector2>();
        //inventory.TryPickUpItem();
        //inventory.FirstUse();
        //inventory.SecondUse();
    }
}