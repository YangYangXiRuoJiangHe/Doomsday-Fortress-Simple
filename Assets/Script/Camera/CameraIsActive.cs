using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MyGame
{

    public class CameraIsActive : MonoBehaviour
    {
        private void OnEnable()
        {
            GetComponentInParent<CharController_Motor>()?.SetPlayerInput(true);
        }
        private void OnDisable()
        {
            GetComponentInParent<CharController_Motor>()?.SetPlayerInput(false);
        }
    }
}