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
            InRace,
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
            ClearTrails();
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

        public void ClearTrails() {
            turtle.GetComponentInChildren<TurtleTrailManager>().sandTrail.Clear();
            turtle.GetComponentInChildren<TurtleTrailManager>().waterTrail.Clear();
            turtle.GetComponentInChildren<TurtleTrailManager>().waterTrail.enabled = false;
        }
    }

    public enum GameState {
        WaitingToStart, // waiting for all players to connect and ready up
        InProgress, // a race has started and nobody has crossed the finish line
        WaitingToFinish, // at least one player has crossed the finish line
        ShowingWinScreen // race is over, showing the win screen
    }
    GameState gameState;

    public GameObject playerPrefab;
    public Dictionary<int, Player> players = new Dictionary<int, Player> (); 
    TrackingCamera cam;
    public Image instructionsPanel;
    public TMPro.TMP_Text statusText, joiningText, excitingText, levelName, levelNumber, waitingReminder, 
            countdownText;
    string code;
    public float secondsToWaitWhenAllPlayersAreHatched = 10;
    public float secondsToWaitForPlayersToFinish = 30;
    float hatchTimer, finishTimer;
    Nest nest;
    float currentRaceStartTime;
    public Sprite[] turtleBodies;
    // public Leaderboard leaderboard;
    // public float leaderboardUpdateInterval = 0.5f;
    // float leaderboardUpdateTimer;
    public AudioSource introMusic, raceMusic;
    string logoText = "MAKE for the WAVES!";
    public WinScreen winScreen;
    public List<Level> levelPrefabs;
    public int levelIndex = 0;
    Level level;

    void Awake () {
        AirConsole.instance.onMessage += OnMessage;		
        AirConsole.instance.onReady += OnReady;		
        AirConsole.instance.onConnect += OnConnect;		
        AirConsole.instance.onDisconnect += OnDisconnect;
        cam = FindObjectOfType<TrackingCamera>();
        statusText.text = "Loading...";
        joiningText.text = "";
        excitingText.gameObject.SetActive(false);
        countdownText.gameObject.SetActive(false);
        gameState = GameState.WaitingToStart;

        LoadLevel();
    }

    string GetJoiningText() {
        if (!Application.isEditor)
            return "";
        if (string.IsNullOrEmpty(code) || code == "0")
            return "To join, go to airconsole.com and enter the code.";
        return "To join, go to airconsole.com on your phone and enter code " + code + ".";
    }

    string GetStatusText() {
        if (GetConnectedPlayerCount() == 0 && gameState == GameState.WaitingToStart)
            return "Waiting for players...";
        if (gameState == GameState.WaitingToStart && !AllPlayersAreHatched())
            return "Tap the egg on your phone to hatch!";
        if (gameState == GameState.WaitingToStart)
            return "Get ready! The race is starting in " + Mathf.CeilToInt(hatchTimer) + " seconds!";
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
        // and reconnected.  if we're waiting to start the race, put them in an egg.
        if (gameState == GameState.WaitingToStart) {
            PutPlayerInEgg(player);
        } else {
            player.HideTurtle();
            player.state = Player.PlayerState.Initial;
            // if we're already in a game, tell the controller it needs to wait
            if (gameState != GameState.WaitingToStart) {
                Dictionary<string, string> data = new Dictionary<string, string>();
                data.Add("action", "gameState");
                data.Add("view", "WaitingView");
                AirConsole.instance.Message(player.deviceID, data);
            }
        }
    }

    void OnDisconnect(int device) {
        Debug.Log("on disconnect for " + device);
        Player player = players[device];
        player.state = Player.PlayerState.Dropped;
        if (gameState == GameState.WaitingToStart)
            ReleaseEgg(player);
        player.HideTurtle();
        try {
            cam.RemoveCameraTarget(player.turtle.transform);
        } catch {
        }
    }

    void PutPlayerInEgg(Player player) {
        if (gameState != GameState.WaitingToStart)
            throw new System.Exception("Wrong game state for putting player in an egg");
        player.GetReadyForNextRace();
        player.HideTurtle();
        // the player may already have an egg if they have hatched and are just waiting for the game to play - in that
        // case we put them back into the same egg they hatched from
        if (!player.HasEgg()) {
            try {
                player.eggIndex = nest.ClaimRandomEgg();
            } catch {
                Debug.Log(string.Format("Too many players for player {0}", player.deviceID));
                player.state = Player.PlayerState.TooManyPlayers;
                if (!player.isKeyboard) {
                    Dictionary<string, string> data = new Dictionary<string, string>();
                    data.Add("action", "gameState");
                    data.Add("view", "WaitingView");
                    AirConsole.instance.Message(player.deviceID, data);
                }
                return;
            }
        }
        player.state = Player.PlayerState.InEgg;
        player.egg = nest.GetEggAtIndex(player.eggIndex);
        player.egg.GetComponent<Egg>().Reset();
        Debug.Log(string.Format("Put player {0} in an egg", player.deviceID));
        if (!player.isKeyboard) {
            Dictionary<string, string> data = new Dictionary<string, string>();
            data.Add("action", "gameState");
            data.Add("view", "IntroView");
            AirConsole.instance.Message(player.deviceID, data);
        }
    }

    // this makes it so a player is ready to play and their turtle is visible rather than their egg
    void HatchPlayer(Player player) {
        if (player.state != Player.PlayerState.InEgg)
            throw new System.Exception("Player is not in an egg");
        Debug.Log(string.Format("Player {0} hatched", player.deviceID));
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

    void LoadLevel() {
        StopAllCoroutines();

        // cleanup
        if (nest)
            nest.ResetAI(); // remove all the AI and release all their eggs
        // put everybody who hasn't dropped into an egg
        gameState = GameState.WaitingToStart;
        foreach (Player player in players.Values) {
            if (player.state == Player.PlayerState.Dropped)
                continue;
            if (player.HasEgg())
                ReleaseEgg(player);
            player.isInCurrentRace = false;
        }

        // instantiate the level prefab and find the nest
        if (levelIndex >= 0) {
            if (level)
                Destroy(level.gameObject);
            level = Instantiate(levelPrefabs[levelIndex]);
        }
        nest = FindObjectOfType<Nest>();
        if (!nest)
            throw new System.Exception("Could not find a nest");
        nest.ResetAI(); // needs to happen for game jam code related reasons
        if (cam) {
            cam.targets.Clear();
            cam.AddCameraTarget(nest.transform);
            cam.Snap();
        }

        // put any non-dropped players into eggs
        foreach (Player player in players.Values) {
            if (player.state == Player.PlayerState.Dropped)
                continue;
            PutPlayerInEgg(player);
        }
    }

    IEnumerator BlinkExcitingText(string text, int times = 5, float onDuration = 0.5f, float offDuration = 0.1f) {
        excitingText.gameObject.SetActive(true);
        for (int i = 0; i < times; i++) {
            excitingText.text = text;
            yield return new WaitForSeconds(onDuration);
            excitingText.text = "";
            yield return new WaitForSeconds(offDuration);
        }
        excitingText.gameObject.SetActive(false);
    }

    void StartRace() {
        if (cam)
            cam.targets.Clear();
        // all players currently ready/hatched are considered in the game
        foreach (Player player in players.Values) {
            if (player.state == Player.PlayerState.Hatched) {
                player.state = Player.PlayerState.InRace;
                player.isInCurrentRace = true; // this will keep track of whether they were playing even if they drop
                if (cam)
                    cam.AddCameraTarget(player.turtle.transform);
            }
        }
        currentRaceStartTime = Time.time;
        gameState = GameState.InProgress;
        // tell the various devices to let the players start moving
        try {
            AirConsole.instance.Broadcast(GetGameStateData());
        } catch (AirConsole.NotReadyException) {
            // only keyboard players exist
        }
        StartCoroutine(BlinkExcitingText(logoText));
        introMusic.Stop();
        StartCoroutine(PlayRaceMusic());
        nest.StartSpawningAI();
    }

    IEnumerator PlayRaceMusic() {
        yield return new WaitForSeconds(2);
        if (gameState == GameState.InProgress)
            raceMusic.Play();
    }

    IEnumerator ShowWinScreen() {
        gameState = GameState.ShowingWinScreen;
        UpdateWinScreenAndBow();

        // broadcast victory view
        Dictionary<string, string> data = new Dictionary<string, string>();
        data.Add("action", "gameState");
        data.Add("view", "VictoryView");
        try {
            AirConsole.instance.Broadcast(data);
        } catch (AirConsole.NotReadyException) {
            // only keyboard players exist
        }

        yield return winScreen.Show();
        if (levelIndex >= 0) {
            levelIndex ++;
            if (levelIndex >= levelPrefabs.Count)
                levelIndex = 0;
        }
        LoadLevel();
        // tell the various devices to switch back to the intro screen
        try {
            AirConsole.instance.Broadcast(GetGameStateData());
        } catch (AirConsole.NotReadyException) {
            // only keyboard players exist
        }
    }

    void FinishRace() {
        raceMusic.Stop();
        introMusic.Play();
        StartCoroutine(ShowWinScreen());
    }

    public void OnTurtleCrossedFinishLine(Turtle turtle) {
        foreach (Player player in players.Values) {
            if (player.turtle == turtle) {
                // player.HideTurtle(); // this hides the trails too :/
                player.turtle.transform.position = new Vector3(0, -5000, 0); // hack!  get them away
                GameSounds.instance.PlayWinSound();
                player.finishTime = Time.time - currentRaceStartTime;
                // get their rank
                int rank = 1;
                foreach (Player p in players.Values)
                    if (p.state == Player.PlayerState.Finished)
                        rank ++;
                player.state = Player.PlayerState.Finished; // set this after doing the above!
                Debug.Log(string.Format("Turtle {0} finished at time {1}", player.deviceID, player.finishTime));
                if (AllPlayersHaveFinished())
                    FinishRace();
                else
                    gameState = GameState.WaitingToFinish;
                if (cam) {
                    try {
                        cam.RemoveCameraTarget(player.turtle.transform);
                    } catch {
                    }
                }
                // send the turtle their time and place
                if (!player.isKeyboard) {
                    Dictionary<string, string> data = new Dictionary<string, string>();
                    data.Add("action", "gameState");
                    data.Add("view", "VictoryView");
                    AirConsole.instance.Message(player.deviceID, data);
                    data = new Dictionary<string, string>();
                    data.Add("action", "turtle");
                    data.Add("time", string.Format("{0:F2}s", player.finishTime));
                    data.Add("place", "" + rank);
                    data.Add("showBow", (rank == 1) ? "true" : "false");
                    AirConsole.instance.Message(player.deviceID, data);
                }
            }
        }
    }

    void UpdateUI() {
        statusText.text = GetStatusText();

        // joiningText.text = GetJoiningText();
        joiningText.gameObject.SetActive(false);

        if (gameState == GameState.WaitingToStart) {
            if (level) {
                levelName.text = level.displayName;
                levelNumber.text = "LEVEL " + (levelIndex + 1);
            } else {
                levelName.text = "";
                levelNumber.text = "";
            }
        } else {
            levelName.text = "";
            levelNumber.text = "";
        }

        if (gameState == GameState.WaitingToStart && AllPlayersAreHatched() && GetConnectedPlayerCount() >= 1
                && hatchTimer <= 5) {
            countdownText.gameObject.SetActive(true);
            countdownText.text = "" + Mathf.CeilToInt(hatchTimer);
        } else {
            countdownText.gameObject.SetActive(false);
        }

        if (gameState == GameState.WaitingToStart && !AllPlayersAreHatched()) {
            waitingReminder.gameObject.SetActive(true);
        } else {
            waitingReminder.gameObject.SetActive(false);
        }
    }

    void Update() {
        UpdateUI();

        // testing keyboard player stuff
        if (gameState == GameState.WaitingToStart && Input.GetKeyDown("r") && players.ContainsKey(-1) 
                && players[-1].state == Player.PlayerState.InEgg)
            HatchPlayer(players[-1]);
        // if (gameState == GameState.WaitingToStart && Input.GetKeyDown("u") && players.ContainsKey(-1) 
        //         && players[-1].state == Player.PlayerState.Hatched)
        //     PutPlayerInEgg(players[-1]);
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
                StartRace();
            }
        } else {
            hatchTimer = secondsToWaitWhenAllPlayersAreHatched;
        }

        // if everyone who was playing has dropped then finish the game
        if (gameState == GameState.InProgress && GetInRacePlayerCount() == 0)
            FinishRace();

        // if everyone has finished or the timer is up then finish the game
        if (gameState == GameState.WaitingToFinish) {
            finishTimer -= Time.deltaTime;
            if (finishTimer <= 0 || AllPlayersHaveFinished())
                FinishRace();
        } else {
            finishTimer = secondsToWaitForPlayersToFinish;
        }

        // // update the leaderboard
        // if (gameState == GameState.InProgress || gameState == GameState.WaitingToFinish) {
        //     leaderboardUpdateTimer += Time.deltaTime;
        //     if (leaderboardUpdateTimer >= leaderboardUpdateInterval) {
        //         leaderboardUpdateTimer = 0;
        //         UpdateLeaderboard(false);
        //     }
        // }
    }

    float GetRankForPlayer(Player p) {
        if (p.state == Player.PlayerState.Finished)
            return p.finishTime;
        if (p.state == Player.PlayerState.InRace)
            return -p.turtle.transform.position.y + 5000;
        return 9999999;
    }

    void UpdateWinScreenAndBow() {
        if (level)
            winScreen.levelName.text = level.displayName;
        foreach (Player p in players.Values)
            p.isTheBest = false; 
        Player[] sortedPlayers = players.Values.Where(
                    p => p.state == Player.PlayerState.Finished || p.state == Player.PlayerState.InRace)
                .OrderBy(p => GetRankForPlayer(p)).ToArray();
        winScreen.SetNumPlayers(sortedPlayers.Length);
        for (int i = 0; i < sortedPlayers.Length; i++) {
            if (i == 0)
                sortedPlayers[i].isTheBest = true;
            winScreen.SetRank(i, sortedPlayers[i].turtle.GetComponent<SpriteRenderer>().sprite, 
                    sortedPlayers[i].deviceID, sortedPlayers[i].finishTime, sortedPlayers[i].isTheBest, 
                    sortedPlayers[i].isKeyboard);
        }
    }

    // void UpdateLeaderboard(bool setBow) {
    //     if (setBow) {
    //         foreach (Player p in players.Values)
    //             p.isTheBest = false; 
    //     }
    //     Player[] sortedPlayers = players.Values.Where(
    //                 p => p.state == Player.PlayerState.Finished || p.state == Player.PlayerState.InRace)
    //             .OrderBy(p => GetRankForPlayer(p)).ToArray();
    //     leaderboard.SetNumPlayers(sortedPlayers.Length);
    //     for (int i = 0; i < sortedPlayers.Length; i++) {
    //         if (i == 0 && setBow)
    //             sortedPlayers[i].isTheBest = true;
    //         leaderboard.SetRank(i, sortedPlayers[i].turtle.GetComponent<SpriteRenderer>().sprite, 
    //                 sortedPlayers[i].deviceID, sortedPlayers[i].finishTime, sortedPlayers[i].isTheBest, 
    //                 sortedPlayers[i].isKeyboard);
    //     }
    // }

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

    // returns true if all non-dropped players who are in the current race are finished -- anyone not InRace is either
    // Finished or not in the race
    bool AllPlayersHaveFinished() {
        foreach (Player p in players.Values) {
            if (p.state == Player.PlayerState.InRace)
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

    // returns the number of users who are InRace (not finished, not dropped)
    int GetInRacePlayerCount() {
        int count = 0;
        foreach (Player player in players.Values)
            if (player.state == Player.PlayerState.InRace)
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
            // if (data["action"].ToString() == "unready" && players[from].state == Player.PlayerState.Hatched)
            //     PutPlayerInEgg(players[from]);
        } else {
            if (players[from].state == Player.PlayerState.InRace) {
                players[from].turtle.ButtonInput(data);
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
