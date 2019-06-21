using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using JWT;
using JWT.Algorithms;
using JWT.Serializers;
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
using System.Security.Cryptography.X509Certificates;

namespace Serveri
{   
    public partial class Form1 : Form
    {
        X509Certificate2 certifikata;
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
            MessageBox.Show("Serveri filloi te degjoj ne portin 3");

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

                        string controlFlow = list[list.Length - 1].Substring(0, 1);

                        if (controlFlow == "1")
                        {
                            if (Users.isUser(list[0], list[1])) 
                            {
                                XElement user = Users.getUserInfo(list[0]);
                                string thisName= user.Element("name").Value.ToString();
                                string thisSurName = user.Element("surname").Value.ToString();
                                string thisDepartament = user.Element("departamenti").Value.ToString();
                                string thisPozita = user.Element("pozita").Value.ToString();
                                string thisPaga = user.Element("paga").Value.ToString();

                                var payload = new Dictionary<string, object>
                                {
                                    { "name", thisName },
                                    { "surname", thisSurName },
                                    { "department", thisDepartament },
                                    { "pozita", thisPozita },
                                    { "paga", thisPaga },

                                };

                                IJwtAlgorithm algorithm = new HMACSHA256Algorithm();
                                IJsonSerializer serializer = new JsonNetSerializer();
                                IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
                                IJwtEncoder encoder = new JwtEncoder(algorithm, serializer, urlEncoder);

                                var token = encoder.Encode(payload,certifikata.PrivateKey.ToString());
                                byte[] data1 = Encoding.Default.GetBytes(token);
                                accept.Send(data1, 0, data1.Length, 0);
                            }
                            else
                            {
                                string errorMsg = "error";
                                errorMsg = encrypt(errorMsg, key, iv);
                                byte[] byteErrorMsg = Encoding.Default.GetBytes(errorMsg);
                                accept.Send(byteErrorMsg, 0, byteErrorMsg.Length, 0);

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
            objDes.Key = Convert.FromBase64String(key);
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

            objRsa = (RSACryptoServiceProvider)certifikata.PrivateKey;
            byte[] byteKey = objRsa.Decrypt(Convert.FromBase64String(key),true);

            key = Convert.ToBase64String(byteKey);
            objDes.Key = byteKey;
            objDes.IV = Encoding.Default.GetBytes(iv);
            objDes.Padding = PaddingMode.Zeros;
            objDes.Mode = CipherMode.CBC;

            byte[] byteCiphertexti =Convert.FromBase64String(info[2]);
            MemoryStream ms = new MemoryStream(byteCiphertexti);
            CryptoStream cs = new CryptoStream(ms,objDes.CreateDecryptor(),CryptoStreamMode.Read);

            byte[] byteTextiDekriptuar = new byte[ms.Length];
            cs.Read(byteTextiDekriptuar, 0, byteTextiDekriptuar.Length);
            cs.Close();

            string decryptedText = Encoding.UTF8.GetString(byteTextiDekriptuar);
            return decryptedText;
        }

        private void PerzgjedhCertifikatenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            X509Store certificateStore =new X509Store(StoreName.My, StoreLocation.CurrentUser);
            certificateStore.Open(OpenFlags.OpenExistingOnly);

            try
            {
                X509Certificate2Collection certCollection = X509Certificate2UI.SelectFromCollection(certificateStore.Certificates,
                    "Zgjedh certifikaten","Zgjedh certifikaten",X509SelectionFlag.SingleSelection);

                certifikata = certCollection[0];
                if (certifikata.HasPrivateKey)
                {
                    MessageBox.Show("Keni perzgjedhur certifikaten e " +
                        certifikata.Subject);
                }
                else
                {
                    MessageBox.Show("Certifikata nuk permbane celes privat!");
                }
            }
            catch (Exception ex)
            {

            }

            byte[] byteCert = certifikata.Export(X509ContentType.Cert, "Visar123");
            accept.Send(byteCert, 0, byteCert.Length, 0);
                       
        }
    }
}
