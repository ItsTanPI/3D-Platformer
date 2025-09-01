using UnityEngine;

public class Gun : MonoBehaviour
{
    [Header("Gun Settings")]
    [SerializeField] Transform _muzzle;
    [SerializeField] GameObject _bulletPrefab; 
    [SerializeField] float _bulletSpeed = 20f;
    [SerializeField] Camera _camera;
    [SerializeField] LayerMask _layerMask;

    [SerializeField] Movement _movement;

    public void AimAtRaycast(Camera cam, float maxDistance = 100f)
    {
        Ray ray = cam.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f));
        RaycastHit hit;

        Vector3 targetPoint;

        if (Physics.Raycast(ray, out hit, maxDistance, _layerMask))
        {
            targetPoint = hit.point; 
        }
        else
        {
            targetPoint = ray.GetPoint(maxDistance); 
        }

        _muzzle.LookAt(targetPoint);
    }

    private void Update()
    {
        if (_movement.isFirstPerson)
        {
            AimAtRaycast(_camera);
        }
        else
        {
            Vector3 targetPoint = _movement.transform.position + _movement.transform.forward * 100f;
            _muzzle.LookAt(targetPoint);
        }
    }

    public void Shoot()
    {
        if (_muzzle != null && _bulletPrefab != null)
        {
            GameObject bullet = Instantiate(_bulletPrefab, _muzzle.position, _muzzle.rotation);

            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = _muzzle.forward * _bulletSpeed;
            }
        }
    }
}
