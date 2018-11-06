using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class OtherPlayer : MonoBehaviour {
    public int playerId;
    public float newX, newY;
    private float oldX, oldY;
    public string position;

    public GameObject playerNameGO;

    void FixedUpdate() {
        float speed = Mathf.Sqrt(Mathf.Pow(this.gameObject.transform.position.x - newX, 2) + Mathf.Pow(this.gameObject.transform.position.y - newY, 2));
        float calibX = newX - Node.sPlayerGO_sPlayerClass.x > 32 ? newX - Node.sMapGO_sMapClass.mapWidth : (newX - Node.sPlayerGO_sPlayerClass.x < -32 ? newX + Node.sMapGO_sMapClass.mapWidth : newX);
        float calibY = newY - Node.sPlayerGO_sPlayerClass.y > 32 ? newY - Node.sMapGO_sMapClass.mapHeight : (newY - Node.sPlayerGO_sPlayerClass.y < -32 ? newY + Node.sMapGO_sMapClass.mapHeight : newY);
        //this.gameObject.transform.position = new Vector2(newX, newY);
        this.gameObject.transform.position = Vector2.MoveTowards(this.gameObject.transform.position, new Vector2(calibX, calibY), speed / 20);
        oldX = newX;
        oldY = newY;
    }

    public void setXY() {
        this.gameObject.transform.position = new Vector2(newX, newY);
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
