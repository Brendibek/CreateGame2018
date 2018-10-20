using UnityEngine;
using UnityEngine.UI;

public class OtherPlayer : MonoBehaviour {
    public int playerId;
    public float x, y;
    public string position;

    public GameObject playerNameGO;

    void FixedUpdate() {
        this.gameObject.transform.position = new Vector2(
        x - Node.sPlayerGO_sPlayerClass.x > 32 ? x - Node.sMapGO_sMapClass.mapWidth : (x - Node.sPlayerGO_sPlayerClass.x < -32 ? x + Node.sMapGO_sMapClass.mapWidth : x),
        y - Node.sPlayerGO_sPlayerClass.y > 32 ? y - Node.sMapGO_sMapClass.mapHeight : (y - Node.sPlayerGO_sPlayerClass.y < -32 ? y + Node.sMapGO_sMapClass.mapHeight : y));
    }

    public void setPlayerName(string playerName) {
        this.transform.Find("OtherPlayerCanvas").Find("PlayerName").GetComponent<Text>().text = playerName;
    }
}
