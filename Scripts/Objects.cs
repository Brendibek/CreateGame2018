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

    public static void setObject(int x, int y, int id) {
        Instantiate(objectsList[id], new Vector2(x, y), Quaternion.identity);
    }
}
