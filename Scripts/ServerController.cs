using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using UnityEngine.UI;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class ServerController : MonoBehaviour {
    public GameObject playerGO;
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
        playersInCells = new List<GameObject>[Node.sMapGO_sMapClass.mapWidth, Node.sMapGO_sMapClass.mapHeight];

        for (int i = 0; i < playersInCells.GetLength(0); i++)
            for (int j = 0; j < playersInCells.GetLength(1); j++)
                playersInCells[i, j] = new List<GameObject>();

        try {
            //client = new TcpClient ("127.0.0.1", 4000);
            client = new TcpClient("109.234.38.91", 4000);
            serverIsConnected = true;
        }
        catch {
            Node.sLoadCameraGO_TextGO.text = "Попытка подключиться к серверу не удалась.";
        }
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
            sendMessage(obj);
        }
	}

    void serverListener() {
        while (true) if (serverMessage.Length == 0) serverMessage = reader.ReadLine();
    }

    void Update() {
        if (serverMessage.Length != 0) {
            try {
                JObject objFromServer = JObject.Parse(serverMessage);
                int messageId = (int)objFromServer.GetValue("id");
                switch (messageId) {
                    case 0: {
                            Node.sLoadCameraGO.SetActive(false);
                            //Node
                            Node.sPlayerGO = Instantiate(playerGO);
                            Node.sPlayerGO.transform.parent = Node.sGO.transform;
                            Node.sPlayerGO.name = "Player";
                            Node.sPlayerGO_sPlayerClass = Node.sPlayerGO.GetComponent<MyPlayer>();
                            Node.sPlayerGO_sMoveClass = Node.sPlayerGO.GetComponent<Move>();

                            Node.sPlayerGO_sPlayerClass.playerId = (long)objFromServer.GetValue("playerId");
                            Node.sPlayerGO_sPlayerClass.playerName = objFromServer.GetValue("name").ToString();
                            Node.sPlayerGO_sPlayerClass.x = (float)objFromServer.GetValue("x");
                            Node.sPlayerGO_sPlayerClass.y = (float)objFromServer.GetValue("y");
                            Node.sPlayerGO_sPlayerClass.position = objFromServer.GetValue("position").ToString();
                            Node.sPlayerGO_sPlayerClass.connect = true;
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
                            float tempOtherPlayerX = optionsOtherPlayer.newX, tempOtherPlayerY = optionsOtherPlayer.newY;
                            optionsOtherPlayer.newX = (float)objFromServer.GetValue("x");
                            optionsOtherPlayer.newY = (float)objFromServer.GetValue("y");
                            optionsOtherPlayer.position = objFromServer.GetValue("position").ToString();

                            //0.5 из-за того что даются координаты центра объекта
                            if (isNew) {
                                optionsOtherPlayer.playerId = playerId;
                                optionsOtherPlayer.setPlayerName(objFromServer.GetValue("name").ToString());
                                optionsOtherPlayer.setXY();
                                playersInCells[Mathf.FloorToInt(Node.sMapGO_sMapClass.calibrationX(optionsOtherPlayer.newX + 0.5f)), Mathf.FloorToInt(Node.sMapGO_sMapClass.calibrationY(optionsOtherPlayer.newY + 0.5f))].Add(otherPlayers[playerId]);
                            }
                            else {
                                if (Mathf.FloorToInt(tempOtherPlayerX + 0.5f) != Mathf.FloorToInt(optionsOtherPlayer.newX + 0.5f)
                                    || Mathf.FloorToInt(tempOtherPlayerY + 0.5f) != Mathf.FloorToInt(optionsOtherPlayer.newY + 0.5f)) {
                                    playersInCells[Mathf.FloorToInt(Node.sMapGO_sMapClass.calibrationX(tempOtherPlayerX + 0.5f)), Mathf.FloorToInt(Node.sMapGO_sMapClass.calibrationY(tempOtherPlayerY + 0.5f))].Remove(otherPlayers[playerId]);
                                    playersInCells[Mathf.FloorToInt(Node.sMapGO_sMapClass.calibrationX(optionsOtherPlayer.newX + 0.5f)), Mathf.FloorToInt(Node.sMapGO_sMapClass.calibrationY(optionsOtherPlayer.newY + 0.5f))].Add(otherPlayers[playerId]);
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
                                        Node.sMapGO_sMapClass.dataToLowerMapArr(blocks);
                                        break;
                                    }
                                case "average": {
                                        Node.sMapGO_sMapClass.dataToAverageMapArr(blocks);
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
                            string message = objFromServer.GetValue("message").ToString();
                            int playerId = (int)objFromServer.GetValue("playerId");
                            OtherPlayer optionsOtherPlayer = otherPlayers[playerId].GetComponent<OtherPlayer>();
                            StartCoroutine(optionsOtherPlayer.setLocalMessage(message));
                            break;
                        }
                    case 6: {
                            Node.sCalendarGO_sCalendarClass.period = 60000 / (long)objFromServer.GetValue("period");
                            Node.sCalendarGO_sCalendarClass.year = (int)objFromServer.GetValue("year");
                            Node.sCalendarGO_sCalendarClass.month = (int)objFromServer.GetValue("month");
                            Node.sCalendarGO_sCalendarClass.day = (int)objFromServer.GetValue("day");
                            Node.sCalendarGO_sCalendarClass.hour = (int)objFromServer.GetValue("hour");
                            Node.sCalendarGO_sCalendarClass.minute = (int)objFromServer.GetValue("minute");
                            break;
                        }
                    case 7: {
                            JArray objects = (JArray)objFromServer.GetValue("objects");
                            Node.sMapGO_sMapClass.dataToObjectMapArr(objects);
                            break;
                        }
                }
            }
            catch { }
            serverMessage = string.Empty;
        }
    }

    public static void sendMessage(JObject obj) {
        try {
            string message = obj.ToString(Formatting.None);
            writer.WriteLine(message);
        } catch {
            Node.sLoadCameraGO.SetActive(true);
            Node.sLoadCameraGO_TextGO.text = "Связь с сервером потеряна.";
            Destroy(Node.sGO);
            //TODO функция переподключения
        }
    }

    void deletePlayer(int playerId) { 
        if (otherPlayers.ContainsKey(playerId)) {
            OtherPlayer optionsOtherPlayer = otherPlayers[playerId].GetComponent<OtherPlayer>();
            playersInCells[Mathf.FloorToInt(optionsOtherPlayer.newX), Mathf.FloorToInt(optionsOtherPlayer.newY)].Remove(otherPlayers[playerId]);
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
    }

    public enum Operation {
        auth = 0,
        sendMyXY = 1,
        updateChunks = 3,
        chat = 5,  
    }
}