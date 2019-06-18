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

namespace Klienti
{
    public partial class Register : Form
    {
        Socket clientSocket;
        public Register(Socket socket)
        {
            InitializeComponent();
            clientSocket = socket;
        }

        

        private void Register_Load(object sender, EventArgs e)
        {
            
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            byte[] data = Encoding.Default.GetBytes(textBox1.Text);
            clientSocket.Send(data, 0, data.Length, 0);
        }
    }
}
