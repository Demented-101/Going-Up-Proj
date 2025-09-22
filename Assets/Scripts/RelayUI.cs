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
        await relayManager.CreateRelay();
    }

    public async void JoinLobby()
    {
        await relayManager.JoinRelay(code);
    }

    public void SetLobbyCode(string newCode)
    {
        code = newCode;
    }
}
