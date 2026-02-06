using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveState
{

    public bool isFirstBossDefeated;
    #region LevelSettings
    public int LevelCounter;
    public int LastLevelIndex;
    #endregion
  
    public void SetInitialValues()
    {

        isFirstBossDefeated = false;
        #region LevelSettings
        LastLevelIndex = 1;
        LevelCounter = 1;
        #endregion


    }
}
