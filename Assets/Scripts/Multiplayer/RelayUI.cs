using UnityEngine;

public class RelayUI : MonoBehaviour
{
    public GameObject relayManagerObj;

    private RelayManager relayManager;
    private string code;

    private void Start()
    {
        relayManager = relayManagerObj.GetComponent<RelayManager>();
        if (!relayManager) { throw new System.Exception("Relay manager obj does not have the correct component!"); }
    }

    public async void HostLobby()
    {
        await relayManager.CreateLobby();
    }

    public async void JoinLobby()
    {
        await relayManager.JoinLobbyByCode(code, "JOINED PLAYER");
    }

    public void SetLobbyCode(string newCode)
    {
        code = newCode;
    }
}
