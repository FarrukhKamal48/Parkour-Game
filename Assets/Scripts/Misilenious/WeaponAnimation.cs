using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAnimation : MonoBehaviour
{
    public Animator animator;
    public string state;

    public void PlayAnimation()
    {
        switch (Inputs.GunAnimstate)
        {
            case Inputs.AnimationState.idle:
                animator.SetBool("Idle", true);
                animator.SetBool("Aiming", false);
                animator.SetBool("Sprinting", false);
                animator.SetBool("Shooting", false);
                animator.SetBool("Reloading", false);
                break;
            case Inputs.AnimationState.shoot:
                animator.SetBool("Idle", false);
                animator.SetBool("Shooting", true);
                break;
            case Inputs.AnimationState.aim:
                animator.SetBool("Idle", false);
                animator.SetBool("Aiming", true);
                animator.SetBool("Sprinting", false);
                break;
            case Inputs.AnimationState.aimShoot:
                animator.SetBool("Idle", false);
                animator.SetBool("Aiming", true);
                animator.SetBool("Sprinting", false);
                animator.SetBool("Shooting", true);
                animator.SetBool("Reloading", false);
                break;
            case Inputs.AnimationState.sprint:
                break;
            case Inputs.AnimationState.reload:
                break;
        }
    }

    void Update()
    {
        PlayAnimation();
        state = Inputs.GunAnimstate.ToString();
    }
}
