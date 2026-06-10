using UnityEngine;
using UnityEngine.UIElements;

public class CameraFollow : MonoBehaviour
{
    public GameObject FollowTarget;
    private Vector3 positionDiff;
    void Start()
    {
        positionDiff = transform.position-FollowTarget.transform.position;
    }

    void Update()
    {
        transform.position = FollowTarget.transform.position+positionDiff;
        transform.LookAt(FollowTarget.transform);
    }
}
