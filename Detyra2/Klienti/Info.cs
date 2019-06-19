using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Klienti
{
    public partial class Info : Form
    {

        private string name;
        private string surname;
        private string departamenti;
        private string pozita;
        private string paga;

        public Info(string name,string surname,string departamenti,string pozita,string paga)
        {
            InitializeComponent();
            this.name = name;
            this.surname = surname;
            this.departamenti = departamenti;
            this.pozita = pozita;
            this.paga = paga;

        }

        private void Info_Load(object sender, EventArgs e)
        {
            txtName.Text = name;
            txtSurname.Text = surname;
            txtDepartamenti.Text = departamenti;
            txtPozita.Text = pozita;
            txtPaga.Text = paga;
        }
    }
}
