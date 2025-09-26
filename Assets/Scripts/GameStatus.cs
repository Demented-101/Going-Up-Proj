using System;
using UnityEngine;

[CreateAssetMenu(fileName = "GameStatus", menuName = "Scriptable Objects/GameStatus")]
public class GameStatus : ScriptableObject
{
    public Utils.GameStates gameState;
    public Action<Utils.GameStates> onStateChange;

    public int currentScore;
    public int totScore;

    public int currentFloor;
    public int currentBuilding;
}
