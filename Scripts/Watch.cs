using UnityEngine;
using UnityEngine.UI;

public class Watch : MonoBehaviour {
    public static string hour = "00";
    string delimiter = ":";
    public static string minute = "00";
    float pastTime;

    public GameObject watchGO;

    void Start() {
        pastTime = Time.time;
    }
	
	void Update () {
        float nowTime = Time.time;
        if (nowTime - pastTime >= 2) {
            pastTime = nowTime;
            if (delimiter == " ") delimiter = ":";
            else delimiter = " ";
        }
        watchGO.GetComponent<Text>().text = hour + delimiter + minute;
	}
}
