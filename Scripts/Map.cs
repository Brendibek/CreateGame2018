using UnityEngine;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Threading;
using UnityEngine.Tilemaps;

public class Map : MonoBehaviour {
    public int mapWidth = 128, mapHeight = 128, blockSize = 8, nWidth, nHeight;
    public List<string> listMapBlocks = new List<string>();

	public int[,] lowerMapArr;

	public int[,] averageMapArr;
	public bool[,] checkUpdateAverageMap;

    public char[,] topMapArr;
    public bool[,] lowerMapMaskArr;

    public MapObject[,] mapObjectMapArr;

    public Tilemap water, send, dirt, rock;
    public Tilemap averageRock;

    void Awake() {
        //инициализация масивов
        lowerMapArr = new int[mapWidth, mapHeight];

        averageMapArr = new int[mapWidth, mapHeight];
        checkUpdateAverageMap = new bool[mapWidth, mapHeight];

        topMapArr = new char[mapWidth, mapHeight];
        lowerMapMaskArr = new bool[mapWidth, mapHeight];

        mapObjectMapArr = new MapObject[mapWidth, mapHeight];

        //добавление игровых объектов на сцену start
        this.water = GameObject.Find("Water").GetComponent<Tilemap>();
        this.send = GameObject.Find("Send").GetComponent<Tilemap>();
        this.dirt = GameObject.Find("Dirt").GetComponent<Tilemap>();
        this.rock = GameObject.Find("Rock").GetComponent<Tilemap>();

        averageRock = GameObject.Find("AverageRock").GetComponent<Tilemap>();
    }

    void Start() {
        nWidth = mapWidth / blockSize;
        nHeight = mapHeight / blockSize;
    }

    public void updateChunks(float playerX, float playerY, string side) {
        //отправка запроса карты
        int tempBlockId = Node.sPlayerGO_sPlayerClass.blockId;
        Node.sPlayerGO_sPlayerClass.blockId = getBlockId(playerX, playerY);

        JObject obj = new JObject();
        obj.Add(new JProperty("id", ServerController.Operation.updateChunks));
        obj.Add(new JProperty("x", playerX));
        obj.Add(new JProperty("y", playerY));
        switch (side) {
            case "all": {
                    obj.Add(new JProperty("side", side));
                    break;
                }
            case "calc": {
                    if (tempBlockId == getBlockId(playerX + blockSize, playerY - blockSize)) obj.Add(new JProperty("side", "LD"));
                    else if (tempBlockId == getBlockId(playerX, playerY - blockSize)) obj.Add(new JProperty("side", "D"));
                    else if (tempBlockId == getBlockId(playerX - blockSize, playerY - blockSize)) obj.Add(new JProperty("side", "RD"));
                    else if (tempBlockId == getBlockId(playerX + blockSize, playerY)) obj.Add(new JProperty("side", "L"));
                    else if (tempBlockId == getBlockId(playerX - blockSize, playerY)) obj.Add(new JProperty("side", "R"));
                    else if (tempBlockId == getBlockId(playerX + blockSize, playerY + blockSize)) obj.Add(new JProperty("side", "LU"));
                    else if (tempBlockId == getBlockId(playerX, playerY + blockSize)) obj.Add(new JProperty("side", "U"));
                    else if (tempBlockId == getBlockId(playerX - blockSize, playerY + blockSize)) obj.Add(new JProperty("side", "RU"));
                    break;
                }
        }
        Node.sServerControllerGO_sServerControllerClass.sendMessage(obj);
    }

