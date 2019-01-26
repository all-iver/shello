using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NDream.AirConsole;
using Newtonsoft.Json.Linq;
using UnityEngine.UI;

public class AirController : MonoBehaviour {

    public class Player {
        public Turtle turtle;
        public bool isReady;
        public int deviceID; // air console's ID for the device, stays the same if they disconnect and reconnect 
        public bool isKeyboard;
        public int place = -1; // the place this turtle finished in the race
        public bool isInCurrentRace;

        public void GetReadyForNextRace() {
            place = -1;
            isReady = false;
            isInCurrentRace = false;
        }

        public void SetConnected() {
            turtle.gameObject.SetActive(true);
        }

        public void SetDisconnected() {
            turtle.gameObject.SetActive(false);
        }

        public bool IsConnected() {
            return turtle.gameObject.activeSelf;
        }

        public bool hasFinished {
            get {
                return place != -1;
            }
        }
    }

    public enum GameState {
        WaitingToStart, // waiting for all players to connect and ready up
        InProgress, // the game has started and nobody has crossed the finish line
        WaitingToFinish // at least one player has crossed the finish line
    }
    GameState gameState;

    public GameObject playerPrefab;
    public Dictionary<int, Player> players = new Dictionary<int, Player> (); 
    TrackingCamera cam;
    public Image instructionsPanel;
    public TMPro.TMP_Text instructionsText;
    string code;
    public float secondsToWaitWhenAllPlayersAreReady = 10;
    public float secondsToWaitForPlayersToFinish = 30;
    float readyTimer, finishTimer;

    void Awake () {
        AirConsole.instance.onMessage += OnMessage;		
        AirConsole.instance.onReady += OnReady;		
        AirConsole.instance.onConnect += OnConnect;		
        AirConsole.instance.onDisconnect += OnDisconnect;
        cam = FindObjectOfType<TrackingCamera>();
        instructionsText.text = "Loading...";
        gameState = GameState.WaitingToStart;
    }

    string GetStatusText() {
        string join = "\nTo join, go to airconsole.com on your phone and enter code " + code + ".";
        if (GetConnectedPlayerCount() == 0)
            return "Waiting for players." + join;
        if (!AllPlayersAreReady())
            return "The game will start when everyone hatches!" + join;
        if (gameState == GameState.WaitingToStart)
            return "The game will start in " + Mathf.CeilToInt(readyTimer) + " seconds!" + join;
        if (gameState == GameState.WaitingToFinish)
            return "You have " + Mathf.CeilToInt(finishTimer) + " seconds to reach the ocean!" + join;
        return "Make for the waves!" + join;
    }

    // this is airconsole's ready message, not related to player.isReady
    void OnReady(string code){
        this.code = code;
        //Since people might be coming to the game from the AirConsole store once the game is live, 
        //I have to check for already connected devices here and cannot rely only on the OnConnect event 
        List<int> connectedDevices = AirConsole.instance.GetControllerDeviceIds();
        foreach (int deviceID in connectedDevices)
            OnConnect(deviceID);
    }

    void OnConnect(int device){
        Debug.Log("on connect for " + device);
        AddNewPlayer(device, device == -1);
        Player player = players[device];
        player.SetConnected();
        if (cam)
            cam.AddCameraTarget(player.turtle.transform);
    }

    void OnDisconnect(int device) {
        Debug.Log("on disconnect for " + device);
        Player player = players[device];
        player.SetDisconnected();
        if (cam)
            cam.RemoveCameraTarget(player.turtle.transform);
    }

    Dictionary<string, string> GetGameStateData() {
        Dictionary<string, string> data = new Dictionary<string, string>();
        data.Add("action", "gameState");
        if (gameState == GameState.WaitingToStart)
            data.Add("view", "IntroView");
        else
            data.Add("view", "PlayView");
        return data;
    }

