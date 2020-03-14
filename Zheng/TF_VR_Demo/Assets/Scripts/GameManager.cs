using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;
    public GameObject cameraCroshair;
    public OVRInput.Button shootingButton;
    public OVRInput.Button aButton;
    public OVRInput.Button bButton;
    public OVRInput.Controller rightController;
    public RightHand rightHand;
    public SerialSample serialPort;
    public Text HUDtext;
    public Image HUDimage;
    public GameObject HUDcanvas;
    public AudioSource bgm;
    public bool isCoolingDown = false;

    private int level = -1;
    private int leftEnemiesCount;
    private float rightTemp;
    private float iceTemp;
    private float coolDownTime = 5.0f;

    void Start()
    {
        // if (instance == null)
        //     instance = this;
        // else if (instance != null)
        //     Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
        InitGame();
    }

    void InitGame()
    {
        cameraCroshair.SetActive(true);
        rightTemp = 38.0f;
        iceTemp = 33.0f;
        serialPort.WriteToMbed("b");
        isCoolingDown = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey("1"))
        {
            serialPort.WriteToMbed("z");
            rightTemp = 0f;
        }

        if (Input.GetKey("2"))
        {
            serialPort.WriteToMbed("x");
            rightTemp = 47f;
        }

        if (Input.GetKey("3"))
        {
            serialPort.WriteToMbed("c");
            rightTemp = 49f;
        }

        if (Input.GetKey("4"))
        {
            serialPort.WriteToMbed("v");
            rightTemp = 51f;
        }

        if (Input.GetKey("5"))
        {
            serialPort.WriteToMbed("b");
            rightTemp = 53f;
        }
        
        if(OVRInput.GetUp(shootingButton, rightController))
        {
            rightHand.Shoot(serialPort.GetTempFromMbed());
            SendSerialMessage("z");
        }

        if(OVRInput.GetDown(shootingButton, rightController))
        {
            Debug.Log("fsgfdgfdj");
            rightHand.Cast();
        }

        if(OVRInput.GetDown(aButton, rightController))
        {
            rightHand.ModeSwitch();
        }
    }

    public void SendSerialMessage(string message)
    {
         serialPort.WriteToMbed(message);
    }

    public void GameOver()
    {
        HUDtext.text = "Game Over";
        Color winColor = Color.black;
        winColor.a = 0.8f;
        HUDimage.color = winColor;
        HUDcanvas.SetActive(true);
    }

    public void GameClear()
    {
        HUDtext.text = "Congratulations!";
        Color winColor = Color.white;
        winColor.a = 0.8f;
        HUDimage.color = winColor;
        HUDcanvas.SetActive(true);
    }

    public int GetLevel()
    {
        return level;
    }

    public void SetLeftEnemiesCount(int _leftEnemiesCount)
    {
        leftEnemiesCount =  _leftEnemiesCount;
        if(leftEnemiesCount == 0) 
        {
            Invoke("GameClear", 2.0f);
        }
    }

    public int GetLeftEnemiesCount()
    {
        return leftEnemiesCount;
    }

    public float GetRightTemp()
    {
        return rightTemp;
    }

    public float GetIceTemp()
    {
        return iceTemp;
    }

    public void CoolDown()
    {
        isCoolingDown = true;
        serialPort.WriteToMbed("z");
        Invoke("WarmUp", coolDownTime);
    }

    void WarmUp()
    {
        isCoolingDown = false;

        switch(rightTemp)
        {
            case 47.0f:
                serialPort.WriteToMbed("x");
                break;
            case 49.0f:
                serialPort.WriteToMbed("c");
                break;
            case 51.0f:
                serialPort.WriteToMbed("v");
                break;
            case 53.0f:
                serialPort.WriteToMbed("b");
                break;
        }
    }
}
