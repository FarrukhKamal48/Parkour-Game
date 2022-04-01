using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grappling : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private LineRenderer lr;
    [SerializeField] private Vector3 grapplePoint;
    [SerializeField] private Transform grappleTransfrom, Camera, Player;

    [Header("Input Settings")]
    [SerializeField] private bool holdGrapple;
    [SerializeField] private KeyCode grapple;

    [Header("Grapple Settings"), Space(2)]
    [SerializeField] private LayerMask grappleMask;
    [SerializeField] private float maxRange = 50f;
    [SerializeField] private Vector2 minMaxGrappleDist = new Vector2(0.8f, 0.25f);
    [SerializeField] private float spring = 4.5f;
    [SerializeField] private float damper = 7f;
    [SerializeField] private float massScale = 4.5f;
    [SerializeField] private float grappleSpeedMultiplier;
    [SerializeField] private float grappleDrag;

    private SpringJoint joint;

    private bool isGrappling, grappleInput;

    bool canGrapple;

    void Update()
    {
        switch (holdGrapple)
        {
            case true:
                if (Input.GetKeyDown(grapple))
                {
                    grappleInput = true;
                    StartGrapple();
                }
                else if (Input.GetKeyUp(grapple))
                {
                    grappleInput = false;
                    StopGrapple();
                }
                break;

            case false:
                if (Input.GetKeyDown(grapple))
                {
                    grappleInput = !grappleInput;
                }
                if (grappleInput)
                {
                    if (canGrapple)
                    {
                        StartGrapple();
                        canGrapple = false;
                    }
                }
                else
                {
                    StopGrapple();
                }
                break;
        }

        PlayerController.grapplemultiplier = grappleSpeedMultiplier;
        PlayerController.grappleDrag = grappleDrag;
    }

    void LateUpdate() { DrawRope(); }

    void ConfigureJoint(SpringJoint joint)
    {
        joint.autoConfigureConnectedAnchor = false;
        joint.connectedAnchor = grapplePoint;

        float distFromPoint = Vector3.Distance(Player.position, grapplePoint);

        joint.maxDistance = distFromPoint * minMaxGrappleDist.x;
        joint.minDistance = distFromPoint * minMaxGrappleDist.y;

        joint.spring = spring;
        joint.damper = damper;
        joint.massScale = massScale;
    }

    void StartGrapple()
    {
        Inputs.grappling = true;        
        isGrappling = true;

        RaycastHit hit;

        if(Physics.Raycast(Camera.position, Camera.forward, out hit, maxRange, grappleMask))
        {
            grapplePoint = hit.point;

            joint = Player.gameObject.AddComponent<SpringJoint>();

            ConfigureJoint(joint);

            lr.positionCount = 2;
        }
    }
    
    void StopGrapple()
    {
        Inputs.grappling = false;
        isGrappling = false;
        canGrapple = true;

        lr.positionCount = 0;

        Destroy(joint);
    }

    void DrawRope()
    {
        if (joint == null) return;

        lr.SetPosition(0, grappleTransfrom.position);
        lr.SetPosition(1, grapplePoint);
    }
}
