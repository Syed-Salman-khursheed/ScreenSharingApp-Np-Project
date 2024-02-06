using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace TeamViewerServer
{

    public partial class Form2 : Form
    {
        private readonly int port;
        private TcpClient client;
        private TcpListener server;
        private NetworkStream mainStream;

        private readonly Thread listening;
        private readonly Thread GetImage;
        private readonly Thread Readmsg;

        public Form2(int port)
        {
            this.port = port;
            client = new TcpClient();
            listening = new Thread(StartListening);
            GetImage = new Thread(ReciveImage);
            Readmsg = new Thread(ReadMsg);


            InitializeComponent();
        }
        public void ReadMsg()
        {
            while (true)
            {
                NetworkStream stream = client.GetStream();
                StreamReader sdr = new StreamReader(stream);
                string msg = sdr.ReadLine();
                textBox2.AppendText(Environment.NewLine);
                textBox2.AppendText("From client -> " + msg);
               // Readmsg.Start();
            }
            
        }
        private void StartListening()
        {

            while (!client.Connected)
            {
                CheckForIllegalCrossThreadCalls = false;
                server.Start();
                client = server.AcceptTcpClient();
            }
            GetImage.Start();
           
        }
        private void StopListening()
        {
            server.Stop();
            client = null;
            if (GetImage.IsAlive) listening.Abort();
        }

        private void ReciveImage()
         {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            while(client.Connected)
            {
                    mainStream = client.GetStream();
                try
                {
                    pictureBox1.Image = (Image)binaryFormatter.Deserialize(mainStream);
                }
                catch (Exception)
                {
                    NetworkStream stream = client.GetStream();
                    StreamReader sdr = new StreamReader(stream);
                    string msg = sdr.ReadLine();
                    textBox2.AppendText(Environment.NewLine);
                    textBox2.AppendText("From client -> " + msg);
                }
                   

            }
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            IPAddress iPAddress = IPAddress.Loopback;
            textBox1.Text = iPAddress.ToString();
            server = new TcpListener(iPAddress, port);
            listening.Start();

        }
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            StopListening();
        }
        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

            NetworkStream stream = client.GetStream();
            StreamWriter sdr = new StreamWriter(stream);
            sdr.WriteLine(textBox3.Text);
            sdr.Flush();

        }
    }
}
