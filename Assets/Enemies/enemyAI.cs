using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    Transform target;
    NavMeshAgent agent;
    public float LookRadius = 10f;
    public float throwRadius = 8f;
    public float pickupRadius = 2f;
    public float meleeDamage = 10f;
    public float meleeRange = 1.5f;
    public float meleeCooldown = 1.5f;
    public float timeReload = 0;
    private float _meleeTimer = 0;

    private GameObject _heldItem = null;
    private Transform _holdPoint;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        target = PlayerManager.instance.player.transform;

        _holdPoint = transform.Find("HoldPoint");
        if (_holdPoint == null)
        {
            GameObject hp = new GameObject("HoldPoint");
            hp.transform.SetParent(transform);
            hp.transform.localPosition = new Vector3(0f, 1f, 1f);
            _holdPoint = hp.transform;
        }
    }

    void Update()
    {
        float distance = Vector3.Distance(target.position, transform.position);
        if (distance > LookRadius) return;

        _meleeTimer += Time.deltaTime;

        if (_heldItem == null)
        {
            // Нет предмета — ищем
            GameObject nearest = FindNearestItem();
            if (nearest != null)
            {
                float itemDist = Vector3.Distance(transform.position, nearest.transform.position);
                if (itemDist <= pickupRadius)
                    PickupItem(nearest);
                else
                    agent.SetDestination(nearest.transform.position);
            }
            else
            {
                agent.SetDestination(target.position);
            }

            // Ближний бой без предмета
            if (distance <= meleeRange)
            {
                agent.ResetPath();
                LookTarget();
                if (_meleeTimer >= meleeCooldown)
                {
                    Health playerHealth = PlayerManager.instance.PlayerHealth;
                    if (playerHealth != null)
                        playerHealth.TakeDamage(meleeDamage);
                    _meleeTimer = 0;
                }
            }
        }
        else
        {
            // Есть предмет
            if (distance <= throwRadius)
            {
                agent.ResetPath();
                LookTarget();

                if (distance <= meleeRange)
                {
                    // Совсем близко — бьёт предметом
                    if (_meleeTimer >= meleeCooldown)
                    {
                        Health playerHealth = PlayerManager.instance.PlayerHealth;
                        if (playerHealth != null)
                            playerHealth.TakeDamage(meleeDamage);
                        _meleeTimer = 0;
                    }
                }
                else
                {
                    // На дистанции — бросает
                    timeReload += Time.deltaTime;
                    if (timeReload >= 2f)
                    {
                        ThrowItem();
                        timeReload = 0;
                    }
                }
            }
            else
            {
                agent.SetDestination(target.position);
            }
        }
    }

    GameObject FindNearestItem()
    {
        GameObject nearest = null;
        float minDist = float.MaxValue;

        foreach (var obj in FindObjectsByType<Rigidbody>(FindObjectsSortMode.None))
        {
            if (obj.GetComponent<IPickupable>() == null) continue;
            if (obj.GetComponent<IUsable>() == null) continue;

            float d = Vector3.Distance(transform.position, obj.transform.position);
            if (d < minDist)
            {
                minDist = d;
                nearest = obj.gameObject;
            }
        }
        return nearest;
    }

    void PickupItem(GameObject item)
    {
        _heldItem = item;

        if (item.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.isKinematic = true;
            rb.detectCollisions = false;
        }
        if (item.TryGetComponent<Collider>(out var col))
            col.enabled = false;

        item.transform.SetParent(_holdPoint);
        item.transform.localPosition = Vector3.zero;
        item.transform.localRotation = Quaternion.identity;
    }

    void ThrowItem()
    {
        if (_heldItem == null) return;

        if (_heldItem.TryGetComponent<Ball>(out var ball))
            ball.SetThrower(gameObject);
        else if (_heldItem.TryGetComponent<Stick>(out var stick))
            stick.SetThrower(gameObject);

        GameObject itemToThrow = _heldItem;
        _heldItem = null;

        itemToThrow.transform.SetParent(null);

        // Безопасная позиция броска — не ниже врага
        Vector3 spawnPos = _holdPoint.position + transform.forward * 1f;
        spawnPos.y = Mathf.Max(spawnPos.y, transform.position.y + 0.5f);
        itemToThrow.transform.position = spawnPos;

        Rigidbody rb = itemToThrow.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.detectCollisions = true;
            Vector3 direction = (target.position - itemToThrow.transform.position).normalized + Vector3.up * 0.2f;
            rb.AddForce(direction * 15f, ForceMode.Impulse);
        }

        StartCoroutine(EnableColliderDelayed(itemToThrow));
    }

    IEnumerator EnableColliderDelayed(GameObject item)
    {
        yield return new WaitForSeconds(0.3f);
        if (item != null && item.TryGetComponent<Collider>(out var col))
            col.enabled = true;
    }

    void LookTarget()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, LookRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, throwRadius);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, pickupRadius);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, meleeRange);
    }
}