using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeUnit : MonoBehaviour {
    public char dir;
    public int x;
    public int z; 



    public void setPos(int _x, int _z)
    {
        x = _x;
        z = _z;
    }

	// Use this for initialization
	void Start () {
		
	}

    public void setDirection(char d)
    {
        dir = d;
    }

    public void deleteWall(char dir)
    {
        GameObject.Destroy(this.transform.FindChild("UnitWall" + dir).gameObject);
    }

    public void deleteFloor()
    {
        GameObject.Destroy(this.transform.FindChild("UnitFloor").gameObject);
    }

    public void deleteCeiling()
    {
        GameObject.Destroy(this.transform.FindChild("UnitCeiling").gameObject);
    }

    public void deleteAllWallCheck()
    {
        if (this.transform.childCount < 3)
        {
           
            GameObject.Destroy(this.transform.FindChild("UnitWallN").gameObject);
            GameObject.Destroy(this.transform.FindChild("UnitWallS").gameObject);
            GameObject.Destroy(this.transform.FindChild("UnitWallE").gameObject);
            GameObject.Destroy(this.transform.FindChild("UnitWallW").gameObject);
        }
    }
	
    public void onlyLeaveCeiling()
    {
        GameObject.Destroy(this.transform.FindChild("UnitWallN").gameObject);
        GameObject.Destroy(this.transform.FindChild("UnitWallS").gameObject);
        GameObject.Destroy(this.transform.FindChild("UnitWallE").gameObject);
        GameObject.Destroy(this.transform.FindChild("UnitWallW").gameObject);
        GameObject.Destroy(this.transform.FindChild("UnitFloor").gameObject);

    }

    public void onlyLeaveFloor()
    {
        GameObject.Destroy(this.transform.FindChild("UnitWallN").gameObject);
        GameObject.Destroy(this.transform.FindChild("UnitWallS").gameObject);
        GameObject.Destroy(this.transform.FindChild("UnitWallE").gameObject);
        GameObject.Destroy(this.transform.FindChild("UnitWallW").gameObject);
        GameObject.Destroy(this.transform.FindChild("UnitCeiling").gameObject);
    }
    // Update is called once per frame
    void Update () {
		
	}
}
