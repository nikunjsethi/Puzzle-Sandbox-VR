using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    // constants that can be used by other classes
    public static string ROOM_NAME = "PuzzleHub";
    public static int MAX_NUM_PLAYERS = 10;

    // serialized fields for this script
    [SerializeField] GameObject teleportMenu;
    [SerializeField] GameObject teleportButton;

    // public variables used by other scripts
    public int numCubesToReplace = 0;
    public bool playerInLevel = false;

    void Start()
    {
        ConnectToServer();
    }

    private void Update()
    {
        // if the number of cubes to place is less than 0, then all cubes are placed and make the teleport system active
        if (playerInLevel && (numCubesToReplace <= 0))
        {
            teleportMenu.SetActive(true); 
            teleportButton.SetActive(true);
        }
    }

    void ConnectToServer()
    {
        PhotonNetwork.ConnectUsingSettings();
        Debug.Log("Try connect to Server....");
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Server.");
        base.OnConnectedToMaster();
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = (byte)MAX_NUM_PLAYERS;
        roomOptions.IsVisible = true;
        roomOptions.IsOpen = true;
        PhotonNetwork.JoinOrCreateRoom(ROOM_NAME, roomOptions, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined a Room");
        base.OnJoinedRoom();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("A new player joined the room.");
        base.OnPlayerEnteredRoom(newPlayer);
    }
}