    public void updateVisibleBlocks() {
        List<string> usedBlocks = new List<string>();
        for (int blockX = Mathf.FloorToInt(Node.sPlayerGO_sPlayerClass.x) - 6; blockX <= Mathf.FloorToInt(Node.sPlayerGO_sPlayerClass.x) + 7; blockX++) {
            for (int blockY = Mathf.FloorToInt(Node.sPlayerGO_sPlayerClass.y) - 4; blockY <= Mathf.FloorToInt(Node.sPlayerGO_sPlayerClass.y) + 5; blockY++) {
                //Добавление не добавленых блоков
                int lowerId = lowerMapArr[Mathf.FloorToInt(calibrationX(blockX)), Mathf.FloorToInt(calibrationY(blockY))];
                int averageId = averageMapArr[Mathf.FloorToInt(calibrationX(blockX)), Mathf.FloorToInt(calibrationY(blockY))];
                if (!listMapBlocks.Contains(blockX + "_" + blockY)) {
                    Node.sTexturesGO_sTexturesClass.setTile(blockX, blockY, lowerId, averageId);
                    listMapBlocks.Add(blockX + "_" + blockY);
                }
                usedBlocks.Add(blockX + "_" + blockY);
                if (blockX < 7 || blockX > 121 || blockY < 5 || blockY > 123) {
                    int newBlockX = blockX, newBlockY = blockY;
                    if (blockX < 7) newBlockX = blockX + mapWidth;
                    else if (blockX > 121) newBlockX = blockX - mapWidth;
                    if (blockY < 5) newBlockY = blockY + mapHeight;
                    else if (blockY > 123) newBlockY = blockY - mapHeight;
                    if (!listMapBlocks.Contains(newBlockX + "_" + newBlockY)) {
                        Node.sTexturesGO_sTexturesClass.setTile(newBlockX, newBlockY, lowerId, averageId);
                        listMapBlocks.Add(newBlockX + "_" + newBlockY);
                    }
                    usedBlocks.Add(newBlockX + "_" + newBlockY);
                }
            }
        }

        //Удаление не использованых блоков
        List<string> removeBlocks = new List<string>();
        for(int i = 0; i < listMapBlocks.Count; i++) {
            if (!usedBlocks.Contains(listMapBlocks[i])) {
                int blockX = int.Parse(listMapBlocks[i].Split('_')[0]);
                int blockY = int.Parse(listMapBlocks[i].Split('_')[1]);
                Node.sTexturesGO_sTexturesClass.removeTile(water, blockX, blockY);
                Node.sTexturesGO_sTexturesClass.removeTile(send, blockX, blockY);
                Node.sTexturesGO_sTexturesClass.removeTile(dirt, blockX, blockY);
                Node.sTexturesGO_sTexturesClass.removeTile(rock, blockX, blockY);
                Node.sTexturesGO_sTexturesClass.removeTile(averageRock, blockX, blockY);
                removeBlocks.Add(listMapBlocks[i]);
            }
        }
        for(int i = 0; i < removeBlocks.Count; i++) listMapBlocks.Remove(removeBlocks[i]);
        //удаление не использованых  маск
        for (int blockX = Mathf.FloorToInt(Node.sPlayerGO_sPlayerClass.x) - 8; blockX <= Mathf.FloorToInt(Node.sPlayerGO_sPlayerClass.x) + 9; blockX++) {
            int blockYD = Mathf.FloorToInt(Node.sPlayerGO_sPlayerClass.y) - 6;
            if (lowerMapMaskArr[(int)calibrationX(blockX), (int)calibrationY(blockYD)]) {
                for (int i = 1; i <= 4; i++) getLowerTilemap(i).SetTile(new Vector3Int(blockX, blockYD, 1), null);
                if (blockX < 9 || blockX > 119 || blockYD < 7 || blockYD > 121) {
                    int newBlockX = blockX, newBlockYD = blockYD;
                    if (blockX < 9) newBlockX = blockX + mapWidth;
                    else if (blockX > 119) newBlockX = blockX - mapWidth;
                    if (blockYD < 7) newBlockYD = blockYD + mapHeight;
                    else if (blockYD > 121) newBlockYD = blockYD - mapHeight;
                    for (int i = 1; i <= 4; i++) getLowerTilemap(i).SetTile(new Vector3Int(newBlockX, newBlockYD, 1), null);
                }
                lowerMapMaskArr[(int)calibrationX(blockX), (int)calibrationY(blockYD)] = false;
            }
            int blockYU = Mathf.FloorToInt(Node.sPlayerGO_sPlayerClass.y) + 7;
            if (lowerMapMaskArr[(int)calibrationX(blockX), (int)calibrationY(blockYU)]) {
                for (int i = 1; i <= 4; i++) getLowerTilemap(i).SetTile(new Vector3Int(blockX, blockYU, 1), null);
                if (blockX < 9 || blockX > 119 || blockYU < 7 || blockYU > 121) {
                    int newBlockX = blockX, newBlockYU = blockYU;
                    if (blockX < 9) newBlockX = blockX + mapWidth;
                    else if (blockX > 119) newBlockX = blockX - mapWidth;
                    if (blockYU < 7) newBlockYU = blockYU + mapHeight;
                    else if (blockYU > 121) newBlockYU = blockYU - mapHeight;
                    for (int i = 1; i <= 4; i++) getLowerTilemap(i).SetTile(new Vector3Int(newBlockX, newBlockYU, 1), null);
                }
                lowerMapMaskArr[(int)calibrationX(blockX), (int)calibrationY(blockYU)] = false;
            }
        }
        for (int blockY = Mathf.FloorToInt(Node.sPlayerGO_sPlayerClass.y) - 5; blockY <= Mathf.FloorToInt(Node.sPlayerGO_sPlayerClass.y) + 6; blockY++) {
            int blockXL = Mathf.FloorToInt(Node.sPlayerGO_sPlayerClass.x) - 8;
            if (lowerMapMaskArr[(int)calibrationX(blockXL), (int)calibrationY(blockY)]) {
                for (int i = 1; i <= 4; i++) getLowerTilemap(i).SetTile(new Vector3Int(blockXL, blockY, 1), null);
                if (blockXL < 9 || blockXL > 119 || blockY < 7 || blockY > 121) {
                    int newBlockXL = blockXL, newBlockY = blockY;
                    if (blockXL < 9) newBlockXL = blockXL + mapWidth;
                    else if (blockXL > 119) newBlockXL = blockXL - mapWidth;
                    if (blockY < 7) newBlockY = blockY + mapHeight;
                    else if (blockY > 121) newBlockY = blockY - mapHeight;
                    for (int i = 1; i <= 4; i++) getLowerTilemap(i).SetTile(new Vector3Int(newBlockXL, newBlockY, 1), null);
                }
                lowerMapMaskArr[(int)calibrationX(blockXL), (int)calibrationY(blockY)] = false;
            }
            int blockXR = Mathf.FloorToInt(Node.sPlayerGO_sPlayerClass.x) + 9;
            if (lowerMapMaskArr[(int)calibrationX(blockXR), (int)calibrationY(blockY)]) {
                for (int i = 1; i <= 4; i++) getLowerTilemap(i).SetTile(new Vector3Int(blockXR, blockY, 1), null);
                if (blockXR < 9 || blockXR > 119 || blockY < 7 || blockY > 121) {
                    int newBlockXR = blockXR, newBlockY = blockY;
                    if (blockXR < 9) newBlockXR = blockXR + mapWidth;
                    else if (blockXR > 119) newBlockXR = blockXR - mapWidth;
                    if (blockY < 7) newBlockY = blockY + mapHeight;
                    else if (blockY > 121) newBlockY = blockY - mapHeight;
                    for (int i = 1; i <= 4; i++) getLowerTilemap(i).SetTile(new Vector3Int(newBlockXR, newBlockY, 1), null);
                }
                lowerMapMaskArr[(int)calibrationX(blockXR), (int)calibrationY(blockY)] = false;
            }
        }
    }

