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
        gameStatus.Updated += UpdateText;

        prefixTextMesh = prefixText.GetComponent<TMP_Text>();
        valueTextMesh = valueText.GetComponent<TMP_Text>();
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
        if (showTotScore)       { AddText("Score", gameStatus.totScore.ToString()); }
        if (showHighScore)      { AddText("Score", gameStatus.highScore.ToString()); }
        if (showFloor)          { AddText("Score", gameStatus.currentFloor.ToString()); }
        if (showBuilding)       { AddText("Score", gameStatus.currentBuilding.ToString()); }
        if (showRunCount)       { AddText("Score", gameStatus.runCount.ToString()); }
    }

    private void AddText(string prefix, string value)
    {
        prefixTextMesh.text += prefix + "\n";
        valueTextMesh.text += value + "\n";
    }
}
