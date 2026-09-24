using UnityEngine;
using System.IO;

//[CreateAssetMenu(fileName = "ScriptInterpret", menuName = "Scriptable Objects/ScriptInterpret")]
// realized i should probably. just make these their own classes.
public class ScriptInterpret {

    // using a plain string
    public string[,] Txt2Prog(string program) {
        var reader = new StringReader(program);

        bool slashMode = false;
        //int sds = 0;

        int i = 0;
        for (i=0;i<program.Length;i++) {
            var chr = reader.Read();
            if (slashMode) { // vastly different behaviours based on slash mode

            } else {

            }
        }

        return new string[2,2];
    }

    // overload for a TextAsset
    public string[,] Txt2Prog(TextAsset programRaw) {
        string program = programRaw.ToString();
        return Txt2Prog(program);
    }


    // Full-Program interpreting
    public void Interpret(ScriptTrigger source, string[,] prog) {

    }

    // synonymous with Interpret(command, arguments, nextCommand)
    public void Interpret(ScriptTrigger source, string command, string[] arguments) {
        Interpret(source, command, arguments, "");
    }

    // synonymous with Interpret(command, arguments, nextCommand)
    public void Interpret(ScriptTrigger source, string command) {
        Interpret(source, command, new string[] {""}, "");
    }

    // Single-Command interpreting
    // (say hello to the full interpreter)
    public void Interpret(ScriptTrigger source, string command, string[] arguments, string nextCommand) {
        // da big switch statement..............
        switch (command) {
            // Incorrect Command Handler //////////////////////////////////////////////////////////////////
            default:
                Debug.Log("<color=red>[SCR-ERR]</color> Incorrect Command, \""+command+"\"", source.gameObject);
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
                source.pos = findLabel(source.myProgram, arguments[0]);
                break;

            case "jumpLine":
                source.pos = int.Parse(arguments[0]);
                break;
            
            case "jumpLineIf":
                bool doJump = false;

                if (arguments[1] == "flagString") {
                    if (source.PlayerStats.scriptFlag.TryGetValue(arguments[2], out string rawFlag)) {

                        // TODO: jumpLineIf

                    }
                }

                // commit to jump if it clears
                if (doJump) {source.pos = int.Parse(arguments[0]);}
                break;

            case "wait":
                source.timerMode = ScriptTrigger.timerModes.cont;
                source.scriptTimer = int.Parse(arguments[0]);
                source.runningScript = false;
                break;
            

            //                                   [                ]                                   //
            //-----------------------------------[ Story Commands ]-----------------------------------//
            //                                   [                ]                                   //

            case "text":
            case "txt":
                source.runningScript = false;

                var txtBox = source.PlayerMove.MakeTextbox();
                Textbox txtScr = txtBox.GetComponent<Textbox>();
                txtScr.nextbox = nextCommand == "txt" || nextCommand == "text";
                txtScr.myScript = source;
                break;
            case "txtDone": // sorry, dumb hacky workaround. i know im a lazy bitch
                source.runningScript = true;
                break;
            
            case "choice":
                // placeholder while I figure out text
                // TODO: all of this
                // REMEMBER: if string is blank, just continue
                break;
            
            case "setFlag":
                if (!source.PlayerStats.scriptFlag.TryAdd(arguments[0], arguments[1])) {
                    source.PlayerStats.scriptFlag.Remove(arguments[0]);
                    if (!source.PlayerStats.scriptFlag.TryAdd(arguments[0], arguments[1])) {
                        Debug.Log("<color=red>[SCR-ERR]</color> Problem attempting to set flag \""+arguments[0]+"\" to "+arguments[1]);
                    }
                }
                break;
            


            //                                   [                   ]                                   //
            //-----------------------------------[ Movement Commands ]-----------------------------------//
            //                                   [                   ]                                   //

            case "freeze":
            case "frz":
                source.PlayerMove.frozen = true;
                break;
            case "unfreeze":
            case "unfrz":
                source.PlayerMove.frozen = false;
                break;
            
            case "teleport":
            case "tp":
                source.Player.transform.position = new Vector3(
                    int.Parse(arguments[0]),
                    int.Parse(arguments[1]),
                    int.Parse(arguments[2])
                );
                source.PlayerMove.EmergencySnap();
                break;
            case "teleportRelative":
            case "tpr":
                source.Player.transform.position = new Vector3(
                    source.Player.transform.position.x + int.Parse(arguments[0]),
                    source.Player.transform.position.y + int.Parse(arguments[1]),
                    source.Player.transform.position.z + int.Parse(arguments[2])
                );
                source.PlayerMove.EmergencySnap();
                break;
        }
    }

    // down here because i have confidence i dont need to bugfix it you better not prove me wrong
    int findLabel(string[,] myProgram, string myLabel) {
        for (int i=0; i<myProgram.GetLength(0); i++) {

            if (myProgram[i,0] == "*") { // we got a label
                if (myProgram[i,1] == myLabel) {return i;} // return with da goods
            }
        }
        return -1;
    }
}


// yea we're doing this