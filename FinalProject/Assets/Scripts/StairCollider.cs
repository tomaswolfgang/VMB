using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StairCollider : MonoBehaviour {
    bool hasPausedLava = false;
    public Material usedStairs;
    private Renderer rend1;
    private Renderer rend2;

    void OnTriggerEnter(Collider other)
    {
        //print("hi there you collided with stairs");
        if (!hasPausedLava)
        {
            InitGame.stairPause();
            hasPausedLava = true;
            print("just stair paused lava");

            rend1.material = usedStairs;
            rend2.material = usedStairs;
        }
       
    }
    // Use this for initialization
    void Start () {
        rend1 = transform.parent.GetComponent<Renderer>();
        rend2 = transform.GetComponent<Renderer>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
