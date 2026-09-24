using UnityEngine;
using UnityEngine.InputSystem;
using System.IO;

public class Textbox : MonoBehaviour {
    
    private InputAction direction;
    private InputAction confirm;
    private InputAction cancel;

    public Vector2 fromSize = new Vector2(0, 0);
    public Vector2 toSize = new Vector2(6, 1.5f);
    private float sizeTimer = 0f;

    private StringReader reader;
    public  string text = "[Text Error]\nReport to Char";
    private int txtProg = 0;
    
    private bool waitScroll = false;

    private TextMesh textmesh;
    private SpriteRenderer render;
    // im lazy
    private Vector3 textmeshpos;

    // whether or not there is a textbox waiting after this
    public bool nextbox = false;
    public ScriptTrigger myScript;

    public PlayerMove player;

    // after realizing that the rest of my game is following Unity Style Guide i would like to make an apology to it
    // (but not really, camelCase is better)
    void SetText(string myTxt) {
        text = myTxt;
    }
    string GetText() {
        return text;
    }

    void Start() {
        direction = InputSystem.actions.FindAction("Move");
        confirm   = InputSystem.actions.FindAction("Confirm");
        cancel    = InputSystem.actions.FindAction("Cancel");

        textmesh = transform.GetChild(0).gameObject.GetComponent<TextMesh>();
        render = GetComponent<SpriteRenderer>();
        textmeshpos = textmesh.transform.position;
        
        render.size = fromSize;
        textmesh.GetComponent<Renderer>().enabled = false;
        
        reader = new StringReader(text);

        textmesh.text = reader.ReadLine()+"\n"+reader.ReadLine()+"\n"+reader.ReadLine();
    }

    void Update() {
        
        bool myConfirm = confirm.WasPressedThisFrame();
        bool myCancel  =  cancel.WasPressedThisFrame();

        if (sizeTimer < 1f) {
            render.size = new Vector2(Mathf.Lerp(fromSize.x, toSize.x, sizeTimer), Mathf.Lerp(fromSize.y, toSize.y, sizeTimer));
            sizeTimer += Time.deltaTime*10f;
            if (sizeTimer >= 1f) {
                sizeTimer = 1f;
                render.size = toSize;
                textmesh.GetComponent<Renderer>().enabled = true;
            }
            return;
        }
        ///////////// (SIZE TIMER DONE) ///////////////
        
        // while waiting for text to scroll
        if (waitScroll) {
            textmesh.transform.position = textmesh.transform.position + (textmesh.transform.up * Time.deltaTime * 4f);
            if (textmesh.transform.position.y > textmeshpos.y + 0.25f) {
                textmesh.text = textmesh.text.Remove(0, 1)+"\n"+reader.ReadLine();
                textmesh.transform.position = textmeshpos;
                txtProg++;
                waitScroll = false;
                if (txtProg % 3 != 0) {NextLine();} // three lines at a time
            }
            return;
        }
        
        if (myConfirm || myCancel) { // on input...


            // if there's more than 3 text left...
            if (txtProg < text.Split('\n').Length - 3) {
                NextLine(); // newline
                return;
            }
            
            // no more text, so remove textbox
            if (!nextbox && !myScript) {player.frozen = false;} // plus unfreeze if not last textbox!
            else { // if there *IS* a next box though...
                if (myScript) { // continue my script so it can make the next box
                    player.ScriptInterpret.Interpret(myScript, "txtDone"); // yuuuup
                }
            }
            Destroy(gameObject);
        }

    }

    void NextLine() {
        waitScroll = true;
        textmesh.text = textmesh.text.Remove(0, textmesh.text.IndexOf('\n'));
    }
}
