using UnityEngine;

public class Stick : MonoBehaviour, IPickupable, IUsable
{
    [SerializeField] private float _damage = 10f;
    [SerializeField] private float _attackRange = 3f;
    private GameObject PlayerMesh;

    [SerializeField] private float _throwDamage = 15f;
    [SerializeField] private float _throwSpinSpeed = 720f;
    [SerializeField] private int _durability = 20;

    private bool _isThrown = false;
    private GameObject _thrower;
    private Rigidbody _rb;

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
        PlayerMesh = user.GetComponent<PlayerMovement>().PlayerMesh;

        Ray ray = new Ray(PlayerMesh.transform.position, PlayerMesh.transform.forward);
        RaycastHit hit;

        Debug.DrawRay(ray.origin, ray.direction * _attackRange, Color.red, 1f);

        if (Physics.Raycast(ray, out hit, _attackRange))
        {
            // Проверяем, есть ли на объекте скрипт Enemy
            Enemy enemy = hit.collider.GetComponent<Enemy>();

            // Если скрипт не на самом объекте — ищем в родителе
            if (enemy == null)
            {
                enemy = hit.collider.GetComponentInParent<Enemy>();
            }

            if (enemy != null)
            {
                enemy.TakeDamage(_damage);
                DurabilityCost(1);
                Debug.Log($"Нанесён урон {_damage} врагу {enemy.name}");
            }
        }
    }

    public void OnSecondaryUse(GameObject user)
    {
        Inventory inventory = user.GetComponent<Inventory>();
        if (inventory != null)
        {
            // Запоминаем кто бросил
            _thrower = user;
            _isThrown = true;

            inventory.ForceDrop();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!_isThrown) return;
        if (_thrower != null && collision.gameObject == _thrower) return;

        // Ищем Enemy на объекте И В РОДИТЕЛЯХ
        Enemy enemy = collision.gameObject.GetComponent<Enemy>();
        if (enemy == null)
            enemy = collision.gameObject.GetComponentInParent<Enemy>();  // ← ВОТ ЭТО ДОЛЖНО НАЙТИ

        if (enemy != null)
        {
            enemy.TakeDamage(_throwDamage);
            Debug.Log($"Бросок нанёс {_throwDamage} урона врагу {enemy.name}");
        }
        else
        {
            Debug.Log($"Enemy не найден на {collision.gameObject.name} и его родителях");
        }

        _isThrown = false;
    }
}