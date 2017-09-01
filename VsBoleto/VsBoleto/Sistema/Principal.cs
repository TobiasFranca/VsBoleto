using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace VsBoleto.Sistema
{
    public partial class Principal : DevExpress.XtraEditors.XtraForm
    {
        public Principal()
        {
            InitializeComponent();
            Text = "VsBoleto - Versão: " + Application.ProductVersion.ToString();
        }

    }
}