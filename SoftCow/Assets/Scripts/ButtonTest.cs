using TMPro;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class ButtonTest : MonoBehaviour
{
    [SerializeField] private Button Scissors = null;
    [SerializeField] private Button Rock = null;
    [SerializeField] private Button Paper = null;

    //private int user1 = 
    //ArrayList[2]

    private int PlayerCount = 2;

    [SerializeField] private float CountDownTime = 5;
    [SerializeField] private TextMeshProUGUI CountDownDisplay = null;


    private void Awake()
    {
        Scissors.enabled = false;
        Rock.enabled = false;
        Paper.enabled = false;
    }
    private void Start()
    {
    }

    private void Update()
    {
        //if(PlayerID == 2)
        if (PlayerCount == 2)
        {
            Scissors.enabled = true;
            Rock.enabled = true;
            Paper.enabled = true;
            CountDown();
        }
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
            //CountDownDisplay.text = "5";
            //yield return new WaitForSeconds(1f);
            //CountDownDisplay.text = "4";
            //yield return new WaitForSeconds(1f);
            //CountDownDisplay.text = "3";
            //yield return new WaitForSeconds(1f);
            //CountDownDisplay.text = "2";
            //yield return new WaitForSeconds(1f);
            //CountDownDisplay.text = "1";
            CountDownTime -= Time.deltaTime;
        }
    }
}
