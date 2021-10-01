using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DamageBlink : Abilities
{
    public int uses;
    public float cooldown;
    public float distance;
    public float speed;
    public float destinationMultiplier;
    public float camHeight;

    public Transform cam;

    public TextMeshProUGUI uiText;
    public LayerMask layerMask;

    int maxUses;
    float cooldownTimer;
    bool blinking = false;
    Vector3 destination;


    private void Start()
    {
        maxUses = uses;
        cooldownTimer = cooldown;
        uiText.text = uses.ToString();
       
    }

    public override void Update()
    {
        base.Update();

        if(uses < maxUses)
        {
            if (cooldownTimer > 0)
                cooldownTimer -= Time.deltaTime;
            else
            {
                uses += 1;
                cooldownTimer = cooldown;
                uiText.text = uses.ToString();
            }
        }

        if(blinking)
        {
            var dist = Vector3.Distance(transform.position, destination);
            if(dist > 0.5f)
            {
                transform.position = Vector3.MoveTowards(transform.position, destination, Time.deltaTime * speed);
            }
            else
            {
                blinking = false;
            }
        }
    }
    public override void LeftShiftAbility()
    {
        if(uses > 0)
        {
            uses -= 1;
            uiText.text = uses.ToString();

            RaycastHit hit;
            if(Physics.Raycast(cam.position, cam.forward, out hit, distance, layerMask))
            {
                destination = hit.point * destinationMultiplier;
            }
            else
            {
                destination = (cam.position + cam.forward.normalized * distance) * destinationMultiplier;
            }

            destination.y += camHeight;
            blinking = true;
        }

    }
}
