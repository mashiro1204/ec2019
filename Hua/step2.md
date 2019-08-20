# step 2 单个peltier驱动电路(serial 通信)  

### version1
mbed出力+ ボルテージフォロア

- 使うamp
lm358(http://www.ti.com/lit/ds/symlink/lm358-n.pdf)

- 使う回路
![ボルテージフォロア回路](..\Hua\screenshot\1.png)



#### 結果
失敗　
ペルチェ素子の両側に電圧がほぼ0Vでした．
#### 原因
ペルチェ素子の抵抗値が小さくてボルテージフォロア回路を使ってもインピーダンスマッチングができないようですね．
> ペルチェ素子は２種類の金属を接合した物なので抵抗値が小さく、低電圧・大電流の駆動回路が必要です。





### version2





## 参考資料
- LM358  http://www.ti.com/lit/ds/symlink/lm358-n.pdf
- ペルチェ素子  https://www.yasuda-elec.com/service/peltier.html