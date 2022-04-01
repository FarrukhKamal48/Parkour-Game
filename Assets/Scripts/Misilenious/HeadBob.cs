using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadBob : MonoBehaviour
{
    [SerializeField] private bool enable = true;

    [SerializeField, Range(0, 10f)] private float amplitude = 0.015f;
    [SerializeField, Range(0, 30f)] private float frequency = 10.3f;
    [SerializeField] private Vector2 frequencyMultiplier;
    [SerializeField] private Vector2 sprintFrequencyMultiplier;
    [SerializeField] private Vector2 crouchFrequencyMultiplier;

    Vector2 _frequencyMultiplier;

    [SerializeField] private Transform cameraHolder;
    [SerializeField] private PlayerController controller;

    public float _speed;

    private float toggleSpeed = 3f;
    private Quaternion startPos;

    void Awake()
    {
        startPos = cameraHolder.localRotation;
    }

    void Update()
    {
        if (!enable) return;

        CalcSpeed();
        SetFrequency();

        CheckMotion();
        ResetPosition();
    }

    void CalcSpeed()
    {
        Vector3 horizontalPlane = new Vector3(controller.rb.velocity.x, 0f, controller.rb.velocity.z);
        _speed = horizontalPlane.magnitude;
    }

    void SetFrequency()
    {
        if (Inputs.walking)
        {
            _frequencyMultiplier = frequencyMultiplier;
        }
        else if (Inputs.Sprinting)
        {
            _frequencyMultiplier = sprintFrequencyMultiplier;
        }
        else if (Inputs.crouching)
        {
            _frequencyMultiplier = crouchFrequencyMultiplier;
        }
    }

    public void PlayMotion(Vector3 motion)
    {
        cameraHolder.localEulerAngles += motion;
    }

    void CheckMotion()
    {
        if (_speed < toggleSpeed) return;
        if (Inputs.grounded == false) return;

        PlayMotion(FootStepMotion());
    }

    Vector3 FootStepMotion()
    {
        Vector3 pos = Vector3.zero;
        pos.x += Mathf.Sin(Time.time * frequency * _frequencyMultiplier.x) * (amplitude * _frequencyMultiplier.y);
        pos.y += Mathf.Cos(Time.time * (frequency * _frequencyMultiplier.x) / 2f) * (amplitude * _frequencyMultiplier.y) * 2f;
		pos.z += Mathf.Sin(Time.time * (frequency * _frequencyMultiplier.x) / 2f) * (amplitude * _frequencyMultiplier.y) / 1.5f;
        return pos;
    }

    private void ResetPosition()
    {
        if (cameraHolder.localRotation != startPos)
            cameraHolder.localRotation = Quaternion.Slerp(cameraHolder.localRotation, startPos, 5 * Time.deltaTime);
    }
}