    void BeginGame() {
        foreach (Player player in players.Values)
            if (player.IsConnected() && player.isReady)
                player.isInCurrentRace = true;
        gameState = GameState.InProgress;
        AirConsole.instance.Broadcast(GetGameStateData());
    }

    void FinishGame() {
        gameState = GameState.WaitingToStart;
        foreach (Player player in players.Values) {
            player.GetReadyForNextRace();
            player.turtle.transform.position = Vector3.zero;
        }
        AirConsole.instance.Broadcast(GetGameStateData());
    }

    public void OnTurtleCrossedFinishLine(Turtle turtle) {
        Debug.Log("Called");
        foreach (Player player in players.Values) {
            if (player.turtle == turtle) {
                player.place = GetNextFinishingPlace();
                Debug.Log(string.Format("Turtle {0} finished with place {1}", player.deviceID, player.place));
                if (AllPlayersHaveFinished())
                    FinishGame();
                else
                    gameState = GameState.WaitingToFinish;
            }
        }
    }

    void Update() {
        instructionsText.text = GetStatusText();

        // testing keyboard player stuff
        if (gameState == GameState.WaitingToStart && Input.GetKeyDown("r") && players.ContainsKey(-1))
            players[-1].isReady = !players[-1].isReady;
        if (Input.GetKeyDown("c") && (!players.ContainsKey(-1) || !players[-1].IsConnected()))
            OnConnect(-1);
        if (players.ContainsKey(-1) && Input.GetKeyDown("d") && players[-1].IsConnected())
            OnDisconnect(-1);

        if (players.Count > 0 && AllPlayersAreReady() && gameState == GameState.WaitingToStart) {
            readyTimer -= Time.deltaTime;
            if (readyTimer <= 0)
                BeginGame();
        } else {
            readyTimer = secondsToWaitWhenAllPlayersAreReady;
        }
        if (gameState == GameState.WaitingToFinish) {
            finishTimer -= Time.deltaTime;
            if (finishTimer <= 0 || AllPlayersHaveFinished())
                FinishGame();
        } else {
            finishTimer = secondsToWaitForPlayersToFinish;
        }
    }

    bool AllPlayersAreReady() {
        foreach (Player p in players.Values)
            if (p.IsConnected() && !p.isReady)
                return false;
        return true;
    }

    bool AllPlayersHaveFinished() {
        foreach (Player p in players.Values)
            if (p.IsConnected() && p.isInCurrentRace && !p.hasFinished)
                return false;
        return true;    
    }

    int GetNextFinishingPlace() {
        int place = 1;
        foreach (Player player in players.Values)
            if (player.isInCurrentRace && player.hasFinished)
                place ++;
        return place;
    }

    int GetConnectedPlayerCount() {
        int count = 0;
        foreach (Player player in players.Values)
            if (player.IsConnected())
                count ++;
        return count;
    }

    void AddNewPlayer(int deviceID, bool isKeyboard = false) {
        if (players.ContainsKey(deviceID)) {
            return;
        }
        //Instantiate player prefab, store device id + player script in a dictionary
        Player player = new Player();
        GameObject newPlayer = Instantiate(playerPrefab, transform.position, transform.rotation) as GameObject;
        player.turtle = newPlayer.GetComponent<Turtle>();
        player.deviceID = deviceID;
        player.isKeyboard = isKeyboard;
        if (isKeyboard)
            player.turtle.useKeyboardInput = true;
        players.Add(deviceID, player);
    }

    void OnMessage(int from, JToken data){
        if (!players.ContainsKey(from) || data["action"] == null)
            return;
        if (gameState == GameState.WaitingToStart) {
            if (data["action"].ToString() == "ready")
                players[from].isReady = true;
            if (data["action"].ToString() == "unready")
                players[from].isReady = false;
        } else {
            if (!players[from].hasFinished) {
                players[from].turtle.ButtonInput(data["action"].ToString());
            }
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
