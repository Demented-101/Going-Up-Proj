using System;
using UnityEngine;

[CreateAssetMenu(fileName = "GameStatus", menuName = "Scriptable Objects/GameStatus")]
public class GameStatus : ScriptableObject
{
    public Action Updated;
    public Action onPauseChanged;
    public Action<Utils.GameStates> onStateChange;

    public bool isPaused = false;
    public Utils.GameStates gameState;

    public int currentScore { get; private set; } = 0;
    public int totScore { get; private set; } = 0;
    public int highScore { get; private set; } = 0;

    public int currentFloor { get; private set; } = 1;
    public int currentBuilding { get; private set; } = 1;
    public bool isOnRoof { get; private set; } = false;
    public int runCount { get; private set; } = 0;


    public void LoadFromSave(bool useGenerics = false) 
    {
        // load values from the save file
        SaveData data = null;

        if (useGenerics)
        {
            data = new SaveData();
            data.LoadGeneric();
        }
        else
        {
            data = SaveManager.Load();
        }

        CopyFromSaveData(data);
    }

    private void CopyFromSaveData(SaveData saveData)
    {
        totScore = saveData.totScore;
        highScore = saveData.highScore;
        currentFloor = saveData.currentFloor;
        currentBuilding = saveData.currentBuilding;
        isOnRoof = saveData.isOnRoof;
        runCount = saveData.runCount;

        Updated.Invoke();
    }

    public void Reset()
    {
        // sets up all the values at the game's load.
        isPaused = false;
        onPauseChanged?.Invoke();

        gameState = Utils.GameStates.Pregame;
        onStateChange?.Invoke(gameState);

        bool useGenerics = false;
        LoadFromSave(useGenerics);
    }

    public void AddScore(int addScore)
    {
        currentScore += addScore;

        Updated.Invoke();
    }

    private int GetPassedFloors(int score, int startFloor)
    {
        int passedFloors = 0;
        int remainingScore = score;
        int nextFloorReq = Utils.GetFloorPointRequirement(currentFloor);

        while (remainingScore > nextFloorReq)
        {
            passedFloors++;
            remainingScore -= nextFloorReq;

            nextFloorReq = Utils.GetFloorPointRequirement(startFloor + passedFloors);
        }

        return passedFloors;
    }

    public void RunEnded() // calculate new floor, building, and score values
    {
        runCount++;
        int passedFloors = GetPassedFloors(currentScore, currentFloor);

        if (isOnRoof) // has passed roof level -> moved to next building
        {
            isOnRoof = false;
            currentFloor = 1;
            currentBuilding++;
        }
        else if (passedFloors < 1) // not enough points gained - hasnt beaten floor
        {
            gameState = Utils.GameStates.GameOver;
            onStateChange?.Invoke(gameState);
            Debug.Log("GAME OVER - ENDING GAME LOOP");
        }
        else // is still mid-building
        {
            currentFloor += passedFloors;

            int maxFloor = Utils.GetBuildingFloorCount(currentBuilding);
            if (currentFloor > maxFloor) // has reached roof
            {
                currentFloor = maxFloor;
                isOnRoof = true;
            }
        }

        // calculate scores
        if (currentScore > highScore) { highScore = currentScore; }
        totScore += currentScore;
        currentScore = 0;

        Updated.Invoke();
    }

}
