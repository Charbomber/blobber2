using UnityEngine;
using System;
using System.Collections.Generic;

public class PlayerStats : MonoBehaviour {
    
    public short level;
    public short exp;
    public short statGrowth;
    // |0.5  |0.75   |1    |1.25 |1.5
    // |Fly  |Rabbit |Ox   |Lion |Salmon
    // divide stat growth by this in levelup times a thing based on level in 1-40 range

    [System.NonSerialized]
    public Dictionary<string, string> scriptFlag = new Dictionary<string, string>();

    void Start() {
        level = 1;
        exp = 0;

        statGrowth = 1;
    }

    //void Update() {
        
    //}
}
