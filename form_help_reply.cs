using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SMS_Sender
{
    public partial class form_help_reply : Form
    {
        string _message = "";

        public form_help_reply(string message)
        {
            InitializeComponent();

            _message = message;
            txtConcern.Text = "Re: " + _message;

            txtResponse.TextChanged += txtResponse_TextChanged;
            btnSend.Click += btnSend_Click;
        }

        void btnSend_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        void txtResponse_TextChanged(object sender, EventArgs e)
        {
            lblLimit.Text = txtResponse.TextLength.ToString() + "/160";
        }
    }
}
