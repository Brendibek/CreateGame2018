using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Node : MonoBehaviour {
    //Инструкция
    //
    //не объязательный параметр, может находится в любом другом классе
    //public GameObject названиеОбъекта_GO;
    //
    //объязательный параметр, находится только в этом классе
    //public static GameObject s_названиеОбъекта_GO;
    //
    //параметр доступа к компонентам класса, находится только в этом классе
    //public static названиеКомпонента s_названиеОбъекта_GO_s_названиеКомпонента_Class;


    public GameObject mapGO;
    public static GameObject sMapGO;
    public static Map sMapGO_sMapClass;

    public static GameObject sLoadCameraGO;
    public static Text sLoadCameraGO_TextGO;

    public GameObject serverControllerGO;
    public static GameObject sServerControllerGO;
    public static ServerController sServerControllerGO_sServerControllerClass;

    public GameObject keyboardGO;
    public static GameObject sKeyboardGO;
    public static KeyboardListener sKeyboardGO_sKeyboardListenerClass;

    public static GameObject sPlayerGO;
    public static MyPlayer sPlayerGO_sPlayerClass;
    public static Move sPlayerGO_sMoveClass;

    public GameObject otherPlayersGO;
    public static GameObject sOtherPlayersGO;

    public GameObject texturesGO;
    public static GameObject sTexturesGO;
    public static Textures sTexturesGO_sTexturesClass;

    //UI
    public GameObject UIGO;
    public static GameObject sUIGO;
    public static GameObject sCalendarGO;
    public static Calendar sCalendarGO_sCalendarClass;

    public static GameObject sLocalChatInputFieldGO;

    void Awake() {
        //добавление игровых объектов на сцену
        sMapGO = Instantiate(mapGO);
        sMapGO.name = "Map";
        sMapGO_sMapClass = sMapGO.GetComponent<Map>();

        sLoadCameraGO = GameObject.Find("LoadCamera");
        sLoadCameraGO_TextGO = sLoadCameraGO.transform.Find("Canvas").Find("Text").GetComponent<Text>();

        sServerControllerGO = Instantiate(serverControllerGO);
        sServerControllerGO.name = "ServerController";
        sServerControllerGO_sServerControllerClass = sServerControllerGO.GetComponent<ServerController>();

        sKeyboardGO = Instantiate(keyboardGO);
        sKeyboardGO.name = "Keyboard";
        sKeyboardGO_sKeyboardListenerClass = sKeyboardGO.GetComponent<KeyboardListener>();

        sOtherPlayersGO = Instantiate(otherPlayersGO);
        sOtherPlayersGO.name = "OtherPlayers";

        sTexturesGO = Instantiate(texturesGO);
        sTexturesGO.name = "Textures";
        sTexturesGO_sTexturesClass = sTexturesGO.GetComponent<Textures>();

        sUIGO = Instantiate(UIGO);
        sUIGO.name = "UI";

        sCalendarGO = sUIGO.transform.Find("WindowsCanvas").Find("Calendar").gameObject;
        sCalendarGO_sCalendarClass = sCalendarGO.GetComponent<Calendar>();

        sLocalChatInputFieldGO = sUIGO.transform.Find("WindowsCanvas").Find("LocalChatInputField").gameObject;
        
    }
}
