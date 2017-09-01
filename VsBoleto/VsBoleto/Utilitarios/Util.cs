using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace VsBoleto.Utilitarios
{
    class Util
    {
        public static void LimparCampos(Control.ControlCollection controles)
        {
            foreach (Control controle in controles)
            {
                LimparCampo(controle);
            }
        }

        /// <summary>
        /// Limpa os Campos de um controle recursivamente
        /// </summary>
        /// <param name="controles">O controle a ser limpo</param>
        public static void LimparCampos(Control controle)
        {
            Util.LimparCampos(controle.Controls);
        }

        /// <summary>
        /// Limpar um controle recursivamente
        /// </summary>
        /// <param name="controle">O Controle a ser limpo</param>
        public static void LimparCampo(Control controle)
        {
            controle.BackColor = Color.Empty;
            if (controle.Controls.Count > 0)
            {
                LimparCampos(controle.Controls);
            }

            switch (controle.GetType().Name)
            {
                case "CheckBox":
                    ((CheckBox)controle).Checked = false;
                    break;
                case "ComboBoxAutoSize":
                case "ComboBox":
                    if (((ComboBox)controle).Items.Count > 0)
                        ((ComboBox)controle).SelectedIndex = 0;
                    break;
                case "C2TextBox":
                case "TextBox":
                    ((TextBox)controle).Text = string.Empty;
                    break;
                case "TextBoxData":
                case "MaskedTextBox":
                    ((MaskedTextBox)controle).Text = string.Empty;
                    break;
                case "NumericUpDown":
                    ((NumericUpDown)controle).Value = 0;
                    break;
                case "TreeView":
                    ((TreeView)controle).Nodes.Clear();
                    break;
                case "ListBox":
                    ((ListBox)controle).ClearSelected();
                    break;
                case "TextEdit":
                    controle.Text = string.Empty;
                    break;
                case "DateEdit":
                    controle.Text = string.Empty;
                    break;
                case "ComboBoxEdit":
                    if (((DevExpress.XtraEditors.ComboBoxEdit)controle).Properties.Items.Count > 0)
                    {
                        ((DevExpress.XtraEditors.ComboBoxEdit)controle).SelectedIndex = 0;
                    }

                    break;
            }
        }
    }
}
