using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animation : MonoBehaviour
{
    [Header("Recoil_Transform")]
    public Transform RecoilPositionTranform;
    public Transform RecoilRotationTranform;
    [Space(10)]
    [Header("Recoil_Settings")]
    public float PositionDampTime;
    public float RotationDampTime;
    [Space(10)]
    public float Recoil1;
    public float Recoil2;
    public float Recoil3;
    public float Recoil4;
    [Space(10)]
    public AnimationCurve[] roation = { new AnimationCurve(), new AnimationCurve(), new AnimationCurve() };
    public AnimationCurve[] position = { new AnimationCurve(), new AnimationCurve(), new AnimationCurve() };
    Vector3 RecoilRotation;
    Vector3 RecoilKickBack;

    public AnimationCurve[] Aim_rotaion = { new AnimationCurve(), new AnimationCurve(), new AnimationCurve() };
    public AnimationCurve[] Aim_position = { new AnimationCurve(), new AnimationCurve(), new AnimationCurve() };
    Vector3 RecoilRotation_Aim;
    Vector3 RecoilKickBack_Aim;

    [Space(10)]
    public Vector3 CurrentRecoil1;
    public Vector3 CurrentRecoil2;
    public Vector3 CurrentRecoil3;
    public Vector3 CurrentRecoil4;
    [Space(10)]
    public Vector3 RotationOutput;

    public bool aim;

    void Update()
    {
        if (Input.GetKey(KeyCode.Mouse1))
        {
            aim = true;
            RecoilRotation_Aim.x = Aim_rotaion[0].Evaluate(Time.time);
            RecoilRotation_Aim.y = Aim_rotaion[1].Evaluate(Time.time);
            RecoilRotation_Aim.z = Aim_rotaion[2].Evaluate(Time.time);
        }
        else
        {
            aim = false;
            RecoilRotation.x = roation[0].Evaluate(Time.time);
            RecoilRotation.y = roation[1].Evaluate(Time.time);
            RecoilRotation.z = roation[2].Evaluate(Time.time);
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
            Fire();
    }

    void FixedUpdate()
    {
        CurrentRecoil1 = Vector3.Lerp(CurrentRecoil1, Vector3.zero, Recoil1 * Time.deltaTime);
        CurrentRecoil2 = Vector3.Lerp(CurrentRecoil2, CurrentRecoil1, Recoil2 * Time.deltaTime);
        CurrentRecoil3 = Vector3.Lerp(CurrentRecoil3, Vector3.zero, Recoil3 * Time.deltaTime);
        CurrentRecoil4 = Vector3.Lerp(CurrentRecoil4, CurrentRecoil3, Recoil4 * Time.deltaTime);

        RecoilPositionTranform.localPosition = Vector3.Slerp(RecoilPositionTranform.localPosition, CurrentRecoil3, PositionDampTime * Time.fixedDeltaTime);
        RotationOutput = Vector3.Slerp(RotationOutput, CurrentRecoil1, RotationDampTime * Time.fixedDeltaTime);
        RecoilRotationTranform.localRotation = Quaternion.Euler(RotationOutput);
    }
    public void Fire()
    {
        /*
        if (aim == true)
        {
            CurrentRecoil1 += new Vector3(RecoilRotation_Aim.x, Random.Range(-RecoilRotation_Aim.y, RecoilRotation_Aim.y), Random.Range(-RecoilRotation_Aim.z, RecoilRotation_Aim.z));
            CurrentRecoil3 += new Vector3(Random.Range(-RecoilKickBack_Aim.x, RecoilKickBack_Aim.x), Random.Range(-RecoilKickBack_Aim.y, RecoilKickBack_Aim.y), RecoilKickBack_Aim.z);
        }
        if (aim == false)
        {
            CurrentRecoil1 += new Vector3(RecoilRotation.x, Random.Range(-RecoilRotation.y, RecoilRotation.y), Random.Range(-RecoilRotation.z, RecoilRotation.z));
            CurrentRecoil3 += new Vector3(Random.Range(-RecoilKickBack.x, RecoilKickBack.x), Random.Range(-RecoilKickBack.y, RecoilKickBack.y), RecoilKickBack.z);
        }*/

        if (aim == true)
        {
            CurrentRecoil1 = new Vector3(RecoilRotation_Aim.x, RecoilRotation_Aim.y, RecoilRotation_Aim.z);
            CurrentRecoil3 = new Vector3(RecoilKickBack_Aim.x, RecoilKickBack_Aim.y, RecoilKickBack_Aim.z);
        }
        if (aim == false)
        {
            CurrentRecoil1 = new Vector3(RecoilRotation.x,RecoilRotation.y, RecoilRotation.z);
            CurrentRecoil3 = new Vector3(RecoilKickBack.x, RecoilKickBack.y, RecoilKickBack.z);
        }
    }
}
