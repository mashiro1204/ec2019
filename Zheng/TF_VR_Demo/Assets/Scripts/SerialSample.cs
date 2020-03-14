using UnityEngine;
using System.IO.Ports;



public class SerialSample : MonoBehaviour
{
    SerialPort serialPort;
    private float tempNow;
    void Start()
    {
        tempNow = 0f;

        serialPort = new SerialPort("COM1", 9600);
        serialPort.Open();

    }

    void Update()
    {

        //　nを文字列で判断
        if (Input.GetKey("n"))
        {
            //Debug.Log("n");
            serialPort.Write("n");
        }
        //　mを文字列で判断
        if (Input.GetKey("m"))
        {
            //Debug.Log("m");
            serialPort.Write("m");
        }
        //　bを文字列で判断
        if (Input.GetKey("b"))
        {
            //Debug.Log("b");
            serialPort.Write("b");
        }

        tempNow = float.Parse (serialPort.ReadLine());
        Debug.Log(tempNow);

    }

    public void WriteToMbed(string message)
    {
        serialPort.Write(message);
    }

    public float GetTempFromMbed()
    {
        return tempNow;
    }
}