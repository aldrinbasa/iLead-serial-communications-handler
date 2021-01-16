using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SMS_Sender
{
    public partial class form_generate : Form
    {
        DataTable data = new DataTable();
        DatabaseHandler db = new DatabaseHandler();

        public form_generate()
        {
            InitializeComponent();
            cmbFor.SelectedIndex = 0;
            LoadDataGrid(true);

            btnGenerate.Click += btnGenerate_Click;
            chkHide.Click += chkHide_Click;
        }

        void chkHide_Click(object sender, EventArgs e)
        {
            LoadDataGrid(chkHide.Checked);
        }

        void btnGenerate_Click(object sender, EventArgs e)
        {
            bool exists = true;
            while (exists)
            {
                string code = GenCode(cmbFor.SelectedIndex);
                exists = db.QueryDT("SELECT * FROM tbl_act_code WHERE _CODE = '" + code + "' AND _TYPE = '" + cmbFor.Text + "' AND _USED = 'NO'").RowCount() > 0;
                if (!exists)
                {
                    db.Query("INSERT INTO tbl_act_code SET _CODE = '" + code + "', _TYPE = '" + cmbFor.Text + "', _USED = 'NO'");
                    LoadDataGrid(chkHide.Checked);
                    break;
                }
            }
        }

        void LoadDataGrid(bool hideUsed)
        {
            Thread th = new Thread(new ThreadStart(delegate
            {
                try
                {
                    string extra = "";
                    if (hideUsed)
                        extra = " WHERE _USED = 'NO'";
                    data = db.QueryDT("SELECT * FROM tbl_act_code" + extra + " ORDER BY _USED");
                    if (dgvCode.Rows.Count > 0)
                        this.Invoke((MethodInvoker)delegate
                        {
                            dgvCode.Rows.Clear();
                        });

                    for (int i = 0; i < data.RowCount(); i++)
                    {
                        dgvCode.Rows.Add(data.ValueString("_ID", i),
                            data.ValueString("_CODE", i),
                            data.ValueString("_TYPE", i),
                            data.ValueString("_USER", i),
                            data.ValueString("_USED", i));
                    }
                }
                catch
                {
                    string extra = "";
                    if (hideUsed)
                        extra = " WHERE _USED = 'NO'";
                    data = db.QueryDT("SELECT * FROM tbl_act_code" + extra + " ORDER BY _USED");
                    if (dgvCode.Rows.Count > 0)
                        this.Invoke((MethodInvoker)delegate
                        {
                            dgvCode.Rows.Clear();
                        });

                    for (int i = 0; i < data.RowCount(); i++)
                    {
                        this.Invoke((MethodInvoker)delegate
                        {
                            dgvCode.Rows.Add(data.ValueString("_ID", i),
                                data.ValueString("_CODE", i),
                                data.ValueString("_TYPE", i),
                                data.ValueString("_USER", i),
                                data.ValueString("_USED", i));
                        });
                    }
                }
            }));
            th.Start();
        }

        public string GenCode(int type)
        {
            string output = "";
            switch (type)
            {
                case 0:
                    {
                        output = "10" + GetLetter;
                        break;
                    }
                case 1:
                    {
                        output = "20" + GetLetter;
                        break;
                    }
                case 2:
                    {
                        output = "30" + GetLetter;
                        break;
                    }
                case 3:
                    {
                        output = "40" + GetLetter;
                        break;
                    }
                case 4:
                    {
                        output = "50" + GetLetter;
                        break;
                    }
            }
            return output;
        }

        public static string GetLetter
        {
            get
            {
                string chars = "123457890ABCDEFGHJKMNPQRSTUVWXYZ";
                Random rand = new Random();
                int num = 0;
                string output = "";
                for (int i = 0; i < 6; i++){
                    num = rand.Next(0, chars.Length - 1);
                    output += chars[num];
                }
                return output;
            }
        }
    }
}
