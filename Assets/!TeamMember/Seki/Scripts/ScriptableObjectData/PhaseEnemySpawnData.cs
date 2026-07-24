using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eSpawnPhase {
    Invalid = -1,
    Phase1,
    Phase2,
    Phase3,

    Max
};
public class PhaseEnemySpawnDataSO : ScriptableObject {
    [System.Serializable]
    public struct PhaseEnemySpawnData {
        public eSpawnPhase phase;
        public List<EnemyBase> enemyList;
    }

    public List<PhaseEnemySpawnData> enemySpawnList;


    
}
