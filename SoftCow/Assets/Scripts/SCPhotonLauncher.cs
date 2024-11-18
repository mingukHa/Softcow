using UnityEngine;
using UnityEngine.UI;

using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.Demo.PunBasics;
using TMPro; // TextMeshPro 네임스페이스 추가



// Photon.PunBehaviour 사용안함
public class SCPhotonLauncher : MonoBehaviourPunCallbacks
{
    [SerializeField] private string gameVersion = "0.0.1";
    //게임 버전이 다르면 접속이 안됨 같은 버전만 매칭이 됨 유니티 버전이랑 다름
    [SerializeField] private byte maxPlayerPerRoom = 3;

    [SerializeField] private string nickName = string.Empty;

    [SerializeField] private Button connectButton = null;

    [SerializeField] private TMP_InputField inputField;



    private void Awake()
    {

        string version = Application.version;
        // unityVersion = 유니티 버전과 같아진다
        // 마스터가 PhotonNetwork.LoadLevel()을 호출하면,
        // 모든 플레이어가 동일한 레벨을 자동으로 로드
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    private void Start()
    {
        connectButton.interactable = true;
        //런처가 시작되면 버튼이 활성화 됨
        inputField.onValueChanged.AddListener(OnValueChangedNickName);
    }


    // Connect Button이 눌러지면 호출
    public void Connect()
    {
        //if (null == nickName || nickName.Lenght == 0) return;
        if (string.IsNullOrEmpty(nickName)) //닉네임 검사 코드
                                            //{
                                            //    Debug.Log("NickName is empty");
                                            //    return;
                                            //}

        if (PhotonNetwork.IsConnected) //서버에 연결 되어 있는지 검사
        {
            PhotonNetwork.JoinRandomRoom();//무작위 방에 접속
                                            //PhotonNetwork.JoinRoom("Room1");
        }
        else
        {
            Debug.LogFormat("Connect : {0}", gameVersion);

            PhotonNetwork.GameVersion = gameVersion;
            // 포톤 클라우드에 접속을 시작하는 지점
            // 접속에 성공하면 OnConnectedToMaster 메서드 호출
            PhotonNetwork.ConnectUsingSettings(); //설정값 기반 커넥트
        }
    }

    // InputField_NickName과 연결해 닉네임을 가져옴
    public void OnValueChangedNickName(string _nickName)
    {
        Debug.Log($"InputField에서 전달된 값: {_nickName}"); // 입력 값 확인
        PhotonNetwork.NickName = _nickName; // PhotonNetwork에 닉네임 설정
    }


    public override void OnConnectedToMaster() //접속 시 자동 호출
    {
        Debug.LogFormat("Connected to Master: {0}", nickName);

        connectButton.interactable = false;
        Debug.LogError($"{nickName}");
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnDisconnected(DisconnectCause cause) //포톤은 오버라이드를 꼭 사용 해야함
    {
        Debug.LogWarningFormat("Disconnected: {0}", cause);

        connectButton.interactable = true;

        // 방을 생성하면 OnJoinedRoom 호출
        isCreateRoom();
    }

    public override void OnJoinedRoom()//방에 들어갈 때 호출
    {
        Debug.Log("Joined Room");

        // 마스터가 동시에 게임을 시작하게하는 구조가 아니기 때문에 각자 씬을 부르면 됨
        //PhotonNetwork.LoadLevel("PUNRoom"); //포톤에서 제공 보통은 이걸 쓰는데 이건 모두가 같이 게임을 시작
        SceneManager.LoadScene("Soft2"); //유니티에서 제공 이건 그냥 입장하면 바로 씬 시작, 지렁이 게임 같은 것
        //Scene List에 등록이 되어 있어야 함
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {//방 찾기를 실패 했을 때 호출
        Debug.LogErrorFormat("JoinRandomFailed({0}): {1}", returnCode, message);

        connectButton.interactable = true;

        isCreateRoom();
        //방 찾기에 실패하면 그냥 내가 방을 만든다
    }

    private void isCreateRoom() //방을 만들어주는 함수
    {
        Debug.Log("Create Room");
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayerPerRoom });
    }
}