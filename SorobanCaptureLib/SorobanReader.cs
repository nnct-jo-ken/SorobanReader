using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OpenCvSharp;

namespace SorobanCaptureLib
{
    public class SorobanReader
    {
        /// <summary>
        /// 画像を分割したときのマージン率
        /// （分割幅のマージン率分だけ左右に多めに画像を分割する）
        /// </summary>
        public const double DIVIDE_MARGIN = 0.2;
        public const int DIVIDE_NUM = 13;

        /// <summary>
        /// 処理画像サイズ
        /// </summary>
        public static CvSize PROCESS_SIZE = new CvSize(320, 240);

        /// <summary>
        /// 0から9のテンプレート画像
        /// </summary>
        private IplImage[][] templates;

        public SorobanReader(string template_sign)
        {
            this.templates = new IplImage[DIVIDE_NUM][];
            // それぞれの数のテンプレートを読み込み
            for (int x = 0; x < DIVIDE_NUM; x++) //列
            {
                this.templates[x] = new IplImage[10];
                for (int i = 0; i < 10; i++) // 数
                {
                    this.templates[x][i] = new IplImage("templates\\" + template_sign + "_" + x.ToString() + "-" + i.ToString() + ".JPG", LoadMode.GrayScale);
                }
            }
        }

        public SorobanReader(IplImage[][] templates)
        {
            this.templates = templates;
        }


        /// <summary>
        /// そろばんの1列のみの画像から最もマッチする数値を返す
        /// </summary>
        /// <param name="source">そろばん1列のみの画像</param>
        /// <param name="temp_num">テンプレート画像</param>
        /// <param name="threshold">しきい値</param>
        /// <returns>最もマッチする数値</returns>
        public int BestMatchNum(IplImage source, IplImage[] temp_num, double threshold)
        {
            return bestMatchNum(source, temp_num, threshold);
        }


        /// <summary>
        /// 指定した分割数でそろばんを全て読み取る
        /// </summary>
        /// <param name="source">そろばんの画像</param>
        /// <param name="threshold">しきい値</param>
        /// <param name="process_img">処理画像</param>
        /// <returns>読み取った数値(-1はエラー)</returns>
        public int[] AllMatching(IplImage source, double threshold, out IplImage process_img)
        {
            // グレースケール画像
            IplImage cap_gray = new IplImage(PROCESS_SIZE, BitDepth.U8, 1);

            // キャプチャとリサイズ，グレースケール変換
            using (IplImage tmp = new IplImage(
                PROCESS_SIZE, source.Depth, source.NChannels))
            {
                source.Resize(tmp);
                tmp.CvtColor(cap_gray, ColorConversion.BgrToGray);
            }

            int[] results = new int[DIVIDE_NUM];
            int width = cap_gray.Width / (DIVIDE_NUM + 1);
            int margin = (int)(width * DIVIDE_MARGIN);

            // 領域ごとに処理
            Parallel.For(0, DIVIDE_NUM, i =>
            {
                IplImage tmp = new IplImage(PROCESS_SIZE, BitDepth.U8, 1);
                cap_gray.Copy(tmp);

                int x = (i + 1) * width - width / 2;
                // 領域を指定
                CvRect rect = new CvRect(x - margin, 0, width + margin * 2, PROCESS_SIZE.Height);
                tmp.SetROI(rect);

                // 0-9の画像とMatchTemplateし一番高い値を得る
                results[i] = bestMatchNum(tmp, this.templates[i], threshold);

                // 領域の指定を解除
                tmp.ResetROI();
            });

            // 分割線の描画
            for (int i = 1; i < DIVIDE_NUM + 2; i++)
            {
                int x = i * width - width / 2;
                cap_gray.Line(x, 0, x, PROCESS_SIZE.Height, CvColor.White);
            }

            // 読み取り数値を表示
            CvFont font = new CvFont(FontFace.HersheyDuplex, 1.0, 1.0);
            for (int i = 0; i < DIVIDE_NUM; i++)
            {
                if (results[i] != -1)
                {
                    int x = i * width + width / 2;
                    cap_gray.PutText(results[i].ToString(), new CvPoint(x, 30),
                        font, CvColor.White);
                }
            }

            // 分割線, 読み取り数値画像を返す
            process_img = cap_gray;

            return results;
        }

        private static int bestMatchNum(IplImage source, IplImage[] temp_num, double threshold)
        {
            CvSize size = new CvSize(
                source.GetROI().Width - temp_num[0].Width + 1,
                source.GetROI().Height - temp_num[0].Height + 1);

            // 0から9の一致度
            double[] max_list = new double[10];

            // 0-9の画像とマッチング
            Parallel.For(0, 10, i =>
            {
                IplImage result = new IplImage(size, BitDepth.F32, 1);
                source.MatchTemplate(temp_num[i], result,
                    MatchTemplateMethod.CCoeffNormed);
                double min;
                CvPoint min_point, max_point;
                result.MinMaxLoc(out min, out max_list[i], out min_point, out max_point);
            });

            // 一致度が最大のものを探す
            double best = 0.0;
            int best_index = -1;

            for (int i = 0; i < 10; i++)
            {
                double tmp = max_list[i];
                if (tmp > best)
                {
                    best = tmp;
                    best_index = i;
                }
            }

            // しきい値以下の精度であれば切り捨て
            if (best < threshold) return -1;

            return best_index;
        }
    }
}
