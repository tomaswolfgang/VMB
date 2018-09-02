using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Camera : MonoBehaviour {
    private Quaternion oRot;
    private float rotY = 0f;
    // Use this for initialization
    void Start()
    {
        oRot = transform.localRotation;
    }

    // Update is called once per frame
    void Update()
    {
      
        rotY += Input.GetAxis("Mouse Y") * 5f;
        rotY = Mathf.Clamp(rotY, -80, 80);
        Quaternion yQuaternion = Quaternion.AngleAxis(rotY, -Vector3.right);
        transform.localRotation = oRot * yQuaternion;
    }
}
