using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class City : MonoBehaviour {
    public Building buildingPrefab;
    static Building[,] cityFloor;
    static List<Building> buildingList;
    static List<Building> findTallestBuilding;
    static List<Building> finalBuildingList;
    static int sizeX = 35;
    static int sizeZ = 35;
    int buildingSizeLow = 3;
    int buildingSizeHigh = 5;
    int buildingDistLow = 7;
    int buildingDistHigh = 9;
    int adjacentBuildingHeightDifference = 3;
    static int xstart;
    static int zstart;
    static bool restart = false;
    static int goalDistance = 0;


    private static float totalWinDist;

    // Update is called once per frame
    void Update()
    {
        if (restart)
        {
            restart = false;
            initCity();
        }
    }

    public static void restartCity()
    {
        for(int i = 0; i < finalBuildingList.Count; i++)
        {
            Destroy(finalBuildingList[i].gameObject);
        }
        cityFloor = new Building[sizeX, sizeZ];
        buildingList = new List<Building>();
        findTallestBuilding = new List<Building>();
        finalBuildingList = new List<Building>();
        xstart = Random.Range(10, sizeX - 10);
        zstart = Random.Range(10, sizeZ - 10);

        restart = true;
        goalDistance = 0;
        
    }

    // Use this for initialization
    void Start () {
        cityFloor = new Building[sizeX, sizeZ];
        buildingList = new List<Building>();
        findTallestBuilding = new List<Building>();
        finalBuildingList = new List<Building>();
        //xstart = Random.Range(10, sizeX - 10);
        //zstart = Random.Range(10, sizeZ - 10);
        xstart = 10;
        zstart = 10;

        //Building startBuilding = Instantiate(buildingPrefab);

        //startBuilding.transform.localPosition = new Vector3(7, 0, 7);
        //startBuilding.transform.SetParent(transform);
        //int startSize = 30;
        //startBuilding.setStartingLevel(0);
        //startBuilding.setPos(10, 10);
        //occupyBuildingSpace(10, 10, startSize, startBuilding);
        //startBuilding.generateMazeFloor(Random.Range(1, startSize - 1), 0, 'N', 0, new MazeUnit[startSize, startSize], startSize);
        //buildingList.Add(startBuilding);
        //findTallestBuilding.Add(startBuilding);
        //tracebackGoalDistance(startBuilding);
        //print("TOTAL PATH LENGTH UP:: " + goalDistance + "   with " + findTallestBuilding.Count + " orbs to collect");

        initCity();
    }

    public void initCity()
    {
        print("initing city!");
        Building startBuilding = Instantiate(buildingPrefab);
        
        startBuilding.transform.localPosition = new Vector3(xstart, 0, zstart);
        startBuilding.transform.SetParent(transform);
        startBuilding.setPos(xstart, zstart);
        startBuilding.setStartingLevel(0);
        int startSize = generateRandomBuildingSize();
        occupyBuildingSpace(xstart, zstart, startSize, startBuilding);
        startBuilding.generateMazeFloor(Random.Range(1, startSize - 1), 0, 'N', 0, new MazeUnit[startSize, startSize], startSize);
        startBuilding.setSource(null);
        buildingList.Add(startBuilding);
        findTallestBuilding.Add(startBuilding);
        finalBuildingList.Add(startBuilding);

        int testcounter = 0;
        while (buildingList.Count > 0 && testcounter < 100)
        {
            testcounter++;
            Building curr = buildingList[0];
            int count = 0;
            char dir = generateRandomDirection();
            int xdir = generateRandomDistanceBetweenBuilding();
            int zdir = generateRandomDistanceBetweenBuilding();
            int newBuildingSize = generateRandomBuildingSize();

            while (count < 4 && !isValidBuildingSpace(XafterDirection(dir, curr, xdir), ZafterDirection(dir, curr, zdir), newBuildingSize))
            {
                count++;
                dir = generateRandomDirection();
            }

            if (!isValidBuildingSpace(XafterDirection(dir, curr, xdir), ZafterDirection(dir, curr, zdir), newBuildingSize))
            {
                buildingList.Remove(curr);
            }
            else
            {
                int newBuildingXPos = XafterDirection(dir, curr, xdir);
                int newBuildingZPos = ZafterDirection(dir, curr, xdir);
                Building newbuilding = Instantiate(buildingPrefab);
                newbuilding.setPos(newBuildingXPos, newBuildingZPos);
                newbuilding.setSize(newBuildingSize, newBuildingSize);
                newbuilding.setSource(curr); 
                newbuilding.transform.localPosition = new Vector3(newBuildingXPos, 0, newBuildingZPos);
                newbuilding.transform.SetParent(transform);
                occupyBuildingSpace(newBuildingXPos, newBuildingZPos, newBuildingSize, newbuilding);
                //print(curr.getEndLevel() + " is endlvl");
                newbuilding.setStartingLevel(curr.getEndLevel() + adjacentBuildingHeightDifference);
                newbuilding.generateMazeFloor(generateRandomXOpening(getOppositeDirection(dir), newBuildingSize), generateRandomZOpening(getOppositeDirection(dir), newBuildingSize), dir, curr.getEndLevel() + adjacentBuildingHeightDifference, new MazeUnit[newBuildingSize, newBuildingSize], newBuildingSize);
                buildingList.Add(newbuilding);
                findTallestBuilding.Add(newbuilding);
                finalBuildingList.Add(newbuilding);
            }
        }

        //place win spheres
        findTallestBuilding = getTallestBuildings(findTallestBuilding);
        //print(findTallestBuilding.Count + " is the tallest building count");
        
        for(int i = 0; i < findTallestBuilding.Count; i++)
        {
            
            Building b = findTallestBuilding[i];
            b.placeWin();
            if (i != 0)
            {
                totalWinDist += Vector3.Distance(findTallestBuilding[i - 1].getMyWinLocation(), findTallestBuilding[i].getMyWinLocation());
            }
        }
        tracebackGoalDistanceList(findTallestBuilding);
        print("TOTAL PATH LENGTH UP:: " + goalDistance + "   with " + findTallestBuilding.Count + " orbs to collect");
        InitGame.setWinReq(findTallestBuilding.Count, goalDistance);

        
    }

    private void tracebackGoalDistanceList(List<Building> l)
    {
        for(int i =0; i < l.Count; i++)
        {
            tracebackGoalDistance(l[i]);
        }
    }

    private void tracebackGoalDistance(Building b)
    {
        Building curr = b;

        while(curr.getSource() != null && !curr.getSource().getTracebackCounted())
        {
            curr = curr.getSource();
            goalDistance += curr.getBuildingPathDistance();
            curr.tracebackCount();
        }
    }

    private List<Building> getTallestBuildings( List<Building> l)
    {
        Building tallest = getTallestBuilding(l);
        List<Building> resList = new List<Building>();
        //print("height of tallest is " + tallest.getStartingLevel());
        //removing all buildings with less than the tallest starting level
        for(int i= 0; i < l.Count; i++)
        {
            //print(l[i].getStartingLevel() + " is level to compare with " + tallest.getStartingLevel());
            if(l[i].getStartingLevel() >= tallest.getStartingLevel())
            {
                //print("just added dat shiit");
                resList.Add(l[i]);
            }
        }

        //get second tallest and place sphere
        if(resList.Count <= 1)
        {
            l.Remove(tallest);
            resList.Add( getTallestBuilding(l));
        }


        return resList;
    }

    private Building getTallestBuilding(List<Building> l)
    {
        Building currentMax = l[0];
        for (int i = 1; i < l.Count; i++)
        {
            if (l[i] != null)
            {
                if (l[i].getStartingLevel() >= currentMax.getStartingLevel())
                {
                    currentMax = l[i];
                }
            }
            
        }

        return currentMax;
    }

    private int generateRandomXOpening(char dir, int size)
    {
        if (dir == 'N' || dir == 'S')
        {
            return Random.Range(1, size - 2);
        }
        else if (dir == 'E')
        {
            return size - 1;
        }
        else
        {
            return 0;
        }
    }

    private int generateRandomZOpening(char dir, int size)
    {
        if (dir == 'E' || dir == 'W')
        {
            return Random.Range(1, size - 2);
        }
        else if (dir == 'S')
        {
            return 0;
        }
        else
        {
            return size - 1;
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
    private int XafterDirection(char dir, Building b, int dist)
    {
        if (dir == 'E')
        {
            return b.getXPos() + dist;
        }
        else if (dir == 'W')
        {
            return b.getXPos() - dist;
        }
        else
        {
            return b.getXPos();
        }
    }

    private int ZafterDirection(char dir, Building b, int dist)
    {
        if (dir == 'N')
        {
            return b.getZPos() + dist;
        }
        else if (dir == 'S')
        {
            return b.getZPos() - dist;
        }
        else
        {
            return b.getZPos();
        }
    }

    private void occupyBuildingSpace(int x, int z, int size, Building b)
    {

        for (int i = x; i < x + size - 1; i++)
        {
            for (int j = z; j < z + size -1 ; j++)
            {
                cityFloor[i, j] = b;
            }
        }
    }

    private char generateRandomDirection()
    {
        int dir = (int)Random.Range(1, 5);
        if (dir == 1)
        {
            return 'N';
        }
        else if (dir == 2)
        {
            return 'E';
        }
        else if (dir == 3)
        {
            return 'S';
        }
        else
        {
            return 'W';
        }
    }

    private int generateRandomBuildingSize()
    {
        return Random.Range(buildingSizeLow, buildingSizeHigh);
    }

    private int generateRandomDistanceBetweenBuilding()
    {
        return Random.Range(buildingDistLow, buildingDistHigh);
    }

    private bool isValidBuildingSpace(int x, int z, int size)
    {

        for(int i = x - 2; i < x + size + 1; i++)
        {
            for(int j = z - 2; j < z + size + 1; j++)
            {
                if(!withinBounds(i, j))
                {
                    return false;
                }
                else
                {
                    if(cityFloor[i,j] != null)
                    {
                        return false;
                    }
                }
            }
        }

        return true;
    }

    private bool withinBounds(int x, int z)
    {
        if( x < sizeX - 1 && x >= 0 && z < sizeZ - 1 && z >= 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    
	
	
}
