using System;
using Unity.Netcode;
using UnityEngine;

public class ColorNetworkManager : MonoBehaviour
{
    void Start(){
        NetworkManager.Singleton.NetworkConfig.ConnectionApproval = true;
        NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;
        // NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnectCallback;
        // NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedCallback;
    }


    void OnGUI()
        {
            GUILayout.BeginArea(new Rect(10, 10, 300, 300));
            if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
            {
                StartButtons();
            }
            else
            {
                StatusLabels();

                SubmitNewPosition();
            }

            GUILayout.EndArea();
        }

        static void StartButtons()
        {
            if (GUILayout.Button("Host")) {
                NetworkManager.Singleton.StartHost(); 
                }
            if (GUILayout.Button("Client")) {
                NetworkManager.Singleton.StartClient();
            }
            if (GUILayout.Button("Server")) NetworkManager.Singleton.StartServer();
        }

        static void StatusLabels()
        {
            var mode = NetworkManager.Singleton.IsHost ?
                "Host" : NetworkManager.Singleton.IsServer ? "Server" : "Client";

            GUILayout.Label("Transport: " +
                NetworkManager.Singleton.NetworkConfig.NetworkTransport.GetType().Name);
            GUILayout.Label("Mode: " + mode);
            GUILayout.Label("Players: " + NetworkManager.Singleton.ConnectedClientsIds.Count + " / 6");
        }
    

        static void SubmitNewPosition()
        {
            if (GUILayout.Button(NetworkManager.Singleton.IsServer ? "Change Color" : "Request Color Change"))
            {
                Debug.Log("Change Color pressed");
                if (NetworkManager.Singleton.IsServer && !NetworkManager.Singleton.IsClient )
                {
                    foreach (ulong uid in NetworkManager.Singleton.ConnectedClientsIds){
                        NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(uid).GetComponent<NetworkPlayer>().SetRandomColor();
                    }
                }
                else
                {
                    var playerObject = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject();
                    var player = playerObject.GetComponent<NetworkPlayer>();
                    player.SetRandomColor();

                }
            }

        }

    void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response){
        if(NetworkManager.Singleton.ConnectedClientsIds.Count < 6){
            response.Approved = true;
        }
        else{
            response.Approved = false;
        }
        response.CreatePlayerObject = true;

    }

    // void OnClientConnectedCallback(ulong clientId)
    // {
    //     var instance = Instantiate(myPrefab);
    //     var instanceNetworkObject = instance.GetComponent<NetworkObject>();
    //     instanceNetworkObject.Spawn();
    // }

    // void OnClientDisconnectCallback(ulong clientId)
    // {
        

    // }

}

