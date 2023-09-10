using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NativeWebSocket;
using System.Threading.Tasks;
using System;
using System.Runtime.InteropServices.ComTypes;
using TMPro;
using UnityEngine.UI;
using static NewBehaviourScript;
using Unity.VisualScripting;
using UnityEngine.U2D;


public class NewBehaviourScript : MonoBehaviour 
{
    

    WebSocket websocket;
    //bool isConnectClose = true;
    public Text txtSmall;
    public Text txtBig;
    public Text txtTime;
    public Text txt_id;
    public Text txtCountDown;
    
    //public Text txtResult;
    public GameObject dice;

    public GameObject Blow;
    

    public SpriteAtlas spriteAtlas;


    public Animator animatorXiu;
    public Animator animatorTai;

    


    IEnumerator ShowResultCoroutine()
    {
        float thoigianbatdau = Time.time;
        float thoigiandichuyen = 0.4f;

        Vector3 startPos = Blow.transform.position; 
        Vector3 endPos = startPos + new Vector3(0.5f, 1f, 0.5f); 

        while (Time.time - thoigianbatdau < thoigiandichuyen)
        {
            
            float lerpTime = (Time.time - thoigianbatdau) / thoigiandichuyen;

           
            Blow.transform.position = Vector3.MoveTowards(startPos, endPos, lerpTime);

            yield return null;

        }


        while (Time.time - thoigianbatdau < thoigiandichuyen * 2)
        {
            float lerpTime = (Time.time - thoigianbatdau - thoigiandichuyen) / thoigiandichuyen;

            Blow.transform.position = Vector3.Lerp(endPos, startPos, lerpTime);

            yield return null;
            Blow.SetActive(false);
        }

        Blow.SetActive(false);
    }

    // Start is called before the first frame update
    async void Start()
    {
   

        dice.SetActive(false);
       
        websocket = new WebSocket("ws://localhost:8080/");

        websocket.OnOpen += () =>
        {
            Debug.Log("Connection open!"  );
            
        };

        websocket.OnError += (e) =>
        {
            Debug.Log("Error! " + e);
        };

        websocket.OnClose += (e) =>
        {
            Debug.Log("Connection closed!");
            //if (isConnectClose)
            //{
            //    Reconnect();
            //}

        };


        websocket.OnMessage += (bytes) =>
        {
        
            Debug.Log("OnMessage!");
            Debug.Log(bytes);

            // getting the message as a string
             var message = System.Text.Encoding.UTF8.GetString(bytes);
            //Debug.Log("OnMessage!-SERVER: " + message);

            RoundData roundData = JsonUtility.FromJson<RoundData>(message);
            if (roundData.countdown > 0)
            {
                txtCountDown.GetComponent<Text>().text = roundData.countdown.ToString();
            }
            else {

                txtSmall.GetComponent<Text>().text = roundData.small_money.ToString();
                txtBig.GetComponent<Text>().text = roundData.big_money.ToString();
                txtTime.GetComponent<Text>().text = (--roundData.counter).ToString("00");
                txt_id.GetComponent<Text>().text = "#" + roundData._id.ToString();
                //txtResult.GetComponent<Text>().text = roundData.result.ToString();



                SpriteRenderer diceImage = dice.GetComponent<SpriteRenderer>();

                if (roundData.counter == 10)
                {
                    dice.SetActive(true);
                    txtTime.enabled = false;
                    txtCountDown.enabled = true;

                    int diceResult = roundData.dice; 
                    Sprite diceSprite = spriteAtlas.GetSprite("dice-" + diceResult);
                    diceImage.sprite = diceSprite;


                    StartCoroutine(ShowResultCoroutine());
                }
                else
                {
                    txtTime.enabled=true;
                    dice.SetActive(false);
                    txtCountDown.enabled = false;
                    Blow.SetActive(true);
                   
                }


                if (roundData.result == 0)
                {


                    animatorXiu.SetTrigger("Animation_connect_Xiu");

                }
                else if (roundData.result == 1)
                {
                    animatorTai.SetTrigger("Animation_connect_Tai");
                }

            }





        };

        // Keep sending messages at every 0.3s
        //InvokeRepeating("SendWebSocketMessage", 0.0f, 5.0f);

        // waiting for messages
        await websocket.Connect();
        
       

    }
    

    // Update is called once per frame
    void Update()
    {
            #if !UNITY_WEBGL || UNITY_EDITOR
                    websocket.DispatchMessageQueue();

            #endif
       

    }
    //async void SendWebSocketMessage()
    //{
    //    if (websocket.State == WebSocketState.Open)
    //    {
    //        // Sending bytes
    //        //await websocket.Send(new byte[] { 10, 20, 30 });

    //        // Sending plain text
    //        await websocket.SendText("Xin chào Tôi là Unity");
    //    }
    //}
    private async void OnApplicationQuit()
    {
        await websocket.Close();
    }

    //async void Reconnect()
    //{
    //    await Task.Delay(5000); // Delay for 5 seconds before reconnecting
    //    await websocket.Connect();
    //}


    [System.Serializable]
    public class RoundData
    {
        public string _id;
        public int small_money;
        public int big_money;
        public int counter;
        public int result;
        public int dice;
        public int countdown;
       
    }





}
