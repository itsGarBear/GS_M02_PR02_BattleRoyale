using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Support : Abilities
{
    [SerializeField]
    private float ampMultiplier = 1;
    public Material speedMat;
    public Material healMat;

    [SerializeField]
    private bool healing = false;
    [SerializeField]
    private bool speeding = false;

    private float defaultSpeed;

    private PlayerController player;

    public float timeELasts = 4;
    public float healInterval = 1;

    public GameObject aura;

    private void Start()
    {
        
        player = GetComponent<PlayerController>();
        defaultSpeed = player.moveSpeed;
        speeding = true;
        Speed();
    }

    public override void Update()
    {
        base.Update();
        if(healing)
        {
            Heal();
            float startTime = Time.time;
            float incTime = Time.time;
            while (incTime < startTime + healInterval)
            {
                incTime += Time.deltaTime;
            }
        }
        else if(speeding)
        {
            Speed();
        }
    }

    public override void LeftShiftAbility()
    {
        if(aura.GetComponent<MeshRenderer>().material.name.Contains("Speed"))
        {
            healing = true;
            speeding = false;
            player.moveSpeed = defaultSpeed;
        }
        else if(aura.GetComponent<MeshRenderer>().material.name.Contains("Healing"))
        {
            speeding = true;
            healing = false;
        }

    }

    [PunRPC]
    public void ChangeAuraMat(int toggle)
    {
        if(toggle == 1)
        {
            aura.GetComponent<MeshRenderer>().material = speedMat;
        }
        else
        {
            aura.GetComponent<MeshRenderer>().material = healMat;
        }
    }


    public void Speed()
    {
        player.photonView.RPC("ChangeAuraMat", RpcTarget.All, 1);
        player.moveSpeed = Mathf.Clamp(defaultSpeed * (ampMultiplier + .6f), 10, 20);
    }

    private void Heal()
    {
        player.photonView.RPC("ChangeAuraMat", RpcTarget.All, 0);
        player.photonView.RPC("Heal", player.photonPlayer, ampMultiplier / 150);
    }

    public override void EAbility()
    {
        ampMultiplier = 2f;

        float startTime = Time.time;
        float incTime = Time.time;
        while(incTime < startTime + timeELasts)
        {
            incTime += Time.deltaTime;
        }
        ampMultiplier = 1f;
    }
}
