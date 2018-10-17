using UnityEngine;
using UnityEngine.UI;

public class FPS : MonoBehaviour {
    float deltaTime = 0;

    void Update() {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        float msec = deltaTime * 1000;
        float fps = 1 / deltaTime;
        this.GetComponent<Text>().text = string.Format("{0:0.0 } ms\n{1:0.0} fps", msec, fps);
    }
}