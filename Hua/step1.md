# step 1 单个peltier驱动电路（无serial 通信）


### ペルチェ素子について(https://www.yasuda-elec.com/service/peltier.html)

> ペルチェ素子は熱が移動するペルチェ効果という現象を利用して、片面を冷却、他面を加熱する素子です。ペルチェ効果とは２種類の金属の接合部に電流を流すと片方から他方に熱が流れることです。

> ペルチェ素子は２種類の金属を接合した物なので抵抗値が小さく、低電圧・大電流の駆動回路が必要です。

> 安田電子設計事務所では PWM駆動の DC-DCコンバータを使って高効率な駆動回路を設計することができます。また、マイコン制御による DC-DCコンバータもできますので、製造コストを抑えた電子回路が設計できます。

> 冷却部または加熱部に温度センサーを付けて温度制御することも可能です。

> ペルチェ素子を加熱に使うことがありますが、それは効率がよくて少ない電力で加熱が可能だからです。エアコンの暖房がヒーターより高効率なのと一緒ですね。



- [今回使うペルチェ素子](https://www.amazon.co.jp/Vktech-TEC1-12706-%E5%8D%8A%E5%B0%8E%E4%BD%93%E7%86%B1%E9%9B%BB-%E3%82%BF%E3%83%96%E3%83%AC%E3%83%83%E3%83%88-6A-%EF%BC%91%EF%BC%90%E6%9E%9A%E3%82%BB%E3%83%83%E3%83%88/dp/B01CTC9CGE/ref=sr_1_fkmr0_2?__mk_ja_JP=%E3%82%AB%E3%82%BF%E3%82%AB%E3%83%8A&keywords=%E3%83%9A%E3%83%AB%E3%83%81%E3%82%A7+%E7%84%A1%E7%B7%9A&qid=1564113786&s=books&sr=8-2-fkmr0)


仕様
> 動作電圧および電流：電流 4.5A （標準） 5.8A (最大) 　電圧 12V（標準） 15V（最大）



### version1 定電流回路

- http://www.setsunan.ac.jp/~shikama/GraduationStudyAbstract/11EOSL/summary_kanamaru.pdf
- [lm358を使う定電流回路](http://www3.airnet.ne.jp/saka/hardware/electric_current/elec_cur01.html)　ただし，これはNPN transistorを使う



手持ちのトランスジスタは[A1413 PNP transistor](https://datasheetspdf.com/pdf-file/526418/NEC/A1413/1)なので，[これ](http://www.nahitech.com/nahitafu/mame/mame3/teid1.html)を参考にする

#### 動作結果
失敗　
負荷の両側に電圧がずっと0.5Vでした．
#### 原因
Vccの設定が問題あるのでは？


---
### version2 定電流回路
レファレンスを使って改良　https://www.analog.com/media/jp/technical-documentation/Shoshinsha_OP_amp/MSJ-004_jp.pdf

#### 動作結果
0.2Aの定電流を設計しましたが，R1両側の電圧が0.5Vのままで0.05Aの定電流ができました．

0.5Vという電圧ははたしてversion1の0.5Vと同じでは？

#### 修正
ampの+側はR1に繋がらないようにしました
#### 動作結果
成功
- 使う回路
![amp+pnp回路](..\Hua\screenshot\2.jpg)
---
### version3 負荷駆動回路(カレントミラー回路)
- [これ](http://www3.airnet.ne.jp/saka/hardware/electric_current/elec_cur01.html)と[これ](https://ameblo.jp/sh1n00n/entry-12481929732.html)を参考にする




## 参考資料
- [ペルチェ素子](https://www.yasuda-elec.com/service/peltier.html)
- [lm358を使う定電流回路](http://www3.airnet.ne.jp/saka/hardware/electric_current/elec_cur01.html)
- [PNPtransistorを使う定電流回路](http://www.nahitech.com/nahitafu/mame/mame3/teid1.html)