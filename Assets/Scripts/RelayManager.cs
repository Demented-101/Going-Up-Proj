using System.Threading.Tasks;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using System.Threading;

public class RelayManager : MonoBehaviour
{
    private Lobby myLobby = null;

    private float heartbeatTimer = 0;
    private const float maxHeartbeatTimer = 0.5f;

    private float updateTimer = 0;
    private const float maxUpdateTimer = 1.1f;

    private void OnEnable()
    {
        DontDestroyOnLoad(gameObject);
    }

    private async void Update()
    {
        // heartbeat current lobby
        if (myLobby != null)
        {
            // heartbeat - keep the lobby open at all times
            heartbeatTimer -= Time.deltaTime;
            if (heartbeatTimer <= 0 && myLobby.HostId == AuthenticationService.Instance.PlayerId) // only heartbeat if local is host
            {
                heartbeatTimer = maxHeartbeatTimer;
                await LobbyService.Instance.SendHeartbeatPingAsync(myLobby.Name);
            }

            // update lobby data
            updateTimer -= Time.deltaTime;
            if (updateTimer <= 0)
            {
                updateTimer = maxUpdateTimer;
                string lobbyID = myLobby.Id;
                myLobby = await LobbyService.Instance.GetLobbyAsync(lobbyID);
            }
        }
    }

    private async Task Authenticate()
    {
        // if not already signed in
        if ((AuthenticationService.Instance != null) && AuthenticationService.Instance.IsSignedIn) { return; }

        // initialize unity services
        await UnityServices.InitializeAsync();

        // log in anonymously - easiest method.
        AuthenticationService.Instance.SignedIn += () => { Debug.Log("Signed In! Player Id = " + AuthenticationService.Instance.PlayerId); };
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    private async Task<string> CreateRelay()
    {
        // log in and authenticate
        await Authenticate();

        try
        {
            // create relay instance with correct amount of open connections
            Debug.Log("Creating relay instance.");
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(Utils.maxLobbySize - 1);

            // get join code of the newly created lobby
            string newJoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            Debug.Log("Instance created. Join code: " + newJoinCode);

            // configure transport connection
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(
                allocation.RelayServer.IpV4,
                (ushort)allocation.RelayServer.Port,
                allocation.AllocationIdBytes,
                allocation.Key,
                allocation.ConnectionData
            );

            // start host, and return the code if this succeeds
            return NetworkManager.Singleton.StartHost() ? newJoinCode : null;
        }
        catch (RelayServiceException e) // catch any relay service errors.
        {
            Debug.Log(e.ToString());
        }

        return null;
    }

    private async Task<bool> JoinRelay(string joinCode)
    {
        // log in and authenticate
        await Authenticate();

        try
        {
            Debug.Log("Joining relay with code " + joinCode);
            JoinAllocation allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

            // configure transport connection
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(
                allocation.RelayServer.IpV4,
                (ushort)allocation.RelayServer.Port,
                allocation.AllocationIdBytes,
                allocation.Key,
                allocation.ConnectionData,
                allocation.HostConnectionData
            );
            
            // start client and return result
            return !string.IsNullOrEmpty(joinCode) && NetworkManager.Singleton.StartClient();
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e.ToString());
        }

        return false;
    }

    public async Task<Lobby> CreateLobby(string playerName = "LobbyHost")
    {
        // connect to services
        await Authenticate();

        try
        {
            // create relay
            string relayCode = await CreateRelay();
            if (string.IsNullOrEmpty(relayCode)) { return null; }

            // set up lobby settings
            CreateLobbyOptions lobbyOpts = new CreateLobbyOptions
            {
                IsPrivate = true, // aviod some potential abuse (hackers cannot just use quick join methods.)
                IsLocked = false,
                Player = new Player { Data = SetupPlayerDataObject(playerName) }, 
                Data = new Dictionary<string, DataObject> { // add relay code to lobby info to allow players to connect using relay
                    { "RelayCode", new DataObject(DataObject.VisibilityOptions.Member, relayCode)}
                }
            };

            // create lobby using the set name and size
            string lobbyName = playerName + "'s lobby"; // lobby name is (host player)'s lobby to simplify UI
            myLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, Utils.maxLobbySize, lobbyOpts);

            Debug.Log("Created lobby - " + myLobby.Name);
            return myLobby;
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }

        return null;
    }

    public async Task<bool> JoinLobbyByCode(string lobbyCode, string playerName)
    {
        // connect to services
        await Authenticate();

        try {
            JoinLobbyByCodeOptions joinLobbyOpts = new JoinLobbyByCodeOptions
            {
                Player = new Player { Data = SetupPlayerDataObject(playerName) }
            };

            // join lobby using ID
            Debug.Log("Joining Lobby by code: " + lobbyCode);
            myLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode);

            await JoinRelay(myLobby.Data["RelayCode"].Value);

            return myLobby != null;

        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
        return false;
    }

    public async void DebugLogAllAvailableLobbies()
    {
        QueryResponse qResult = await LobbyService.Instance.QueryLobbiesAsync();
        Debug.Log(qResult.Results.ToString());
    }

    private Dictionary<string, PlayerDataObject> SetupPlayerDataObject(string playerName)
    {
        return new Dictionary<string, PlayerDataObject> {
            { "PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, playerName) },
        };
    }
}
