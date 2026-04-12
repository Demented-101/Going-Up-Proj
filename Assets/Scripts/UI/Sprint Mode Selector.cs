using TMPro;
using UnityEngine;
using static Utils;

public class SprintModeSelector : MonoBehaviour
{
    [SerializeField] private GameStatus gameStatus;
    [SerializeField] private SprintRefProvider[] options;
    [SerializeField] private MoveStateSprint sprintState;
    private int currentSelectionIndex;

    [SerializeField] private TMP_Text title;
    [SerializeField] private TMP_Text desc;

    private void Start()
    {
        gameStatus.Updated += UpdateSprintMode;
        UpdateSprintMode();
    }

    public void Change()
    {
        currentSelectionIndex++;
        if (currentSelectionIndex >= options.Length)
        {
            currentSelectionIndex = 0;
        }

        gameStatus.sprintModeIndex = currentSelectionIndex;
        UpdateSprintMode();
    }

    private void UpdateSprintMode()
    {
        if (gameStatus.sprintModeIndex != currentSelectionIndex) { currentSelectionIndex = gameStatus.sprintModeIndex; }

        SprintRefProvider reference = options[currentSelectionIndex];
        sprintState.sprintRef = reference;

        // update UI text
        title.text = reference.displayName;
        desc.text = "\"" + reference.description + "\"";
    }
}
