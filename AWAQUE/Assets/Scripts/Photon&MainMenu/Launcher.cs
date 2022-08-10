using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;


public class Launcher : MonoBehaviourPunCallbacks
{
    public static Launcher Instance;

    [SerializeField] private TMP_InputField roomNameInputField;
    [SerializeField] private TMP_InputField playerNameInputField;

    [SerializeField] private TMP_Text roomNameText;
    [SerializeField] private TMP_Text errorText;

    [SerializeField] private Transform roomListContent;
    [SerializeField] private Transform playerListContent;

    [SerializeField] private GameObject roomListItemPrefab;
    [SerializeField] private GameObject playerListItemPrefab;
    [SerializeField] private GameObject startGameButton;

    void Awake()
    {
        Instance = this;
    }

    public void StartGame()
    {
        //Hidding and closing the room from other players after the game started
        PhotonNetwork.CurrentRoom.IsVisible = false;
        PhotonNetwork.CurrentRoom.IsOpen = false;
    
        //Load level 1 On start game
        PhotonNetwork.LoadLevel(1);
    }


    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
        PhotonNetwork.AutomaticallySyncScene = true;
    }
    public override void OnJoinedLobby()
    {
        if (PhotonNetwork.NickName == "")
            MenuManagerScript.Instance.OpenMenu("enterName");
        else
            MenuManagerScript.Instance.OpenMenu("main");
    }

    public void CreateRoom()
    {
        //If the input is null or empty do nothing
        if (string.IsNullOrEmpty(roomNameInputField.text))
        {
            return;
        }

        PhotonNetwork.CreateRoom(roomNameInputField.text);
    }

    public void EnterPlayerName()
    {
        //If the input is null or empty do nothing
        if (string.IsNullOrEmpty(playerNameInputField.text))
        {
            return;
        }
        PhotonNetwork.NickName = playerNameInputField.text;
        MenuManagerScript.Instance.OpenMenu("main");
    }

    public override void OnJoinedRoom()
    {
        //Hide the room if the number of players in the current room is equal to MaxPlayers
        if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            PhotonNetwork.CurrentRoom.IsVisible = false;
            PhotonNetwork.CurrentRoom.IsOpen = false;
        }

        //Creating empty hashtables for player stats when entered the room
        Hashtable hash = new Hashtable();
        hash.Add("Deaths", 0);
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        hash = new Hashtable();
        hash.Add("Kills", 0);
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);

        MenuManagerScript.Instance.OpenMenu("game");
        roomNameText.text = PhotonNetwork.CurrentRoom.Name;
        Player[] players = PhotonNetwork.PlayerList;

        //After player joined the room delete all the items in playeListContent and rewrite it with new values
        foreach (Transform child in playerListContent)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < players.Length; i++)
        {
            Instantiate(playerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(players[i]);
        }

        //Set the button active only for the creator of the room
        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        //If the Master client switched show the Start button for the new master client
        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        //Display error message on room creation failed
        errorText.text = "Room creation failed" + message;
        MenuManagerScript.Instance.OpenMenu("error");
    }

    public override void OnCreatedRoom()
    {
        MenuManagerScript.Instance.OpenMenu("loading");
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        MenuManagerScript.Instance.OpenMenu("loading");
    }

    public override void OnLeftRoom()
    {
        MenuManagerScript.Instance.OpenMenu("main");
    }

    public void JoinRoom(RoomInfo info)
    {
        PhotonNetwork.JoinRoom(info.Name);
        MenuManagerScript.Instance.OpenMenu("loading");
    }

    public override void OnRoomListUpdate(List<RoomInfo> gameList)
    {
        //Clears the room list and rewrites it with the new values
        foreach (Transform trans in roomListContent)
        {
            Destroy(trans.gameObject);
        }

        for (int i = 0; i < gameList.Count; i++)
        {
            if (gameList[i].RemovedFromList)
                continue;
            Instantiate(roomListItemPrefab, roomListContent).GetComponent<RoomListItem>().SetUp(gameList[i]);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        //Add new player to the list in the playerListItemPrefab for the existing players
        Instantiate(playerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(newPlayer);
    }

    public void QuitGame()
    {
        //For quit button to close the application
        Application.Quit();
    }
}
