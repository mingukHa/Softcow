using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using ExitGames.Client.Photon;

public class S_2GameManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject playerPrefab = null; // 플레이어 프리팹
    [SerializeField] private Vector3[] spawnPositions = new Vector3[3]; // 플레이어 생성 좌표

    private GameObject[] playerGoList = new GameObject[3];

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    private void Start()
    {
        // PhotonNetwork.PrefabPool이 DefaultPool인지 확인 후 프리팹 등록
        DefaultPool pool = PhotonNetwork.PrefabPool as DefaultPool;
        if (pool != null && !pool.ResourceCache.ContainsKey("PlayerPf_B"))
        {
            pool.ResourceCache.Add("PlayerPf_B", playerPrefab);
        }
        else
        {
            Debug.LogError("Prefab Pool 설정하세요");
        }
    }

    private void SpawnPlayer(int spawnIndex)
    {
        // 지정된 위치에 프리팹 생성
        if (spawnIndex >= spawnPositions.Length)
        {
            Debug.LogError($"잘못된 spawnIndex: {spawnIndex}");
            return;
        }

        Vector3 spawnPosition = spawnPositions[spawnIndex];
        GameObject go = PhotonNetwork.Instantiate("PlayerPf_B", spawnPosition, Quaternion.identity, 0);
        Debug.Log($"Player instantiated at: {spawnPosition}");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("룸에 입장했습니다.");
        int playerIndex = PhotonNetwork.LocalPlayer.ActorNumber - 1; // ActorNumber는 1부터 시작
        SpawnPlayer(playerIndex); // 플레이어를 ActorNumber 기반으로 스폰

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
        int playerIndex = otherPlayer.ActorNumber - 1; // ActorNumber는 1부터 시작
        ApplyPlayerList();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log($"{otherPlayer.NickName} has left the room.");
        ApplyPlayerList(); // 플레이어 목록 갱신
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

