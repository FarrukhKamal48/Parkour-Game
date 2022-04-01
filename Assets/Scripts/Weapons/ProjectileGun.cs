using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileGun : MonoBehaviour
{
    public GunSettings Settings;

    [SerializeField] private WeaponAnimation gunAnimation;    
    
    //bullet
    private GameObject bullet;
    private GameObject muzzleFlash;
    //bullet Force
    private float shootForce;
    private float upwardForce;
    //Gun
    private float fireRate, spread, reloadTime, timeBetweenShots;
    private int magSize, bulletsPerTap;
    private bool allowButtonHold;

    int bulletsLeft, bulletsShot;

    //bools
    bool shooting, readyToShoot, reloading;

    //Reference
    [SerializeField] private Transform attackPoint;
    public Transform fpsCam;

    public bool allowInvoke = true;
    
    
    void Awake()
    {
        SetupSettings();

        bulletsLeft = magSize;
        readyToShoot = true;
    }

    void Update()
    {
        SetupSettings();

        //gun animation
        if (readyToShoot == false)
        {
            Inputs.GunAnimstate = Inputs.AnimationState.shoot;
        }
        if (GunManager.aiming == true)
        {
            Inputs.GunAnimstate = Inputs.AnimationState.aim;
            if (readyToShoot == false)
            {
                Inputs.GunAnimstate = Inputs.AnimationState.aimShoot;
            }
            else
            {
                Inputs.GunAnimstate = Inputs.AnimationState.aim;
            }
        }
        if(readyToShoot == true && GunManager.aiming == false)
        {
            Inputs.GunAnimstate = Inputs.AnimationState.idle;
        }

        MyInput();
    }

    void SetupSettings()
    {
        name = Settings.name;
        bullet = Settings.bullet;
        muzzleFlash = Settings.muzzleFlash;
        shootForce = Settings.shootForce;
        upwardForce = Settings.upwardForce;
        fireRate = Settings.fireRate;
        spread = Settings.spread;
        reloadTime = Settings.reloadTime;
        timeBetweenShots = Settings.timeBetweenShots;
        magSize = Settings.magSize;
        bulletsPerTap = Settings.bulletsPerTap;
        allowButtonHold = Settings.allowButtonHold;
    }

    void MyInput()
    {
        if (allowButtonHold) shooting = Input.GetKey(KeyCode.Mouse0);
        else shooting = Input.GetKeyDown(KeyCode.Mouse0);

        if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magSize && reloading == false) Reload();

        if (readyToShoot && shooting && reloading == false && bulletsLeft <= 0) Reload();

        if(readyToShoot && shooting && reloading == false && bulletsLeft > 0)
        {
            bulletsShot = 0;
            Shoot();
        }
    }

    void Shoot()
    {
        readyToShoot = false;

        Vector3 shootDirnoSpread = fpsCam.forward;

        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);

        Vector3 shootDirspread = shootDirnoSpread + new Vector3(x, y, 0);

        GameObject currentBullet = Instantiate(bullet, attackPoint.position, Quaternion.identity);

        Instantiate(muzzleFlash, attackPoint.position, Quaternion.identity);

        currentBullet.transform.forward = shootDirspread.normalized;

        currentBullet.GetComponent<Rigidbody>().AddForce(shootDirspread.normalized * shootForce, ForceMode.Impulse);
        currentBullet.GetComponent<Rigidbody>().AddForce(fpsCam.up * upwardForce, ForceMode.Impulse);

        bulletsLeft--;
        bulletsShot++;

        if (allowInvoke)
        {
            Invoke("ResetShot", fireRate);
            allowInvoke = false;
        }

        if (bulletsShot < bulletsPerTap && bulletsLeft > 0)
            Invoke("Shoot", timeBetweenShots);
    }

    void ResetShot()
    {
        readyToShoot = true;
        allowInvoke = true;
    }

    void Reload()
    {
        reloading = true;
        Invoke("ReloadFinished", reloadTime);
    }

    void ReloadFinished()
    {
        bulletsLeft = magSize;
        reloading = false;
    }
}
