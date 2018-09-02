using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Player : NetworkBehaviour {
    private float speed = 4.0F;
    private float jumpSpeed = 8.0F;
    private float gravity = 14.0F;
    private Quaternion oRot;
    private float rotY = 0f;
    private float rotX = 0f;
    private Vector3 moveDirection = Vector3.zero;
    float yPos;
    GameObject cam;
    bool win = false;
    bool paused = false;

    public void restartPlayer()
    {
        win = false;
        transform.FindChild("Camera").localPosition = new Vector3(0, 0.672f, 0f);
        transform.FindChild("Camera").localEulerAngles = new Vector3(30,0, 0);
        GameObject.Find("Floor").transform.GetChild(0).gameObject.SetActive(false);
    }

    void Start()
    {
        yPos = transform.position.y;
        cam = this.transform.FindChild("Camera").gameObject;
        oRot = cam.transform.localRotation;

    }

    public void changeText(string s)
    {
        GameObject.Find("Floor").transform.GetChild(0).gameObject.SetActive(true);
        UnityEngine.UI.Text t;
        print("in here");
        t = GameObject.Find("Floor/Canvas/Panel/Text").gameObject.GetComponent<UnityEngine.UI.Text>();
        t.text = s;
    }

    private void displayPause()
    {
        GameObject.Find("Floor").transform.GetChild(1).gameObject.SetActive(true);
    }

    private void unPause()
    {
        GameObject.Find("Floor").transform.GetChild(1).gameObject.SetActive(false);
    }

    public void playerWin()
    {
        win = true;
        transform.FindChild("Camera").localPosition = new Vector3(0, 2.5f, 3f);
        transform.FindChild("Camera").localEulerAngles = new Vector3(30, 180, 0);
    }

    public void playerLose()
    {
        win = true;
        transform.FindChild("Camera").localPosition = new Vector3(0, 2.5f, 3f);
        transform.FindChild("Camera").localEulerAngles = new Vector3(30, 180, 0);
        changeText("YOU LOSE \n Press R to try again");
    }

    

    void Update()
    {
        if (!win)
        {
            if (paused)
            {
                print("paused");
                if (Input.GetKeyDown(KeyCode.P))
                {
                    unPause();
                    paused = false;
                }
                else if (Input.GetKeyDown(KeyCode.R))
                {
                    unPause();
                    paused = false;
                    InitGame.loseGame();
                }
                else
                    return;
            }
            else if (Input.GetKeyDown(KeyCode.P) && !paused)
            {
                paused = true;
                
                displayPause();
                
            }
            
            if (yPos == transform.position.y)
            {
                moveDirection.y = 0;
            }
            yPos = transform.position.y;
            CharacterController controller = GetComponent<CharacterController>();
            if (controller.isGrounded)
            {
                moveDirection = new Vector3(-Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")  );
                moveDirection = transform.TransformDirection(moveDirection);
                moveDirection *= speed;
                if (Input.GetButton("Jump"))
                    moveDirection.y = jumpSpeed;


            }

            rotY += Input.GetAxis("Mouse Y") * 3f;
            rotY = Mathf.Clamp(rotY, -80, 80);
            Quaternion yQuaternion = Quaternion.AngleAxis(rotY, -Vector3.right);
            cam.transform.localRotation = oRot * yQuaternion;

            transform.Rotate(0, 4 * Input.GetAxis("Mouse X"), 0);

            moveDirection.y -= gravity * Time.deltaTime;
            controller.Move(moveDirection * Time.deltaTime);
        }
    }
    
}
