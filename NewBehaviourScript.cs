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


public class NewBehaviourScript : MonoBehaviour 
{
   
    WebSocket websocket;
    //bool isConnectClose = true;
    public Text txtSmall;
    public Text txtBig;
    public Text txtTime;
    public Text txt_id;
    //public Text txtResult;
    public GameObject dice;


    public Animator animatorXiu;
    public Animator animatorTai;






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
            txtSmall.GetComponent<Text>().text = roundData.small_money.ToString();
            txtBig.GetComponent<Text>().text = roundData.big_money.ToString();
            txtTime.GetComponent<Text>().text = (--roundData.counter).ToString("00");
            txt_id.GetComponent<Text>().text = "#" + roundData._id.ToString();
            //txtResult.GetComponent<Text>().text = roundData.result.ToString();
            

            if (roundData.counter == 10)
            {

                dice.SetActive(true);
            }
            else
            {
                dice.SetActive(false);
            }

            if (roundData.result == 0) {


                animatorXiu.SetTrigger("Animation_connect_Xiu");

            }else if (roundData.result == 1){
                animatorTai.SetTrigger("Animation_connect_Tai");
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
    }





}
