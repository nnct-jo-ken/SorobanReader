using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OpenCvSharp;

namespace SorobanCaptureLib
{
    /// <summary>
    /// そろばんをリアルタイムに読み取るクラス
    /// キャプチャ，画像認識などは別スレッドで行う
    /// </summary>
    public class SorobanRealtimeReader
    {
        private int[] results;
        private IplImage process_img;
        private CaptureImage capture_img;
        private bool isReading;
        private double threshold;
        private CaptureCamera camera;
        private SorobanReader reader;
        private Thread thread;
        private CvWindow process_window;

        /// <summary>
        /// 読み取り結果
        /// </summary>
        public int[] Results
        {
            get { return results; }
        }

        /// <summary>
        /// 処理画像
        /// </summary>
        public IplImage Process_img
        {
            get { return process_img; }
        }
        
        /// <summary>
        /// カメラで撮影された画像
        /// </summary>
        public CaptureImage Capture_img
        {
            get { return capture_img; }
        }
        /// <summary>
        /// スレッドの動作状態
        /// </summary>
        public bool IsReading
        {
            get { return isReading; }
        }


        /// <summary>
        /// リーダー初期設定
        /// </summary>
        /// <param name="threshold">閾値</param>
        /// <param name="template_sign">テンプレート画像のシグネチャ</param>
        private void initialize(double threshold, string template_sign)
        {
            this.threshold = threshold;
            this.reader = new SorobanReader(template_sign);
            this.results = new int[SorobanReader.DIVIDE_NUM];
            this.isReading = false;
            this.capture_img = new CaptureImage(new System.Drawing.Bitmap(320, 240));
            camera.Activate(320, 240);
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="camera_index">使用するカメラ：CaptureCamera.GetDevices()で取得されたカメラデバイスのインデックス</param>
        /// <param name="div">読み取る列数</param>
        /// <param name="threshold">しきい値（読み取り精度）</param>
        /// <param name="template_sign">テンプレート画像のシグネチャ</param>
        public SorobanRealtimeReader(int camera_index, double threshold, string template_sign)
        {
            this.camera = new CaptureCamera(camera_index);
            initialize(threshold, template_sign);
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="camera_name">使用するカメラデバイス名</param>
        /// <param name="div">読み取る列数</param>
        /// <param name="threshold">しきい値（読み取り精度）</param>
        /// <param name="template_sign">テンプレート画像のシグネチャ</param>
        public SorobanRealtimeReader(String camera_name, double threshold, string template_sign)
        {
            this.camera = new CaptureCamera(camera_name);
            initialize(threshold, template_sign);
        }
        /// <summary>
        /// そろばんの読み取りを開始する
        /// </summary>
        public void Start()
        {
            if (thread == null)
            {
                process_window = new CvWindow();
                thread = new Thread(new ThreadStart(this.Update));
                thread.Start();
                this.isReading = true;
            }
        }

        /// <summary>
        /// そろばんの読み取りを停止する
        /// </summary>
        public void Stop()
        {
            if (thread != null)
            {
                thread.Abort();
                thread = null;
                this.isReading = false;
                process_window.Close();
                process_window = null;
            }
        }

        /// <summary>
        /// カメラから画像を取得しそろばんを読み取る
        /// </summary>
        private void Update()
        {
            while (true)
            {
                CaptureImage cap;
                IplImage img;
                cap = this.camera.Capture();
                img = cap.ToIplImage();

                lock (this.capture_img)
                lock (this.results)
                {
                    this.capture_img = cap;
                    this.results = reader.AllMatching(img, threshold, out process_img);

                }
                process_window.ShowImage(Process_img);
                Cv.WaitKey(2);
                Thread.Sleep(10);
            }
        }
        
    }
}
