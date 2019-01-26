using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NDream.AirConsole;
using Newtonsoft.Json.Linq;
using UnityEngine.UI;

public class AirController : MonoBehaviour {

    public struct Player {
        public Turtle turtle;
        public bool isReady;
        public int deviceID; // air console's ID for the device, stays the same if they disconnect and reconnect 
        public int playerID; // our internal player ID - the turtle number
        public bool isKeyboard;
    }

    public enum GameState {
        Waiting,
        InProgress
    }
    public GameState gameState;

    public GameObject playerPrefab;
    public Dictionary<int, Player> players = new Dictionary<int, Player> (); 
    TrackingCamera cam;
    public Image instructionsPanel;
    public TMPro.TMP_Text instructionsText;
    string code;
    public float secondsToWaitWhenAllPlayersAreReady = 10;
    float readyTimer;

    void Awake () {
        AirConsole.instance.onMessage += OnMessage;		
        AirConsole.instance.onReady += OnReady;		
        AirConsole.instance.onConnect += OnConnect;		
        AirConsole.instance.onDisconnect += OnDisconnect;
        cam = FindObjectOfType<TrackingCamera>();
        instructionsText.text = "Loading...";
    }

    string GetStatusText() {
        string join = "\nTo join, go to airconsole.com on your phone and enter code " + code + ".";
        if (players.Count == 0)
            return "Waiting for players." + join;
        if (!AllPlayersAreReady())
            return "The game will start when everyone taps \"Ready\"!" + join;
        if (gameState == GameState.Waiting)
            return "The game will start in " + Mathf.CeilToInt(readyTimer) + " seconds!" + join;
        return "Make for the waves!" + join;
    }

    // this is airconsole's ready message, not related to player.isReady
    void OnReady(string code){
        this.code = code;
        //Since people might be coming to the game from the AirConsole store once the game is live, 
        //I have to check for already connected devices here and cannot rely only on the OnConnect event 
        List<int> connectedDevices = AirConsole.instance.GetControllerDeviceIds();
        foreach (int deviceID in connectedDevices) {
            AddNewPlayer(deviceID);
        }
    }

    void OnConnect(int device){
        AddNewPlayer(device);
    }

    void OnDisconnect(int device) {
        Player player = players[device];
        if (cam)
            cam.RemoveCameraTarget(player.turtle.transform);
        Destroy(player.turtle.gameObject);
        players.Remove(device);
        ReMapPlayerIDs();
    }

    void BeginGame() {
        gameState = GameState.InProgress;
    }

    void Update() {
        instructionsText.text = GetStatusText();
        if (Input.GetKeyDown("space"))
            AddNewPlayer(-1, true);
        if (AllPlayersAreReady() && gameState == GameState.Waiting) {
            readyTimer -= Time.deltaTime;
            if (readyTimer <= 0)
                BeginGame();
        } else {
            readyTimer = secondsToWaitWhenAllPlayersAreReady;
        }
    }

    bool AllPlayersAreReady() {
        foreach (Player p in players.Values)
            if (!p.isReady)
                return false;
        return true;
    }

    void ReMapPlayerIDs() {
        AirConsole.instance.SetActivePlayers();
        foreach (Player player in players.Values) {
            if (player.isKeyboard)
                player.turtle.playerID = -1;
            player.turtle.playerID = AirConsole.instance.ConvertDeviceIdToPlayerNumber(player.deviceID) + 1;
        }
    }

    void AddNewPlayer(int deviceID, bool isKeyboard = false) {
        if (players.ContainsKey(deviceID)) {
            return;
        }
        //Instantiate player prefab, store device id + player script in a dictionary
        Player player = new Player();
        GameObject newPlayer = Instantiate(playerPrefab, transform.position, transform.rotation) as GameObject;
        player.turtle = newPlayer.GetComponent<Turtle>();
        player.isReady = false;
        player.deviceID = deviceID;
        player.isKeyboard = isKeyboard;
        if (isKeyboard)
            player.turtle.useKeyboardInput = true;
        players.Add(deviceID, player);
        if (cam)
            cam.AddCameraTarget(player.turtle.transform);
        ReMapPlayerIDs();
    }

    void OnMessage(int from, JToken data){
        //When I get a message, I check if it's from any of the devices stored in my device Id dictionary
        if (players.ContainsKey(from) && data["action"] != null) {
            //I forward the command to the relevant player script, assigned by device ID
            players[from].turtle.ButtonInput(data["action"].ToString());
        }
    }

    void OnDestroy () {
        if (AirConsole.instance != null) {
            AirConsole.instance.onMessage -= OnMessage;		
            AirConsole.instance.onReady -= OnReady;		
            AirConsole.instance.onConnect -= OnConnect;	
            AirConsole.instance.onDisconnect -= OnDisconnect;	
        }
    }
}
