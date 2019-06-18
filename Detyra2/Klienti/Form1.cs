using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Klienti
{
    public partial class Form1 : Form
    {

        Socket clientSocket;

        Socket socket()
        {
            return new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public Form1()
        {
            InitializeComponent();
            clientSocket = socket();
            connect();
        }

        private void Label4_Click(object sender, EventArgs e)
        {
            new Register(clientSocket).Show();
            this.Hide();
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            send();
        }

        private void connect()
        {
            string ipaddress = "127.0.0.1";
            int portNumber = 3;

            try
            {
                clientSocket.Connect(new IPEndPoint(IPAddress.Parse(ipaddress), portNumber));

                new Thread(() =>
                {
                    MessageBox.Show("Connection Established");
                }).Start();
            }
            catch
            {
                MessageBox.Show("Connection Failed");
            }
        }

      

        private void send()
        {
            byte[] data = Encoding.Default.GetBytes(txtUsername.Text + txtPassword.Text);
            clientSocket.Send(data, 0, data.Length, 0);
        }
    }
}
