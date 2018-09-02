using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour {

    private int xPos;
    private int zPos;
    private Building Source;
    private int startingLevel;
    private int height;
    public MazeUnit mazeUnitPrefab;
    public Stairs stairsPrefab;
    MazeUnit[,] mazeFloor;
    List<MazeUnit> mazeUnitList;
    MazeUnit lastMazeUnit;
    public Win winPrefab;
    private Win myWin;
    private int lastStairX;
    private int lastStairZ;
    int sizeX = 3;
    int sizeZ = 3;
    private int distance = 0;
    private bool tracebackCounted = false;
    
    public void tracebackCount()
    {
        tracebackCounted = true;
    }

    public bool getTracebackCounted()
    {
        return tracebackCounted;
    }

    public Building getSource()
    {
        return Source;
    }

    public void setSource(Building b)
    {
        Source = b;
    }


    //during generation get sum of distance for each floor

    public Vector3 getMyWinLocation()
    {
        return myWin.transform.position;
    }

    public void placeWin()
    {
        myWin = Instantiate(winPrefab);
        myWin.transform.SetParent(transform);
        for(int i = lastStairX - 1; i < lastStairX + 2; i++)
        {
            for(int j = lastStairZ - 1; j < lastStairZ + 2; j++)
            {
                if (withinBounds(i, j, 'C') && !(i==lastStairX && j == lastStairZ))
                {
                    if (mazeFloor[i, j] != null)
                    {
                        //print(getEndLevel() + 2 + " is end level");
                        myWin.transform.localPosition = new Vector3(i, getEndLevel() + 2, j);
                    }
                }
            }
        }
        
    }

    public int getBuildingPathDistance()
    {
        return distance;
    }

    
    
    public void setPos(int x, int z)
    {
        zPos = z;
        xPos = x;
    }

    public int getXPos()
    {
        return xPos;
    }

    public int getZPos()
    {
        return zPos;
    }

    public int getEndLevel()
    {
        return  height - 1;
    }

    public void setSize(int x, int z)
    {
        sizeX = x;
        sizeZ = z;
    }

    public int getXSize()
    {
        return sizeX;
    }

    public int getZSize()
    {
        return sizeZ;
    }

    public int getStartingLevel()
    {
        return startingLevel;
    }

    public void setStartingLevel(int s)
    {
        startingLevel = s;
        height = startingLevel + 2;
        for (int i = 0; i < startingLevel; i++)
        {
            createEmptyFloor(i);
        }
    }
	// Use this for initialization
	void Start () {
        
        
        //MazeUnit[,] floor = new MazeUnit[sizeX, sizeZ];
        //generateMazeFloor(2 , 0, 'N', startingLevel, floor);
	}



    public void generateMazeFloor(int xStart, int zStart, char direction, int level, MazeUnit[,] floor, int size)
    {
        sizeX = size;
        sizeZ = size;
        
        mazeFloor = floor;
        mazeUnitList = new List<MazeUnit>();
        MazeUnit firstUnit = Instantiate(mazeUnitPrefab);
        mazeFloor[xStart, zStart] = firstUnit;
        mazeFloor[xStart, zStart].setPos(xStart, zStart);
        mazeFloor[xStart, zStart].transform.SetParent(transform);
        mazeFloor[xStart, zStart].transform.localPosition = new Vector3(xStart, level, zStart);
        if (isOnEdge(xStart, zStart))
        {
            mazeFloor[xStart, zStart].deleteWall('E');
            mazeFloor[xStart, zStart].deleteWall('W');
            mazeFloor[xStart, zStart].deleteWall('S');
            mazeFloor[xStart, zStart].deleteWall('N');
            if (level == startingLevel)
            {
                for (int i = xStart - 2; i < xStart + 3; i++)
                {
                    for (int j = zStart - 2; j < zStart + 3; j++)
                    {
                        if (i != xStart || j != zStart)
                        {
                            MazeUnit edgeUnit = Instantiate(mazeUnitPrefab);
                            edgeUnit.setPos(i, j);
                            edgeUnit.transform.SetParent(transform);
                            edgeUnit.transform.localPosition = new Vector3(i, level, j);
                            edgeUnit.onlyLeaveFloor();
                        }
                    }
                }
            }
        }
        if (level != startingLevel)
        {
            mazeFloor[xStart, zStart].deleteFloor();
        }

        int xcurr = XafterDirection(xStart, direction);
        int zcurr = ZafterDirection(zStart, direction);

        if(!withinBounds(xStart, zStart, direction))
        {
            mazeFloor[xStart, zStart].deleteWall(direction);
            MazeUnit[,] f = new MazeUnit[sizeX, sizeZ];
            f[xStart, zStart] = firstUnit;

            for(int i = xStart - 1; i < xStart + 2; i++)
            {
                for(int j = zStart - 1; j < zStart + 2; j++)
                {
                    if(withinBounds(i, j, 'C') && isOnEdge(i, j))
                    {
                        generateMazeFloor(i, j, getOppositeDirection(direction), level, f, size);
                        return;
                    }
                }
            }
           
            
            
        }

        char randDir = generateRandomDirection();


        mazeUnitList.Add(createNewUnitInMaze(direction, xStart, zStart));
        int superCount = 0; 
        while(mazeUnitList.Count > 0 && superCount < 200)
        {
            superCount++;
            xcurr = mazeUnitList[0].x;
            zcurr = mazeUnitList[0].z;
            int count = 0;
            while (!isEmptyMazeSpot(xcurr, zcurr, randDir, mazeFloor) && count < 4 )
            {
                count++;
                randDir = generateRandomDirection();
            }

            if (!withinBounds(xcurr,zcurr, randDir))
            {
                mazeUnitList.Remove(mazeUnitList[0]);
            }
            else if(!isEmptyMazeSpot(xcurr, zcurr, randDir, mazeFloor))
            {
                mazeFloor[xcurr, zcurr].deleteWall(randDir);
                mazeFloor[XafterDirection(xcurr, randDir), ZafterDirection(zcurr, randDir)].deleteWall(getOppositeDirection(randDir));
                mazeUnitList.Remove(mazeUnitList[0]);
            }
            else
            {
                mazeUnitList.Add(createNewUnitInMaze(randDir, xcurr, zcurr));
            }

        }
        
        
        for(int i = 0; i < sizeX; i++)
        {
            for(int j = 0; j < sizeZ; j++)
            {
                if(mazeFloor[i,j] != null)
                {
                    mazeFloor[i, j].deleteAllWallCheck();
                }
            }
        }

        

        //placing stairs!
        int xStair = lastMazeUnit.x;
        int zStair = lastMazeUnit.z;
        char dirStair = lastMazeUnit.dir;
        mazeFloor[xStair, zStair].deleteCeiling();

        Stairs s = Instantiate(stairsPrefab);
        s.transform.SetParent(transform);
        s.transform.localPosition = new Vector3(xStair, level, zStair);
        s.setDirOfStairs(dirStair);
        for (int i = xStair - 1; i < xStair + 2; i++)
        {
            for (int j = zStair - 1; j < zStair + 2; j++)
            {
                if (i != xStair || j != zStair)
                {
                    MazeUnit edgeUnit = Instantiate(mazeUnitPrefab);
                    edgeUnit.setPos(i, j);
                    edgeUnit.transform.SetParent(transform);
                    edgeUnit.transform.localPosition = new Vector3(i, level, j);
                    edgeUnit.onlyLeaveCeiling();
                }
            }
        }

        //run traceback
        getTraceBackDistance(xStart, zStart, xStair, zStair);
        

        if (level < height)
        {
            generateMazeFloor(xStair, zStair, dirStair, level + 1, new MazeUnit[sizeX, sizeZ], sizeX);
        }
        else
        {
            lastStairX = xStair;
            lastStairZ = zStair;
        }


    }

    private void getTraceBackDistance(int xstart, int zstart, int xend, int zend)
    {
        int xcurr = xend;
        int zcurr = zend;
        distance++;
        //print("Traceback " + xstart + "," + zstart + "   to   " + xend + "," + zend);
        while(xcurr != xstart || zcurr != zstart)
        {

            //print("increment through " + xcurr + "," + zcurr);

            distance++;
            char dir = getOppositeDirection( mazeFloor[xcurr, zcurr].dir);
            xcurr = XafterDirection(xcurr, dir);
            zcurr = ZafterDirection(zcurr, dir);
        }
    }

    private void createEmptyFloor(int level)
    {
        for(int i= 0; i < sizeX; i++)
        {
            int j = 0;
            int j2 = sizeZ-1;
            MazeUnit edgeUnit = Instantiate(mazeUnitPrefab);
            edgeUnit.setPos(i, j);
            edgeUnit.transform.SetParent(transform);
            edgeUnit.transform.localPosition = new Vector3(i, level, j);
            MazeUnit edgeUnit2 = Instantiate(mazeUnitPrefab);
            edgeUnit2.setPos(i, j2);
            edgeUnit2.transform.SetParent(transform);
            edgeUnit2.transform.localPosition = new Vector3(i, level, j2);
            MazeUnit edgeUnit3 = Instantiate(mazeUnitPrefab);
            edgeUnit3.setPos(j, i);
            edgeUnit3.transform.SetParent(transform);
            edgeUnit3.transform.localPosition = new Vector3(j, level, i);
            MazeUnit edgeUnit4 = Instantiate(mazeUnitPrefab);
            edgeUnit4.setPos(j2, i);
            edgeUnit4.transform.SetParent(transform);
            edgeUnit4.transform.localPosition = new Vector3(j2, level, i);

        }
    }

    private bool isOnEdge(int x, int z)
    {
        if(x<1 || x > sizeX - 2 || z < 1 || z > sizeZ - 2)
        {
            return true;
        }
        else
        {
            return false;
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

    private int XafterDirection(int x, char dir)
    {
        if(dir == 'E')
        {
            return x + 1;
        }
        else if( dir == 'W')
        {
            return x - 1;
        }
        else
        {
            return x;
        }
    }

    private int ZafterDirection(int z, char dir)
    {
        if (dir == 'N')
        {
            return z + 1;
        }
        else if (dir == 'S')
        {
            return z - 1;
        }
        else
        {
            return z;
        }
    }

    private bool isEmptyMazeSpot(int x, int z, char dir, MazeUnit[,] mazeFloor)
    {
        if (withinBounds(x, z, dir))
        {
            //print("x is " + x + "    |  z is " + z + "   | dir is " + dir);
            if (mazeFloor[XafterDirection(x, dir), ZafterDirection(z, dir)] == null)
            {
                //print("x is " + x + "    |  z is " + z + "   | dir is " + dir);
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    private bool withinBounds(int x, int z, char dir)
    {
        if (dir == 'N' && z <= sizeZ - 2)
        {
            return true;
        }
        else if (dir == 'E' && x <= sizeX - 2)
        {
            return true;
        }
        else if (dir == 'S' && z >= 1)
        {
            return true;
        }
        else if (dir == 'W' && x >= 1)
        {
            return true;
        }
        else if (dir == 'C' && x <= sizeX - 1 && x >= 0 && z <= sizeZ - 1 && z >= 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private MazeUnit createNewUnitInMaze(char dir, int xstart, int zstart)
    {
        MazeUnit oldUnit = mazeFloor[xstart, zstart];
        MazeUnit newUnit = Instantiate(mazeUnitPrefab);
        newUnit.setDirection(dir);
        
        mazeFloor[XafterDirection(xstart, dir),ZafterDirection(zstart, dir)] = newUnit;
        newUnit.setPos(XafterDirection(xstart, dir), ZafterDirection(zstart, dir));
        placeNewMazeUnit(newUnit, oldUnit, dir);
        lastMazeUnit = newUnit;
        return newUnit;
    }

    private char generateRandomDirection()
    {
        int dir = (int) Random.Range(1, 5);
        //print("your dir is " + dir);
        if(dir == 1)
        {
            return 'N';
        }
        else if(dir == 2)
        {
            return 'E';
        }
        else if(dir == 3)
        {
            return 'S';
        }
        else
        {
            return 'W';
        }
       
    }

    private void placeNewMazeUnit(MazeUnit newMazeUnit, MazeUnit prevMazeUnit, char dir)
    {
        Vector3 prevLocation = prevMazeUnit.transform.localPosition;
        if(dir == 'N')
        {
            newMazeUnit.transform.SetParent(transform);
            newMazeUnit.transform.localPosition = new Vector3(prevLocation.x, prevLocation.y, prevLocation.z + 1);
            newMazeUnit.deleteWall('S');
            prevMazeUnit.deleteWall('N');
        }
        else if(dir == 'E')
        {
            newMazeUnit.transform.SetParent(transform);
            newMazeUnit.transform.localPosition = new Vector3(prevLocation.x + 1, prevLocation.y, prevLocation.z );
            newMazeUnit.deleteWall('W');
            prevMazeUnit.deleteWall('E');
        }
        else if(dir == 'S')
        {
            newMazeUnit.transform.SetParent(transform);
            newMazeUnit.transform.localPosition = new Vector3(prevLocation.x, prevLocation.y, prevLocation.z - 1);
            newMazeUnit.deleteWall('N');
            prevMazeUnit.deleteWall('S');
        }
        else
        {
            newMazeUnit.transform.SetParent(transform);
            newMazeUnit.transform.localPosition = new Vector3(prevLocation.x - 1, prevLocation.y, prevLocation.z);
            newMazeUnit.deleteWall('E');
            prevMazeUnit.deleteWall('W');
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
