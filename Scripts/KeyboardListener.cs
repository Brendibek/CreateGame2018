using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardListener : MonoBehaviour {

    void FixedUpdate () {
        if (Node.sPlayerGO != null) {
            if (!Node.sPlayerGO_sMoveClass.joystick) {
                if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) Node.sPlayerGO_sMoveClass.left = true;
                if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) Node.sPlayerGO_sMoveClass.right = true;
                if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) Node.sPlayerGO_sMoveClass.up = true;
                if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) Node.sPlayerGO_sMoveClass.down = true;
            }
        }
    }
}
