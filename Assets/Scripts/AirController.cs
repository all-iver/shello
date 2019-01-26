using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NDream.AirConsole;
using Newtonsoft.Json.Linq;

public class AirController : MonoBehaviour {

    public GameObject playerPrefab;
    public Dictionary<int, Turtle> players = new Dictionary<int, Turtle> (); 
    TrackingCamera cam;

    void Awake () {
        AirConsole.instance.onMessage += OnMessage;		
        AirConsole.instance.onReady += OnReady;		
        AirConsole.instance.onConnect += OnConnect;		
        AirConsole.instance.onDisconnect += OnDisconnect;
        cam = FindObjectOfType<TrackingCamera>();
    }

    void OnReady(string code){
        //Since people might be coming to the game from the AirConsole store once the game is live, 
        //I have to check for already connected devices here and cannot rely only on the OnConnect event 
        List<int> connectedDevices = AirConsole.instance.GetControllerDeviceIds();
        foreach (int deviceID in connectedDevices) {
            AddNewPlayer (deviceID);
        }
    }

    void OnConnect(int device){
        AddNewPlayer(device);
    }

    void OnDisconnect(int device) {
        Turtle player = players[device];
        if (cam)
            cam.RemoveCameraTarget(player.transform);
        Destroy(player.gameObject);
        players.Remove(device);
    }

    void Update() {
        if (Input.GetKeyDown("space")) {
            AddNewPlayer(-1);
            players[-1].useKeyboardInput = true;
        }
    }

    void AddNewPlayer(int deviceID) {
        if (players.ContainsKey(deviceID)) {
            return;
        }
        //Instantiate player prefab, store device id + player script in a dictionary
        GameObject newPlayer = Instantiate(playerPrefab, transform.position, transform.rotation) as GameObject;
        players.Add(deviceID, newPlayer.GetComponent<Turtle>());
        if (cam)
            cam.AddCameraTarget(newPlayer.transform);
    }

    void OnMessage(int from, JToken data){
        //When I get a message, I check if it's from any of the devices stored in my device Id dictionary
        if (players.ContainsKey(from) && data["action"] != null) {
            //I forward the command to the relevant player script, assigned by device ID
            players[from].ButtonInput(data["action"].ToString());
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
