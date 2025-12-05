using UnityEngine;

public class HUDScript : MonoBehaviour
{
    [SerializeField] private GameStatus status;
    [SerializeField] private TMPro.TMP_Text text;

    void Update()
    {
        string newText = "Floor " + status.currentFloor.ToString();
        newText += "\nScore: " + status.currentScore.ToString();
        text.text = newText;
    }
}
