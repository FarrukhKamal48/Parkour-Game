using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dash : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private KeyCode dashKey;
    [SerializeField] private float maxFeul = 80f;
    [SerializeField] private float maxDashForce;

    [SerializeField] private float chargeAmount;
    [SerializeField] private float refeulAmount;
    [SerializeField] private float refeulDelay;

    [SerializeField] private float dashCoolDown;
    [SerializeField] private bool useCamDir;


    public static bool dashing;

    float dashforce;

    float remainingFeul;
    bool canDash = true;

    void Awake()
    {
        remainingFeul = maxFeul;
    }

    void dash(float dashSpeed, Vector3 direction, float dashDuration)
    {
        Vector3 dashVector = direction * dashSpeed;

        rb.AddForce(dashVector, ForceMode.Impulse);

        dashforce = 0f;

        CancelInvoke();
    }

    float Charge(float value, float targetValue, float increaseAmount)
    {
        value = Mathf.Lerp(value, targetValue, increaseAmount * Time.deltaTime);

        return value;
    }

    IEnumerator DashCoolDown(float duration)
    {
        yield return new WaitForSeconds(duration);
        
        canDash = true;
    }

    void Refeul()
    {
        remainingFeul = Charge(remainingFeul, maxFeul, refeulAmount * Time.deltaTime);
    }

    Vector3 DashDirection(Vector3 Dir)
    {
        if (Dir.z > 0 && Dir.x == 0 && Dir.y == 0 && useCamDir == true)
        {            
            return Inputs.cameraLook.forward;
        }
        else
        {
            return PlayerController.moveDir;
        }
    }

    void Update()
    {
        if (remainingFeul > maxFeul) remainingFeul = maxFeul;

        Vector3 dashDirection = DashDirection(Inputs.MoveInput());

        if (Input.GetKey(dashKey))
        {
            dashforce = Charge(dashforce, remainingFeul, chargeAmount * Time.deltaTime);
        }

        if (Input.GetKeyUp(dashKey) && canDash && dashDirection.magnitude > 0.5f && remainingFeul >= dashforce)
        {
            remainingFeul -= dashforce;
            dash(dashforce, dashDirection.normalized, dashCoolDown);            

            if (remainingFeul <= 0)
            {
                canDash = false;
                StartCoroutine(DashCoolDown(dashCoolDown));
            }
        }

        if (remainingFeul < maxFeul) Invoke("Refeul", refeulDelay);
    }
}
