using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] private Transform _holdPoint;

    private GameObject _currentItem;
    private IPickupable _pickupInterface;
    private IUsable _usableInterface;
    private Player _playerScript;

    [SerializeField] private float dropForce =1f;
    [SerializeField] private GameObject PlayerMesh;

    private List<GameObject> _nearbyItems = new List<GameObject>();

    private void Awake()
    {
        _playerScript = GetComponent<Player>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (_currentItem == null)
                TryPickupNearest();
            else
                DropItem();
        }

        if (_currentItem != null)
        {
            if (Input.GetMouseButtonDown(0)) _usableInterface?.OnPrimaryUse(gameObject);

            if (Input.GetMouseButtonDown(1)) _usableInterface?.OnSecondaryUse(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<IPickupable>(out _))
        {
            if (!_nearbyItems.Contains(other.gameObject))
            {
                _nearbyItems.Add(other.gameObject);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (_nearbyItems.Contains(other.gameObject))
        {
            _nearbyItems.Remove(other.gameObject);
        }
    }

    private void TryPickupNearest()
    {
        _nearbyItems.RemoveAll(item => item == null);

        if (_nearbyItems.Count == 0)
        {
            Debug.Log("Нет предметов поблизости");
            return;
        }

        GameObject nearest = GetNearestItem();

        if (nearest != null)
        {
            PickupItem(nearest);
        }
    }

    private GameObject GetNearestItem()
    {
        GameObject nearest = null;
        float minDistance = float.MaxValue;

        foreach (var item in _nearbyItems)
        {
            if (item == null) continue;

            float dist = Vector3.Distance(transform.position, item.transform.position);
            if (dist < minDistance)
            {
                minDistance = dist;
                nearest = item;
            }
        }

        return nearest;
    }

    private void PickupItem(GameObject item)
    {

        _nearbyItems.Remove(item);

        _currentItem = item;

        _pickupInterface = item.GetComponent<IPickupable>();
        _usableInterface = item.GetComponent<IUsable>();

        _pickupInterface.OnPickedUp(gameObject);


        item.transform.SetParent(_holdPoint);
        item.transform.localPosition = Vector3.zero;
        item.transform.localRotation = Quaternion.identity;

        if (item.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.isKinematic = true;
            rb.detectCollisions = false;
        }

        if (item.TryGetComponent<Collider>(out var col))
            col.enabled = false;
    }

    private void DropItem()
    {
        if (_currentItem == null) return;

        if (_currentItem.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.isKinematic = false;
            rb.detectCollisions = true;
        }

        if (_currentItem.TryGetComponent<Collider>(out var col))
            col.enabled = true;

        _currentItem.transform.SetParent(null);

        _pickupInterface?.OnDropped();

        rb?.AddForce(PlayerMesh.transform.forward * 3f, ForceMode.Impulse);

        if (_currentItem != null && !_nearbyItems.Contains(_currentItem))
        {
            _nearbyItems.Add(_currentItem);
        }

        _currentItem = null;
        _pickupInterface = null;
        _usableInterface = null;
    }

    public void ForceDrop()
    {
        if (_currentItem == null) return;

        if (_currentItem.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.isKinematic = false;
            rb.detectCollisions = true;
        }

        if (_currentItem.TryGetComponent<Collider>(out var col))
            col.enabled = true;

        _currentItem.transform.SetParent(null);

        _pickupInterface?.OnDropped();

        if (_currentItem.TryGetComponent<Rigidbody>(out var rb2))
        {
            rb2.AddForce(PlayerMesh.transform.forward * dropForce + Vector3.up * 3f, ForceMode.Impulse);
        }

        _currentItem = null;
        _pickupInterface = null;
        _usableInterface = null;
    }

    public bool IsHandsFree() => _currentItem == null;
}