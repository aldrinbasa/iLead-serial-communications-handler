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
    public partial class form_edit_promo : Form
    {
        DatabaseHandler db = new DatabaseHandler();
        string id = "";

        public form_edit_promo(string _id)
        {
            InitializeComponent();
            id = _id;
            if (id == "")
                this.Close();

            btnSave.Click += btnSave_Click;
            cmbNetwork.SelectedValueChanged += cmbNetwork_SelectedValueChanged;
            LoadData(_id);
        }
        void cmbNetwork_SelectedValueChanged(object sender, EventArgs e)
        {
            txtKeyword.Enabled = cmbNetwork.SelectedIndex != 2;
            txtUssd.Enabled = cmbNetwork.SelectedIndex == 2;
        }

        void btnSave_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to save changes?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                string keyword = txtKeyword.Text.Trim();

                if (keyword != "" || keyword != null)
                    keyword += " {0}";

                db.Query("UPDATE tbl_codes SET _LOAD = '" + txtDesc.Text.Trim() +
                    "', _CODE = '" + txtCode.Text.Trim() +
                    "', _SRP = '" + nudSRP.Value.ToString() +
                    "', _CARRIER = '" + cmbNetwork.Text.Trim() +
                    "', _ACTUAL_CODE = '" + txtActualCode.Text.Trim() +
                    "', _USSD_CODE ='" + txtUssd.Text.Trim() +
                    "', _KEYWORD = '" + keyword + "' WHERE _ID = '" + id + "'");

                this.Close();
            }
        }

        void LoadData(string _id)
        {
            DataTable dt = db.QueryDT("SELECT * FROM tbl_codes WHERE _ID = '" + _id + "'");
            cmbNetwork.Text = dt.ValueString("_CARRIER", 0);
            txtDesc.Text = dt.ValueString("_LOAD", 0);
            txtCode.Text = dt.ValueString("_CODE", 0);
            nudSRP.Value = dt.ValueDecimal("_SRP", 0);
            txtActualCode.Text = dt.ValueString("_ACTUAL_CODE", 0);
            txtUssd.Text = dt.ValueString("_USSD_CODE", 0);
            txtKeyword.Text = dt.ValueString("_KEYWORD", 0).Replace(" {0}", "");
        }
    }
}
