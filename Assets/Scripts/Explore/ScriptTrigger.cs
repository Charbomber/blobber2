using UnityEngine;
using System;
using System.Collections.Generic;

public class ScriptTrigger : MonoBehaviour {

    public enum triggerType {
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
    private int pos;

    // timer for various things
    private float scriptTimer = 0;
    // timer mode (what we do with the timer)
    private enum timerModes {
        nothing,
        cont, // 'continue' is a keyword
        stop
    }
    private timerModes timerMode = timerModes.nothing; // totally not confusing variable names right? :]
    // whether we should be running or not
    private bool runningScript = false;

    // Player reference
    GameObject Player;
    PlayerMove   PlayerMove;
    PlayerStats  PlayerStats;
    BattleEntity PlayerBattle;

    void Start() {
        // very cool, very convenient
        Player = GameObject.Find("Player");

        PlayerMove   = Player.GetComponent<PlayerMove>();
        PlayerStats  = Player.GetComponent<PlayerStats>();
        PlayerBattle = Player.GetComponent<BattleEntity>();
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

    // Full-Program interpreting
    void Interpret(string[,] prog) {

    }
    // Single-Command interpreting
    // (say hello to the full interpreter)
    void Interpret(string command, params string[] arguments) {
        
        // da big switch statement..............
        switch (command) {
            // Incorrect Command Handler //////////////////////////////////////////////////////////////////
            default:
                Debug.Log("<color=red>[SCR-ERR]</color> Incorrect Command, \""+command+"\"", gameObject);
                break;
            ///////////////////////////////////////////////////////////////////////////////////////////////

            
            //                                   [                      ]                                   //
            //-----------------------------------[ Script Flow Commands ]-----------------------------------//
            //                                   [                      ]                                   //
            case "*":
                // do nothing.....
                // (label)
                // (this doesn't have to be recognized by the parser but it's good for coder eyes)
                break;
            case "{":
                break;
            case "}":
                break;
                // These will probably be defined by the scripts that use them.



            case "jumpLabel":
                pos = findLabel(arguments[0]);
                break;

            case "jumpLine":
                pos = int.Parse(arguments[0]);
                break;
            
            case "jumpLineIf":
                bool doJump = false;

                if (arguments[1] == "flagString") {
                    if (PlayerStats.scriptFlag.TryGetValue(arguments[2], out string rawFlag)) {

                        // TODO: jumpLineIf

                    }
                }

                // commit to jump if it clears
                if (doJump) {pos = int.Parse(arguments[0]);}
                break;

            case "wait":
                timerMode = timerModes.cont;
                scriptTimer = int.Parse(arguments[0]);
                runningScript = false;
                break;
            

            //                                   [                ]                                   //
            //-----------------------------------[ Story Commands ]-----------------------------------//
            //                                   [                ]                                   //

            case "text":
            case "txt":
                // placeholder while I figure out text
                Debug.Log(arguments[0]);
                //runningScript = false;
                //PlayerMove.frozen = true;
                // TODO: this. we'll stop the script then spawn a textbox that continues it.
                // TODO: if no text is after this script unfreeze player.
                break;
            
            case "choice":
                // placeholder while I figure out text
                // TODO: all of this
                // REMEMBER: if string is blank, just continue
                break;
            
            case "setFlag":
                if (!PlayerStats.scriptFlag.TryAdd(arguments[0], arguments[1])) {
                    PlayerStats.scriptFlag.Remove(arguments[0]);
                    if (!PlayerStats.scriptFlag.TryAdd(arguments[0], arguments[1])) {
                        Debug.Log("<color=red>[SCR-ERR]</color> Problem attempting to set flag \""+arguments[0]+"\" to "+arguments[1]);
                    }
                }
                break;
            


            //                                   [                   ]                                   //
            //-----------------------------------[ Movement Commands ]-----------------------------------//
            //                                   [                   ]                                   //

            case "freeze":
            case "frz":
                PlayerMove.frozen = true;
                break;
            case "unfreeze":
            case "unfrz":
                PlayerMove.frozen = false;
                break;
            
            case "teleport":
            case "tp":
                Player.transform.position = new Vector3(
                    int.Parse(arguments[0]),
                    int.Parse(arguments[1]),
                    int.Parse(arguments[2])
                );
                PlayerMove.EmergencySnap();
                break;
            case "teleportRelative":
            case "tpr":
                Player.transform.position = new Vector3(
                    Player.transform.position.x + int.Parse(arguments[0]),
                    Player.transform.position.y + int.Parse(arguments[1]),
                    Player.transform.position.z + int.Parse(arguments[2])
                );
                PlayerMove.EmergencySnap();
                break;
        }
    }

    // down here because i have confidence i dont need to bugfix it you better not prove me wrong
    int findLabel(string myLabel) {
        for (int i=0; i<myProgram.GetLength(0); i++) {

            if (myProgram[i,0] == "*") { // we got a label
                if (myProgram[i,1] == myLabel) {return i;} // return with da goods
            }
        }
        return -1;
    }
}
