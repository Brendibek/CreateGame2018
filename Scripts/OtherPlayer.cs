using UnityEngine;
using UnityEngine.UI;

public class OtherPlayer : MonoBehaviour {
    public int playerId;
    public float x, y;
    public string position;

    public GameObject playerNameGO;

    void FixedUpdate() {
        this.gameObject.transform.position = new Vector2(
        x - Node.sPlayerClass.x > 32 ? x - Node.sMapClass.mapWidth : (x - Node.sPlayerClass.x < -32 ? x + Node.sMapClass.mapWidth : x),
        y - Node.sPlayerClass.y > 32 ? y - Node.sMapClass.mapHeight : (y - Node.sPlayerClass.y < -32 ? y + Node.sMapClass.mapHeight : y));
    }

    public void setPlayerName(string playerName) {
        this.transform.Find("OtherPlayerCanvas").Find("PlayerName").GetComponent<Text>().text = playerName;
    }
}
