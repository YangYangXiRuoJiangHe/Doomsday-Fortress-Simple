using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponControl : MonoBehaviour
{
    private Player player;
    [Header("Éä»÷ÏêÇé")]
    [SerializeField] private GameObject bulletPrefab;
    public float bulletSpeed;
    public float bulletDanage;
    public Transform gunPoint;
    public Transform aimPoint;
    public LayerMask notIsGun;
    private void Start()
    {
        player = GetComponent<Player>();
        player.inputActions.PlayerInput.Fire.performed += context => Shoot();
    }
    private void Update()
    {
        aimPoint.position = gunPoint.position + CameraManager.instance.currentCamera.transform.forward * 100;
    }
    private void Shoot()
    {
        GameObject newBullet = Instantiate(bulletPrefab, gunPoint.position, Quaternion.LookRotation(gunPoint.forward));
        Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
        Vector3 targetPoint;
        Ray ray = CameraManager.instance.currentCamera.ScreenPointToRay(screenCenter);
        if (Physics.Raycast(ray, out RaycastHit hit, 1000, notIsGun))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = gunPoint.position + CameraManager.instance.currentCamera.transform.forward  * 100;
        }
        newBullet.GetComponent<Bullet>().SetupProjectile(targetPoint, bulletDanage, bulletSpeed, hit.transform?.GetComponent<IDamagable>());
        GetComponentInChildren<Animator>().SetTrigger("Fire");
    }
}
