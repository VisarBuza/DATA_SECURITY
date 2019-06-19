using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Security.Cryptography;


namespace Klienti
{
    public partial class Form1 : Form
    {
        Socket clientSocket;
        RSACryptoServiceProvider objRsa = new RSACryptoServiceProvider();
        DESCryptoServiceProvider objDes = new DESCryptoServiceProvider();
        string key;
        string IV;
        

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
                    read();
                }).Start();
            }
            catch
            {
                MessageBox.Show("Connection Failed");
            }
        }


        void read()
        {
            while (true)
            {
                try
                {
                    byte[] buffer = new byte[512];
                    int rec = clientSocket.Receive(buffer, 0, buffer.Length, 0);
                    if (rec <= 0)
                    {
                        throw new SocketException();
                    }
                    Array.Resize(ref buffer, rec);


                    Invoke((MethodInvoker)delegate
                    {
                        string data = Encoding.Default.GetString(buffer);
                        data = decrypt(data);
                        string[] filteredData = data.Split('.');
                        new Info(filteredData[0],filteredData[1],filteredData[2],filteredData[3],filteredData[4]).Show();
                    });
                }
                catch
                {
                    MessageBox.Show("Disconnected");
                    Application.Exit();
                }
            }
        }

        private void send()
        {
            string username = txtUsername.Text;
            string password = txtPassword.Text;
            string login = "1";

            string msg =  username+"."+ password+"." + login;

            msg = encrypt(msg);
            byte[] data = Encoding.Default.GetBytes(msg);
            clientSocket.Send(data, 0, data.Length, 0);
        }

        private string encrypt(string plaintext)
        {
            objDes.GenerateKey();
            objDes.GenerateIV();
            objDes.Padding = PaddingMode.Zeros;
            objDes.Mode = CipherMode.CBC;
            key = Encoding.Default.GetString(objDes.Key);
            IV = Encoding.Default.GetString(objDes.IV);


            byte[] bytePlaintexti = Encoding.UTF8.GetBytes(plaintext);

            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms,objDes.CreateEncryptor(),CryptoStreamMode.Write);
            cs.Write(bytePlaintexti, 0, bytePlaintexti.Length);
            cs.Close();

            byte[] byteCiphertexti = ms.ToArray();

            return IV+"."+key+"."+Convert.ToBase64String(byteCiphertexti);

        }

        private string decrypt(string ciphertext)
        {
            objDes.Key = Encoding.Default.GetBytes(key);
            objDes.IV = Encoding.Default.GetBytes(IV);
            objDes.Padding = PaddingMode.Zeros;
            objDes.Mode = CipherMode.CBC;

            byte[] byteCiphertexti = Convert.FromBase64String(ciphertext);
            MemoryStream ms = new MemoryStream(byteCiphertexti);
            CryptoStream cs = new CryptoStream(ms, objDes.CreateDecryptor(), CryptoStreamMode.Read);

            byte[] byteTextiDekriptuar = new byte[ms.Length];
            cs.Read(byteTextiDekriptuar, 0, byteTextiDekriptuar.Length);
            cs.Close();


            return Encoding.UTF8.GetString(byteTextiDekriptuar);
        }
    }
}
