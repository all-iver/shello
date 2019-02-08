using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NDream.AirConsole;
using Newtonsoft.Json.Linq;
using UnityEngine.UI;
using System.Linq;

public class AirController : MonoBehaviour {

    public class Player {
        public enum PlayerState {
            Initial,
            InEgg,
            Hatched,
            InGame,
            Finished,
            TooManyPlayers,
            Dropped
        }

        public PlayerState state = PlayerState.Initial;
        public int eggIndex = -1;
        public GameObject egg;
        public Turtle turtle;
        public int deviceID; // air console's ID for the device, stays the same if they disconnect and reconnect 
        public bool isKeyboard;
        public bool isInCurrentRace; // in case the player drops, this will let us know if they were in the race
        public float finishTime;
        bool _isTheBest;
        public bool isTheBest {
            get { return _isTheBest; }
            set { 
                _isTheBest = value;
                turtle.SetHasBow(_isTheBest);
            }
        }

        public bool IsAbleToPlay() {
            return state != Player.PlayerState.Dropped && state != Player.PlayerState.TooManyPlayers;
        }

        public void GetReadyForNextRace() {
            state = PlayerState.Initial;
            finishTime = -1;
            turtle.GetComponentInChildren<TurtleTrailManager>().sandTrail.Clear();
            turtle.GetComponentInChildren<TurtleTrailManager>().waterTrail.Clear();
            turtle.GetComponentInChildren<TurtleTrailManager>().waterTrail.enabled = false;
        }

        public bool HasEgg() {
            return eggIndex >= 0;
        }

        public void ShowTurtle() {
            turtle.gameObject.SetActive(true);
        }

        public void HideTurtle() {
            turtle.gameObject.SetActive(false);
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
    public TMPro.TMP_Text statusText, joiningText, excitingText;
    string code;
    public float secondsToWaitWhenAllPlayersAreHatched = 10;
    public float secondsToWaitForPlayersToFinish = 30;
    float hatchTimer, finishTimer;
    Nest nest;
    float currentGameStartTime;
    public Transform startCameraPosition;
    public Sprite[] turtleBodies;
    public Leaderboard leaderboard;
    public float leaderboardUpdateInterval = 0.5f;
    float leaderboardUpdateTimer;
    public AudioSource introMusic, raceMusic;
    string logoText = "MAKE for the WAVES!";

    void Awake () {
        AirConsole.instance.onMessage += OnMessage;		
        AirConsole.instance.onReady += OnReady;		
        AirConsole.instance.onConnect += OnConnect;		
        AirConsole.instance.onDisconnect += OnDisconnect;
        cam = FindObjectOfType<TrackingCamera>();
        statusText.text = "Loading...";
        joiningText.text = "";
        gameState = GameState.WaitingToStart;
        nest = FindObjectOfType<Nest>();

        ResetGame();
    }

    string GetJoiningText() {
        if (string.IsNullOrEmpty(code) || code == "0")
            return "To join, go to airconsole.com and enter the code.";
        return "To join, go to airconsole.com on your phone and enter code " + code + ".";
    }

    string GetStatusText() {
        if (GetConnectedPlayerCount() == 0)
            return "Waiting for players...";
        if (gameState == GameState.WaitingToStart && !AllPlayersAreHatched())
            return "The game will start when everyone hatches.\nTap the egg on your phone!";
        if (gameState == GameState.WaitingToStart)
            return "Get ready! The game is starting in " + Mathf.CeilToInt(hatchTimer) + " seconds!";
        if (gameState == GameState.WaitingToFinish)
            return "Hurry up! " + Mathf.CeilToInt(finishTimer) + "s";
        return "";
    }

    // this is airconsole's ready message, not related to the player's ready/hatched state
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
        if (!players.ContainsKey(device))
            AddNewPlayer(device, device == -1);
        Player player = players[device];
        // initialize this player into the nest.  note that this player could have already existed but just dropped
        // and reconnected.  either way we put them into an egg.
        PutPlayerInEgg(player);
        // if we're already in a game, tell the controller it needs to wait
        if (gameState != GameState.WaitingToStart) {
            Dictionary<string, string> data = new Dictionary<string, string>();
            data.Add("action", "gameState");
            data.Add("view", "WaitingView");
            AirConsole.instance.Message(player.deviceID, data);
        }
    }

    void OnDisconnect(int device) {
        Debug.Log("on disconnect for " + device);
        Player player = players[device];
        player.state = Player.PlayerState.Dropped;
        ReleaseEgg(player);
        player.HideTurtle();
        if (cam)
            cam.RemoveCameraTarget(player.turtle.transform);
    }

    void PutPlayerInEgg(Player player) {
        player.GetReadyForNextRace();
        player.HideTurtle();
        // the player may already have an egg if they have hatched and are just waiting for the game to play - in that
        // case we put them back into the same egg they hatched from
        if (!player.HasEgg()) {
            try {
                player.eggIndex = nest.ClaimRandomEgg();
            } catch {
                player.state = Player.PlayerState.TooManyPlayers;
                return;
            }
        }
        player.state = Player.PlayerState.InEgg;
        player.egg = nest.GetEggAtIndex(player.eggIndex);
        player.egg.GetComponent<Egg>().Reset();
    }

