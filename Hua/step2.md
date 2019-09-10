# step 2 单个peltier驱动电路(serial 通信)  

### version1
mbed出力+ ボルテージフォロア

- [今回使うペルチェ素子](https://www.amazon.co.jp/Vktech-TEC1-12706-%E5%8D%8A%E5%B0%8E%E4%BD%93%E7%86%B1%E9%9B%BB-%E3%82%BF%E3%83%96%E3%83%AC%E3%83%83%E3%83%88-6A-%EF%BC%91%EF%BC%90%E6%9E%9A%E3%82%BB%E3%83%83%E3%83%88/dp/B01CTC9CGE/ref=sr_1_fkmr0_2?__mk_ja_JP=%E3%82%AB%E3%82%BF%E3%82%AB%E3%83%8A&keywords=%E3%83%9A%E3%83%AB%E3%83%81%E3%82%A7+%E7%84%A1%E7%B7%9A&qid=1564113786&s=books&sr=8-2-fkmr0)


仕様
> 動作電圧および電流：電流 4.5A （標準） 5.8A (最大) 　電圧 12V（標準） 15V（最大）
- 使うamp
lm358(http://www.ti.com/lit/ds/symlink/lm358-n.pdf)

- 使う回路
![ボルテージフォロア回路](..\Hua\screenshot\1.png)


#### 動作結果
失敗　
ペルチェ素子の両側に電圧がほぼ0Vでした．
#### 原因
ペルチェ素子の抵抗値が小さくてボルテージフォロア回路を使ってもインピーダンスマッチングができないようですね．
> ペルチェ素子は２種類の金属を接合した物なので抵抗値が小さく、低電圧・大電流の駆動回路が必要です。





### version2
先生と相談した結果，PWM駆動による
- 使う回路
![回路](..\Hua\screenshot\4.jpg)

- 使うモジュール
model:DBH-12v
![接続](..\Hua\screenshot\6.jpg)
![接続](..\Hua\screenshot\5.png)

#### 1.mbedからpwm信号の出力ーー成功
#### 2.pwn信号をDBH-12vに入れて，ペルチェ素子を駆動させるーー成功

![駆動](..\Hua\screenshot\7.jpg)
- mbed側のソースコード
```C#
#include "mbed.h"
 
PwmOut thermistor(p21);
 
int main() {
    // specify period first, then everything else
    thermistor.period(4.0f);  // 4 second period
    thermistor.pulsewidth(2); // 2 second pulse (on)
    while(1);          // led flashing
}
```

#### 3.サーミスタから温度データをmbedへ--成功
- 参考
https://www.electronicwings.com/mbed/thermistor-interfacing-with-arm-mbed
- 使うサーミスタ
https://docs-apac.rs-online.com/webdocs/162b/0900766b8162bf78.pdf

- mbed側のソースコード
```C#
#include "mbed.h"
AnalogIn thr(p15);

int main() {
    float thr_val=0, thr_res=0, temperature=0;
    while(1) {
        thr_val = (thr.read()*4095);
        thr_val = ((thr_val*3.3)/4095); //voltage of p15
        thr_res = log(((3.3*(1/thr_val))-1)*1000);
        //temperature = ((1/(0.001129148+(0.000234125*thr_res)+(0.0000000876741*thr_res*thr_res*thr_res)))-273.15);
        temperature = 700*thr_res-675;
        printf("Temperature = %3.3f\r\n", temperature);
        printf("Resistance = %3.3f\r\n", thr_res);
        wait(1);
    }
}
```

- 今回使うサーミスタの温度と抵抗の関係を調べる必要がある
- 調べた温度と抵抗の関係
![温度と抵抗の関係](..\Hua\screenshot\8.png)

- 温度と抵抗の関係を調べつためのコード
```python

import matplotlib.pyplot as plt
from pylab import mpl
"""
https://blog.csdn.net/deramer1/article/details/79055281
"""
 
x = [0.980,1.000,1.021,1.042,1.063,1.084,1.106]
y = [20,25,30,35,40,45,50]
 
 
"""完成拟合曲线参数计算"""
def liner_fitting(data_x,data_y):
      size = len(data_x)
      i=0
      sum_xy=0
      sum_y=0
      sum_x=0
      sum_sqare_x=0
      average_x=0
      average_y=0
      while i<size:
          sum_xy+=data_x[i]*data_y[i]
          sum_y+=data_y[i]
          sum_x+=data_x[i]
          sum_sqare_x+=data_x[i]*data_x[i]
          i+=1
      average_x=sum_x/size
      average_y=sum_y/size
      return_k=(size*sum_xy-sum_x*sum_y)/(size*sum_sqare_x-sum_x*sum_x)
      return_b=average_y-average_x*return_k
      print(return_k,return_b)
      return [return_k,return_b]
 
 
"""完成完后曲线上相应的函数值的计算"""
def calculate(data_x,k,b):
    datay=[]
    for x in data_x:
        datay.append(k*x+b)
    return datay
 
 
"""完成函数的绘制"""
def draw(data_x,data_y_new,data_y_old):
    plt.plot(data_x,data_y_new,label="curve",color="black")
    plt.scatter(data_x,data_y_old,label="data")
    mpl.rcParams['font.sans-serif'] = ['SimHei']
    mpl.rcParams['axes.unicode_minus'] = False
    plt.title("one dimension ")
    plt.legend(loc="upper left")
    plt.show()
 
 
parameter = liner_fitting(x,y)
draw_data = calculate(x,parameter[0],parameter[1])

draw(x,draw_data,y)
```
計算結果：
k=238.06769543990123
b=-213.13455798993132
T=k*Rth+b

- 9/3更新したmbed側のソースコード
```C#
#include "mbed.h"
AnalogIn thr(p15);

int main() {
    float thr_val=0, thr_res=0, temperature=0;
    while(1) {
        thr_val = (thr.read()*4095);
        thr_val = ((thr_val*3.3)/4095); //voltage of p15
        //thr_res = log(((3.3*(1/thr_val))-1)*1000);
        //temperature = ((1/(0.001129148+(0.000234125*thr_res)+(0.0000000876741*thr_res*thr_res*thr_res)))-273.15);
        thr_res = 3.3*1/thr_val-1;
        temperature = 238.07*thr_res-213.13;
        //printf("voltage = %3.3f\r\n", thr_val);
        printf("Temperature = %3.3f\r\n", temperature);
        printf("Resistance = %3.3f\r\n", thr_res);
        wait(1);
    }
}
```
- mbedからパソコンへの入力
![温度をmbedに入力する](..\Hua\screenshot\9.png)



- 使うツール
mbed-thermistor
https://www.electronicwings.com/mbed/thermistor-interfacing-with-arm-mbed


#### 4.設定した温度になるようにFeedback
- 実装図
![mbed側](..\Hua\screenshot\14.jpg)


##### version 1　温度とpwn出力時間の対応関係の設定
32-40度
32度　on時間ーー100%
40度　on時間ーー0%

y=kx+b(y=%,x=温度)
k=-12.5
b=400

mbed側のソースコード
```C#
#include "mbed.h"
AnalogIn thr(p15);
PwmOut thermistor(p21);
int main() {
    float thr_val=0, thr_res=0, temperature=0;
    while(1) {
        //thermistor
        thr_val = (thr.read()*4095);
        thr_val = ((thr_val*3.3)/4095); //voltage of p21
        thr_res = 3.3*1/thr_val-1;
        temperature = 238.07*thr_res-213.13;
        printf("Temperature = %3.3f\r\n", temperature);
        printf("Resistance = %3.3f\r\n", thr_res);
        // pwn output
        thermistor.period(2.0f);  // 2 second period
        thermistor.pulsewidth((-12.5*temperature+400)/50); // setted second pulse (on)
        wait(2);
    }
}
```
##### 結果
サーミスタが28度のまま維持されていた
pwm出力が出されていないと判断した

##### version 2　まずはペルチェ素子を駆動させながら温度を計測できるコードーー成功
mbed側のソースコード
```C#
#include "mbed.h"
AnalogIn thr(p15);
PwmOut thermistor(p21);


int main() {
    // specify period first, then everything else
    thermistor.period(4.0f);  // 4 second period
    thermistor.pulsewidth(2); // 2 second pulse (on)
    //while(1);          // led flashing
    float thr_val=0, thr_res=0, temperature=0;
    while(1) {
        thr_val = (thr.read()*4095);
        thr_val = ((thr_val*3.3)/4095); //voltage of p15
        //thr_res = log(((3.3*(1/thr_val))-1)*1000);
        //temperature = ((1/(0.001129148+(0.000234125*thr_res)+(0.0000000876741*thr_res*thr_res*thr_res)))-273.15);
        thr_res = 3.3*1/thr_val-1;
        temperature = 238.07*thr_res-213.13;
        //printf("voltage = %3.3f\r\n", thr_val);
        printf("Temperature = %3.3f\r\n", temperature);
        //printf("Resistance = %3.3f\r\n", thr_res);
        wait(1);
    }
}
```

##### version 3　version2に基づいてフィードバックを加えてみるーー成功
[これ](https://teratail.com/questions/21694)を参考にした
mbed側のソースコード
```C#
#include "mbed.h"
AnalogIn thr(p15);
PwmOut thermistor(p21);


int main() {
    float thr_val=0, thr_res=0, temperature=0;
    int T=40,p,start=1,min=0,max=3;
    thermistor.period(4.0f);  // 4 second period
    //thermistor.pulsewidth(2); // 2 second pulse (on)
    p = start;                          //初期
    thermistor.pulsewidth_us(p);    
    
    while(1) {
        thr_val = (thr.read()*4095);
        thr_val = ((thr_val*3.3)/4095); //voltage of p15
        thr_res = 3.3*1/thr_val-1;
        temperature = 238.07*thr_res-213.13;
        printf("Temperature = %3.3f\r\n", temperature);
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
![温度が40度に維持されている](..\Hua\screenshot\10.png)




#### 5.unityから温度指令を出す


## 参考資料
- [LM358](http://www.ti.com/lit/ds/symlink/lm358-n.pdf)
- [ペルチェ素子](https://www.yasuda-elec.com/service/peltier.html)
- [mbed-thermistor](https://www.electronicwings.com/mbed/thermistor-interfacing-with-arm-mbed)
- [python最小二乗法](https://blog.csdn.net/deramer1/article/details/79055281)