    public float calibrationX (float x) {
        x -= Mathf.Floor(x / (mapWidth)) * mapWidth;
        return x;
    }

    public float calibrationY(float y) {
        y -= Mathf.Floor(y / mapHeight) * mapHeight;
        return y;
    }

    public int getBlockId (float x, float y) {
        x = calibrationX(x);
        y = calibrationY(y);
        int idBlock, idBlockX, idBlockY;
        idBlockX = (int)(x / blockSize);
        idBlockY = (int)(y / blockSize);
        idBlock = idBlockX + idBlockY * nWidth;
        return idBlock;
    }

    public int getXbyBlockId(int blockId) {
        return (blockId - Mathf.FloorToInt(blockId / nWidth) * nWidth) * blockSize;
    }

    public int getYbyBlockId(int blockId) {
        return Mathf.FloorToInt(blockId / nWidth) * blockSize;
    }

    //установка текстуры на нижний слой
    public void dataToLowerMapArr(JArray blocks) {
        for (int blockN = 0; blockN < blocks.Count; blockN++) {
            JObject block = (JObject)blocks[blockN];
            int blockId = (int)block.GetValue("id");
            JArray blockData = (JArray)block.GetValue("block");
            int x = getXbyBlockId(blockId);
            int y = getYbyBlockId(blockId);
            for (int blockHeightN = 0; blockHeightN < blockData.Count; blockHeightN++) {
                JArray blockHeight = (JArray)blockData[blockHeightN];
                for (int blockWidthN = 0; blockWidthN < blockHeight.Count; blockWidthN++) {
                    lowerMapArr[x + blockWidthN, y + blockHeightN] = idToLowerId(getCharID(blockHeight[blockWidthN]));
                }
            }
        }
    }

