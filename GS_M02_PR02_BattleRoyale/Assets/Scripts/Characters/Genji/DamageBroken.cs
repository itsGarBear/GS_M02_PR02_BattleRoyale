using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;


public class DamageBroken : Abilities
{
    [SerializeField] private float dashForce;
    [SerializeField] private float dashDuration;

    public GenjiMovementController genjiMovementController;
    public Camera myCam;

    public override void Update()
    {
        base.Update();
    }

    [PunRPC]
    public override void LeftShiftAbility()
    {
        print("LShift");

        float startTime = Time.time;
        float incTime = Time.time;

        genjiMovementController.AddForce(myCam.transform.forward, dashForce);

        while(incTime < startTime + dashDuration)
        {
            incTime += Time.deltaTime;
        }
        
        genjiMovementController.ResetImpact();

    }
}
