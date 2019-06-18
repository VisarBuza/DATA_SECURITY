using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Serveri
{
    public partial class Form1 : Form
    {

        Socket serverSocket;
        Socket accept;

        Socket socket()
        {
            return new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public Form1()
        {
            InitializeComponent();
        }


        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void BtnListen_Click(object sender, EventArgs e)
        {
            serverSocket = socket();

            serverSocket.Bind(new IPEndPoint(0, 3));
            serverSocket.Listen(0);

            new Thread(() =>
            {
                accept = serverSocket.Accept();
                MessageBox.Show("Connection Accepted");
                serverSocket.Close();

                while (true)
                {
                    try
                    {
                        byte[] buffer = new byte[512];
                        int rec = accept.Receive(buffer, 0, buffer.Length, 0);

                        if (rec <= 0)
                        {
                            throw new SocketException();
                        }

                        Array.Resize(ref buffer, rec);

                        Invoke((MethodInvoker)delegate
                        {
                            listBox1.Items.Add(accept.RemoteEndPoint.ToString());
                            listBox1.Items.Add(Encoding.Default.GetString(buffer));
                        });
                    }
                    catch
                    {
                        MessageBox.Show("Connection lost");
                        Application.Exit();
                    }
                }
            }).Start();
        }
    }
}
