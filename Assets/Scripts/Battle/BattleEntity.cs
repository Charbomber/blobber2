using UnityEngine;
//using UnityEditor;

public class BattleEntity : MonoBehaviour {
    //public BattleGlobal btl;
    
    short health;
    short maxHealth;
    short food;
    short maxFood;
    byte[] stats;
    BattleGlobal.status[] effects;
    BattleGlobal.affinity[] elem;
    BattleGlobal.emotion emotion;

    short expWorth;

    Skill[] skillList = new Skill[]{};

    void Start() {

        // Init Bar Stats
        maxHealth = 10;
        health = maxHealth;
        maxFood = 10;
        food = maxFood;
        
        // Init Stats
        /*stats[(short)BattleGlobal.stat.str] = 5;
        stats[(short)BattleGlobal.stat.dex] = 5;
        stats[(short)BattleGlobal.stat.mnd] = 5;
        stats[(short)BattleGlobal.stat.inf] = 5;
        stats[(short)BattleGlobal.stat.per] = 5;

        // Init Elements
        elem[(short)BattleGlobal.element.physical] = BattleGlobal.affinity.neutral;
        elem[(short)BattleGlobal.element.fire]     = BattleGlobal.affinity.neutral;
        elem[(short)BattleGlobal.element.ice]      = BattleGlobal.affinity.neutral;
        elem[(short)BattleGlobal.element.electric] = BattleGlobal.affinity.neutral;
        elem[(short)BattleGlobal.element.light]    = BattleGlobal.affinity.neutral;
        elem[(short)BattleGlobal.element.dark]     = BattleGlobal.affinity.neutral;
        elem[(short)BattleGlobal.element.nuclear]  = BattleGlobal.affinity.neutral;*/
    }

    //void Update() {
        
    //}
}
