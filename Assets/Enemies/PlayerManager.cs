using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    #region Singleton
    public static PlayerManager instance;

    private void Awake()
    {
        instance = this;
    }
    #endregion

    public GameObject player;

    public Health PlayerHealth => player.GetComponent<Health>();

    private void Update()
    {
        if (PlayerHealth != null && PlayerHealth.health <= 0)
        {
            Debug.Log("DEAD");
        }
    }
}