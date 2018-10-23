using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MyPlayer : MonoBehaviour {
    public long playerId;
    public string playerName;
    public float x, y;
    public string position;
    public int blockId;
    public bool connect = false;

    public IEnumerator setLocalMessage(string message) {
        this.transform.Find("MyPlayerCanvas").Find("LocalMessage").gameObject.SetActive(true);
        this.transform.Find("MyPlayerCanvas").Find("LocalMessage").Find("Text").GetComponent<Text>().text = message;
        yield return new WaitForSeconds(3f);
        this.transform.Find("MyPlayerCanvas").Find("LocalMessage").gameObject.SetActive(false);
        Node.sKeyboardGO_sKeyboardListenerClass.localChat = false;
    }
}