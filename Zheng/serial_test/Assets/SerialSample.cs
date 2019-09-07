using UnityEngine;
using System.IO.Ports;



public class SerialSample : MonoBehaviour
{
    SerialPort serialPort;
    public int x;
    void Start()
    {
        x = 0;
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
        Debug.Log(serialPort.ReadLine());

    }

   

}