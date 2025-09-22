using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

public class TestRelay : MonoBehaviour
{
    private string joinCode;
    private bool codeIsValid;

    private async Task Authenticate()
    {
        // initialize unity services
        await UnityServices.InitializeAsync();

        // if not already signed in
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            // log in anonymously - easiest method.
            AuthenticationService.Instance.SignedIn += () =>{ Debug.Log("Signed In! Player Id = " + AuthenticationService.Instance.PlayerId); };
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
    }

    public async Task<string> CreateRelay()
    {
        // log in and authenticate
        await Authenticate();
        string newJoinCode;

        try {
            // create relay instance with correct amount of open connections
            Debug.Log("Creating relay instance.");
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(Utils.maxLobbySize -1);

            // get join code of the newly created lobby
            newJoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            Debug.Log("Instance created. Join code: " + joinCode);

            // configure transport connection
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(
                allocation.RelayServer.IpV4,
                (ushort)allocation.RelayServer.Port,
                allocation.AllocationIdBytes,
                allocation.Key,
                allocation.ConnectionData
            );
        }
        catch (RelayServiceException e) // catch any relay service errors.
        {
            newJoinCode = null;
            Debug.Log(e.ToString());
        }

        // start host, and return the code if this succeeds
        return NetworkManager.Singleton.StartHost() ? newJoinCode : null;
    }

    public async Task<bool> JoinRelay()
    {
        // check provided code is valid
        if (!codeIsValid) { return false; }

        // log in and authenticate
        await Authenticate();

        try {
            Debug.Log("Joining relay with code " + joinCode);
            JoinAllocation allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

            // configure transport connection
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(
                allocation.RelayServer.IpV4,
                (ushort)allocation.RelayServer.Port,
                allocation.AllocationIdBytes,
                allocation.Key,
                allocation.ConnectionData
            );
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e.ToString());
        }

        // start client and return result
        return !string.IsNullOrEmpty(joinCode) && NetworkManager.Singleton.StartClient();
    }

    public void SetCode(string code)
    {
        joinCode = code;

        codeIsValid = code.Length == 4;
    }
}
