using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class Abilities : MonoBehaviourPun
{
    public float leftShiftCooldownTime;
    public float eCooldownTime;

    protected float nextLeftShift;
    protected float nextE;


    public virtual void Update()
    {
        if(Time.time > nextLeftShift)
        {
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                nextLeftShift = Time.time + leftShiftCooldownTime;
                LeftShiftAbility();
            }
        }

        if (Time.time > nextE)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                nextE = Time.time + eCooldownTime;
                EAbility();
            }
        }
    }

    public virtual void LeftShiftAbility()
    {
        print("Left Shift Ability");
    }

    public virtual void EAbility()
    {
        print("E Ability");
    }
}
