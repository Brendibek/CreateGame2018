using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Threading;
using System.IO;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class ServerController : MonoBehaviour {
    public GameObject otherPlayerGO;
    

    Dictionary<int, GameObject> otherPlayers = new Dictionary<int, GameObject>();
    public List<GameObject>[,] playersInCells;

    private TcpClient client;
    private bool serverIsConnected;
    private NetworkStream stream;
    private StreamReader reader;
    public static StreamWriter writer;
    Thread threadListener;

    string serverMessage = string.Empty;

    void Awake(){
        //инициализация масивов
        playersInCells = new List<GameObject>[Node.sMapClass.mapWidth, Node.sMapClass.mapHeight];

        for (int i = 0; i < playersInCells.GetLength(0); i++)
            for (int j = 0; j < playersInCells.GetLength(1); j++)
                playersInCells[i, j] = new List<GameObject>();

        try {
            //client = new TcpClient ("127.0.0.1", 4000);
            client = new TcpClient("109.234.38.91", 4000);
            serverIsConnected = true;
        }
        catch { Debug.LogError("Попытка подключиться к серверу не удалась."); }
    }

    void Start () {
        if (serverIsConnected) {
            stream = client.GetStream();
            reader = new StreamReader(stream);
            writer = new StreamWriter(stream);
            writer.AutoFlush = true;

            new Thread(new ThreadStart(serverListener)).Start();

            JObject obj = new JObject();
            obj.Add(new JProperty("id", Operation.auth));
            obj.Add(new JProperty("name", "player" + Random.Range(0, 100)));
            send(obj);
        }
	}

    void serverListener() {
        while (true) if (serverMessage.Length == 0) serverMessage = reader.ReadLine();
    }

    void FixedUpdate() {
        if (serverMessage.Length != 0) {
            Debug.Log(serverMessage);
            try {
                JObject objFromServer = JObject.Parse(serverMessage);
                int messageId = (int)objFromServer.GetValue("id");
                switch (messageId) {
                    case 0: {
                            Node.sPlayerClass.playerId = (long)objFromServer.GetValue("playerId");
                            Node.sPlayerClass.playerName = objFromServer.GetValue("name").ToString();
                            Node.sPlayerClass.x = (float)objFromServer.GetValue("x");
                            Node.sPlayerClass.y = (float)objFromServer.GetValue("y");
                            Node.sPlayerClass.position = objFromServer.GetValue("position").ToString();
                            Node.sPlayerClass.connect = true;
                            break;
                        }
                    case 1: {
                            int playerId = (int)objFromServer.GetValue("playerId");

                            bool isNew = !otherPlayers.ContainsKey(playerId);
                            if (isNew) {
                                GameObject otherPlayerGON = Instantiate(otherPlayerGO);
                                otherPlayers.Add(playerId, otherPlayerGON);
                                otherPlayerGON.transform.parent = Node.sOtherPlayersGO.transform;
                                isNew = true;
                            }

                            OtherPlayer optionsOtherPlayer = otherPlayers[playerId].GetComponent<OtherPlayer>();
                            float tempOtherPlayerX = optionsOtherPlayer.x, tempOtherPlayerY = optionsOtherPlayer.y;
                            optionsOtherPlayer.x = (float)objFromServer.GetValue("x");
                            optionsOtherPlayer.y = (float)objFromServer.GetValue("y");
                            optionsOtherPlayer.position = objFromServer.GetValue("position").ToString();

                            //0.5 из-за того что даются координаты центра объекта
                            if (isNew) {
                                optionsOtherPlayer.playerId = playerId;
                                optionsOtherPlayer.setPlayerName(objFromServer.GetValue("name").ToString());
                                playersInCells[Mathf.FloorToInt(Node.sMapClass.calibrationX(optionsOtherPlayer.x + 0.5f)), Mathf.FloorToInt(Node.sMapClass.calibrationY(optionsOtherPlayer.y + 0.5f))].Add(otherPlayers[playerId]);
                            }
                            else {
                                if (Mathf.FloorToInt(tempOtherPlayerX + 0.5f) != Mathf.FloorToInt(optionsOtherPlayer.x + 0.5f)
                                    || Mathf.FloorToInt(tempOtherPlayerY + 0.5f) != Mathf.FloorToInt(optionsOtherPlayer.y + 0.5f)) {
                                    playersInCells[Mathf.FloorToInt(Node.sMapClass.calibrationX(tempOtherPlayerX + 0.5f)), Mathf.FloorToInt(Node.sMapClass.calibrationY(tempOtherPlayerY + 0.5f))].Remove(otherPlayers[playerId]);
                                    playersInCells[Mathf.FloorToInt(Node.sMapClass.calibrationX(optionsOtherPlayer.x + 0.5f)), Mathf.FloorToInt(Node.sMapClass.calibrationY(optionsOtherPlayer.y + 0.5f))].Add(otherPlayers[playerId]);
                                }
                            }
                            break;
                        }
                    case 2: {
                            deletePlayer((int)objFromServer.GetValue("playerId"));
                            break;
                        }
                    case 3: {
                            //type lower average top
                            JArray blocks = (JArray)objFromServer.GetValue("blocks");
                            switch ((string)objFromServer.GetValue("type")) {
                                case "lower": {
                                        Node.sMapClass.dataToLowerMapArr(blocks);
                                        break;
                                    }
                                case "average": {
                                        Node.sMapClass.dataToAverageMapArr(blocks);
                                        break;
                                    }
                            }
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
                            Node.sCalendarClass.period = 60000 / (long)objFromServer.GetValue("period");
                            Node.sCalendarClass.year = (int)objFromServer.GetValue("year");
                            Node.sCalendarClass.month = (int)objFromServer.GetValue("month");
                            Node.sCalendarClass.day = (int)objFromServer.GetValue("day");
                            Node.sCalendarClass.hour = (int)objFromServer.GetValue("hour");
                            Node.sCalendarClass.minute = (int)objFromServer.GetValue("minute");
                            break;
                        }
                    case 7: {
                            JArray objects = (JArray)objFromServer.GetValue("objects");
                            Node.sMapClass.dataToObjectMapArr(objects);
                            break;
                        }
                }
            }
            catch { }
            serverMessage = string.Empty;
        }
    }

    public static void send(JObject obj) {
        string message = obj.ToString(Formatting.None);
        writer.WriteLine(message);
    }

    void deletePlayer(int playerId) { 
        if (otherPlayers.ContainsKey(playerId)) {
            OtherPlayer optionsOtherPlayer = otherPlayers[playerId].GetComponent<OtherPlayer>();
            playersInCells[Mathf.FloorToInt(optionsOtherPlayer.x), Mathf.FloorToInt(optionsOtherPlayer.y)].Remove(otherPlayers[playerId]);
            Destroy(otherPlayers[playerId]);
            otherPlayers.Remove(playerId);
        }
    }

    void OnApplicationQuit() {
        if (serverIsConnected) {
            reader.Close();
            writer.Close();
        }
        Application.Quit();
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