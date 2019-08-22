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

## 参考資料
- [LM358](http://www.ti.com/lit/ds/symlink/lm358-n.pdf)
- [ペルチェ素子](https://www.yasuda-elec.com/service/peltier.html)