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
    public partial class form_adjust : Form
    {
        DatabaseHandler db = new DatabaseHandler();

        public form_adjust()
        {
            InitializeComponent();
            btnAdjust.Click += btnAdjust_Click;
            loadData();
        }

        void btnAdjust_Click(object sender, EventArgs e)
        {
            db.Query("UPDATE tbl_config SET _VALUE = '" + (nudLoad.Value / 100m).ToString() + "' WHERE _CONFIG = 'RET_PERCENT'");
            db.Query("UPDATE tbl_config SET _VALUE = '" + (nudTransfer.Value / 100m).ToString() + "' WHERE _CONFIG = 'TLC_PRECENT'");
            MessageBox.Show("Successfully Adjusted", "Success");
            this.Close();
        }

        void loadData()
        {
            var tmpDat = db.QueryDT("SELECT * FROM tbl_config");
            nudLoad.Value = tmpDat.ValueDecimal("_VALUE", 0) * 100m;
            nudTransfer.Value = tmpDat.ValueDecimal("_VALUE", 1) * 100m;
        }
    }
}
