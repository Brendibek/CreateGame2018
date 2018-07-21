using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Threading;
using System.IO;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class ServerController : MonoBehaviour {
    public GameObject playerGO;
    public GameObject otherPlayerGO;
    public GameObject otherPlayersGO;

    Dictionary<int, GameObject> otherPlayers = new Dictionary<int, GameObject>();

	private TcpClient client;
	private NetworkStream stream;
    private StreamReader reader;
    public static StreamWriter writer;
    Thread threadListener;

    string serverMessage = string.Empty;

    void Awake(){
        //добавление игровых объектов на сцену
        otherPlayersGO = Instantiate(otherPlayersGO);

        //client = new TcpClient ("127.0.0.1", 4000);
        client = new TcpClient ("109.234.38.91", 4000);
    }

    void Start () {
        stream = client.GetStream();
        reader = new StreamReader(stream);
        writer = new StreamWriter(stream);
        writer.AutoFlush = true;

        new Thread(new ThreadStart(serverListener)).Start();

        JObject obj = new JObject ();
		obj.Add (new JProperty ("id", Operation.auth));
		obj.Add (new JProperty ("name", "player" + Random.Range(0, 100)));
		send (obj);
	}

    void serverListener() {
        while (true) if (serverMessage.Length == 0) serverMessage = reader.ReadLine();
    }

    void FixedUpdate() {
        if (serverMessage.Length != 0) {
            JObject objFromServer = JObject.Parse(serverMessage);
            int messageId = (int)objFromServer.GetValue("id");
            switch (messageId) {
                case 0: {
                        Instantiate(playerGO);
                        Player.playerId = (long)objFromServer.GetValue("playerId");
                        Player.playerName = objFromServer.GetValue("name").ToString();
                        Player.x = (float)objFromServer.GetValue("x");
                        Player.y = (float)objFromServer.GetValue("y");
                        //Player.position = objFromServer.GetValue("position").ToString();
                        Player.connect = true;
                        break;
                    }
                case 1: {
                        int playerId = (int)objFromServer.GetValue("playerId");
                        if (!otherPlayers.ContainsKey(playerId)) {
                            GameObject otherPlayerGON = Instantiate(otherPlayerGO);
                            otherPlayers.Add(playerId, otherPlayerGON);
                            otherPlayerGON.transform.parent = otherPlayersGO.transform;
                        }
                        OtherPlayer optionsOtherPlayer = otherPlayers[playerId].GetComponent<OtherPlayer>();
                        optionsOtherPlayer.playerId = playerId;
                        optionsOtherPlayer.playerName = objFromServer.GetValue("name").ToString();
                        optionsOtherPlayer.x = (float)objFromServer.GetValue("x");
                        optionsOtherPlayer.y = (float)objFromServer.GetValue("y");
                        //optionsOtherPlayer.position = objFromServer.GetValue("position").ToString();
                        break;
                    }
                case 2: {
                        deletePlayer((int)objFromServer.GetValue("playerId"));
                        break;
                    }
                case 3: {
                        JArray blocks = (JArray)objFromServer.GetValue("blocks");
                        Map.dataToMapArr(blocks);
                        break;
                    }
                case 4: {
                        deletePlayer((int)objFromServer.GetValue("playerId"));
                        break;
                    }
                case 5: {
                        //я message
                        //мне message playerId
                        break;
                    }
                case 6: {
                        //lowerLevelMap
                        break;
                    }
                case 7: {
                        //time 
                        //мне hour minute
                        break;
                    }

            }
            serverMessage = string.Empty;
        }
    }

    public static void send(JObject obj) {
        string message = obj.ToString(Formatting.None);
        writer.WriteLine(message);
    }

    void deletePlayer(int playerId) { 
        if (otherPlayers.ContainsKey(playerId)) {
            Destroy(otherPlayers[playerId]);
            otherPlayers.Remove(playerId);
        }
    }

    void OnApplicationQuit() {
        reader.Close();
        writer.Close();
        //disconnect();
    }

    private void disconnect() {
        JObject obj = new JObject(); obj.Add(new JProperty("id", Operation.disconnect)); send(obj);
        stream.Close ();
		client.Close ();
	}

    public enum Operation {
        auth = 0,
        sendMyXY = 1,
        updateChunks = 3,
        disconnect = 4,
        chat = 5,  
    }
}