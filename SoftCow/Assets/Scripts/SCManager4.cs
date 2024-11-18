using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Pun.Demo.Asteroids;
using Photon.Realtime;
using TMPro;

public class SC4Manager : MonoBehaviour
{
    [SerializeField]
    private Button restartButton = null;
    [SerializeField]
    private TextMeshProUGUI inputField1 = null; // ù ��° �÷��̾� �г��� ǥ��
    [SerializeField]
    private TextMeshProUGUI inputField2 = null; // �� ��° �÷��̾� �г��� ǥ��
    [SerializeField]
    private TextMeshProUGUI inputField3 = null; // �� ��° �÷��̾� �г��� ǥ��

    private int Player1Result;
    private int Player2Result;
    private int Player3Result;

    Player[] players = PhotonNetwork.PlayerList;

    private void Awake()
    {
        Player1Result = PlayerPrefs.GetInt("Player1Result");
        Player2Result = PlayerPrefs.GetInt("Player2Result");
        Player3Result = PlayerPrefs.GetInt("Player3Result");

        if (restartButton == null)
        {
            restartButton = GetComponent<Button>(); // Button ������Ʈ�� ����
        }
    }

    private void Start()
    {
        PlayerRsult();

        if (!PhotonNetwork.IsMasterClient)
        {
            restartButton.gameObject.SetActive(false);
        }
        else
        {
            if (restartButton != null)
            {
                restartButton.interactable = true;
                restartButton.onClick.AddListener(GoScene2); // ��ư Ŭ�� �̺�Ʈ ���
            }

            else
            {
                Debug.LogError("RestartButton�� ������� �ʾҽ��ϴ�.");
            }
        }
        
    }
    [PunRPC]
    private void GoScene2()
    {
        PhotonNetwork.LoadLevel("Soft2"); // "Soft2" ������ �̵�
        
    }
    [PunRPC]
    private void PlayerRsult()
    {

        // ���� �ٸ��� ���� ��� ����� ��
        if (Player1Result == 7 && Player2Result == 7 && Player3Result == 7)
        {
            inputField1.text = players[0].NickName;
            inputField2.text = players[1].NickName;
            inputField3.text = players[2].NickName;
        }
        else if (Player1Result == 4 && Player2Result == 5 && Player3Result == 5)
        {
            inputField1.text = players[0].NickName;
        }
        else if (Player2Result == 4 && Player1Result == 5 && Player3Result == 5)
        {
            inputField2.text = players[1].NickName;
        }
        else if (Player3Result == 4 && Player1Result == 5 && Player2Result == 5) 
        {
            inputField3.text = players[2].NickName;
        }
        else if(Player1Result == 4 &&  Player2Result == 4 && Player3Result == 5)
        {
            inputField1.text = players[0].NickName;
            inputField2.text = players[1].NickName;
        }
        else if(Player1Result == 4 &&  Player3Result == 4 && Player2Result ==5)
        {
            inputField1.text = players[0].NickName;
            inputField3.text = players[2].NickName;
        }
        else if(Player2Result == 4 &&  Player3Result == 4 && Player1Result == 5)
        {
            inputField2.text = players[1].NickName;
            inputField3.text = players[2].NickName;
        }
        else if(Player1Result == 6 && Player2Result == 6 && Player3Result == 6)
        {
            inputField1.text = players[0].NickName;
            inputField2.text = players[1].NickName;
            inputField3.text = players[2].NickName;
        }
    }
}

