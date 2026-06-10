using UnityEngine;

public class animationStateController : MonoBehaviour
{
    Animator animator;
    public CharacterMovement characterMovement;
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        float speed = new Vector3(characterMovement.moveVals.x, 0, characterMovement.moveVals.y).magnitude;
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
