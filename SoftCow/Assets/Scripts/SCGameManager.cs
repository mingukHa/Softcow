using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using ExitGames.Client.Photon;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using Photon.Pun.UtilityScripts;

public class SCGameManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private Button Scissors = null;
    [SerializeField] private Button Rock = null;
    [SerializeField] private Button Paper = null;

    [SerializeField] private float CountDownTime = 5;
    [SerializeField] private TextMeshProUGUI CountDownDisplay = null;

    //NPC의 값
    private int AIvalue = 0;
    //player의 값
    private int Player1ChoiceValue = 0;
    private int Player2ChoiceValue = 0;
    private int Player3ChoiceValue = 0;

    [SerializeField] private GameObject playerPrefab = null;
    //[SerializeField] private Vector3[] spawnPositions = new Vector3[3];

    private GameObject[] playerGoList = new GameObject[3];


    private void Awake()
    {
        Scissors.enabled = false;
        Rock.enabled = false;
        Paper.enabled = false;
    }

    private void Start()
    {
        if (playerPrefab != null)
        {
            //
            DefaultPool pool = PhotonNetwork.PrefabPool as DefaultPool;
            if (!pool.ResourceCache.ContainsKey(playerPrefab.name))
                pool.ResourceCache.Add(playerPrefab.name, playerPrefab);

        }
        if (photonView.Owner.IsMasterClient)
        {
            AIvalue = AIchoice();
        }

        Scissors.onClick.AddListener(PlayerChoiceScissor);
        Rock.onClick.AddListener(PlayerChoiceRock);
        Paper.onClick.AddListener(PlayerChoicePaper);
        //판정
        int result = SCJudgment(AIvalue, Player1ChoiceValue);

        PlayerPrefs.SetInt("AIvalue", AIvalue);
        PlayerPrefs.SetInt("Player1ChoiceValue", Player1ChoiceValue);
        PlayerPrefs.SetInt("Player12ChoiceValue", Player2ChoiceValue);
        
        if(CountDownTime == 0)
        {
            photonView.RPC("SetAIResult", RpcTarget.Others, AIvalue);
        }
    }

    private void Update()
    {
        if (playerGoList.Length == 2)
        {
            Scissors.enabled = true;
            Rock.enabled = true;
            Paper.enabled = true;
            CountDown();
        }
    }

    private void SpawnPlayer(int spawnIndex)
    {
       // GameObject go = 만들어야 함

        photonView.RPC("ApplyPlayerList", RpcTarget.All);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("룸에 입장했습니다.");

    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player otherPlayer)
    {
        Debug.LogFormat("Player Entered Room: {0}",
                        otherPlayer.NickName);


    }



    [PunRPC]
    public void ApplyPlayerList()
    {

        Debug.LogError("CurrentRoom PlayerCount : " + PhotonNetwork.CurrentRoom.PlayerCount);

        PhotonView[] photonViews =
            FindObjectsByType<PhotonView>(FindObjectsSortMode.None);


        System.Array.Clear(playerGoList, 0, playerGoList.Length);

        for (int i = 0; i < PhotonNetwork.CurrentRoom.PlayerCount; ++i)
        {

            int key = i + 1;
            for (int j = 0; j < photonViews.Length; ++j)
            {

                if (photonViews[j].isRuntimeInstantiated == false) continue;

                if (PhotonNetwork.CurrentRoom.Players.ContainsKey(key) == false) continue;


                int viewNum = photonViews[j].Owner.ActorNumber;

                int playerNum = PhotonNetwork.CurrentRoom.Players[key].ActorNumber;

                if (viewNum == playerNum)
                {

                    playerGoList[playerNum - 1] = photonViews[j].gameObject;

                    playerGoList[playerNum - 1].name = "Player_" + photonViews[j].Owner.NickName;
                }
            }
        }

        PrintPlayerList();
    }

    private void PrintPlayerList()
    {
        foreach (GameObject go in playerGoList)
        {
            if (go != null)
            {
                Debug.LogError(go.name);
            }
        }
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        Debug.LogFormat("Player Left Room: {0}",
                        otherPlayer.NickName);
    }

    public void LeaveRoom()
    {
        Debug.Log("Leave Room");

        PhotonNetwork.LeaveRoom();
    }


    private void CountDown()
    {
        StartCoroutine(CountDownToStart());
    }

    IEnumerator CountDownToStart()
    {

        while (CountDownTime > 0)
        {
            CountDownDisplay.text = ((int)CountDownTime).ToString();
            yield return new WaitForSeconds(1f);

            CountDownTime -= Time.deltaTime;
        }
    }

    private int AIchoice()
    {
        int aiChoice = Random.Range(0, 3);

        return aiChoice;

    }

    private void PlayerChoiceScissor()
    {
        Player1ChoiceValue = 0;
        
    }

    private void PlayerChoiceRock()
    {
        Player1ChoiceValue = 1;
    }

    private void PlayerChoicePaper()
    {
        Player1ChoiceValue = 2;
    }

    [PunRPC]
    public void SetAIResult(int _AIresult)
    {
        AIvalue = _AIresult;

        // 만약에 선택을 했으면
        SCJudgment(AIvalue, Player1ChoiceValue);
    }

    private int SCJudgment(int _aiChoice, int _playerChoice)
    {
        int aiChoice = _aiChoice;
        int playerChoice = _playerChoice;

        int result = playerChoice - aiChoice;

        if (result == 0)
        {
            //무승부
            return 1;

        }
        else if ((result == 1) || (result == -2))
        {
            //승리
            return 2;
        }
        else
        {
            //패배
            return 3;
        }

    }

}
