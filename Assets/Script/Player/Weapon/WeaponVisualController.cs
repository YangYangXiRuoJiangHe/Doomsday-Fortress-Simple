using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class WeaponVisualController : MonoBehaviour
{
    [SerializeField] private Transform[] gunTransforms;
    [SerializeField] private Transform postol;
    [SerializeField] private Transform revolver;
    [SerializeField] private Transform autoRifle;
    [SerializeField] private Transform shotGun;
    [SerializeField] private Transform rifle;
    private Transform currentGun;
    [Header("Left IK")]
    [SerializeField] private Transform leftHand;
    [Header("Rig")]
    private Rig rig;
    [SerializeField] private float rigIncreaseStep;
    private bool rigShouldBeIncreased;
    [Header("Animator")]
    private Animator anim;
    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rig = GetComponentInChildren<Rig>();
    }
    private void Start()
    {
        SwitchOn(postol);
    }
    private void Update()
    {
        CheckSwitchWeapon();

        if (Input.GetKeyDown(KeyCode.R))
        {
            anim.SetTrigger("Reload");
            rig.weight = 0;
        }

        if (rigShouldBeIncreased)
        {
            rig.weight += rigIncreaseStep * Time.deltaTime;
            if(rig.weight >= .9f)
            {
                rig.weight = 1;
                rigShouldBeIncreased = false;
            }
        }
    }

    public void ReturnRigWeightToOne()
    {
        rigShouldBeIncreased = true;
    }

    private void CheckSwitchWeapon()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SwitchOn(postol);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SwitchOn(revolver);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SwitchOn(autoRifle);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SwitchOn(shotGun);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            SwitchOn(rifle);
        }
    }

    private void SwitchOn(Transform gunTransform)
    {
        SwitchOffGuns();
        gunTransform.gameObject.SetActive(true);
        currentGun = gunTransform;
        AttachLeftHand();
    }

    private void SwitchOffGuns()
    {
        for(int i = 0; i < gunTransforms.Length; i++)
        {
            gunTransforms[i].gameObject.SetActive(false);
        }
    }

    private void AttachLeftHand()
    {
        Transform targetTransform = currentGun.GetComponentInChildren<LeftHandTargetTransform>().transform;

        leftHand.localPosition = targetTransform.localPosition;
        leftHand.localRotation = targetTransform.localRotation;
    }
}
