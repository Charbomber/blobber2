using UnityEngine;
using System;
using System.Collections.Generic;

public class ScriptTrigger : MonoBehaviour {

    public enum triggerType : byte {
        touch,
        interact,
        constant,
        timer,
        dont
    }

    public triggerType myTrigger;

    [SerializeField]
    private float interactDist = PlayerMove.GRID_SIZE / 2;
    // so, you need to be on the tile to interact
    // (in case I need anything like something you can interact with at a distance E.G. looking at a sign)

    // String used when checking something out
    public string interactString = "Look";

    // program to run when interacting with this script trigger
    [System.NonSerialized]
    public string[,] myProgram;
    // Unity doesn't seralize this :[
    // ig I just have to make a hacky solution

    // nvm ill just do a non-hacky solution instead
    [SerializeField]
    private TextAsset progLoad;

    // Current program position
    public int pos;

    // timer for various things
    public float scriptTimer = 0;
    // timer mode (what we do with the timer)
    public enum timerModes {
        nothing,
        cont, // 'continue' is a keyword
        stop
    }
    public timerModes timerMode = timerModes.nothing; // totally not confusing variable names right? :]
    // whether we should be running or not
    public bool runningScript = false;

    // Player reference
    public GameObject Player;
    public PlayerMove   PlayerMove; // sorry this sucks superass
    public PlayerStats  PlayerStats;
    public BattleEntity PlayerBattle;

    void Start() {
        // very cool, very convenient
        Player = GameObject.Find("Player");

        PlayerMove   = Player.GetComponent<PlayerMove>();
        PlayerStats  = Player.GetComponent<PlayerStats>();
        PlayerBattle = Player.GetComponent<BattleEntity>();

        if (progLoad) { myProgram = PlayerMove.ScriptInterpret.Txt2Prog(progLoad); progLoad = null; } // load program based on progLoad and free up its memory after
    }
    
    void Update() {
        if (scriptTimer > 0f) {
            scriptTimer -= Time.deltaTime;

            // when timer runs out...
            if (scriptTimer <= 0f) {
                scriptTimer = 0f;

                // do things as needed
                switch (timerMode) {
                    case timerModes.cont:
                        runningScript = true;
                        break;
                    
                    case timerModes.stop:
                        runningScript = false;
                        break;

                    // i bet this will look really dumb if i never need to add any more types
                }
            }
        }
    }

    // shorthand for Interpret(myProgram);
    public void RunScript() {
        PlayerMove.ScriptInterpret.Interpret(this, myProgram);
    }
}
