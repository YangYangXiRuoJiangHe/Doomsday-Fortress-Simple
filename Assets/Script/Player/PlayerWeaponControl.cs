using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponControl : MonoBehaviour
{
    private Player player;
    private void Start()
    {
        player = GetComponent<Player>();
        player.inputActions.PlayerInput.Fire.performed += context => Shoot();
    }
    private void Shoot()
    {
        GetComponentInChildren<Animator>().SetTrigger("Fire");
    }
}
