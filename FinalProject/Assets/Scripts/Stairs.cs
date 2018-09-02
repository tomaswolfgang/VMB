using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stairs : MazeUnit {
    
	

    public void setDirOfStairs(char dir)
    {
        //print("setting dir of stairs");
        Transform target = this.transform.FindChild("Quad");
        target.eulerAngles = new Vector3(220, convertDirToDegrees(dir), 0);
        Transform target2 = this.transform.FindChild("Quad2");
        target.eulerAngles = new Vector3(40, convertDirToDegrees(dir), 0);
    }

    private int convertDirToDegrees(char dir)
    {
        if(dir == 'N')
        {
            return 0;
        }
        else if(dir == 'E')
        {
            return 90;
        }
        else if(dir == 'S')
        {
            return 180;
        }
        else
        {
            return 270;
        }
    }

    private char getOppositeDirection(char dir)
    {
        if (dir == 'N')
            return 'S';
        else if (dir == 'E')
            return 'W';
        else if (dir == 'S')
            return 'N';
        else
            return 'E';
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
