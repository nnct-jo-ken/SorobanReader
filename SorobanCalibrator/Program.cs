using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenCvSharp;
using SorobanCaptureLib;
using System.Threading;

namespace SorobanCalibrator
{
    class Program
    {
        static IplImage captureImg;
        static CaptureCamera camera;
        static int ROW_MARGIN = 70;
        public static IplImage CaptureImg
        {
            get { return Program.captureImg; }
            set { Program.captureImg = value; }
        }

        static void Main(string[] args)
        {
            camera = new CaptureCamera(0);
            camera.Activate(320, 240);
            Thread.Sleep(500);
            
            //CaptureUpdate();
            Thread t = new Thread(new ThreadStart(CaptureUpdate));
            t.Start();
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine("please set {0} and press enter key", i);
                Console.ReadLine();
                lock (CaptureImg)
                {
                    // save
                    makeTemplates(CaptureImg, "1007", i);
                }
            }

            t.Abort();
        }

        static void CaptureUpdate()
        {
            CvWindow win1 = new CvWindow();
            captureImg = new IplImage(SorobanReader.PROCESS_SIZE, BitDepth.U8, 1);
            IplImage source;

            // グレースケール画像
            IplImage cap_gray = new IplImage(SorobanReader.PROCESS_SIZE, BitDepth.U8, 1);
            while (true)
            {
                CaptureImage img = camera.Capture();
                source = img.ToIplImage();
                //source = capture.QueryFrame();

                // キャプチャとリサイズ，グレースケール変換
                using (IplImage tmp = new IplImage(
                   SorobanReader.PROCESS_SIZE, source.Depth, source.NChannels))
                {
                    source.Resize(tmp);
                    tmp.CvtColor(cap_gray, ColorConversion.BgrToGray);
                }

                lock(CaptureImg)
                {
                    cap_gray.Copy(CaptureImg);
                }
                // 分割線の描画
                for (int i = 1; i < SorobanReader.DIVIDE_NUM + 2; i++)
                {
                    int width = cap_gray.Width / (SorobanReader.DIVIDE_NUM + 1);
                    int x = i * width - width / 2;
                    cap_gray.Line(x, 0, x, SorobanReader.PROCESS_SIZE.Height, CvColor.White);
                }
                cap_gray.Line(0, ROW_MARGIN, SorobanReader.PROCESS_SIZE.Width, ROW_MARGIN, CvColor.White);
                cap_gray.Line(0, SorobanReader.PROCESS_SIZE.Height - ROW_MARGIN,
                    SorobanReader.PROCESS_SIZE.Width, SorobanReader.PROCESS_SIZE.Height - ROW_MARGIN, CvColor.White);

                win1.ShowImage(cap_gray);
                Cv.WaitKey(2);
            }
        }

        static void makeTemplates(IplImage cap_gray, string template_sign, int num)
        {
            int width = cap_gray.Width / (SorobanReader.DIVIDE_NUM + 1);

            // 領域ごとに処理
            for (int i = 0; i < SorobanReader.DIVIDE_NUM; i++)
            {
                int x = (i + 1) * width - width / 2;
                // 領域を指定
                CvRect rect = new CvRect(x + 2, ROW_MARGIN, width - 4, SorobanReader.PROCESS_SIZE.Height - ROW_MARGIN * 2);
                cap_gray.SetROI(rect);
                
                // save
                cap_gray.SaveImage(String.Format("templates\\{0}_{1}-{2}.JPG", template_sign, i, num));

                // 領域の指定を解除
                cap_gray.ResetROI();
            }
        }
    }
}
