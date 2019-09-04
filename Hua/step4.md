# step 4 unity连携 

#### unityからserial portへ
[これ](https://teratail.com/questions/198853)を参考にする

[UnityでC#によりCOMポートからのデータ入力](https://tomosoft.jp/design/?p=7256)

SerialPort クラスの使用設定
メインメニューから「Edit」→「Project Settings」→「Player」を選択し、次のようにInspectorタブの API Compatibility Level を .NET 2.0  から .NET 4.0 へと変更すると、System.IO.Ports.SerialPort クラスが扱えるようになります。ただし、delegateやSerialDataReceivedEventHandlerの使用には制限（使用できない？）があります。



#### serial portからmbedへ
[これ]()を参考にする

http://taka8.hateblo.jp/entry/2016/08/29/190112

##### 9/4結果
errorはないが，consoleに何の表示も出てこない

NET4.0のせいか？　今疑問を持っている

https://qiita.com/Fox_Kei/items/dbe10141e36f6a91ee83