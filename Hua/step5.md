# step 5 多チャネル   

ペルチェ素子　最大電圧：5.6V／最大電流：8A

9/9結果
同じ出力の2チャネルを出してみたが，channel Bしか動かない
分析：
パワー？
接続の違い？

9/10結果
1. 二つのチャネルを別々のモーターで駆動してみて，うまくできた
![mbed側](..\Hua\screenshot\15.png)

2. 元々のモジュールを使って，電源を最大電流4Aと設定したらうまく動作できるようになった

2チャネル温度制御ソースコード
```
#include "mbed.h"
PwmOut p1(p21),p2(p23);
AnalogIn thr(p15);
Serial pc(USBTX, USBRX); // tx, rx
 
int main() {
    float thr_val=0, thr_res=0, temperature=0;
    int T,T1=32,T2=35,T3=40,p,start=1,min=0,max=3;
    p1.period(4.0f);  // 4 second period
    p2.period(4.0f);
    //thermistor.pulsewidth(2); // 2 second pulse (on)
    p = start;                          //初期
    p1.pulsewidth(2);    
    p2.pulsewidth(2);    
    
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
            p1.pulsewidth(p);
            p2.pulsewidth(p);
       }else if(temperature > T){             //T度以上はminで固定
            p = min;
            p1.pulsewidth(p);
            p2.pulsewidth(p);
       }else if(temperature < T){            //T度以下はmaxで固定
            p = max;
            p1.pulsewidth(p);
            p2.pulsewidth(p);
       }
        wait(1);
    }
}
```


9/13 4CH実装ーー成功，但しslave driver Achの方がギリギリ出せる．
```
#include "mbed.h"
PwmOut p1(p21),p2(p22),p3(p23),p4(p24);
AnalogIn thr(p15);
Serial pc(USBTX, USBRX); // tx, rx
 
int main() {
    float thr_val=0, thr_res=0, temperature=0;
    int T,T1=32,T2=40,p,start=1,min=0,max=3;
    p1.period(4.0f);  p2.period(4.0f);p3.period(4.0f);p3.period(4.0f);// 4 second period
    p = start;                          //初期
    p1.pulsewidth(2);p2.pulsewidth(2);p3.pulsewidth(2);p4.pulsewidth(2);             
     
    T = T1;
    while(1) {
        thr_val = (thr.read()*4095);
        thr_val = ((thr_val*3.3)/4095); //voltage of p15
        thr_res = 3.3*1/thr_val-1;
        temperature = 238.07*thr_res-213.13;
        printf("now=%3.1f\r\n", temperature);
        
        if (pc.readable()){
            char c = pc.getc();
            if (c == 'm'){
                T = T2;
            }else if (c =='b'){
                T = T1;
            }
        }
        printf("goal=%d\r\n", T);
        
        if(temperature == T){                  //温度がTなら初期位置に
            p = start;
       }else if(temperature > T){             //T度以上はminで固定
            p = min;
       }else if(temperature < T-2){            //T度以下はmaxで固定
            p = max;
       }
        p1.pulsewidth(p);
        p2.pulsewidth(p);
        p3.pulsewidth(p);
        p4.pulsewidth(p);
        wait(1);
    }
}
```