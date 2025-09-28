using TMPro;
using UnityEngine;

public class GameStatusDataLayout : MonoBehaviour
{
    [SerializeField] private GameStatus gameStatus;
    [SerializeField] private GameObject prefixText;
    [SerializeField] private GameObject valueText;
    private TMP_Text prefixTextMesh;
    private TMP_Text valueTextMesh;

    public bool showCurrentScore;
    public bool showTotScore;
    public bool showHighScore;
    public bool showFloor;
    public bool showBuilding;
    public bool showRunCount;

    private void Start()
    {
        prefixTextMesh = prefixText.GetComponent<TMP_Text>();
        valueTextMesh = valueText.GetComponent<TMP_Text>();

        gameStatus.Updated += UpdateText;
        UpdateText();
    }

    private void UpdateText()
    {
        if (prefixTextMesh == null || valueTextMesh == null)
        {
            Debug.LogError("Value or Prefix label not setup!");
            return;
        }

        prefixTextMesh.text = "";
        valueTextMesh.text = "";

        // add the correct scores when needed.
        if (showCurrentScore)   { AddText("Score", gameStatus.currentScore.ToString()); }
        if (showTotScore)       { AddText("Total Score", gameStatus.totScore.ToString()); }
        if (showHighScore)      { AddText("High Score", gameStatus.highScore.ToString()); }
        if (showFloor)          { AddText("Floor No", gameStatus.currentFloor.ToString()); }
        if (showBuilding)       { AddText("Building", gameStatus.currentBuilding.ToString()); }
        if (showRunCount)       { AddText("Run Count", gameStatus.runCount.ToString()); }
    }

    private void AddText(string prefix, string value)
    {
        prefixTextMesh.text += prefix + "\n";
        valueTextMesh.text += value + "\n";
    }
}
