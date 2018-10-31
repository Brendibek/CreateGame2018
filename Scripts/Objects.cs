using UnityEngine;
using System.Collections.Generic;

public class Objects : MonoBehaviour {
    public GameObject[] objects;
    public static List<GameObject> objectsList = new List<GameObject>();

    void Start() {
        for (int i = 0; i < objects.Length; i++) {
            objectsList.Add(objects[i]);
        }
    }

    public static void setObject(int x, int y, string type) {
        GameObject tempObj = Instantiate(objectsList[getObjectId(type)], new Vector2(x, y), Quaternion.identity);
        tempObj.GetComponent<ObjectListener>().type = type;
    }

    private static int getObjectId(string type)
    {
        if (type == "bush") return 0;
        else return 1;
    }
}
