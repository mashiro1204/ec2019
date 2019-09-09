# step 5 pmwのマイナス電流出力   

![接続](..\Hua\screenshot\6.jpg)
![接続](..\Hua\screenshot\5.png)

冷却--問題ありそう
- mbed側のソースコード
```C#
#include "mbed.h"
AnalogOut  aout(p18);
AnalogIn thr(p15);
PwmOut thermistor(p21);
int main() {
    float thr_val=0, thr_res=0, temperature=0;
    int T=25,p,start=1,min=0,max=3;
    thermistor.period(4.0f);  // 4 second period
    //thermistor.pulsewidth(2); // 2 second pulse (on)
    p = start;                          //初期
    thermistor.pulsewidth_us(p);    
    
    while(1) {
        aout = 1.0f;
        thr_val = (thr.read()*4095);
        thr_val = ((thr_val*3.3)/4095); //voltage of p15
        thr_res = 3.3*1/thr_val-1;
        temperature = 238.07*thr_res-213.13;
        printf("%3.3f\r\n", temperature);
        //printf("Temperature = %3.3f\r\n", temperature);
        //printf("Resistance = %3.3f\r\n", thr_res);
        //wait(1);
        if(temperature == T){                  //温度がTなら初期位置に
            p = start;
            thermistor.pulsewidth(p);
       }else if(temperature >T){             //T度以上はminで固定
            p = min;
            thermistor.pulsewidth(p);
       }else if(temperature <T){            //T度以下はmaxで固定
            p = max;
            thermistor.pulsewidth(p);
       }
        wait(1);
    }
}
```

分析：
逆電位で冷却させることが難しい