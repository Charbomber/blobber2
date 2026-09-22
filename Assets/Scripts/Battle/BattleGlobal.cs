using UnityEngine;

public class BattleGlobal : ScriptableObject {

    public enum stat : byte {
        str, // Strength
        dex, // Dexterity
        mnd, // Mind
        inf, // Influence
        per, // Perception

        nil = 0xFF
    }
    public enum element : byte {
        physical,
        fire,
        ice,
        electric,
        light,
        dark,
        nuclear
    }
    public enum affinity : byte {
        weak,
        neutral,
        strong,
        block,
        reflect,
        absorb
    }
    public enum status : byte {
        death,
        blind,
        poison,
        xsealed // not having the x causes errors. seriously.
    }
    public enum emotion : byte {
        neutral,
        mad,
        happy,
        anguish,
        interested,
        scared,
        calm
    }

    public enum targetType : byte {
        self,
        ally,
        enemy,
        allyParty,
        enemyParty
    }

    /*void Start() {
        
    }

    void Update() {
        
    }*/
}

// [                 ]
// [ GENERIC CLASSES ]
// [                 ]

// class for skills
public class Skill {
    short level;
    short priority;

    string name;
    string description;

    BattleGlobal.targetType target;


    // Constructor
    public Skill (string myName, string myDescription, short myLevel, short myPriority, BattleGlobal.targetType myTarget) {
        name = myName;
        description = myDescription;
        level = myLevel;
        priority = myPriority;
        target = myTarget;
    }


    
    void onbattleUse() {
        // TODO: report 'nothing happened'
    }
    void onWorldUse() {
        // TODO: report 'nothing happened'
    }
}


// [        ]
// [ SKILLS ]
// [        ]

public class GenericAttack : Skill {
    BattleGlobal.element atkElement;
    BattleGlobal.stat atkStat = BattleGlobal.stat.str;
    BattleGlobal.stat defStat = BattleGlobal.stat.str;

    public GenericAttack() : base("Generic Attack", "A basic attack.", -1, 0, BattleGlobal.targetType.enemy) {}
}

public class GenericStatusMove : Skill {
    BattleGlobal.element atkElement;
    BattleGlobal.status atkStatus;
    BattleGlobal.stat atkStat = BattleGlobal.stat.nil;
    BattleGlobal.stat defStat = BattleGlobal.stat.mnd;

    public GenericStatusMove() : base("Generic Status Move", "A basic attack.", -1, 0, BattleGlobal.targetType.enemy) {}
}