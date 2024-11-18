using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class SCResultManager : MonoBehaviourPunCallbacks
{

    private int PlayerChoice1;
    private int PlayerChoice2;
    private int PlayerChoice3;
    private float CountDown = 5f; // ī��Ʈ�ٿ� �ð�
    [SerializeField] private TextMeshProUGUI CountDownDisplay = null; // ī��Ʈ ui

    [SerializeField]
    private GameObject[] PlayerPrefab1 = null;
    [SerializeField]
    private GameObject[] PlayerPrefab2 = null;
    [SerializeField]
    private GameObject[] PlayerPrefab3 = null;

    [SerializeField]
    private TextMeshProUGUI inputField1 = null; // ù ��° �÷��̾� �г��� ǥ��
    [SerializeField]
    private TextMeshProUGUI inputField2 = null; // �� ��° �÷��̾� �г��� ǥ��
    [SerializeField]
    private TextMeshProUGUI inputField3 = null; // �� ��° �÷��̾� �г��� ǥ��

    private void Awake()
    {
        PlayerChoice1 = PlayerPrefs.GetInt("Player1");
        PlayerChoice2 = PlayerPrefs.GetInt("Player2");
        PlayerChoice3 = PlayerPrefs.GetInt("Player3");
    }

    private void Start()
    {
        SetPrefabs();
        Player[] players = PhotonNetwork.PlayerList;

        inputField1.text = players[0].NickName;
        inputField2.text = players[1].NickName;
        inputField3.text = players[2].NickName;
        StartCoroutine(CountDownToStart());
        Invoke("Next", 5f); // 5�� �� �� �̵�
    }

    [PunRPC]
    private void SetPrefabs()
    {
        // �÷��̾� ���ÿ� ���� ������ Ȱ��ȭ
        PlayerPrefab1[PlayerChoice1].SetActive(true);
        PlayerPrefab2[PlayerChoice2].SetActive(true);
        PlayerPrefab3[PlayerChoice3].SetActive(true);
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


    }


    private void Next()
    {
        SceneManager.LoadScene("Soft4"); // ���� ������ �̵�
    }
}

