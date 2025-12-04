using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "LevelsConfig", menuName = "Game/Levels Config")]
public class LevelsConfig : ScriptableObject
{
    public List<Level> levels;
    
    [System.Serializable]
    public class Level
    {
        public List<ParkingLevelGenerator.CarPlacement> values = new();
    }
}
