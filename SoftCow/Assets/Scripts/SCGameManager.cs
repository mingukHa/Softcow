using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class SCGameManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private Button Scissors = null; // 가위 버튼
    [SerializeField] private Button Rock = null; // 묵 버튼
    [SerializeField] private Button Paper = null; // 보 버튼

    private int PlayerChoiceValue = -1; // 기본값을 -1로 해서 선택하지 않음으로 표시

    [SerializeField] private float CountDown = 10f; // 카운트다운 시간
    [SerializeField] private TextMeshProUGUI CountDownDisplay = null; // 카운트 ui
    
    [SerializeField]private GameObject[] prefab = null;
    [SerializeField]private TextMeshProUGUI usernum = null;

    private GameObject[] playerGoList = new GameObject[3]; // 각 플레이어 오브젝트를 저장하는 배열
    private Dictionary<int, int> playerChoices = new Dictionary<int, int>(); //플레이어의 선택값을 엑트넘버와 함께 저장

    private bool countdownStarted = false; // 카운트다운 시작 확인 기본은 off

    private void Awake()
    {
        usernum = GetComponent<TextMeshProUGUI>();
        // 게임이 시작되면 버튼을 비활성화
        Scissors.interactable = false;
        Rock.interactable = false;
        Paper.interactable = false;
        Debug.Log("버튼 꺼졌다");
        
    }

    private void Start()
    {
        // 버튼 클릭 리스너 추가
        Scissors.onClick.AddListener(() => PlayerChoice(0)); // 가위 선택
        Rock.onClick.AddListener(() => PlayerChoice(1)); // 바위 선택
        Paper.onClick.AddListener(() => PlayerChoice(2)); // 보 선택
        Debug.Log("버튼리스너 켜졌나?");
        
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        photonView.RPC("OnPrefab", RpcTarget.All);
        
        usernum.text =PhotonNetwork.CurrentRoom.PlayerCount.ToString();
        // 사람이 들어오면 호출 됨
        
        Debug.Log($"Player {newPlayer.NickName} 들어왔다");
        if (PhotonNetwork.IsMasterClient) //처음 들어 온 방장의 클라이언트면
        {
            int playerIndex = newPlayer.ActorNumber - 1;
            
            //포톤의 엑트 넘버 기준으로 배열 인덱스를 계산 해야해서 1을 뺀다
            //if (playerIndex >= 0 && playerIndex < 3)
            {//인덱스가 정상적인 범위에 있으면
                // 배열로 받아 둔 좌표 대로 프리팹 생성
                
                playerGoList[playerIndex].name = "Player_" + newPlayer.NickName; // 플레이어 이름 지정
               
            }
        }
    }
    public override void OnPlayerLeftRoom(Player lostPlayer)
    {
        OffPrefab(1);
    }
    [PunRPC]
    private void OffPrefab(int _num)
    {
        prefab[_num].SetActive(false);
    }
    [PunRPC]
    private void OnPrefab()
    {
        prefab[0].SetActive(true);
    }
    [PunRPC]
    private void OnPrefab(int num, int nums)
    {
        prefab[num].SetActive(true);
        prefab[nums].SetActive(true);
    }
    private void Update()
    {
        // 만약에 3명이 다 들어 왔으면
        if (PhotonNetwork.CurrentRoom.PlayerCount == 3)
        {
            Debug.Log("다 들어왔다!");
            photonView.RPC("StartCountdown", RpcTarget.AllBuffered);
            // 모든 클라이언트에서 카운트다운 시작
            OnPrefab(0, 1);
        }
    }

    [PunRPC] //모두에게 쏜다
    private void StartCountdown() //버튼 활성화 함수
    {
        if (!countdownStarted)  //만약에 카운트 다운 실행이 안되었다면
        {
            countdownStarted = true; // 이제 실행 (오류가 엉청 났다)

            // 버튼들 다 키고
            Scissors.interactable = true;
            Rock.interactable = true;
            Paper.interactable = true;
            Debug.Log("버튼 다 켰다!");

            //코루틴 실행
            StartCoroutine(CountDownToStart());
        }
    }

    private void PlayerChoice(int choice)
    {
        // 플레이어 선택값 저장
        PlayerChoiceValue = choice;
        Debug.Log($"나 이거 냈다 {choice}");

        // 선택 하면 버튼 끈다
        Scissors.interactable = false;
        Rock.interactable = false;
        Paper.interactable = false;

        // 오너에게 전달하기
        //photonView.RPC("SubmitChoice", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer.ActorNumber, choice);
        photonView.RPC("SubmitChoice", RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber, choice);
    }

    [PunRPC] //플레이어 선택 받기
    private void SubmitChoice(int actorNumber, int choice)
    { //플레이어랑 선택한 번호 받아서

        if (!playerChoices.ContainsKey(actorNumber))
        { //딕셔너리 활용해서 플레이어가 키를 선택한걸
            playerChoices[actorNumber] = choice;
            //각 플레이어의 선택을 딕셔너리에 저장한다
            Debug.LogError($"{actorNumber}번인데 {choice}이거 골랐다.");
        }

        // 모든 플레이어가 선택을 완료했는지 확인
        if (playerChoices.Count == 3)
        {
            Debug.LogError("다 골랐다");
            Winner(); //승자 선택하기

            ApplyPlayerList();

            PlayerPrefs.SetString("Player1NickName", playerGoList[0].GetComponent<PhotonView>().Owner.NickName);
            //Debug.LogError("playerlist ; {0}", playerGoList[].name);
        }
    }
    [PunRPC]
    public void ApplyPlayerList()
    {
        // 현재 방에 접속해 있는 플레이어의 수
        Debug.LogError("CurrentRoom PlayerCount : " + PhotonNetwork.CurrentRoom.PlayerCount);

        // 현재 생성되어 있는 모든 포톤뷰 가져오기
        //PhotonView[] photonViews = FindObjectsOfType<PhotonView>();
        PhotonView[] photonViews =
            FindObjectsByType<PhotonView>(FindObjectsSortMode.None);

        // 매번 재정렬을 하는게 좋으므로 플레이어 게임오브젝트 리스트를 초기화
        System.Array.Clear(playerGoList, 0, playerGoList.Length);

        // 현재 생성되어 있는 포톤뷰 전체와
        // 접속중인 플레이어들의 액터넘버를 비교해,
        // 액터넘버를 기준으로 플레이어 게임오브젝트 배열을 채움
        for (int i = 0; i < PhotonNetwork.CurrentRoom.PlayerCount; ++i)
        {
            // 키는 0이 아닌 1부터 시작
            int key = i + 1;
            for (int j = 0; j < photonViews.Length; ++j)
            {
                // 만약 PhotonNetwork.Instantiate를 통해서 생성된 포톤뷰가 아니라면 넘김
                if (photonViews[j].isRuntimeInstantiated == false) continue;
                // 만약 현재 키 값이 딕셔너리 내에 존재하지 않는다면 넘김
                if (PhotonNetwork.CurrentRoom.Players.ContainsKey(key) == false) continue;

                // 포톤뷰의 액터넘버
                int viewNum = photonViews[j].Owner.ActorNumber;
                // 접속중인 플레이어의 액터넘버
                int playerNum = PhotonNetwork.CurrentRoom.Players[key].ActorNumber;

                // 액터넘버가 같은 오브젝트가 있다면,
                if (viewNum == playerNum)
                {
                    // 실제 게임오브젝트를 배열에 추가
                    playerGoList[playerNum - 1] = photonViews[j].gameObject;
                    // 게임오브젝트 이름도 알아보기 쉽게 변경
                    playerGoList[playerNum - 1].name = "Player_" + photonViews[j].Owner.NickName;
                }
            }
        }
    }
        private IEnumerator CountDownToStart()
    {
        float time = CountDown;
        //while 반복문 돌릴 변수는 카운트 다운 타임
        while (time >= 0)
        {
            CountDownDisplay.text = Mathf.Ceil(time).ToString(); // 카운트다운 UI 업데이트
            yield return new WaitForSeconds(1f); //1초를 기다리고
            time--; //1초씩 뺀다.
        }

        // 카운트다운 종료 후 선택 안 하면
        if (PlayerChoiceValue == -1) //아직 안 뽑았으면
        {
            int autoChoice = Random.Range(0, 3); // 랜덤으로 값 저장한다
            Debug.Log($"얘 안냄 자동으로 {autoChoice}.");
            PlayerChoice(autoChoice);
        }
    }

    public void Winner()
    {
        // 플레이어 선택값 리스트
        List<int> choices = new List<int>(playerChoices.Values);

        
        //1번하고 2번
        int result = (3 + choices[0] - choices[1]) % 3; // 첫 번째와 두 번째 플레이어 비교
        int result2 = (3 + choices[0] - choices[2]) % 3;//1번이랑 3번 비교
        int result3 = (3 + choices[1] - choices[2]) % 3;
        Debug.LogError($"{choices[0]},{choices[1]},{choices[2]},{result}");
        if (result == 0) // 무승부
        {
            //1번이랑 3번 비교
            //두번 째 비교하는 것
            if (result2 == 0)
            {
                //무승부 다음 씬으로 넘어가기
                //4 : 이김 5: 짐 6: 비김
                PlayerPrefs.SetInt("Player1Result", 6);
                PlayerPrefs.SetInt("Player2Result", 6);
                PlayerPrefs.SetInt("Player3Result", 6);
                PlayerPrefs.SetInt("Player1", choices[0]);
                PlayerPrefs.SetInt("Player2", choices[1]);
                PlayerPrefs.SetInt("Player3", choices[2]);
                Debug.LogError("Result: 무승부");
                SceneManager.LoadScene("Soft3");
            }
            else if(result2 == 1 || result2 == -2)
            {
                PlayerPrefs.SetInt("Player1Result", 4);
                PlayerPrefs.SetInt("Player2Result", 4);
                PlayerPrefs.SetInt("Player3Result", 5);
                PlayerPrefs.SetInt("Player1", choices[0]);
                PlayerPrefs.SetInt("Player2", choices[1]);
                PlayerPrefs.SetInt("Player3", choices[2]);
                Debug.LogError("Result: 1, 2번이 완전히이김");
                SceneManager.LoadScene("Soft3");
            }
            else
            {
                //완전 3번이 이김
                PlayerPrefs.SetInt("Player1Result", 5);
                PlayerPrefs.SetInt("Player2Result", 5);
                PlayerPrefs.SetInt("Player3Result", 4);
                PlayerPrefs.SetInt("Player1", choices[0]);
                PlayerPrefs.SetInt("Player2", choices[1]);
                PlayerPrefs.SetInt("Player3", choices[2]);
                Debug.LogError("Result: 3번이 완전히이김");
                SceneManager.LoadScene("Soft3");
            }
        }
        else if(result == 1 || result == -2) // 1번이 이김(첫 단계에서)2번이 진상태
        {
            if(result2 == 0 )//1, 3 비기는 경우
            {
                PlayerPrefs.SetInt("Player1Result", 4);
                PlayerPrefs.SetInt("Player2Result", 5);
                PlayerPrefs.SetInt("Player3Result", 4);
                PlayerPrefs.SetInt("Player1", choices[0]);
                PlayerPrefs.SetInt("Player2", choices[1]);
                PlayerPrefs.SetInt("Player3", choices[2]);
                Debug.LogError("Result: 1,3 이기는 경우");
                SceneManager.LoadScene("Soft3");
            }
            else if(result2 == 1 || result2 == -2)// 1번 완전 이기는 경우
            {
                PlayerPrefs.SetInt("Player1Result", 4);
                PlayerPrefs.SetInt("Player2Result", 5);
                PlayerPrefs.SetInt("Player3Result", 5);
                PlayerPrefs.SetInt("Player1", choices[0]);
                PlayerPrefs.SetInt("Player2", choices[1]);
                PlayerPrefs.SetInt("Player3", choices[2]);
                Debug.LogError("Result: 1 완전히이기는 경우");
                SceneManager.LoadScene("Soft3");
            }
            else // 기는 경우
            {
                PlayerPrefs.SetInt("Player1Result", 7);
                PlayerPrefs.SetInt("Player2Result", 7);
                PlayerPrefs.SetInt("Player3Result", 7);
                PlayerPrefs.SetInt("Player1", choices[0]);
                PlayerPrefs.SetInt("Player2", choices[1]);
                PlayerPrefs.SetInt("Player3", choices[2]);
                Debug.LogError("Result: 다 달라서 비긴 경우");
                SceneManager.LoadScene("Soft3");
            }
        }
        else
        {
            //첫 번째 단계에서 2번 이김
            //2번이랑 3번 비교
            if (result3 == 0)
            {
                //무승부 다음 씬으로 넘어가기
                PlayerPrefs.SetInt("Player1Result", 5);
                PlayerPrefs.SetInt("Player2Result", 4);
                PlayerPrefs.SetInt("Player3Result", 4);
                PlayerPrefs.SetInt("Player1", choices[0]);
                PlayerPrefs.SetInt("Player2", choices[1]);
                PlayerPrefs.SetInt("Player3", choices[2]);
                Debug.LogError("Result: 2,3 이기는 경우 ");
                SceneManager.LoadScene("Soft3");
            }
            else if (result3 == 1 || result3 == -2)
            {
                //2번이 완전히이김

                PlayerPrefs.SetInt("Player1Result", 5);
                PlayerPrefs.SetInt("Player2Result", 4);
                PlayerPrefs.SetInt("Player3Result", 5);
                PlayerPrefs.SetInt("Player1", choices[0]);
                PlayerPrefs.SetInt("Player2", choices[1]);
                PlayerPrefs.SetInt("Player3", choices[2]);
                Debug.LogError("Result: 2번이 이기는 경우");
                SceneManager.LoadScene("Soft3");
            }
            else
            {
                //3번이 완전히이김
                PlayerPrefs.SetInt("Player1Result", 5);
                PlayerPrefs.SetInt("Player2Result", 5);
                PlayerPrefs.SetInt("Player3Result", 4);
                PlayerPrefs.SetInt("Player1", choices[0]);
                PlayerPrefs.SetInt("Player2", choices[1]);
                PlayerPrefs.SetInt("Player3", choices[2]);
                Debug.LogError("Result: 3번이 이기는 경우");
                SceneManager.LoadScene("Soft3");
            }
        }

    }
}
