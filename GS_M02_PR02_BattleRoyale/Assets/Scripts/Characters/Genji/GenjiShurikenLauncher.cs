using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GenjiShurikenLauncher : PlayerWeapon
{
    [Header("Shurikens")]
    [SerializeField] private float delayBetweenShurikens;
    [SerializeField] private float shurikenLauchForce;
    [SerializeField] private List<Transform> projectSpawnTransforms = new List<Transform>();

    public override void TryShoot(int num)
    {
        if (currAmmo <= 0 || Time.time - lastShootTime < shootRate)
            return;

        player.photonView.RPC("SpawnBullet", RpcTarget.All, Vector3.zero, Vector3.zero, num);
    }

    [PunRPC]
    public override void SpawnBullet(Vector3 pos, Vector3 dir, int num)
    {
        if(num == 0)
        {
            StartCoroutine(LeftClickAttack());
        }
        else
        {
            foreach (Transform spawnLocation in projectSpawnTransforms)
            {
                GameObject projectileInstance = Instantiate(bulletPrefab, spawnLocation.position, spawnLocation.rotation);
                projectileInstance.GetComponent<Rigidbody>().AddForce(projectileInstance.transform.forward * shurikenLauchForce, ForceMode.VelocityChange);
            }
        }
        currAmmo -= 3;
        GameUI.instance.UpdateAmmoText();
    }

    private IEnumerator LeftClickAttack()
    {
        int shurikensFired = 0;

        while(shurikensFired < 3)
        {
            shurikensFired++;
            GameObject projectileInstance = Instantiate(bulletPrefab, projectSpawnTransforms[1].position, projectSpawnTransforms[1].rotation);
            projectileInstance.GetComponent<Rigidbody>().AddForce(projectileInstance.transform.forward * shurikenLauchForce, ForceMode.VelocityChange);
            yield return new WaitForSeconds(delayBetweenShurikens);
        }

        yield return null;
    }
}

