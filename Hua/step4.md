# step 4 unity连携 

#### 1.unityからserial portへ
[これ](https://teratail.com/questions/198853)を参考にする

[UnityでC#によりCOMポートからのデータ入力](https://tomosoft.jp/design/?p=7256)

SerialPort クラスの使用設定
メインメニューから「Edit」→「Project Settings」→「Player」を選択し、次のようにInspectorタブの API Compatibility Level を .NET 2.0  から .NET 4.0 へと変更すると、System.IO.Ports.SerialPort クラスが扱えるようになります。ただし、delegateやSerialDataReceivedEventHandlerの使用には制限（使用できない？）があります。

[これ](https://www.shibuya24.info/entry/unity_arduino_serial)を参考にする

mbed側 pc.getc()
https://os.mbed.com/handbook/SerialPC
```
#include "mbed.h"
AnalogIn thr(p15);
PwmOut thermistor(p21);
Serial pc(USBTX, USBRX); // tx, rx

int main() {
    float thr_val=0, thr_res=0, temperature=0;
    int T,T1=32,T2=35,T3=45,p,start=1,min=0,max=3;
    thermistor.period(4.0f);  // 4 second period
    //thermistor.pulsewidth(2); // 2 second pulse (on)
    p = start;                          //初期
    thermistor.pulsewidth_us(p);    
    
    T = T1;
    while(1) {
        thr_val = (thr.read()*4095);
        thr_val = ((thr_val*3.3)/4095); //voltage of p15
        thr_res = 3.3*1/thr_val-1;
        temperature = 238.07*thr_res-213.13;
        printf("now=%3.3f\r\n", temperature);
        
        if (pc.readable()){
            char c = pc.getc();
            if (c == 'n'){
                T = T2;
            }else if (c == 'm'){
                T = T3;
            }else if (c =='b'){
                T = T1;
            }
        }
        printf("goal=%d\r\n", T);
        
        if(temperature == T){                  //温度がTなら初期位置に
            p = start;
            thermistor.pulsewidth(p);
       }else if(temperature > T){             //T度以上はminで固定
            p = min;
            thermistor.pulsewidth(p);
       }else if(temperature < T){            //T度以下はmaxで固定
            p = max;
            thermistor.pulsewidth(p);
       }
        wait(1);
    }
}
```
![mbed側](..\Hua\screenshot\12.png)






#### 2.serial portからmbedへーー成功
[これ](http://taka8.hateblo.jp/entry/2016/08/29/190112)を参考にする



##### 9/4結果
errorはないが，consoleに何の表示も出てこない

NET4.0のせいか？　今疑問を持っている

https://qiita.com/Fox_Kei/items/dbe10141e36f6a91ee83


##### 9/7結果ーー成功

```
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

    private void Update()
    {
        Debug.Log(serialPort.ReadLine());
    }

    void OnGUI()
    {

        GUILayout.Label(serialPort.ReadLine());

    }

}
```
![unityが受け取ったシリアルデータ](..\Hua\screenshot\11.png)


#### 3.mbedとunityの連携ーー成功
![unityが受け取ったシリアルデータ](..\Hua\screenshot\13.png)