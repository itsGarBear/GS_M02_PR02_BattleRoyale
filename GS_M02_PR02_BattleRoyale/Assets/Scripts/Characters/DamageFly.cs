using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DamageFly : Abilities
{
    public float rocketJumpForce;
    public float hoverForce;

    private Rigidbody rb;


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public override void Update()
    {
        if (Time.time > nextLeftShift)
        {
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                nextLeftShift = Time.time + leftShiftCooldownTime;
                LeftShiftAbility();
            }
        }

        if (Time.time > nextE)
        {
            if (Input.GetKey(KeyCode.E))
            {
                nextE = Time.time + eCooldownTime;
                EAbility();
            }
        }

    }

    public override void LeftShiftAbility()
    {
        rb.AddForce(0, rocketJumpForce, 0);
    }

    public override void EAbility()
    {
        rb.AddForce(0, hoverForce, 0);
    }
}
