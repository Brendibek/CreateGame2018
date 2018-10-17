using UnityEngine;
using UnityEngine.UI;

public class Calendar : MonoBehaviour {
    public float period = 1;
    public int year = 0;
    public int month = 1;
    public int day = 1;
    public int hour = 0;
    public int minute = 0;
    string delimiter = ":";
    float pastTime;

    void Start() {
        pastTime = Time.time;
    }

    void Update() {
        float nowTime = Time.time;
        if (nowTime - pastTime >= period) {
            pastTime = nowTime;
            if (delimiter == " ") delimiter = ":";
            else delimiter = " ";
        }
        this.GetComponent<Text>().text = string.Format("{0:D2}", hour) + delimiter + string.Format("{0:D2}", minute) + "\n" + day + "-е " + getMonth(month) + " " + string.Format("{0:D4}", year) + " год";
    }

    private string getMonth(int monthN) {
        switch (month) {
            case 1: {
                    return "Января";
                }
            case 2: {
                    return "Февраля";
                }
            case 3: {
                    return "Марта";
                }
            case 4: {
                    return "Апреля";
                }
            case 5: {
                    return "Мая";
                }
            case 6: {
                    return "Июня";
                }
            case 7: {
                    return "Июля";
                }
            case 8: {
                    return "Августа";
                }
            case 9: {
                    return "Сентября";
                }
            case 10: {
                    return "Октября";
                }
            case 11: {
                    return "Ноября";
                }
            case 12: {
                    return "Декабря";
                }
            default: {
                    return "err";
                }
        }
    }
}
