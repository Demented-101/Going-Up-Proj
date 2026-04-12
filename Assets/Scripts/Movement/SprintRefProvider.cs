using UnityEngine;

[CreateAssetMenu(fileName = "SprintRefProvider", menuName = "Scriptable Objects/SprintRefProvider")]
public class SprintRefProvider : ScriptableObject
{
    // shows in elevator UI
    public string displayName;
    public string description;

    // provide multiple move state refs that the sprint state can pick from
    public MovementStateReference Mach2;
    public MovementStateReference Mach3;
    public MovementStateReference Mach4;
    public MovementStateReference Turning;
    public MovementStateReference Dash;
}
