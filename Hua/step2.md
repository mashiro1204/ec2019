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

![回路](https://github.com/mashiro1204/ec2019/blob/master/Hua/screenshot/1.png)

- 使うモジュール
model:DBH-12v
![接続](..\Hua\screenshot\6.jpg)
![接続](..\Hua\screenshot\5.png)

#### 1.mbedからpwm信号の出力ーー成功
#### 2.pwn信号をDBH-12vに入れて，ペルチェ素子を駆動させるーー成功

![駆動](..\Hua\screenshot\7.jpg)
mbed側のソースコード
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
参考：https://www.electronicwings.com/mbed/thermistor-interfacing-with-arm-mbed
- 使うサーミスタ
https://docs-apac.rs-online.com/webdocs/162b/0900766b8162bf78.pdf

mbed側のソースコード
```C#
#include "mbed.h"
AnalogIn thr(p15);

int main() {
    float thr_val=0, thr_res=0, temperature=0;
    while(1) {
        thr_val = (thr.read()*4095);
        thr_val = ((thr_val*3.3)/4095); //voltage of p15
        thr_res = log(((3.3*(1/thr_val))-1)*1000);
        temperature = ((1/(0.001129148+(0.000234125*thr_res)+(0.0000000876741*thr_res*thr_res*thr_res)))-273.15);
        printf("Temperature = %3.3f\r\n", temperature);
        printf("Resistance = %3.3f\r\n", thr_res);
        wait(1);
    }
}
```
今回使うサーミスタの温度と抵抗の関係を調べる必要がある
- 使うツール



mbed-thermistor
https://www.electronicwings.com/mbed/thermistor-interfacing-with-arm-mbed



## 参考資料
- [LM358](http://www.ti.com/lit/ds/symlink/lm358-n.pdf)
- [ペルチェ素子](https://www.yasuda-elec.com/service/peltier.html)