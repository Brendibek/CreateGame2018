using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine.UI;

public class KeyboardListener : MonoBehaviour {
    public bool focus = true;

    public bool localChat = false;

    void FixedUpdate () {
        if (Node.sPlayerGO != null) {
            if (!Node.sPlayerGO_sMoveClass.joystick && focus) {
                if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) Node.sPlayerGO_sMoveClass.left = true;
                if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) Node.sPlayerGO_sMoveClass.right = true;
                if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) Node.sPlayerGO_sMoveClass.up = true;
                if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) Node.sPlayerGO_sMoveClass.down = true;
            }

            if (Input.GetKey(KeyCode.T) && focus && !localChat) {
                focus = false;
                localChat = true;
                Node.sLocalChatInputFieldGO.SetActive(true);
                Node.sLocalChatInputFieldGO.GetComponent<InputField>().ActivateInputField();
            }

            if (Input.GetKey(KeyCode.Escape)) {
                focus = true;
                Node.sLocalChatInputFieldGO.SetActive(false);
            }

            //Enter
            if (Input.GetKey(KeyCode.Return)) {
                focus = true;
                if (localChat) {
                    string message = Node.sLocalChatInputFieldGO.GetComponent<InputField>().text;
                    Node.sLocalChatInputFieldGO.GetComponent<InputField>().text = "";
                    Node.sLocalChatInputFieldGO.SetActive(false);
                    if (message != "") {
                        JObject obj = new JObject();
                        obj.Add(new JProperty("id", ServerController.Operation.chat));
                        obj.Add(new JProperty("message", message));
                        ServerController.send(obj);
                        StartCoroutine(Node.sPlayerGO_sPlayerClass.setLocalMessage(message));
                    }
                }
            }
        }
    }
}
