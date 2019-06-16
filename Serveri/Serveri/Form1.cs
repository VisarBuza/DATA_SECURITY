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

namespace Serveri
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Button1_Click(object sender, EventArgs e)
        {

            if (Users.isUser(textBox1.Text, textBox2.Text))
            {
                XElement info = Users.getUserInfo(textBox1.Text);
                textBox3.Text = info.Element("name").Value.ToString();
                textBox4.Text = info.Element("surname").Value.ToString();
                textBox5.Text = info.Element("paga").Value.ToString();
                textBox6.Text = info.Element("pozita").Value.ToString();
                textBox6.Text = info.Element("departamenti").Value.ToString();
                Form1.
            }
            else
            {
                MessageBox.Show("Sen spo din ");
            }

        }
    }
}
