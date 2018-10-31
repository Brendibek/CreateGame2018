using UnityEngine;
using System.Collections.Generic;

public class ObjectListener : MonoBehaviour
{
    public string type;

    public void OnMouseDown()
    {
        Debug.Log(type);
    }
}
