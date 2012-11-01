# そろばん読み取りライブラリ
そろばんを読み取るライブラリです．

このソリューションには以下の3つのプロジェクトが同梱されています．

* SorobanCaptureLib - そろばん読み取りライブラリ本体
	* そろばんの読み取りに関するクラスがあります
* SorobanCalibrator - テンプレート画像作成プログラム
	* そろばんの読み取りに必要な学習画像を作成するプログラムです
* SorobanTest - 読み取りサンプル
	* 読み取りの動作サンプルです

## 必要なライブラリ等
* OpenCV2.2
	* <http://sourceforge.net/projects/opencvlibrary/files/opencv-win/2.2/>
		1. OpenCV-2.2.0-win.zip をダウンロード
		2. C:直下などに展開
		3. binに対してPATHを通しておく

## SorobanCaptureLibについて
そろばんの読み取りに関するライブラリです．

以下のクラスがあります．

* CaptureCamera
	* カメラに関するクラス
	* DirectShowを使ってWebカメラから画像をキャプチャします
* CaptureImage
	* CaptureCameraでキャプチャされた画像を扱うクラス
	* OpenCVのIplImageやXNAのTexture2Dへの変換メソッドが提供されています
* SorobanReader
	* 画像からそろばんの目を読み取るクラスです
	* キャプチャ画像(IplImage)を受け取り，列ごとの領域分割をして画像認識を行います．
	* 分割数や処理画像サイズなどの多くの定数がこのクラスで宣言されています．
* SorobanRealtimeReader
	* リアルタイムにそろばんの目を読み取るクラス
	* 別スレッドでWebカメラからリアルタイムキャプチャ→画像認識
	* 別スレッドで走らせているため，XNA側へ動作の影響を与えません
	* 読み取り結果はResulutsのGetterメソッドでアクセスします．

XNA側からの呼び出しについては，SorobanTestのサンプルを参考にしてください．
主に使うのはSorobanRealtimeReaderになります．

## 注意

実行にはSorobanCalibratorで作成されたtemplatesディレクトリを中身ごとSorobanTestのbinディレクトリにコピーする必要があります．
