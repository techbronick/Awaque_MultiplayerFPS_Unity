using System;
using System.Collections;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using System.IO;
using ExitGames.Client.Photon;
using Hashtable = ExitGames.Client.Photon.Hashtable;


public class RoomManager : MonoBehaviourPunCallbacks
{
    public static RoomManager Instance;

    private Coroutine timerCoroutine;

    [SerializeField]
    private byte maxPlayers;

    [SerializeField]
    private int matchLength;
    private int currentMatchTime;

    private string[] pickupbles;

    private bool gameStarting;
    private bool gameEnding;
    private bool gameOver;




    public override void OnCreatedRoom()
    {
        //Setting max Player for current Room
        PhotonNetwork.CurrentRoom.MaxPlayers = maxPlayers;
    }

        private IEnumerator GameStarting()
    {
        //Creating a hashtable to store a Game starting bool which will be accessed by the player's hud
        Hashtable hash = new Hashtable();
        gameStarting = true;
        hash.Add("GameStarting", gameStarting);

        //Adding the early created hash to room custom properties
        PhotonNetwork.CurrentRoom.SetCustomProperties(hash);
        yield return new WaitForSeconds(15f);
        gameStarting = false;

        //Clearing the hashtable from room custom properties
        hash.Clear();
        hash.Add("GameStarting", gameStarting);
        PhotonNetwork.CurrentRoom.SetCustomProperties(hash);

        //Call the initialize timer function
        InitializeTimer();
    }

   
    private void InitializeTimer()
    {
        currentMatchTime = matchLength;

        //Start the timer coroutine
        timerCoroutine = StartCoroutine(Timer());
    }
    private IEnumerator Timer()
    {
        yield return new WaitForSeconds(1f);

        //Creating a hashtable to store a Current match time variable which will be accessed by the player's hud
        Hashtable hash = new Hashtable();

        //Decrement the time variable
        currentMatchTime -= 1;

        //Add the time variable to the early created hash and store it in room custom properties
        hash.Add("CurrentMatchTime", currentMatchTime);
        PhotonNetwork.CurrentRoom.SetCustomProperties(hash);

        //If time ends
        if (currentMatchTime <= 0)
        {
            timerCoroutine = null;

            //Start Game Ending Coroutine
            StartCoroutine(GameEnding());
        }
        //If time didn't end
        else
        {
            //Start the coroutine again
            timerCoroutine = StartCoroutine(Timer());
        }
    }

    private IEnumerator GameEnding()
    {
        //Create a hashtable to store game ending bool which will be accessed by players HUD
        Hashtable hash = new Hashtable();
        gameEnding = true;
        hash.Add("GameEnding", gameEnding);
        
        //Storing the early created hashtable to room custom properties
        PhotonNetwork.CurrentRoom.SetCustomProperties(hash);
        yield return new WaitForSeconds(5f);
        gameOver = true;
        
        //Sending a new bool to room custom properties which will end the game
        hash.Clear();
        hash.Add("GameOver", gameOver);
        PhotonNetwork.CurrentRoom.SetCustomProperties(hash);
    }




    private void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        Instance = this;
    }

    private void Update()
    {
        //If the current room exists, access the GameOver in room custom properties and if it's value is true leave the room
        if (PhotonNetwork.CurrentRoom != null)
            if ((Convert.ToBoolean(PhotonNetwork.CurrentRoom.CustomProperties["GameOver"])) == true && PhotonNetwork.InRoom)
            {
                PhotonNetwork.LeaveRoom();
            }
    }

    public override void OnLeftRoom()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene(0);
        Destroy(gameObject);
    }

    public override void OnEnable()
    {
        base.OnEnable();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        SceneManager.sceneLoaded -= OnSceneLoaded;

    }


    void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (scene.buildIndex == 1)
        {
            //If the Scene Loaded and this player is master client start the game and instantiate the pickupbles at random spawnpoints
            PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerManager"), Vector3.zero, Quaternion.identity);
            if (PhotonNetwork.IsMasterClient)
            {
                PickupSpawnPoint[] pickupSpawnPoints = SpawnManager.Instance.GetPickupSpawnPoints();
                pickupbles = new string[] { "Weapon_Blaster 2", "Weapon_Launcher 1", "Weapon_Shotgun 1", "Pickup_Health" };
                foreach (var point in pickupSpawnPoints)
                {
                    PhotonNetwork.InstantiateRoomObject(Path.Combine("PhotonPrefabs", pickupbles[UnityEngine.Random.Range(0, pickupbles.Length)]), new Vector3(point.transform.position.x, point.transform.position.y + 0.8f, point.transform.position.z), Quaternion.identity);
                }
                StartCoroutine(GameStarting());
            }
        }
    }
}
