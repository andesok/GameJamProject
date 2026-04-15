using UnityEngine;

public class Ball : MonoBehaviour, IPickupable, IUsable
{

    public void OnPickedUp(GameObject picker)
    {
    }

    public void OnDropped()
    {
    }

    public void OnPrimaryUse(GameObject user)
    {
    }

    public void OnSecondaryUse(GameObject user)
    {
        user.GetComponent<Inventory>()?.ForceDrop();
    }
}