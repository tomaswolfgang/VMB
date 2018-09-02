using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lava : MonoBehaviour {

    bool isMoving = false;
    float lavaPauseTimer = 0f;
    float riseVelocity = 0;
	// Use this for initialization
	void Start () {
		
	}

    public void restartLava()
    {
        isMoving = false;
        riseVelocity = 0;
        lavaPauseTimer = 0f;
        transform.position = new Vector3(21, -1.5f, 21);
    }

    public void startLava(float s)
    {
        print("start LAva");
        riseVelocity = s;
        isMoving = true;
    }

    public void stopLava()
    {
        isMoving = false;
    }
    public void pauseLava(float t)
    {
        lavaPauseTimer = t;
    }

    void OnTriggerEnter(Collider other)
    {

        InitGame.loseGame();
    }
    // Update is called once per frame
    void Update () {
        
        if (isMoving && lavaPauseTimer <= 0)
        {
            this.transform.position += new Vector3(0, riseVelocity * Time.deltaTime);
        }
        else if (lavaPauseTimer > 0)
        {
            lavaPauseTimer -= Time.deltaTime;
        }
    }
}
