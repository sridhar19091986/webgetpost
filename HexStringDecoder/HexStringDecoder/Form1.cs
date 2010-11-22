using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Net;
using System.IO;

namespace HexStringDecoder
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        //hex流可能的编码方式
        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

            Byte[] srcBytes = ToByteArray(richTextBox1.Text);
            string a = checkedListBox1.GetItemText(checkedListBox1.SelectedItem);
            for (int i = 0; i < checkedListBox1.Items.Count; i++)
            {
                if (checkedListBox1.GetItemChecked(i))
                {
                    checkedListBox1.SetItemChecked(i, false);
                }
            }
            checkedListBox1.SetItemChecked(checkedListBox1.SelectedIndex, true);
            switch (a)
            {
                case "ASCII":
                    richTextBox2.Text = System.Text.Encoding.ASCII.GetString(srcBytes);
                    break;
                case "Unicode":
                    richTextBox2.Text = System.Text.Encoding.Unicode.GetString(srcBytes);
                    break;
                case "BigEndianUnicode":
                    richTextBox2.Text = System.Text.Encoding.BigEndianUnicode.GetString(srcBytes);
                    break;
                case "UTF7":
                    richTextBox2.Text = System.Text.Encoding.UTF7.GetString(srcBytes);
                    break;
                case "UTF8":
                    richTextBox2.Text = System.Text.Encoding.UTF8.GetString(srcBytes);
                    break;
                case "GB2312":
                    richTextBox2.Text = System.Text.Encoding.GetEncoding("GB2312").GetString(srcBytes);  //属性不能获取时，此处可以通过方法获取
                    break;
                case "UTF32":
                    richTextBox2.Text = System.Text.Encoding.UTF32.GetString(srcBytes);
                    break;
                default:
                    richTextBox2.Text = System.Text.Encoding.Default.GetString(srcBytes);
                    break;
            }
        }

        //加载hexstring
        private void Form1_Load(object sender, EventArgs e)
        {
            richTextBox1.Text = "7B4962114E0B73ED5C318D70";
        }

        //hex字符串转换成字节数组
        byte[] ToByteArray(String HexString)
        {
            int NumberChars = HexString.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(HexString.Substring(i, 2), 16);
            }
            return bytes;
        }

        //网页代码和原始网页写入richbox和webbrowser
        void GetPostTest(string method)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            WebRequest req = WebRequest.Create("http://www.google.com.hk/");
            req.Method = method;
            //req.ContentType = "application/x-www-form-urlencoded";
            //req.ContentLength = 1;
            WebResponse response = req.GetResponse();
            using (WebResponse res = req.GetResponse())
            {
                using (Stream s = res.GetResponseStream())
                {
                    using (StreamReader sr = new StreamReader(s, System.Text.Encoding.GetEncoding("gb2312")))
                    {
                        richTextBox2.Text = sr.ReadToEnd();
                        webBrowser1.DocumentText = richTextBox2.Text;
                    }
                }
            }
            sw.Stop();
        }

        //get 和 post 方法测试
        void checkedListBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string a = checkedListBox2.GetItemText(checkedListBox2.SelectedItem);
                switch (a)
                {
                    case "POST":
                        GetPostTest("POST");
                        break;
                    case "GET":
                        GetPostTest("GET");
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        //获取url的图片
        void pictureBox1_Click(object sender, EventArgs e)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            System.Net.WebRequest request = System.Net.WebRequest.Create("http://www.google.com.hk/intl/zh-CN/images/logo_cn.gif");
            System.Net.WebResponse response = request.GetResponse();
            System.IO.Stream responseStream = response.GetResponseStream();
            Bitmap bitmap2 = new Bitmap(responseStream);
            pictureBox1.Image = bitmap2;
            StreamToFile(bitmap2);
            sw.Stop();
        }

        //把 bmp 转换成 hex 字符串
        void StreamToFile(Bitmap bmp)
        {
            byte[] bmpData = null;
            using (MemoryStream ms = new MemoryStream())
            {
                bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                ms.Seek(0, 0);
                bmpData = ms.ToArray();
            }
            string hexData = "";
            foreach (byte b in bmpData)
            {
                hexData += b.ToString("x").PadLeft(2, '0');
            }
            richTextBox1.Text = hexData;
        }

        //把 hex 字符串转换成 bmp
        void pictureBox2_Click(object sender, EventArgs e)
        {
            byte[] buffer = ToByteArray(richTextBox1.Text);
            MemoryStream ms = new MemoryStream(buffer);
            Bitmap bmp = new Bitmap(ms);
            pictureBox2.Image = bmp;
        }

        //音频和视频的解码
        //把hex字符串转换成byte数组，写入文件，标识用户，时间，开始帧好，uri等信息
        private void button1_Click(object sender, EventArgs e)
        {

            byte[] buffer = ToByteArray(richTextBox1.Text);
            File.WriteAllBytes("voiceVidieo.wav", buffer);
       
        }
    }
}
