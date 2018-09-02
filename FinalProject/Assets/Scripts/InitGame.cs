using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InitGame : MonoBehaviour {
    
    public Player playerPrefab;
    public Lava lavaPrefab;
    private static float difficulty = 1f;
    private static Lava myLava;
    private static Player myPlayer;
    static int winRequirement;
    static float winDist;
    static int winCount = 0;
    static bool win = false;
    static bool restart = false;
    

    private bool lavaStarted = false;
	// Use this for initialization
	void Start () {
        myLava = Instantiate(lavaPrefab);
        myLava.transform.position = new Vector3(21, -1.5f, 21);

        myPlayer = Instantiate(playerPrefab);
        myPlayer.transform.position = new Vector3(42f, 1, 42f);
		
	}

    public static void setWinReq(int r, float t)
    { 
        winRequirement = r;
        winDist = t;
    }

    public static void stairPause()
    {
        if (myLava != null)
        {
            myLava.pauseLava(1.5f);
        }
    }

    public static void winGame()
    {
        winCount++;
        print("hi");
        if (winCount == winRequirement)
        {
            print("you have officially won!");
            myLava.stopLava();
            myPlayer.playerWin();
            //Text t = GameObject.FindObjectOfType<Text>();
            //t.text = "YOU WIN!!!! \n Press R to restart, \n Press M to return to the main menu";
            myPlayer.changeText( "YOU WIN!!!! \n Press R to restart");

            win = true;
            //press r to restart
            difficulty += 0.1f;
            //press s to restart with super jumping

        }
    }

    public static void loseGame()
    {
        win = true;
        myPlayer.playerLose();
        myLava.stopLava();
        if (difficulty > .05)
        {
            difficulty  = difficulty - .05f;
        }

    }

    public void restartGame()
    {
        print("restart");
        winRequirement = 0;
        winDist = 0;
        winCount = 0;
        win = false;
        restart = false;
        lavaStarted = false;
        myPlayer.restartPlayer();
        myLava.restartLava();
        City.restartCity();
        myPlayer.transform.position = new Vector3(35f, 1, 35f);
        


    }

    // Update is called once per frame
    void Update () {
		if(myPlayer.transform.position.y > 3 && !lavaStarted)
        {
            //start lava
            //velocity is 0.1 / # of win reqs
            myLava.startLava( difficulty * winRequirement /Mathf.Sqrt(winDist)) ;
            lavaStarted = true;
            print(difficulty + " is difficulty");
            print(winRequirement + " is winReq");
            print(winDist + " is winDist");
            print(difficulty / Mathf.Sqrt(winDist) * winRequirement + " is my lava velocity");
        }

        if(myPlayer.transform.position.y < -3)
        {
            loseGame();
        }

        if(win && !restart)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                restartGame();
            }
        }
	}
}
