using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;

public class GameManager : MonoBehaviourPun
{
    [Header("Players")]
    public PlayerController[] players;
    public Transform[] spawnPoints;
    public int alivePlayers;

    private int playersInGame;

    public float postGameTime;

    public static GameManager instance;

    private NetworkManager nm;

    private void Awake()
    {
        instance = this;
        nm = GameObject.FindObjectOfType<NetworkManager>();
    }

    private void Start()
    {
        players = new PlayerController[PhotonNetwork.PlayerList.Length];
        alivePlayers = players.Length;

        photonView.RPC("ImInGame", RpcTarget.AllBuffered);
    }

    public void CheckWinCondition()
    {
        if (alivePlayers == 1)
            photonView.RPC("WinGame", RpcTarget.All, players.First(x => !x.dead).id);
    }

    [PunRPC]
    void WinGame(int winningPlayer)
    {
        GameUI.instance.SetWinText(GetPlayer(winningPlayer).photonPlayer.NickName);
        Invoke("GoBackToMenu", postGameTime);
    }
    void GoBackToMenu()
    {
        NetworkManager.instance.ChangeScene("Menu");
    }
    [PunRPC]
    void ImInGame()
    {
        playersInGame++;

        
        if (PhotonNetwork.IsMasterClient && playersInGame == PhotonNetwork.PlayerList.Length)
            photonView.RPC("SpawnPlayer", RpcTarget.All);
    }

    [PunRPC]
    void SpawnPlayer()
    {
        GameObject playerObj = PhotonNetwork.Instantiate(nm.selection, spawnPoints[Random.Range(0, spawnPoints.Length)].position, Quaternion.identity);

        playerObj.GetComponent<PlayerController>().photonView.RPC("Initialize", RpcTarget.All, PhotonNetwork.LocalPlayer);
    }

    public PlayerController GetPlayer(int playerID)
    {
        foreach (PlayerController player in players)
        {
            if (player != null && player.id == playerID)
                return player;
        }

        return null;
    }
    public PlayerController GetPlayer(GameObject playerObj)
    {
        foreach (PlayerController player in players)
        {
            if (player != null && player.gameObject == playerObj)
                return player;
        }

        return null;
    }
}
