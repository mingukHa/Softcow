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
    [SerializeField] private Button Scissors = null; // ���� ��ư
    [SerializeField] private Button Rock = null; // �� ��ư
    [SerializeField] private Button Paper = null; // �� ��ư

    private int PlayerChoiceValue = -1; // �⺻���� -1�� �ؼ� �������� �������� ǥ��

    [SerializeField] private float CountDown = 10f; // ī��Ʈ�ٿ� �ð�
    [SerializeField] private TextMeshProUGUI CountDownDisplay = null; // ī��Ʈ ui
    
    [SerializeField]private GameObject[] prefab = null;
    [SerializeField]private TextMeshProUGUI usernum = null;

    private GameObject[] playerGoList = new GameObject[3]; // �� �÷��̾� ������Ʈ�� �����ϴ� �迭
    private Dictionary<int, int> playerChoices = new Dictionary<int, int>(); //�÷��̾��� ���ð��� ��Ʈ�ѹ��� �Բ� ����

    private bool countdownStarted = false; // ī��Ʈ�ٿ� ���� Ȯ�� �⺻�� off

    private void Awake()
    {
        usernum = GetComponent<TextMeshProUGUI>();
        // ������ ���۵Ǹ� ��ư�� ��Ȱ��ȭ
        Scissors.interactable = false;
        Rock.interactable = false;
        Paper.interactable = false;
        Debug.Log("��ư ������");
        
    }

    private void Start()
    {
        // ��ư Ŭ�� ������ �߰�
        Scissors.onClick.AddListener(() => PlayerChoice(0)); // ���� ����
        Rock.onClick.AddListener(() => PlayerChoice(1)); // ���� ����
        Paper.onClick.AddListener(() => PlayerChoice(2)); // �� ����
        Debug.Log("��ư������ ������?");
        
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        photonView.RPC("OnPrefab", RpcTarget.All);
        
        usernum.text =PhotonNetwork.CurrentRoom.PlayerCount.ToString();
        // ����� ������ ȣ�� ��
        
        Debug.Log($"Player {newPlayer.NickName} ���Դ�");
        if (PhotonNetwork.IsMasterClient) //ó�� ��� �� ������ Ŭ���̾�Ʈ��
        {
            int playerIndex = newPlayer.ActorNumber - 1;
            
            //������ ��Ʈ �ѹ� �������� �迭 �ε����� ��� �ؾ��ؼ� 1�� ����
            //if (playerIndex >= 0 && playerIndex < 3)
            {//�ε����� �������� ������ ������
                // �迭�� �޾� �� ��ǥ ��� ������ ����
                
                playerGoList[playerIndex].name = "Player_" + newPlayer.NickName; // �÷��̾� �̸� ����
               
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
        // ���࿡ 3���� �� ��� ������
        if (PhotonNetwork.CurrentRoom.PlayerCount == 3)
        {
            Debug.Log("�� ���Դ�!");
            photonView.RPC("StartCountdown", RpcTarget.AllBuffered);
            // ��� Ŭ���̾�Ʈ���� ī��Ʈ�ٿ� ����
            OnPrefab(0, 1);
        }
    }

    [PunRPC] //��ο��� ���
    private void StartCountdown() //��ư Ȱ��ȭ �Լ�
    {
        if (!countdownStarted)  //���࿡ ī��Ʈ �ٿ� ������ �ȵǾ��ٸ�
        {
            countdownStarted = true; // ���� ���� (������ ��û ����)

            // ��ư�� �� Ű��
            Scissors.interactable = true;
            Rock.interactable = true;
            Paper.interactable = true;
            Debug.Log("��ư �� �״�!");

            //�ڷ�ƾ ����
            StartCoroutine(CountDownToStart());
        }
    }

    private void PlayerChoice(int choice)
    {
        // �÷��̾� ���ð� ����
        PlayerChoiceValue = choice;
        Debug.Log($"�� �̰� �´� {choice}");

        // ���� �ϸ� ��ư ����
        Scissors.interactable = false;
        Rock.interactable = false;
        Paper.interactable = false;

        // ���ʿ��� �����ϱ�
        //photonView.RPC("SubmitChoice", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer.ActorNumber, choice);
        photonView.RPC("SubmitChoice", RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber, choice);
    }

    [PunRPC] //�÷��̾� ���� �ޱ�
    private void SubmitChoice(int actorNumber, int choice)
    { //�÷��̾�� ������ ��ȣ �޾Ƽ�

        if (!playerChoices.ContainsKey(actorNumber))
        { //��ųʸ� Ȱ���ؼ� �÷��̾ Ű�� �����Ѱ�
            playerChoices[actorNumber] = choice;
            //�� �÷��̾��� ������ ��ųʸ��� �����Ѵ�
            Debug.LogError($"{actorNumber}���ε� {choice}�̰� �����.");
        }

        // ��� �÷��̾ ������ �Ϸ��ߴ��� Ȯ��
        if (playerChoices.Count == 3)
        {
            Debug.LogError("�� �����");
            Winner(); //���� �����ϱ�

            ApplyPlayerList();

            PlayerPrefs.SetString("Player1NickName", playerGoList[0].GetComponent<PhotonView>().Owner.NickName);
            //Debug.LogError("playerlist ; {0}", playerGoList[].name);
        }
    }
    [PunRPC]
    public void ApplyPlayerList()
    {
        // ���� �濡 ������ �ִ� �÷��̾��� ��
        Debug.LogError("CurrentRoom PlayerCount : " + PhotonNetwork.CurrentRoom.PlayerCount);

        // ���� �����Ǿ� �ִ� ��� ����� ��������
        //PhotonView[] photonViews = FindObjectsOfType<PhotonView>();
        PhotonView[] photonViews =
            FindObjectsByType<PhotonView>(FindObjectsSortMode.None);

        // �Ź� �������� �ϴ°� �����Ƿ� �÷��̾� ���ӿ�����Ʈ ����Ʈ�� �ʱ�ȭ
        System.Array.Clear(playerGoList, 0, playerGoList.Length);

        // ���� �����Ǿ� �ִ� ����� ��ü��
        // �������� �÷��̾���� ���ͳѹ��� ����,
        // ���ͳѹ��� �������� �÷��̾� ���ӿ�����Ʈ �迭�� ä��
        for (int i = 0; i < PhotonNetwork.CurrentRoom.PlayerCount; ++i)
        {
            // Ű�� 0�� �ƴ� 1���� ����
            int key = i + 1;
            for (int j = 0; j < photonViews.Length; ++j)
            {
                // ���� PhotonNetwork.Instantiate�� ���ؼ� ������ ����䰡 �ƴ϶�� �ѱ�
                if (photonViews[j].isRuntimeInstantiated == false) continue;
                // ���� ���� Ű ���� ��ųʸ� ���� �������� �ʴ´ٸ� �ѱ�
                if (PhotonNetwork.CurrentRoom.Players.ContainsKey(key) == false) continue;

                // ������� ���ͳѹ�
                int viewNum = photonViews[j].Owner.ActorNumber;
                // �������� �÷��̾��� ���ͳѹ�
                int playerNum = PhotonNetwork.CurrentRoom.Players[key].ActorNumber;

                // ���ͳѹ��� ���� ������Ʈ�� �ִٸ�,
                if (viewNum == playerNum)
                {
                    // ���� ���ӿ�����Ʈ�� �迭�� �߰�
                    playerGoList[playerNum - 1] = photonViews[j].gameObject;
                    // ���ӿ�����Ʈ �̸��� �˾ƺ��� ���� ����
                    playerGoList[playerNum - 1].name = "Player_" + photonViews[j].Owner.NickName;
                }
            }
        }
    }
        private IEnumerator CountDownToStart()
    {
        float time = CountDown;
        //while �ݺ��� ���� ������ ī��Ʈ �ٿ� Ÿ��
        while (time >= 0)
        {
            CountDownDisplay.text = Mathf.Ceil(time).ToString(); // ī��Ʈ�ٿ� UI ������Ʈ
            yield return new WaitForSeconds(1f); //1�ʸ� ��ٸ���
            time--; //1�ʾ� ����.
        }

        // ī��Ʈ�ٿ� ���� �� ���� �� �ϸ�
        if (PlayerChoiceValue == -1) //���� �� �̾�����
        {
            int autoChoice = Random.Range(0, 3); // �������� �� �����Ѵ�
            Debug.Log($"�� �ȳ� �ڵ����� {autoChoice}.");
            PlayerChoice(autoChoice);
        }
    }

    public void Winner()
    {
        // �÷��̾� ���ð� ����Ʈ
        List<int> choices = new List<int>(playerChoices.Values);

        
        //1���ϰ� 2��
        int result = (3 + choices[0] - choices[1]) % 3; // ù ��°�� �� ��° �÷��̾� ��
        int result2 = (3 + choices[0] - choices[2]) % 3;//1���̶� 3�� ��
        int result3 = (3 + choices[1] - choices[2]) % 3;
        Debug.LogError($"{choices[0]},{choices[1]},{choices[2]},{result}");
        if (result == 0) // ���º�
        {
            //1���̶� 3�� ��
            //�ι� ° ���ϴ� ��
            if (result2 == 0)
            {
                //���º� ���� ������ �Ѿ��
                //4 : �̱� 5: �� 6: ���
                PlayerPrefs.SetInt("Player1Result", 6);
                PlayerPrefs.SetInt("Player2Result", 6);
                PlayerPrefs.SetInt("Player3Result", 6);
                PlayerPrefs.SetInt("Player1", choices[0]);
                PlayerPrefs.SetInt("Player2", choices[1]);
                PlayerPrefs.SetInt("Player3", choices[2]);
                Debug.LogError("Result: ���º�");
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
                Debug.LogError("Result: 1, 2���� �������̱�");
                SceneManager.LoadScene("Soft3");
            }
            else
            {
                //���� 3���� �̱�
                PlayerPrefs.SetInt("Player1Result", 5);
                PlayerPrefs.SetInt("Player2Result", 5);
                PlayerPrefs.SetInt("Player3Result", 4);
                PlayerPrefs.SetInt("Player1", choices[0]);
                PlayerPrefs.SetInt("Player2", choices[1]);
                PlayerPrefs.SetInt("Player3", choices[2]);
                Debug.LogError("Result: 3���� �������̱�");
                SceneManager.LoadScene("Soft3");
            }
        }
        else if(result == 1 || result == -2) // 1���� �̱�(ù �ܰ迡��)2���� ������
        {
            if(result2 == 0 )//1, 3 ���� ���
            {
                PlayerPrefs.SetInt("Player1Result", 4);
                PlayerPrefs.SetInt("Player2Result", 5);
                PlayerPrefs.SetInt("Player3Result", 4);
                PlayerPrefs.SetInt("Player1", choices[0]);
                PlayerPrefs.SetInt("Player2", choices[1]);
                PlayerPrefs.SetInt("Player3", choices[2]);
                Debug.LogError("Result: 1,3 �̱�� ���");
                SceneManager.LoadScene("Soft3");
            }
            else if(result2 == 1 || result2 == -2)// 1�� ���� �̱�� ���
            {
                PlayerPrefs.SetInt("Player1Result", 4);
                PlayerPrefs.SetInt("Player2Result", 5);
                PlayerPrefs.SetInt("Player3Result", 5);
                PlayerPrefs.SetInt("Player1", choices[0]);
                PlayerPrefs.SetInt("Player2", choices[1]);
                PlayerPrefs.SetInt("Player3", choices[2]);
                Debug.LogError("Result: 1 �������̱�� ���");
                SceneManager.LoadScene("Soft3");
            }
            else // ��� ���
            {
                PlayerPrefs.SetInt("Player1Result", 7);
                PlayerPrefs.SetInt("Player2Result", 7);
                PlayerPrefs.SetInt("Player3Result", 7);
                PlayerPrefs.SetInt("Player1", choices[0]);
                PlayerPrefs.SetInt("Player2", choices[1]);
                PlayerPrefs.SetInt("Player3", choices[2]);
                Debug.LogError("Result: �� �޶� ��� ���");
                SceneManager.LoadScene("Soft3");
            }
        }
        else
        {
            //ù ��° �ܰ迡�� 2�� �̱�
            //2���̶� 3�� ��
            if (result3 == 0)
            {
                //���º� ���� ������ �Ѿ��
                PlayerPrefs.SetInt("Player1Result", 5);
                PlayerPrefs.SetInt("Player2Result", 4);
                PlayerPrefs.SetInt("Player3Result", 4);
                PlayerPrefs.SetInt("Player1", choices[0]);
                PlayerPrefs.SetInt("Player2", choices[1]);
                PlayerPrefs.SetInt("Player3", choices[2]);
                Debug.LogError("Result: 2,3 �̱�� ��� ");
                SceneManager.LoadScene("Soft3");
            }
            else if (result3 == 1 || result3 == -2)
            {
                //2���� �������̱�

                PlayerPrefs.SetInt("Player1Result", 5);
                PlayerPrefs.SetInt("Player2Result", 4);
                PlayerPrefs.SetInt("Player3Result", 5);
                PlayerPrefs.SetInt("Player1", choices[0]);
                PlayerPrefs.SetInt("Player2", choices[1]);
                PlayerPrefs.SetInt("Player3", choices[2]);
                Debug.LogError("Result: 2���� �̱�� ���");
                SceneManager.LoadScene("Soft3");
            }
            else
            {
                //3���� �������̱�
                PlayerPrefs.SetInt("Player1Result", 5);
                PlayerPrefs.SetInt("Player2Result", 5);
                PlayerPrefs.SetInt("Player3Result", 4);
                PlayerPrefs.SetInt("Player1", choices[0]);
                PlayerPrefs.SetInt("Player2", choices[1]);
                PlayerPrefs.SetInt("Player3", choices[2]);
                Debug.LogError("Result: 3���� �̱�� ���");
                SceneManager.LoadScene("Soft3");
            }
        }

    }
}
