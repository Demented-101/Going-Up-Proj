[System.Serializable]

// this handles the data in the save file, i.e. formatting it or giving it to the variables that need it, IE PlayerData
// loading or saving the data itself is done seperately by the SaveManager
public class SaveData
{
    public int currentScore = 0;
    public int totScore = 0;
    public int highScore = 0;
    public int currentFloor = 1;
    public int currentBuilding = 1;
    public bool isOnRoof = false;
    public int runCount = 0;

    public void loadFromGameStatus(GameStatus status)
    {
        currentScore = status.currentScore;
        totScore = status.totScore;
        highScore = status.highScore;
        currentFloor = status.currentFloor;
        currentBuilding = status.currentBuilding;
        isOnRoof= status.isOnRoof;
        runCount = status.runCount;
    }

    public void loadBasics()
    {
        currentScore = 0;
        totScore = 0;
        highScore = 0;
        currentFloor = 1;
        currentBuilding = 1;
        isOnRoof = false;
        runCount = 0;
    }
}
