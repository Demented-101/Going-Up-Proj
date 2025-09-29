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


    public void LoadFromSave()
    {
        SaveData data = SaveManager.Load();
        CopyFromSaveData(data);
    }

    public void LoadFromGeneric()
    {
        SaveData data = new SaveData();
        data.loadBasics();

        CopyFromSaveData(data);
    }

    private void CopyFromSaveData(SaveData saveData)
    {
        totScore = saveData.totScore; // dont need to save current score
        highScore = saveData.highScore;
        currentFloor = saveData.currentFloor;
        currentBuilding = saveData.currentBuilding;
        isOnRoof = saveData.isOnRoof;
        runCount = saveData.runCount;
    }

    public void Reset()
    {
        isPaused = false;
        onPauseChanged?.Invoke();

        gameState = Utils.GameStates.Pregame;
        onStateChange?.Invoke(gameState);

        LoadFromGeneric();
    }

    public void AddScore(int addScore)
    {
        currentScore += addScore;

        Updated.Invoke();
    }

    public void RunEnded()
    {
        // calculate new floor and building
        // Floors per building (x) = 90 + x*10
        // Score per floor = 1000

        runCount++;

        if (currentScore < 1000) // not enough points gained - hasnt beaten floor
        {
            gameState = Utils.GameStates.GameOver;
            onStateChange?.Invoke(gameState);
            Debug.Log("Game over! State changed - Game Over");

            return;
        }

        if (isOnRoof) // has beaten roof level -> moved to next building
        {
            isOnRoof = false;
            currentFloor = 1;
            currentBuilding++;
        }
        else // is still mid-building
        {
            currentFloor += currentScore / 1000;
            int maxFloor = 90 + (currentBuilding * 10);

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
