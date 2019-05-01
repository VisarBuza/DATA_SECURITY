using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace BealeCipher
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void BtnEncrypt_Click(object sender, EventArgs e)
        {
            StreamReader reader = new StreamReader(txtKeyPath.Text);
            string key = reader.ReadToEnd();
            BealeCipher objBeale = new BealeCipher(key, txtPlainText.Text);
            objBeale.encrypt();
            txtCipherText.Text = objBeale.getCipherText();
        }

        private void BtnBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog opf = new OpenFileDialog();
            if (opf.ShowDialog() == DialogResult.OK)
            {
                txtKeyPath.Text = opf.FileName;
            }
        }

        private void BtnDecrypt_Click(object sender, EventArgs e)
        {
            StreamReader reader = new StreamReader(txtKeyPath.Text);
            string key = reader.ReadToEnd();
            BealeCipher objBeale = new BealeCipher(key);
            txtDecryptedText.Text = objBeale.decrypt(txtCipherText.Text);
        }
    }
    
}
