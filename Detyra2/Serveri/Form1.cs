using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Security.Cryptography;

namespace Serveri
{
    public partial class Form1 : Form
    {

        RSACryptoServiceProvider objRsa = new RSACryptoServiceProvider();
        DESCryptoServiceProvider objDes = new DESCryptoServiceProvider();
        string key;
        string iv;
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
                serverSocket.Close();

                while (true)
                {
                    try
                    {

                        byte[] buffer = new byte[2048];
                        int rec = accept.Receive(buffer, 0, buffer.Length, 0);

                        if (rec <= 0)
                        {
                            throw new SocketException();
                        }

                        Array.Resize(ref buffer, rec);

                        string data = Encoding.Default.GetString(buffer);

                        data = decrypt(data);

                        string[] list = data.Split('.');
                         

                        if (Int32.Parse(list[list.Length-1]) == 1)
                        {
                            if (Users.isUser(list[0], list[1])) 
                            {
                                XElement user = Users.getUserInfo(list[0]);
                                string thisName= user.Element("name").Value.ToString();
                                string thisSurName = user.Element("surname").Value.ToString();
                                string thisDepartament = user.Element("departamenti").Value.ToString();
                                string thisPozita = user.Element("pozita").Value.ToString();
                                string thisPaga = user.Element("paga").Value.ToString();
                                string info = thisName + "." + thisSurName + "." + thisDepartament + "." + thisPozita + "." + thisPaga;
                                info = encrypt(info,key,iv);
                                byte[] data1 = Encoding.Default.GetBytes(info);
                                accept.Send(data1, 0, data1.Length, 0);
                            }
                            else
                            {
                                byte[] data1 = Encoding.Default.GetBytes("wrong");
                                accept.Send(data1, 0, data1.Length, 0);

                            }
                        }
                        else if (Int32.Parse(list[list.Length - 1]) == 2)
                        {
                            Users.insert(list[0], list[1], list[2], list[3], list[4], list[5], list[6]);
                        }

                        Invoke((MethodInvoker)delegate
                        {
                            listBox1.Items.Add(accept.RemoteEndPoint);
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
        private string encrypt(string plaintext,string key,string iv)
        {
            objDes.Key = Encoding.Default.GetBytes(key);
            objDes.IV = Encoding.Default.GetBytes(iv);
            objDes.Padding = PaddingMode.Zeros;
            objDes.Mode = CipherMode.CBC;        


            byte[] bytePlaintexti = Encoding.UTF8.GetBytes(plaintext);

            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, objDes.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(bytePlaintexti, 0, bytePlaintexti.Length);
            cs.Close();

            byte[] byteCiphertexti = ms.ToArray();

            return  Convert.ToBase64String(byteCiphertexti);

        }
        private string decrypt(string ciphertext)
        {
            string[] info = ciphertext.Split('.');
            key = info[1];
            iv = info[0];
            objDes.Key = Encoding.Default.GetBytes(info[1]);
            objDes.IV = Encoding.Default.GetBytes(info[0]);
            objDes.Padding = PaddingMode.Zeros;
            objDes.Mode = CipherMode.CBC;

            byte[] byteCiphertexti =Convert.FromBase64String(info[2]);
            MemoryStream ms = new MemoryStream(byteCiphertexti);
            CryptoStream cs = new CryptoStream(ms,objDes.CreateDecryptor(),CryptoStreamMode.Read);

            byte[] byteTextiDekriptuar = new byte[ms.Length];
            cs.Read(byteTextiDekriptuar, 0, byteTextiDekriptuar.Length);
            cs.Close();

            
            return Encoding.UTF8.GetString(byteTextiDekriptuar);
        }
    }
}
