using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinCollider : MonoBehaviour {

    void OnTriggerEnter(Collider other)
    {
        
        Destroy(gameObject);
        InitGame.winGame();
    }
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
