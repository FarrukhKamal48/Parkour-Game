using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSway : MonoBehaviour
{
    [SerializeField] private Transform pivot;

    [SerializeField] private float SwayAmount;
    [Tooltip("How quickly the gun rotates (higher the value, slower the speed)")]
    [SerializeField] private float SwaySmoothing;
    [Tooltip("how quickly the gun resets it's rotation (higher the value, slower the speed)")]
    [SerializeField] private float SwayResetSmoothing;
    [SerializeField] private float SwayClampX, SwayClampY;

    [SerializeField] private bool xInvert, yInvert;

    Vector2 inputView;

    Vector3 newWeaponRotaion;
    Vector3 newWeaponRotaionVelecity;

    Vector3 targetWeaponRotaion;
    Vector3 targetWeaponRotaionVelecity;

    void Awake()
    {
        newWeaponRotaion = pivot.localRotation.eulerAngles;
    }

    void Update()
    {
        inputView = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        targetWeaponRotaion.y += SwayAmount * (xInvert ? -inputView.x : inputView.x) * Time.deltaTime;
        targetWeaponRotaion.x += SwayAmount * (yInvert ? inputView.y : -inputView.y) * Time.deltaTime;

        targetWeaponRotaion.x = Mathf.Clamp(targetWeaponRotaion.x, -SwayClampX, SwayClampX);
        targetWeaponRotaion.y = Mathf.Clamp(targetWeaponRotaion.y, -SwayClampY, SwayClampY);

        targetWeaponRotaion = Vector3.SmoothDamp(targetWeaponRotaion, Vector3.zero, ref targetWeaponRotaionVelecity, 1 / SwayResetSmoothing);
        newWeaponRotaion = Vector3.SmoothDamp(newWeaponRotaion, targetWeaponRotaion, ref newWeaponRotaionVelecity, 1 / SwaySmoothing);

        pivot.localRotation = Quaternion.Euler(newWeaponRotaion);
    }
}
