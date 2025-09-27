using UnityEngine;

public class VisibilityGameState : GameState
{
    [SerializeField] bool showOnState = true;
    public override void OnStateChanged(Utils.GameStates newState)
    {
        base.OnStateChanged(newState);
        gameObject.SetActive(IsActive == showOnState);
    }
}
