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
    private float CountDown = 5f; // 카운트다운 시간
    [SerializeField] private TextMeshProUGUI CountDownDisplay = null; // 카운트 ui

    [SerializeField]
    private GameObject[] PlayerPrefab1 = null;
    [SerializeField]
    private GameObject[] PlayerPrefab2 = null;
    [SerializeField]
    private GameObject[] PlayerPrefab3 = null;

    [SerializeField]
    private TextMeshProUGUI inputField1 = null; // 첫 번째 플레이어 닉네임 표시
    [SerializeField]
    private TextMeshProUGUI inputField2 = null; // 두 번째 플레이어 닉네임 표시
    [SerializeField]
    private TextMeshProUGUI inputField3 = null; // 세 번째 플레이어 닉네임 표시

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
        Invoke("Next", 5f); // 5초 후 씬 이동
    }

    [PunRPC]
    private void SetPrefabs()
    {
        // 플레이어 선택에 따라 프리팹 활성화
        PlayerPrefab1[PlayerChoice1].SetActive(true);
        PlayerPrefab2[PlayerChoice2].SetActive(true);
        PlayerPrefab3[PlayerChoice3].SetActive(true);
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


    }


    private void Next()
    {
        SceneManager.LoadScene("Soft4"); // 다음 씬으로 이동
    }
}

