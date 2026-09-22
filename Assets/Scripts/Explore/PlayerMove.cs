using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour {
    float lerp = 0.0f; // how far player is in moving (if thats confusing)

    [System.NonSerialized]
    public bool frozen = false;
    [System.NonSerialized]
    public bool forcedMove = false; // whether the player is moving against their will, for encounter stuff

    // [       ]
    // [ INPUT ]
    // [       ]
    private InputAction direction;
    private InputAction confirm;
    private InputAction cancel;

    // [          ]
    // [ MOVEMENT ]
    // [          ]
    // lerp movement starts
    private Vector3    startPos;
    private Quaternion startAngle;
    // lerp movement targets
    private Vector3    targetPos;
    private Quaternion targetAngle;

    // [                  ]
    // [ COLLISION LAYERS ]
    // [                  ]
    private LayerMask colLayer;
    private LayerMask trigLayer;
    private LayerMask modLayer;

    // [              ]
    // [ WORLD HEIGHT ]
    // [              ]
    [System.NonSerialized]
    public short worldHeight = 0;

    // [        ]
    // [ SKYBOX ]
    // [        ]
    // this probably sucks but sorry
    private GameObject skyBox;
    private Material skyBoxTex;

    // [    ]
    // [ UI ]
    // [    ]

    private GameObject UI;

    // [           ]
    // [ CONSTANTS ]
    // [           ]
    [System.NonSerialized]
    public const float GRID_SIZE = 4.0f;


    // [            ]
    // [ GAME STATE ]
    // [            ]

    // decided to just do this with other objects handling input. its cleaner.
    /* public enum gameState {
        explore,
        waiting,
        textbox,
        battle
    }
    public gameState playerState = gameState.explore;*/

    // [         ]
    // [ PREFABS ]
    // [         ]

    private GameObject P_TEXTBOX;
    

    void Awake() {
        colLayer  = LayerMask.GetMask("Collision");
        trigLayer = LayerMask.GetMask("Trigger");
        modLayer  = LayerMask.GetMask("Modifier");

        skyBox = gameObject.transform.Find("RENDERUNDER").GetChild(0).gameObject;
        skyBoxTex = skyBox.GetComponent<Renderer>().material;

        UI = gameObject.transform.Find("UI").gameObject;

        P_TEXTBOX = Resources.Load("Objects/Textbox") as GameObject;
    }
    
    void Start() {
        direction = InputSystem.actions.FindAction("Move");
        confirm   = InputSystem.actions.FindAction("Confirm");
        cancel    = InputSystem.actions.FindAction("Cancel");

        // ik this is only for emergencies but..... sorry im lazy :]
        EmergencySnap();

        // set start stuff so movement doesn't break
        startPos   = transform.position;
        startAngle = transform.rotation;
    }

    void Update() {

        Vector2 dir = direction.ReadValue<Vector2>();
        bool myConfirm = confirm.WasPressedThisFrame();
        bool myCancel  =  cancel.WasPressedThisFrame();

        // move if moving
        if (lerp > 0f) {
            lerp -= 0.05f; // progress the lerp

            // use lerp to lerp positions
            transform.position = Vector3.Lerp(targetPos, startPos, lerp);
            transform.rotation = Quaternion.Slerp(targetAngle, startAngle, lerp);

            // rotate skybox if rotating
            skyBoxTex.SetTextureOffset("_MainTex", new Vector2(transform.rotation.eulerAngles.y / 360, 0));

            if (lerp <= 0f) { // lerp finished so lets to some things
                FinishMovement();
            }
            return;
        }


        if (frozen)    {return;} // No input while events are happening

        // TODO: Remove this testing script
        if (myConfirm) {
            var txtbox = Instantiate(P_TEXTBOX, UI.transform.position + (UI.transform.forward * 5) + new Vector3(0, -1.2f, 0), UI.transform.rotation);
            txtbox.transform.SetParent(UI.transform);
            frozen = true;
            var txtScr = txtbox.GetComponent<Textbox>();
            txtScr.player = this;
            txtScr.text = "What a long string!\nI hope the player reads it.\nI wonder if it cuts off?\n...\n...Oh!\nHere's the rest!";
        }

        if (dir.y > 0.5f) { // [FORWARDS]
            
            bool moveCast = Physics.Raycast(transform.position, transform.forward, GRID_SIZE, colLayer);

            if (moveCast) {return;} // no movement if there's something in the way

            // set movement target
            targetPos = transform.position + (transform.forward * GRID_SIZE);
            targetAngle = startAngle;

            CheckModifier(transform.forward);

            lerp = 1.0f;
            return;
        }

        if (dir.x < -0.5f) { // [TURN LEFT]
            targetPos = startPos;
            // i dont know..... thats scary...................
            targetAngle = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y - 90, transform.rotation.eulerAngles.z);

            lerp = 1.0f;
            return;
        }

        if (dir.x > 0.5f) { // [TURN RIGHT]
            targetPos = startPos;
            targetAngle = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y + 90, transform.rotation.eulerAngles.z);

            lerp = 1.0f;
            return;
        }

        if (dir.y < -0.5f) { // [BACKWARDS ALTHOUGH PERSONALLY I PREFER WHEN IT'S A U TURN BUT PEOPLE DONT LIKE THAT APPARENTLY]
            
            bool moveCast = Physics.Raycast(transform.position, -transform.forward, GRID_SIZE, colLayer);

            if (moveCast) {return;} // no movement if there's something in the way

            targetPos = transform.position + (-transform.forward * GRID_SIZE);
            targetAngle = startAngle;

            CheckModifier(-transform.forward);

            lerp = 1.0f;
            return;
        }
    }



    public void EmergencySnap() {
        // failsafe for if the player gets in a weird spot and we just wanna
        // snap em' to the grid in a direction so the game gets as little broken as possible
        transform.position = new Vector3((Mathf.Round(transform.position.x/GRID_SIZE)*GRID_SIZE) + GRID_SIZE/2, transform.position.y, (Mathf.Round(transform.position.z/GRID_SIZE)*GRID_SIZE) + GRID_SIZE/2);
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, Mathf.Round(transform.eulerAngles.y / 90) * 90, transform.eulerAngles.z);
    }



    void CheckModifier(Vector3 checkDir) {
        // if there's movement modifiers check for that
        RaycastHit hit;
        bool modCast = Physics.Raycast(transform.position, checkDir, out hit, GRID_SIZE, modLayer);
        //Debug.Log("modCast: "+modCast.ToString());
        if (modCast) {
            // lets get the MovementModifier
            var hitOb = hit.collider.gameObject.GetComponent<MovementModifier>();
            // If on the same layer as us or on layer -1...
            if (hitOb.myHeight == worldHeight || hitOb.myHeight == -1) {
                // affect movement target
                targetPos += hitOb.moveMod;
                targetAngle = Quaternion.Euler(targetAngle.eulerAngles + hitOb.rotMod);
                if (hitOb.myHeight != -1 && hitOb.myHeight == worldHeight) {worldHeight = hitOb.newHeight;} // update world height if applicable
            }
        }
    }


    // on every movement...
    void FinishMovement() {
        // TODO: encounter stuff
        if (!(startPos == targetPos) && !forcedMove) {
            // Still, TODO: encounter stuff
        }

        // i noticed position is weird in the inspector idk i dont want it to cause issues so im just gonna
        transform.position = new Vector3(Mathf.Round(transform.position.x), transform.position.y, Mathf.Round(transform.position.z));
        // this is probably ass for performance but :[

        // reset this for movement
        startPos   = transform.position;
        startAngle = transform.rotation;

        // TODO: event stuff
    }
}
