using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GunSettings : ScriptableObject
{
    public new string name;
    public GameObject bullet;
    public GameObject muzzleFlash;

    //bullet Force
    public float shootForce;
    public float upwardForce;

    //Gun
    public float fireRate, spread, reloadTime, timeBetweenShots;
    public int magSize, bulletsPerTap;
    public bool allowButtonHold;

    //aim
    public bool aimHold;
}
