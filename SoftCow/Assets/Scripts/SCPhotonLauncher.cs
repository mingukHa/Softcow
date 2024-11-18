using UnityEngine;
using UnityEngine.UI;

using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.Demo.PunBasics;
using TMPro; // TextMeshPro ���ӽ����̽� �߰�



// Photon.PunBehaviour ������
public class SCPhotonLauncher : MonoBehaviourPunCallbacks
{
    [SerializeField] private string gameVersion = "0.0.1";
    //���� ������ �ٸ��� ������ �ȵ� ���� ������ ��Ī�� �� ����Ƽ �����̶� �ٸ�
    [SerializeField] private byte maxPlayerPerRoom = 3;

    [SerializeField] private string nickName = string.Empty;

    [SerializeField] private Button connectButton = null;

    [SerializeField] private TMP_InputField inputField;



    private void Awake()
    {

        string version = Application.version;
        // unityVersion = ����Ƽ ������ ��������
        // �����Ͱ� PhotonNetwork.LoadLevel()�� ȣ���ϸ�,
        // ��� �÷��̾ ������ ������ �ڵ����� �ε�
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    private void Start()
    {
        connectButton.interactable = true;
        //��ó�� ���۵Ǹ� ��ư�� Ȱ��ȭ ��
        inputField.onValueChanged.AddListener(OnValueChangedNickName);
    }


    // Connect Button�� �������� ȣ��
    public void Connect()
    {
        //if (null == nickName || nickName.Lenght == 0) return;
        if (string.IsNullOrEmpty(nickName)) //�г��� �˻� �ڵ�
                                            //{
                                            //    Debug.Log("NickName is empty");
                                            //    return;
                                            //}

        if (PhotonNetwork.IsConnected) //������ ���� �Ǿ� �ִ��� �˻�
        {
            PhotonNetwork.JoinRandomRoom();//������ �濡 ����
                                            //PhotonNetwork.JoinRoom("Room1");
        }
        else
        {
            Debug.LogFormat("Connect : {0}", gameVersion);

            PhotonNetwork.GameVersion = gameVersion;
            // ���� Ŭ���忡 ������ �����ϴ� ����
            // ���ӿ� �����ϸ� OnConnectedToMaster �޼��� ȣ��
            PhotonNetwork.ConnectUsingSettings(); //������ ��� Ŀ��Ʈ
        }
    }

    // InputField_NickName�� ������ �г����� ������
    public void OnValueChangedNickName(string _nickName)
    {
        Debug.Log($"InputField���� ���޵� ��: {_nickName}"); // �Է� �� Ȯ��
        PhotonNetwork.NickName = _nickName; // PhotonNetwork�� �г��� ����
    }


    public override void OnConnectedToMaster() //���� �� �ڵ� ȣ��
    {
        Debug.LogFormat("Connected to Master: {0}", nickName);

        connectButton.interactable = false;
        Debug.LogError($"{nickName}");
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnDisconnected(DisconnectCause cause) //������ �������̵带 �� ��� �ؾ���
    {
        Debug.LogWarningFormat("Disconnected: {0}", cause);

        connectButton.interactable = true;

        // ���� �����ϸ� OnJoinedRoom ȣ��
        isCreateRoom();
    }

    public override void OnJoinedRoom()//�濡 �� �� ȣ��
    {
        Debug.Log("Joined Room");

        // �����Ͱ� ���ÿ� ������ �����ϰ��ϴ� ������ �ƴϱ� ������ ���� ���� �θ��� ��
        //PhotonNetwork.LoadLevel("PUNRoom"); //���濡�� ���� ������ �̰� ���µ� �̰� ��ΰ� ���� ������ ����
        SceneManager.LoadScene("Soft2"); //����Ƽ���� ���� �̰� �׳� �����ϸ� �ٷ� �� ����, ������ ���� ���� ��
        //Scene List�� ����� �Ǿ� �־�� ��
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {//�� ã�⸦ ���� ���� �� ȣ��
        Debug.LogErrorFormat("JoinRandomFailed({0}): {1}", returnCode, message);

        connectButton.interactable = true;

        isCreateRoom();
        //�� ã�⿡ �����ϸ� �׳� ���� ���� �����
    }

    private void isCreateRoom() //���� ������ִ� �Լ�
    {
        Debug.Log("Create Room");
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayerPerRoom });
    }
}