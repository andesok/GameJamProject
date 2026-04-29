using UnityEngine;

public class animationStateController : MonoBehaviour
{
    Animator animator;
    public PlayerMovement playerMovement;
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        float speed = new Vector3(playerMovement.moveVals.x, 0, playerMovement.moveVals.y).magnitude;
        if (speed > 0)
        {
            animator.SetBool("isWalking", true);
        }
        else
        {
            animator.SetBool("isWalking", false);
        }
    }
}
