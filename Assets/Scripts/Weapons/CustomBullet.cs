using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomBullet : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private GameObject impact;
    [SerializeField] private float damage;
    [SerializeField] private bool useGravity;
    [SerializeField] private bool allowBounce;
    [SerializeField] private float bounciness;
    [SerializeField] private int maxCollisons;

    int collisions;
    private PhysicMaterial material;

    void Start()
    {
        SetUp();
    }

    void Update()
    {
        if (collisions >= maxCollisons) DestroyThis();        
    }

    void SetUp()
    {
        material = new PhysicMaterial();
        rb.useGravity = useGravity;

        material.dynamicFriction = 0;
        material.staticFriction = 0;
        material.frictionCombine = PhysicMaterialCombine.Minimum;

        material.bounceCombine = allowBounce ? PhysicMaterialCombine.Maximum : PhysicMaterialCombine.Minimum;
        material.bounciness = Mathf.Clamp01(bounciness);

        GetComponent<Collider>().material = material;
    }

    void OnCollisionEnter(Collision collider)
    {
        Hit(collider.transform);
        collisions++;
    }

    void Hit(Transform hitTransform)
    {
        Damagable target = hitTransform.GetComponent<Damagable>();

        if(target != null)
        {
            target.TakeDamage(damage);
            DestroyThis();
        }
        else if (allowBounce == false)
        {
            DestroyThis();
        }
    }

    void DestroyThis()
    {
        Instantiate(impact, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
