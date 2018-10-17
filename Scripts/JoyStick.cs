using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class JoyStick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler {

    private Image bgImage;
    [SerializeField]
    private Image joystickImg;
    private Vector2 inputVector;

    private void Start() {
        bgImage = GetComponent<Image>();
        joystickImg = transform.GetChild(0).GetComponent<Image>();
    }

    public virtual void OnPointerDown(PointerEventData ped) {
        Node.sMoveClass.joystick = true;
        OnDrag(ped);
    }

    public virtual void OnPointerUp(PointerEventData ped) {
        Node.sMoveClass.joystick = false;
        inputVector = Vector2.zero;
        joystickImg.rectTransform.anchoredPosition = Vector2.zero;
    }

    public virtual void OnDrag(PointerEventData ped) {
        Vector2 pos;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(bgImage.rectTransform, ped.position, ped.pressEventCamera, out pos)) {
            pos.x = (pos.x / bgImage.rectTransform.sizeDelta.x);
            pos.y = (pos.y / bgImage.rectTransform.sizeDelta.y);
            inputVector = new Vector3(pos.x * 2 + 1, pos.y * 2 - 1);
            inputVector = (inputVector.magnitude > 1.0f) ? inputVector.normalized : inputVector;

            joystickImg.rectTransform.anchoredPosition = new Vector2(inputVector.x * (bgImage.rectTransform.sizeDelta.x / 2), inputVector.y * (bgImage.rectTransform.sizeDelta.y / 2));
        }

        double angle = 0;
        if ((inputVector.x >= 0 && inputVector.y >= 0) || (inputVector.x < 0 && inputVector.y > 0))
            angle = Mathf.Acos((inputVector.x * 1 + inputVector.y * 0) / (Mathf.Sqrt(inputVector.x * inputVector.x + inputVector.y * inputVector.y))) * Mathf.Rad2Deg;
        else if ((inputVector.x <= 0 && inputVector.y <= 0) || (inputVector.x > 0 && inputVector.y < 0))
            angle = Mathf.Acos((inputVector.x * -1 + inputVector.y * 0) / (Mathf.Sqrt(inputVector.x * inputVector.x + inputVector.y * inputVector.y))) * Mathf.Rad2Deg + 180;

        if ((angle >= 337.5 && angle < 360) || (angle >= 0 && angle < 22.5)) {
            Node.sMoveClass.left = false;
            Node.sMoveClass.right = true;
            Node.sMoveClass.up = false;
            Node.sMoveClass.down = false;
        }
        else if (angle >= 22.5 && angle < 67.5) {
            Node.sMoveClass.left = false;
            Node.sMoveClass.right = true;
            Node.sMoveClass.up = true;
            Node.sMoveClass.down = false;
        }
        else if (angle >= 67.5 && angle < 112.5) {
            Node.sMoveClass.left = false;
            Node.sMoveClass.right = false;
            Node.sMoveClass.up = true;
            Node.sMoveClass.down = false;
        }
        else if (angle >= 112.5 && angle < 157.5) {
            Node.sMoveClass.left = true;
            Node.sMoveClass.right = false;
            Node.sMoveClass.up = true;
            Node.sMoveClass.down = false;
        }
        else if (angle >= 157.5 && angle < 202.5) {
            Node.sMoveClass.left = true;
            Node.sMoveClass.right = false;
            Node.sMoveClass.up = false;
            Node.sMoveClass.down = false;
        }
        else if (angle >= 202.5 && angle < 247.5) {
            Node.sMoveClass.left = true;
            Node.sMoveClass.right = false;
            Node.sMoveClass.up = false;
            Node.sMoveClass.down = true;
        }
        else if (angle >= 247.5 && angle< 292.5) {
            Node.sMoveClass.left = false;
            Node.sMoveClass.right = false;
            Node.sMoveClass.up = false;
            Node.sMoveClass.down = true;
        }
        else if (angle >= 292.5 && angle < 337.5) {
            Node.sMoveClass.left = false;
            Node.sMoveClass.right = true;
            Node.sMoveClass.up = false;
            Node.sMoveClass.down = true;
        }
    }
}