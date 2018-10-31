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

    public static GameObject sGO;

    public GameObject mapGO;
    public static GameObject sMapGO;
    public static Map sMapGO_sMapClass;

    public static GameObject sLoadCameraGO;
    public static Text sLoadCameraGO_TextGO;

    public GameObject ObjectsGO;
    public static GameObject sObjctsGO;
    public static Objects sObjectsGO_sObjectsClass;

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
        sGO = new GameObject();
        sGO.name = "TEST";

        //добавление игровых объектов на сцену
        sMapGO = Instantiate(mapGO);
        sMapGO.transform.parent = sGO.transform;
        sMapGO.name = "Map";
        sMapGO_sMapClass = sMapGO.GetComponent<Map>();

        sLoadCameraGO = GameObject.Find("LoadCamera");
        sLoadCameraGO_TextGO = sLoadCameraGO.transform.Find("Canvas").Find("Text").GetComponent<Text>();

        sObjctsGO = Instantiate(ObjectsGO);
        sObjctsGO.transform.parent = sGO.transform;
        sObjctsGO.name = "Objects";


        sServerControllerGO = Instantiate(serverControllerGO);
        sServerControllerGO.transform.parent = sGO.transform;
        sServerControllerGO.name = "ServerController";
        sServerControllerGO_sServerControllerClass = sServerControllerGO.GetComponent<ServerController>();

        sKeyboardGO = Instantiate(keyboardGO);
        sKeyboardGO.transform.parent = sGO.transform;
        sKeyboardGO.name = "Keyboard";
        sKeyboardGO_sKeyboardListenerClass = sKeyboardGO.GetComponent<KeyboardListener>();

        sOtherPlayersGO = Instantiate(otherPlayersGO);
        sOtherPlayersGO.transform.parent = sGO.transform;
        sOtherPlayersGO.name = "OtherPlayers";

        sTexturesGO = Instantiate(texturesGO);
        sTexturesGO.transform.parent = sGO.transform;
        sTexturesGO.name = "Textures";
        sTexturesGO_sTexturesClass = sTexturesGO.GetComponent<Textures>();

        sUIGO = Instantiate(UIGO);
        sUIGO.transform.parent = sGO.transform;
        sUIGO.name = "UI";

        sCalendarGO = sUIGO.transform.Find("WindowsCanvas").Find("Calendar").gameObject;
        sCalendarGO_sCalendarClass = sCalendarGO.GetComponent<Calendar>();

        sLocalChatInputFieldGO = sUIGO.transform.Find("WindowsCanvas").Find("LocalChatInputField").gameObject;
        
    }
}
