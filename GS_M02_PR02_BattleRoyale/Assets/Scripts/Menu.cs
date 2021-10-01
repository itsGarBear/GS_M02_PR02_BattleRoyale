using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class Menu : MonoBehaviourPunCallbacks, ILobbyCallbacks
{
    [Header("Screens")]
    public GameObject mainScreen;
    public GameObject createRoomScreen;
    public GameObject lobbyScreen;
    public GameObject lobbyBrowserScreen;

    [Header("Main Screen")]
    public Button createRoomBtn;
    public Button findRoomBtn;

    [Header("Lobby Screen")]
    public TextMeshProUGUI playerListText;
    public TextMeshProUGUI roomInfoText;
    public Button startGameBtn;

    [Header("Lobby Screen")]
    public RectTransform roomListContainer;
    public GameObject roomBtnPrefab;

    [Header("Hero Selection")]
    public List<Button> heroBtns;
    public List<TextMeshProUGUI> heroLbls;

    private List<GameObject> roomButtons = new List<GameObject>();
    private List<RoomInfo> roomList = new List<RoomInfo>();


    private void Start()
    {
        createRoomBtn.interactable = false;
        findRoomBtn.interactable = false;

        Cursor.lockState = CursorLockMode.None;

        foreach (Button b in heroBtns)
        {
            b.gameObject.SetActive(true);
        }
        foreach (TextMeshProUGUI t in heroLbls)
        {
            t.gameObject.SetActive(true);
        }

        if (PhotonNetwork.InRoom)
        {
            SetScreen(lobbyScreen);
            UpdateLobbyUI();

            PhotonNetwork.CurrentRoom.IsVisible = true;
            PhotonNetwork.CurrentRoom.IsOpen = true;
        }

    }

    public override void OnConnectedToMaster()
    {
        createRoomBtn.interactable = true;
        findRoomBtn.interactable = true;
    }

    void SetScreen(GameObject screen)
    {
        mainScreen.SetActive(false);
        createRoomScreen.SetActive(false);
        lobbyScreen.SetActive(false);
        lobbyBrowserScreen.SetActive(false);

        screen.SetActive(true);

        foreach (Button b in heroBtns)
        {
            b.gameObject.SetActive(false);
        }
        foreach (TextMeshProUGUI t in heroLbls)
        {
            t.gameObject.SetActive(false);
        }

        if (screen == lobbyBrowserScreen)
            UpdateLobbyBrowserUI();
    }

    public void OnBackButton()
    {
        
        SetScreen(mainScreen);
        foreach (Button b in heroBtns)
        {
            b.gameObject.SetActive(true);
        }
        foreach (TextMeshProUGUI t in heroLbls)
        {
            t.gameObject.SetActive(true);
        }
    }

    public void OnCreateRoomButton()
    {
        SetScreen(createRoomScreen);
    }

    public void OnFindRoomButton()
    {
        SetScreen(lobbyBrowserScreen);
    }

    public void OnPlayerNameUpdate(TMP_InputField playerNameInput)
    {
        PhotonNetwork.NickName = playerNameInput.text;
    }

    public void OnCreateButton(TMP_InputField roomNameInput)
    {
        NetworkManager.instance.CreateRoom(roomNameInput.text);
    }

    public override void OnJoinedRoom()
    {
        SetScreen(lobbyScreen);

        photonView.RPC("UpdateLobbyUI", RpcTarget.All);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdateLobbyUI();
    }

    [PunRPC]
    public void UpdateLobbyUI()
    {
        startGameBtn.interactable = PhotonNetwork.IsMasterClient;

        playerListText.text = "";

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            playerListText.text += player.NickName + "\n";
        }

        roomInfoText.text = "<b>Room Name</b>\n" + PhotonNetwork.CurrentRoom.Name;
    }

    public void OnLeaveLobbyButton()
    {
        PhotonNetwork.LeaveRoom();
        SetScreen(mainScreen);
        foreach (Button b in heroBtns)
        {
            b.gameObject.SetActive(true);
        }
        foreach (TextMeshProUGUI t in heroLbls)
        {
            t.gameObject.SetActive(true);
        }
    }

    public void OnStartGameButton()
    {
        PhotonNetwork.CurrentRoom.IsVisible = false;
        PhotonNetwork.CurrentRoom.IsOpen = false;

        NetworkManager.instance.photonView.RPC("ChangeScene", RpcTarget.All, "Game");
    }

    GameObject CreateRoomButton()
    {
        GameObject buttonObj = Instantiate(roomBtnPrefab, roomListContainer.transform);
        roomButtons.Add(buttonObj);

        return buttonObj;
    }

    private void UpdateLobbyBrowserUI()
    {
        foreach(GameObject button in roomButtons)
        {
            button.SetActive(false);
        }

        for(int i = 0; i < roomList.Count; i++)
        {
            GameObject button = i >= roomButtons.Count ? CreateRoomButton() : roomButtons[i];

            button.SetActive(true);

            button.transform.Find("RoomNameText").GetComponent<TextMeshProUGUI>().text = roomList[i].Name;
            button.transform.Find("PlayerCountText").GetComponent<TextMeshProUGUI>().text = roomList[i].PlayerCount + " / " + roomList[i].MaxPlayers;

            Button buttonComp = button.GetComponent<Button>();

            string roomName = roomList[i].Name;
            buttonComp.onClick.RemoveAllListeners();
            buttonComp.onClick.AddListener(() => { OnJoinRoomButton(roomName); });

        }
    }

    public void OnRefreshButton()
    {
        UpdateLobbyBrowserUI();
    }

    public void OnJoinRoomButton(string roomName)
    {
        NetworkManager.instance.JoinRoom(roomName);
    }

    public override void OnRoomListUpdate(List<RoomInfo> allRooms)
    {
        roomList = allRooms;
    }
}
