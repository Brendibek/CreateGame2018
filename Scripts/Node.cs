using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour {

    public GameObject mapGO;
    public static GameObject sMapGO;
    public static Map sMapClass;

    public GameObject serverControllerGO;
    public static GameObject sServerControllerGO;
    public static ServerController sServerControllerClass;

    public GameObject playerGO;
    public static GameObject sPlayerGO;
    public static Player sPlayerClass;
    public static Move sMoveClass;

    public GameObject otherPlayersGO;
    public static GameObject sOtherPlayersGO;

    public GameObject texturesGO;
    public static GameObject sTexturesGO;
    public static Textures sTexturesClass;

    public GameObject UIGO;
    public static GameObject sUIGO;
    public static Calendar sCalendarClass;

    void Awake() {
        //добавление игровых объектов на сцену
        sMapGO = Instantiate(mapGO);
        sMapGO.name = "Map";
        sMapClass = sMapGO.GetComponent<Map>();

        sServerControllerGO = Instantiate(serverControllerGO);
        sServerControllerGO.name = "ServerController";
        sServerControllerClass = sServerControllerGO.GetComponent<ServerController>();

        sPlayerGO = Instantiate(playerGO);
        sPlayerGO.name = "Player";
        sPlayerClass = sPlayerGO.GetComponent<Player>();
        sMoveClass = sPlayerGO.GetComponent<Move>();

        sOtherPlayersGO = Instantiate(otherPlayersGO);
        sOtherPlayersGO.name = "OtherPlayers";

        sTexturesGO = Instantiate(texturesGO);
        sTexturesGO.name = "Textures";
        sTexturesClass = sTexturesGO.GetComponent<Textures>();

        sUIGO = Instantiate(UIGO);
        sUIGO.name = "UI";

        sCalendarClass = sUIGO.transform.Find("WindowsCanvas").Find("Calendar").GetComponent<Calendar>();
    }
}
