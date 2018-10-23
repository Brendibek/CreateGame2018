using UnityEngine;
using UnityEngine.UI;
using System.Collections;

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

    public IEnumerator setLocalMessage(string message) {
        this.transform.Find("OtherPlayerCanvas").Find("LocalMessage").gameObject.SetActive(true);
        this.transform.Find("OtherPlayerCanvas").Find("LocalMessage").Find("Text").GetComponent<Text>().text = message;
        yield return new WaitForSeconds(3f);
        this.transform.Find("OtherPlayerCanvas").Find("LocalMessage").gameObject.SetActive(false);
    }
}
