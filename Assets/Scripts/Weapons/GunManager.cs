using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunManager : MonoBehaviour
{
    [SerializeField] private Transform weaponHolder;
    [SerializeField] private List<Transform> weapons = new List<Transform>();

    [Header("WeaponEquip Settings")]
    [SerializeField] private KeyCode equip, drop;
    [SerializeField] private float pickUpRange;
    [SerializeField] private int maxSlot;

    public int weaponsInSlot;
    private bool slotFull;
    public int selectedIndex = 0;

    public static bool aiming;
    public bool _aiming;

    Transform currentGun;


    void Start()
    {
        SelectWeapon();
    }

    void input()
    {
        ProjectileGun currentgun = currentGun.GetComponent<ProjectileGun>();

        if (currentgun.Settings.aimHold) aiming = Input.GetKey(KeyCode.Mouse1);
        else
        {
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                aiming = !aiming;
            }
        }
    }
    
    void Update()
    {
        if (weapons.Count != 0)
        {
            currentGun = weapons[selectedIndex];
            input();
        }

        if (weaponsInSlot >= maxSlot) slotFull = true;
        else slotFull = false;

        RaycastHit hit;

        if(Physics.Raycast(weaponHolder.position, weaponHolder.forward, out hit, pickUpRange))
        {
            if(slotFull == false && Input.GetKeyDown(equip) && hit.transform.GetComponent<Item>() != null)
            {
                Equip(hit.transform);
            }
        }

        if(weaponsInSlot > 0 && Input.GetKeyDown(drop))
        {
            Drop(weapons[selectedIndex].gameObject);
        }


        // input

        int prevSelectIndex = selectedIndex;

        if(Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            if (selectedIndex >= weaponsInSlot - 1)
                selectedIndex = 0;
            else
                selectedIndex++;
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            if (selectedIndex <= 0)
                selectedIndex = weaponsInSlot - 1;
            else
                selectedIndex--;
        }

        if(prevSelectIndex != selectedIndex)
        {
            SelectWeapon();
        }

        _aiming = aiming;
    }

    void SelectWeapon()
    {
        int i = 0;
        foreach(Transform _weapon in weapons)
        {
            if (i == selectedIndex)
                _weapon.gameObject.SetActive(true);
            else
                _weapon.gameObject.SetActive(false);
            i++;
        }
    }

    void Equip(Transform itemTransform)
    {
        weaponsInSlot++;

        Item _item = itemTransform.GetComponent<Item>();

        GameObject itemobj = Instantiate(_item.item);
        itemobj.transform.SetParent(weaponHolder);
        itemobj.transform.localEulerAngles = Vector3.zero;
        itemobj.transform.localPosition = _item.placement;

        ProjectileGun gunScript = itemobj.GetComponent<ProjectileGun>();
        gunScript.fpsCam = weaponHolder;

        weapons.Add(itemobj.transform);

        SelectWeapon();
    }

    void Drop(GameObject weapon)
    {
        weaponsInSlot--;

        Destroy(weapon);
        weapons.Remove(weapon.transform);

        selectedIndex = weaponsInSlot - 1;
        SelectWeapon();
    }
}
