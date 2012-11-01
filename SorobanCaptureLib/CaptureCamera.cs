using System;
using System.Collections.Generic;
using System.Linq;

using OpenCvSharp;
using DShowNET;
using DShowNET.Device;
using System.Collections;
using JUNKBOX.IO;
using SWF = System.Windows.Forms;
using SD = System.Drawing;
using SDI = System.Drawing.Imaging;

namespace SorobanCaptureLib
{
    /// <summary>
    /// JUNKBOX.IO.CaptureDeviceのラップクラス
    /// そのままでは扱うのは煩雑なのでシンプルに
    /// </summary>
    public class CaptureCamera
    {
        JUNKBOX.IO.CaptureDevice dev;

        /// <summary>
        /// カメラデバイスの一覧を返すメソッド
        /// </summary>
        /// <returns>カメラデバイスの一覧</returns>
        public static ArrayList GetDevices()
        {
            ArrayList devs;
            if (DsDev.GetDevicesOfCat(FilterCategory.VideoInputDevice, out devs) == false)
            {
                //Exception
                System.Windows.Forms.MessageBox.Show("Camera Devices not found.");
            }

            return devs;
        }

        /// <summary>
        /// カメラデバイス名の一覧を返すメソッド
        /// </summary>
        /// <returns>カメラデバイス名の一覧</returns>
        public static ArrayList GetDeviceNames()
        {
            ArrayList devs = GetDevices();
            ArrayList list = new ArrayList();
            foreach(DsDevice d in devs)
            {
                list.Add(d.Name);
            }

            return list;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="index">カメラデバイスの一覧のインデックス</param>
        public CaptureCamera(int index)
        {
            ArrayList devs = GetDevices();
            dev = new JUNKBOX.IO.CaptureDevice((DsDevice)devs[index]);           
        }


        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="name">カメラデバイス名/param>
        public CaptureCamera(String name)
        {
            ArrayList devs = GetDevices();
            ArrayList list = GetDeviceNames();

            int index = list.IndexOf(name);
            dev = new JUNKBOX.IO.CaptureDevice((DsDevice)devs[index]);
        }

        /// <summary>
        /// カメラを使用可能な状態にする
        /// </summary>
        /// <param name="width">キャプチャ画像の幅</param>
        /// <param name="height">キャプチャ画像の高さ</param>
        public void Activate(int width, int height)
        {
            dev.Activate(new SWF.Panel(), width, height);
            // Activateのすぐ後にCaptureしたときAccessViolationExceptionが発生するのを防ぐ
            System.Threading.Thread.Sleep(1000);
        }

        /// <summary>
        /// カメラから画像をキャプチャする
        /// </summary>
        /// <returns>カメラから取得した画像</returns>
        public CaptureImage Capture()
        {
            CaptureImage cap = new CaptureImage(dev.Capture());
            return cap;
        }
    }
}
