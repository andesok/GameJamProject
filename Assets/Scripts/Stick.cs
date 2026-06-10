using UnityEngine;

public class Stick : MonoBehaviour, IPickupable, IUsable
{
    [SerializeField] private float _damage = 10f;
    [SerializeField] private float _attackRange = 3f;
    private GameObject PlayerMesh;

    [SerializeField] private float _durability = 20;

    private bool _isThrown = false;
    private GameObject _thrower;
    private Rigidbody _rb;


    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }
    public void OnPickedUp(GameObject picker)
    {

    }

    public void OnDropped()
    {
    }

    private void DurabilityCost(int _cost)
    {
        _durability -= _cost;
        if (_durability <=0)
        {
            Destroy(gameObject);
        }
    }

    public void OnPrimaryUse(GameObject user)
    {
        Ray ray = new Ray(user.transform.position, user.transform.forward);
        RaycastHit hit;

        Debug.DrawRay(ray.origin, ray.direction * _attackRange, Color.red, 1f);

        if (Physics.Raycast(ray, out hit, _attackRange))
        {
            Enemy enemy = hit.collider.GetComponent<Enemy>();

            if (enemy == null)
            {
                enemy = hit.collider.GetComponentInParent<Enemy>();
            }

            if (enemy != null)
            {
                enemy.health.TakeDamage(_damage);
                DurabilityCost(1);
            }
        }
    }

    public void OnSecondaryUse(GameObject user)
    {
        Inventory inventory = user.GetComponent<Inventory>();
        if (inventory != null)
        {
            _thrower = user;
            _isThrown = true;

            inventory.ForceDrop();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!_isThrown) return;
        if (_thrower != null && collision.gameObject == _thrower) return;

        if (collision.gameObject.TryGetComponent<Health>(out Health targetHealth))
        {
            float finalDamage = CalculateThrowDamage();
            targetHealth.TakeDamage(finalDamage);
            AttackReset();
        }
    }

    private float CalculateThrowDamage()
    {
        float speed = _rb.linearVelocity.magnitude;

        float bonus = speed * 0.5f;

        return _damage + bonus;
    }

    private void AttackReset()
    {
        DurabilityCost(2);
        _isThrown = false;
    }
}