using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using JWT;
using JWT.Algorithms;
using JWT.Serializers;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;


namespace Klienti
{
    public partial class Form1 : Form
    {
        Socket clientSocket;
        RSACryptoServiceProvider objRsa = new RSACryptoServiceProvider();
        DESCryptoServiceProvider objDes = new DESCryptoServiceProvider();
        X509Certificate2 certifikata = new X509Certificate2();
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
            new Register(clientSocket,certifikata).Show();
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
                    byte[] buffer = new byte[2048];
                    int rec = clientSocket.Receive(buffer, 0, buffer.Length, 0);
                    if (rec <= 0)
                    {
                        throw new SocketException();
                    }
                    Array.Resize(ref buffer, rec);
                    
                    Invoke((MethodInvoker)delegate
                    {
                        if (buffer.Length > 900)
                        {
                            certifikata.Import(buffer);
                        }
                        else
                        {
                            string data = Encoding.Default.GetString(buffer);
                            //data = decrypt(data);
                            if (data.Substring(0,5) == "error")
                            {
                                MessageBox.Show("Wrong Credentials");
                            }
                            else
                            {
                                var jo = JObject.Parse(verifyToken(data));
                                new Info(jo["name"].ToString(), jo["surname"].ToString(), jo["department"].ToString(), jo["pozita"].ToString(), jo["paga"].ToString()).Show();
                            }
                        }
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

            objRsa = (RSACryptoServiceProvider)certifikata.PublicKey.Key;
            byte[] byteKey = objRsa.Encrypt(objDes.Key, true);
            string encryptedKey = Convert.ToBase64String(byteKey);

            byte[] bytePlaintexti = Encoding.UTF8.GetBytes(plaintext);

            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms,objDes.CreateEncryptor(),CryptoStreamMode.Write);
            cs.Write(bytePlaintexti, 0, bytePlaintexti.Length);
            cs.Close();

           

            byte[] byteCiphertexti = ms.ToArray();

            return IV+"."+encryptedKey+"."+Convert.ToBase64String(byteCiphertexti);

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

        private string verifyToken(string token)
        {

            try
            {
                IJsonSerializer serializer = new JsonNetSerializer();
                IDateTimeProvider provider = new UtcDateTimeProvider();
                IJwtValidator validator = new JwtValidator(serializer, provider);
                IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
                IJwtDecoder decoder = new JwtDecoder(serializer, validator, urlEncoder);


                var json = decoder.Decode(token, certifikata.PublicKey.Key.ToString(), verify: true);
                return json;
            }
            catch (TokenExpiredException)
            {
                MessageBox.Show("Token has expired");
                return null;
            }
            catch (SignatureVerificationException)
            {
                MessageBox.Show("Token has invalid signature");
                return null;
            }
        }

    }
}
