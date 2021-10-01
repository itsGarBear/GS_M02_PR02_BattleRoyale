using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerWeapon : MonoBehaviour
{

    [Header("Stats")]
    public int damage;
    public int currAmmo;
    public int maxAmmo;
    public float bulletSpeed;
    public float shootRate;

    protected float lastShootTime;

    public GameObject bulletPrefab;
    public List<Transform> bulletSpawnPos;

    protected PlayerController player;

    private void Awake()
    {
        player = GetComponent<PlayerController>();
    }

    public virtual void TryShoot(int num)
    {
        if (currAmmo <= 0 || Time.time - lastShootTime < shootRate)
            return;

        currAmmo -= bulletSpawnPos.Count;
        lastShootTime = Time.time;

        GameUI.instance.UpdateAmmoText();
        
        foreach(Transform t in bulletSpawnPos)
        {
            player.photonView.RPC("SpawnBullet", RpcTarget.All, t.transform.position, Camera.main.transform.forward, 2);
        }
        
    }

    [PunRPC]
    public virtual void SpawnBullet(Vector3 pos, Vector3 dir, int num)
    {
        GameObject bulletObj = Instantiate(bulletPrefab, pos, Quaternion.identity);
        bulletObj.transform.forward = dir;

        Bullet bulletScript = bulletObj.GetComponent<Bullet>();

        bulletScript.Initialize(damage, player.id, player.photonView.IsMine);
        bulletScript.rig.velocity = dir * bulletSpeed;
    }

    [PunRPC]
    public void GiveAmmo(float ammoToGive)
    {
        currAmmo = Mathf.Clamp(currAmmo + (int)ammoToGive, 0, maxAmmo);
        GameUI.instance.UpdateAmmoText();
    }
}
