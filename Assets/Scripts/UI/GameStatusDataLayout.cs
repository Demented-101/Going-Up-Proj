using TMPro;
using UnityEngine;

public class GameStatusDataLayout : MonoBehaviour
{
    [SerializeField] private GameStatus gameStatus;
    private TMP_Text textMesh;
    
    void Update()
    {
        if (textMesh == null) { textMesh = GetComponent<TMP_Text>(); }
        if (textMesh == null) { // return if none is found
            Debug.Log("No TMP found");
            return;
        } 

        textMesh.text = $"{gameStatus.currentScore}\n{gameStatus.totScore}\n\n{gameStatus.currentFloor}\n{gameStatus.currentBuilding}";
    }
}
