using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Net.Sockets;
using System.Net;
using System.Drawing.Imaging;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.IO;

namespace TeamViewerClone
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            btnScreenShare.Enabled = false;
        }
        TcpClient client = new TcpClient();
        private NetworkStream mainStream;
        private int portNumber;
        private void Form1_Load(object sender, EventArgs e)
        {
            
        }
        public void MsgRead()
        {
            while (true)
            {
                NetworkStream stream = client.GetStream();
                StreamReader sdr = new StreamReader(stream);
                string msg = sdr.ReadLine();
                textBox1.AppendText(Environment.NewLine);
                textBox1.AppendText("From Server: " + msg);

            }
        }
 
        private static Image GrabDesktop() 
        {
            Rectangle bound = Screen.PrimaryScreen.Bounds;
            Bitmap screenshot = new Bitmap(bound.Width,bound.Height,PixelFormat.Format32bppArgb);
            Graphics graphics = Graphics.FromImage(screenshot);
            graphics.CopyFromScreen(bound.X,bound.Y,0,0,bound.Size, CopyPixelOperation.SourceCopy);
            return screenshot;
        }

        private void SendDesktopImage()
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            mainStream = client.GetStream();
            binaryFormatter.Serialize(mainStream,GrabDesktop());
        }

        

        private void btnConnect_Click(object sender, EventArgs e)
        {
            portNumber = int.Parse(textBox3.Text);

            try
            {

                CheckForIllegalCrossThreadCalls = false;
                client.Connect(IPAddress.Parse(textBox2.Text), portNumber);
                btnConnect.Text = "Connected";
                btnConnect.Enabled = false;
                MessageBox.Show("Connected");
                btnScreenShare.Enabled = true;
                Thread t = new Thread(MsgRead);
                t.Start();
            }
            catch (Exception)
            {
                MessageBox.Show("Failed to Connect");
            }
        }

        private void btnScreenShare_Click(object sender, EventArgs e)
        {
            if (btnScreenShare.Text.StartsWith("Screen Share"))
            {
                timer1.Start();
                btnScreenShare.Text = "stop sharing";
            }
            else
            {
                timer1.Stop();
                btnScreenShare.Text = "Screen Share";
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            SendDesktopImage();
        }

        

        private void button1_Click(object sender, EventArgs e)
        {

            NetworkStream stream = client.GetStream();
            StreamWriter sdr = new StreamWriter(stream);
            sdr.WriteLine(textBox4.Text);
            sdr.Flush();
        }
    }
}
