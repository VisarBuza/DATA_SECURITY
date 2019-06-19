using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace Klienti
{
    public partial class Register : Form
    {
        X509Certificate2 certifikata;
        RSACryptoServiceProvider objRsa = new RSACryptoServiceProvider();
        DESCryptoServiceProvider objDes = new DESCryptoServiceProvider();
        Socket clientSocket;

        public Register(Socket socket,X509Certificate2 certificate2)
        {
            InitializeComponent();
            clientSocket = socket;
            certifikata=certificate2;
        }

        

        private void Register_Load(object sender, EventArgs e)
        {
       
        }

        private void BtnSignUp_Click(object sender, EventArgs e)
        {
            send();
            MessageBox.Show("Te dhenat u ruajten me sukses");
        }
   

        private void send()
        {
            string name = txtName.Text;
            string surname = txtSurname.Text;
            string username = txtUsername.Text;
            string password = txtPassword.Text;
            string departamenti = txtDepartamenti.Text;
            string pozita = txtPozita.Text;
            string paga = txtPaga.Text;
            string register = "2";

            string msg = name + "." + surname + "." + username + "." + password + "." + pozita + "." +paga + "." + departamenti+ "." +register;
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
            string key = Encoding.Default.GetString(objDes.Key);
            string IV = Encoding.Default.GetString(objDes.IV);


            objRsa = (RSACryptoServiceProvider)certifikata.PublicKey.Key;
            byte[] byteKey = objRsa.Encrypt(objDes.Key, true);
            string encryptedKey = Convert.ToBase64String(byteKey);

            byte[] bytePlaintexti = Encoding.UTF8.GetBytes(plaintext);


            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, objDes.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(bytePlaintexti, 0, bytePlaintexti.Length);
            cs.Close();

            byte[] byteCiphertexti = ms.ToArray();

            return IV + "." + encryptedKey + "." + Convert.ToBase64String(byteCiphertexti);

        }
    }
}
