using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

public class RelayManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField codeInput;

    private string CONNECTION_TYPE = "wss";
    async void Start()
    {
        await UnityServices.InitializeAsync();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    public async void StartHost()
    {
        string joinCode = await StartHostWithRelay();
        codeInput.text = joinCode;
        Debug.Log(joinCode);
    }

    public async void StarClient()
    {
        Debug.Log(codeInput.text);
        await StartClientWithRelay(codeInput.text);
    }
    
    private async Task<string> StartHostWithRelay(int maxConnections = 4)
    {
        // Create an allocation with the specified number of connections
        Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxConnections);

        // RelayServerData relayServerData = new RelayServerData(
        //     allocation.RelayServer.IpV4,
        //     (ushort)allocation.RelayServer.Port,
        //     allocation.AllocationIdBytes,
        //     allocation.ConnectionData,
        //     allocation.ConnectionData,
        //     allocation.Key,
        //     true
        // );
        //
        // NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(allocation, CONNECTION_TYPE));
        string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

        return NetworkManager.Singleton.StartHost() ? joinCode : null;
    }

    private async Task<bool> StartClientWithRelay(string joinCode)
    {
        JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
        // RelayServerData relayServerData = new RelayServerData(
        //     joinAllocation.RelayServer.IpV4,
        //     (ushort)joinAllocation.RelayServer.Port,
        //     joinAllocation.AllocationIdBytes,
        //     joinAllocation.ConnectionData,
        //     joinAllocation.ConnectionData,
        //     joinAllocation.Key,
        //     true
        // );
        // NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, CONNECTION_TYPE));

        return !string.IsNullOrEmpty(joinCode) && NetworkManager.Singleton.StartClient();
    }
}