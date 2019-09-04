using UnityEngine;
using System.IO.Ports;


 
public class SerialHandler : MonoBehaviour
{
    SerialPort serialPort;

    void Start()
    {

        serialPort = new SerialPort("COM1", 9600);
        serialPort.Open();

    }

    void OnGUI()
    {

        GUILayout.Label(serialPort.ReadLine());

    }

}