    //установка текстуры на средний слой
    public void dataToAverageMapArr(JArray blocks) {
        for (int blockN = 0; blockN < blocks.Count; blockN++) {
            JObject block = (JObject)blocks[blockN];
            int blockId = (int)block.GetValue("id");
            JArray blockData = (JArray)block.GetValue("block");
            int x = getXbyBlockId(blockId);
            int y = getYbyBlockId(blockId);
            for (int blockHeightN = 0; blockHeightN < blockData.Count; blockHeightN++) {
                JArray blockHeight = (JArray)blockData[blockHeightN];
                for (int blockWidthN = 0; blockWidthN < blockHeight.Count; blockWidthN++) {
                    averageMapArr[x + blockWidthN, y + blockHeightN] = idToAverageId(getCharID(blockHeight[blockWidthN]));
                }
            }
        }
        updateVisibleBlocks();
    }

    public void dataToObjectMapArr(JArray objects) {
        for (int objectN = 0; objectN < objects.Count; objectN++) {
            JObject obj = (JObject)objects[objectN];
            int blockId = (int)obj.GetValue("blockId");
            JArray blockData = (JArray)obj.GetValue("blockObjects");
            for(int i = 0; i < blockData.Count; i++) {
                JObject obj2 = (JObject)blockData[i];
                MapObject mapObject = new MapObject();
                int x = (int)obj2.GetValue("x");
                int y = (int)obj2.GetValue("y");
                mapObject.type = (string)obj2.GetValue("type");
                mapObjectMapArr[x, y] = mapObject;
                switch (mapObject.type) {
                    case "bush": {
                            Objects.setObject(x, y, "bush");
                            break;
                        }
                    case "stick": {
                            Objects.setObject(x, y, "stick");
                            break;
                        }
                }
            }
        }
    }

    public int idToLowerId(int id) {
        if (id == 34) return 1;
        else if (id == 100) return 2;
        else if (id == 101) return 3;
        else if (id == 200) return 4;
        else return 0;
    }

    public Tilemap getLowerTilemap(int id) {
        if (id == 1) return Node.sMapGO_sMapClass.water;
        else if (id == 2) return Node.sMapGO_sMapClass.send;
        else if (id == 3) return Node.sMapGO_sMapClass.dirt;
        else if (id == 4) return Node.sMapGO_sMapClass.rock;
        else return null;
    }

    public int idToAverageId(int id) {
        if (id == 200) return 33;
        else return 32;
    }

    public int getCharID(JToken symbol) {
        return Encoding.Unicode.GetBytes((string)symbol)[0];
    }

    private string getPosition(int i, int j) {
        if (i == -1 && j == -1) return "LD";
        else if (i == -1 && j == 0) return "L";
        else if (i == -1 && j == 1) return "LU";
        else if (i == 0 && j == -1) return "D";
        else if (i == 0 && j == 0) return "C";
        else if (i == 0 && j == 1) return "U";
        else if (i == 1 && j == -1) return "RD";
        else if (i == 1 && j == 0) return "R";
        else if (i == 1 && j == 1) return "RU";
        return null;
    }

    public struct MapObject {
        public string type;
    }
}
