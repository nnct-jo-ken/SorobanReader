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
    /// JUNKBOX.IO.CaptureDevice�̃��b�v�N���X
    /// ���̂܂܂ł͈����͔̂ώG�Ȃ̂ŃV���v����
    /// </summary>
    public class CaptureCamera
    {
        JUNKBOX.IO.CaptureDevice dev;

        /// <summary>
        /// �J�����f�o�C�X�̈ꗗ��Ԃ����\�b�h
        /// </summary>
        /// <returns>�J�����f�o�C�X�̈ꗗ</returns>
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
        /// �J�����f�o�C�X���̈ꗗ��Ԃ����\�b�h
        /// </summary>
        /// <returns>�J�����f�o�C�X���̈ꗗ</returns>
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
        /// �R���X�g���N�^
        /// </summary>
        /// <param name="index">�J�����f�o�C�X�̈ꗗ�̃C���f�b�N�X</param>
        public CaptureCamera(int index)
        {
            ArrayList devs = GetDevices();
            dev = new JUNKBOX.IO.CaptureDevice((DsDevice)devs[index]);           
        }


        /// <summary>
        /// �R���X�g���N�^
        /// </summary>
        /// <param name="name">�J�����f�o�C�X��/param>
        public CaptureCamera(String name)
        {
            ArrayList devs = GetDevices();
            ArrayList list = GetDeviceNames();

            int index = list.IndexOf(name);
            dev = new JUNKBOX.IO.CaptureDevice((DsDevice)devs[index]);
        }

        /// <summary>
        /// �J�������g�p�\�ȏ�Ԃɂ���
        /// </summary>
        /// <param name="width">�L���v�`���摜�̕�</param>
        /// <param name="height">�L���v�`���摜�̍���</param>
        public void Activate(int width, int height)
        {
            dev.Activate(new SWF.Panel(), width, height);
            // Activate�̂������Capture�����Ƃ�AccessViolationException����������̂�h��
            System.Threading.Thread.Sleep(1000);
        }

        /// <summary>
        /// �J��������摜���L���v�`������
        /// </summary>
        /// <returns>�J��������擾�����摜</returns>
        public CaptureImage Capture()
        {
            CaptureImage cap = new CaptureImage(dev.Capture());
            return cap;
        }
    }
}
