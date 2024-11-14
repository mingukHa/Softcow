using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using ExitGames.Client.Photon;

public class S_2GameManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject playerPrefab = null; // �÷��̾� ������
    [SerializeField] private Vector3[] spawnPositions = new Vector3[3]; // �÷��̾� ���� ��ǥ

    private GameObject[] playerGoList = new GameObject[3];

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    private void Start()
    {
        // PhotonNetwork.PrefabPool�� DefaultPool���� Ȯ�� �� ������ ���
        DefaultPool pool = PhotonNetwork.PrefabPool as DefaultPool;
        if (pool != null && !pool.ResourceCache.ContainsKey("PlayerPf_B"))
        {
            pool.ResourceCache.Add("PlayerPf_B", playerPrefab);
        }
        else
        {
            Debug.LogError("Prefab Pool �����ϼ���");
        }
    }

    private void SpawnPlayer(int spawnIndex)
    {
        // ������ ��ġ�� ������ ����
        if (spawnIndex >= spawnPositions.Length)
        {
            Debug.LogError($"�߸��� spawnIndex: {spawnIndex}");
            return;
        }

        Vector3 spawnPosition = spawnPositions[spawnIndex];
        GameObject go = PhotonNetwork.Instantiate("PlayerPf_B", spawnPosition, Quaternion.identity, 0);
        Debug.Log($"Player instantiated at: {spawnPosition}");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("�뿡 �����߽��ϴ�.");
        int playerIndex = PhotonNetwork.LocalPlayer.ActorNumber - 1; // ActorNumber�� 1���� ����
        SpawnPlayer(playerIndex); // �÷��̾ ActorNumber ������� ����

        AssignPlayerNumber(playerIndex + 1);
    }

    private void AssignPlayerNumber(int playerNumber)
    {
        Debug.Log($"My Player Number: {playerNumber}");
        PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable
        {
            { "PlayerNumber", playerNumber }
        });
    }

    public override void OnPlayerEnteredRoom(Player otherPlayer)
    {
        Debug.LogFormat($"Player Entered Room: {otherPlayer.NickName}");
        int playerIndex = otherPlayer.ActorNumber - 1; // ActorNumber�� 1���� ����
        ApplyPlayerList();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log($"{otherPlayer.NickName} has left the room.");
        ApplyPlayerList(); // �÷��̾� ��� ����
    }

    [PunRPC]
    public void ApplyPlayerList()
    {
        PhotonView[] photonViews = FindObjectsByType<PhotonView>(FindObjectsSortMode.None);
        System.Array.Clear(playerGoList, 0, playerGoList.Length);

        foreach (var player in PhotonNetwork.CurrentRoom.Players.Values)
        {
            int playerIndex = player.ActorNumber - 1;
            foreach (var view in photonViews)
            {
                if (!view.isRuntimeInstantiated) continue;
                if (view.Owner.ActorNumber == player.ActorNumber)
                {
                    playerGoList[playerIndex] = view.gameObject;
                    playerGoList[playerIndex].name = $"Player_{view.Owner.NickName}";
                }
            }
        }
    }
}

