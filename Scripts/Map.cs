using UnityEngine;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class Map : MonoBehaviour {
    public static int mapWidth = 128, mapHeight = 128, blockSize = 8, nWidth, nHeight;
    public GameObject chunks, sprite;
    public static Dictionary<int, GameObject> listChunk = new Dictionary<int, GameObject>();
    public static char[,] MapArr = new char[mapWidth, mapHeight];

    void Awake() {
        //добавление игровых объектов на сцену start
        chunks = Instantiate(chunks);
    }

    void Start() {
        nWidth = mapWidth / blockSize;
        nHeight = mapHeight / blockSize;
    }

    public void updateChunks(float playerX, float playerY, string side) {
        List<int> usedChunks = new List<int>();
        for (int i = -1; i <= 1; i++) {
            for (int j = -1; j <= 1; j++) {
                float chunkX = Mathf.Floor((playerX + i * blockSize) / blockSize) * blockSize;
                float chunkY = Mathf.Floor((playerY + j * blockSize) / blockSize) * blockSize;
                int chunkBlockId = getBlockId(chunkX, chunkY);
                //Добавление не добавленых чанков
                if (!listChunk.ContainsKey(chunkBlockId)) addChunk(chunkBlockId, calibrationX(chunkX), calibrationY(chunkY));
                usedChunks.Add(chunkBlockId);
            }
        }
        //Удаление не использованых чанков
        List<int> removeChunks = new List<int>();
        foreach (var i in listChunk) {
            if (!usedChunks.Contains(i.Key)) {
                Destroy(i.Value);
                removeChunks.Add(i.Key);
            }
        }
        foreach (var key in removeChunks) listChunk.Remove(key);
        //отправка запроса карты
        int tempBlockId = Player.blockId;
        Player.blockId = getBlockId(playerX, playerY);

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
        ServerController.send(obj);
    }

    private void addChunk(int blockId, float x, float y) {
        GameObject chunk = new GameObject();
        chunk.transform.position = new Vector2(x, y);
        chunk.name = "Chunk" + blockId;
        chunk.transform.parent = chunks.transform;
        for (int i = 0; i < blockSize; i++) {
            for (int j = 0; j < blockSize; j++) {
                GameObject test = Instantiate(sprite, new Vector2(x + i, y + j), Quaternion.identity);
                test.name = i + "_" + j;
                test.transform.parent = chunk.transform;
            }
        }
        listChunk.Add(blockId, chunk);
    }

    public static float calibrationX (float x) {
        x -= Mathf.Floor(x / (mapWidth)) * mapWidth;
        return x;
    }

    public static float calibrationY(float y) {
        y -= Mathf.Floor(y / mapHeight) * mapHeight;
        return y;
    }

    public static int getBlockId (float x, float y) {
        x = calibrationX(x);
        y = calibrationY(y);
        int idBlock, idBlockX, idBlockY;
        idBlockX = (int)(x / blockSize);
        idBlockY = (int)(y / blockSize);
        idBlock = idBlockX + idBlockY * nWidth;
        return idBlock;
    }

    public static int getXbyBlockId(int blockId) {
        return (blockId - Mathf.FloorToInt(blockId / nWidth) * nWidth) * blockSize;
    }

    public static int getYbyBlockId(int blockId) {
        return Mathf.FloorToInt(blockId / nWidth) * blockSize;
    }

    public static void dataToMapArr(JArray blocks) {
        for (int blockN = 0; blockN < blocks.Count; blockN++) {
            JObject block = (JObject)blocks[blockN];
            int blockId = (int)block.GetValue("id");
            JArray blockData = (JArray)block.GetValue("block");
            int x = getXbyBlockId(blockId);
            int y = getYbyBlockId(blockId);
            GameObject chunk = listChunk[blockId];
            for (int blockHeightN = 0; blockHeightN < blockData.Count; blockHeightN++) {
                JArray blockHeight = (JArray)blockData[blockHeightN];
                for (int blockWidthN = 0; blockWidthN < blockHeight.Count; blockWidthN++) {
                    MapArr[x + blockWidthN, y + blockHeightN] = (char)blockHeight[blockWidthN];
                    SpriteRenderer spriteR = chunk.transform.Find(blockWidthN + "_" + blockHeightN).GetComponent<SpriteRenderer>();
                    if ((char)blockHeight[blockWidthN] == '9') spriteR.sprite = Textures.getSprite(x + blockWidthN,  y + blockHeightN, 0);
                    if ((char)blockHeight[blockWidthN] == '!') spriteR.sprite = Textures.getSprite(x + blockWidthN, y + blockHeightN, 1);
                }
            }
        }
    }
}