    // this makes it so a player is ready to play and their turtle is visible rather than their egg
    void HatchPlayer(Player player) {
        if (player.state != Player.PlayerState.InEgg)
            throw new System.Exception("Player is not in an egg");
        player.state = Player.PlayerState.Hatched;
        player.turtle.GetComponent<SpriteRenderer>().sprite = turtleBodies[Random.Range(0, turtleBodies.Length)];
        player.ShowTurtle();
        player.turtle.transform.rotation = Quaternion.identity;
        player.egg.GetComponent<Egg>().Hatch(player.turtle.gameObject);
        // send the controller what their turtle color and number is
        if (!player.isKeyboard) {
            Dictionary<string, string> data = new Dictionary<string, string>();
            data.Add("action", "turtle");
            data.Add("color", player.turtle.GetComponent<SpriteRenderer>().sprite.name.Split('_')[1]);
            data.Add("number", "" + player.deviceID);
            data.Add("showBow", player.isTheBest ? "true" : "false");
            AirConsole.instance.Message(player.deviceID, data);
        }
    }

    void ReleaseEgg(Player player) {
        if (player.state == Player.PlayerState.InEgg)
            throw new System.Exception("Can't release an egg when the player is in it");
        if (!player.HasEgg())
            return;
        nest.ReleaseEgg(player.eggIndex);
        player.eggIndex = -1;
        player.egg = null;
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

    void ResetGame() {
        StopAllCoroutines();
        nest.ResetAI(); // remove all the AI and release all the eggs
        // put everybody who hasn't dropped into an egg
        gameState = GameState.WaitingToStart;
        foreach (Player player in players.Values) {
            if (player.state == Player.PlayerState.Dropped)
                continue;
            if (player.HasEgg())
                ReleaseEgg(player);
            PutPlayerInEgg(player);
            player.isInCurrentRace = false;
        }
        if (cam) {
            cam.targets.Clear();
            cam.AddCameraTarget(startCameraPosition);
        }
        excitingText.text = logoText;
    }

    IEnumerator BlinkExcitingText(string text, int times = 5, float onDuration = 0.5f, float offDuration = 0.1f) {
        for (int i = 0; i < times; i++) {
            excitingText.text = text;
            yield return new WaitForSeconds(onDuration);
            excitingText.text = "";
            yield return new WaitForSeconds(offDuration);
        }
    }

    void BeginGame() {
        if (cam)
            cam.targets.Clear();
        // all players currently ready/hatched are considered in the game
        foreach (Player player in players.Values) {
            if (player.state == Player.PlayerState.Hatched) {
                player.state = Player.PlayerState.InGame;
                player.isInCurrentRace = true; // this will keep track of whether they were playing even if they drop
                if (cam)
                    cam.AddCameraTarget(player.turtle.transform);
            }
        }
        currentGameStartTime = Time.time;
        gameState = GameState.InProgress;
        // tell the various devices to let the players start moving
        AirConsole.instance.Broadcast(GetGameStateData());
        StartCoroutine(BlinkExcitingText(logoText));
        introMusic.Stop();
        StartCoroutine(PlayRaceMusic());
        nest.StartSpawning();
    }

    IEnumerator PlayRaceMusic() {
        yield return new WaitForSeconds(2);
        if (gameState == GameState.InProgress)
            raceMusic.Play();
    }

    void FinishGame() {
        UpdateLeaderboard(true);
        ResetGame();
        // tell the various devices to switch back to the intro screen
        AirConsole.instance.Broadcast(GetGameStateData());
        raceMusic.Stop();
        introMusic.Play();
    }

    public void OnTurtleCrossedFinishLine(Turtle turtle) {
        foreach (Player player in players.Values) {
            if (player.turtle == turtle) {
                // player.HideTurtle(); // this hides the trails too :/
                player.turtle.transform.position = new Vector3(0, -5000, 0); // hack!  get them away
                GameSounds.instance.PlayWinSound();
                player.finishTime = Time.time - currentGameStartTime;
                player.state = Player.PlayerState.Finished; // set this after doing the above!
                Debug.Log(string.Format("Turtle {0} finished at time {1}", player.deviceID, player.finishTime));
                if (AllPlayersHaveFinished())
                    FinishGame();
                else
                    gameState = GameState.WaitingToFinish;
                if (cam) {
                    try {
                        cam.RemoveCameraTarget(player.turtle.transform);
                    } catch {
                    }
                }
            }
        }
    }

    void Update() {
        statusText.text = GetStatusText();
        joiningText.text = GetJoiningText();

        // testing keyboard player stuff
        if (gameState == GameState.WaitingToStart && Input.GetKeyDown("r") && players.ContainsKey(-1) 
                && players[-1].state == Player.PlayerState.InEgg)
            HatchPlayer(players[-1]);
        if (gameState == GameState.WaitingToStart && Input.GetKeyDown("u") && players.ContainsKey(-1) 
                && players[-1].state == Player.PlayerState.Hatched)
            PutPlayerInEgg(players[-1]);
        if (Input.GetKeyDown("c") && (!players.ContainsKey(-1) || players[-1].state == Player.PlayerState.Dropped))
            OnConnect(-1);
        if (players.ContainsKey(-1) && Input.GetKeyDown("d") && players[-1].state != Player.PlayerState.Dropped)
            OnDisconnect(-1);
        // end test keyboard stuff

        // if everyone has hatched then start the game
        if (GetConnectedPlayerCount() > 0 && AllPlayersAreHatched() && gameState == GameState.WaitingToStart) {
            float oldHatchTimer = hatchTimer;
            hatchTimer -= Time.deltaTime;
            if (oldHatchTimer > 3 && hatchTimer <= 3) {
                introMusic.Stop();
                GameSounds.instance.PlayBeep();
            }
            if (oldHatchTimer > 2 && hatchTimer <= 2)
                GameSounds.instance.PlayBeep();
            if (oldHatchTimer > 1 && hatchTimer <= 1)
                GameSounds.instance.PlayBeep();
            if (hatchTimer <= 0) {
                GameSounds.instance.PlayGoBeep();
                BeginGame();
            }
        } else {
            hatchTimer = secondsToWaitWhenAllPlayersAreHatched;
        }

        // if everyone who was playing has dropped then finish the game
        if (gameState == GameState.InProgress && GetInGamePlayerCount() == 0)
            FinishGame();

        // if everyone has finished or the timer is up then finish the game
        if (gameState == GameState.WaitingToFinish) {
            finishTimer -= Time.deltaTime;
            if (finishTimer <= 0 || AllPlayersHaveFinished())
                FinishGame();
        } else {
            finishTimer = secondsToWaitForPlayersToFinish;
        }

        // update the leaderboard
        if (gameState == GameState.InProgress || gameState == GameState.WaitingToFinish) {
            leaderboardUpdateTimer += Time.deltaTime;
            if (leaderboardUpdateTimer >= leaderboardUpdateInterval) {
                leaderboardUpdateTimer = 0;
                UpdateLeaderboard(false);
            }
        }
    }

    float GetRankForPlayer(Player p) {
        if (p.state == Player.PlayerState.Finished)
            return p.finishTime;
        if (p.state == Player.PlayerState.InGame)
            return -p.turtle.transform.position.y + 5000;
        return 9999999;
    }

    void UpdateLeaderboard(bool setBow) {
        if (setBow) {
            foreach (Player p in players.Values)
                p.isTheBest = false; 
        }
        Player[] sortedPlayers = players.Values.Where(
                    p => p.state == Player.PlayerState.Finished || p.state == Player.PlayerState.InGame)
                .OrderBy(p => GetRankForPlayer(p)).ToArray();
        leaderboard.SetNumPlayers(sortedPlayers.Length);
        for (int i = 0; i < sortedPlayers.Length; i++) {
            if (i == 0 && setBow)
                sortedPlayers[i].isTheBest = true;
            leaderboard.SetRank(i, sortedPlayers[i].turtle.GetComponent<SpriteRenderer>().sprite, 
                    sortedPlayers[i].deviceID, sortedPlayers[i].finishTime, sortedPlayers[i].isTheBest, 
                    sortedPlayers[i].isKeyboard);
        }
    }

    // returns true if all non-dropped, able-to-play players are hatched
    bool AllPlayersAreHatched() {
        foreach (Player p in players.Values) {
            if (!p.IsAbleToPlay())
                continue;
            if (p.state != Player.PlayerState.Hatched)
                return false;
        }
        return true;
    }

    // returns true if all non-dropped players who are in the current race are finished -- anyone not InGame is either
    // Finished or not in the race
    bool AllPlayersHaveFinished() {
        foreach (Player p in players.Values) {
            if (p.state == Player.PlayerState.InGame)
                return false;
        }
        return true;    
    }

    bool AnyPlayersHaveFinished() {
        foreach (Player p in players.Values)
            if (p.state == Player.PlayerState.Finished)
                return true;
        return false;
    }

    int GetConnectedPlayerCount() {
        int count = 0;
        foreach (Player player in players.Values)
            if (player.state != Player.PlayerState.Dropped)
                count ++;
        return count;
    }

    // returns the number of users who are InGame (not finished, not dropped)
    int GetInGamePlayerCount() {
        int count = 0;
        foreach (Player player in players.Values)
            if (player.state == Player.PlayerState.InGame)
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
        player.turtle.playerID = deviceID;
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
            if (data["action"].ToString() == "ready" && players[from].state == Player.PlayerState.InEgg)
                HatchPlayer(players[from]);
            if (data["action"].ToString() == "unready" && players[from].state == Player.PlayerState.Hatched)
                PutPlayerInEgg(players[from]);
        } else {
            if (players[from].state == Player.PlayerState.InGame) {
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
