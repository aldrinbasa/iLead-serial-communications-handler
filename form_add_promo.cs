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
    public partial class form_add_promo : Form
    {
        DatabaseHandler db = new DatabaseHandler();

        public form_add_promo()
        {
            InitializeComponent();

            //EVENT INITIALIZATION
            cmbNetwork.SelectedValueChanged += cmbNetwork_SelectedValueChanged;
            dgvCodes.CellClick += dgvCodes_CellClick;
            btnAdd.Click += btnAdd_Click;


            //SET DEFAULTS HERE
            cmbNetwork.SelectedIndex = 0;
            using (DataGridViewButtonColumn btn = new DataGridViewButtonColumn())
            {
                btn.Text = "Edit";
                btn.Name = "EDIT";
                btn.HeaderText = "";
                btn.UseColumnTextForButtonValue = true;
                btn.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                dgvCodes.Columns.Add(btn);
            }
            using (DataGridViewButtonColumn btn = new DataGridViewButtonColumn())
            {
                btn.Text = " X ";
                btn.Name = "DELETE";
                btn.HeaderText = "";
                btn.UseColumnTextForButtonValue = true;
                btn.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                dgvCodes.Columns.Add(btn);
            }
            LoadDataGrid();
        }

        void btnAdd_Click(object sender, EventArgs e)
        {
            string keyword = txtKeyword.Text.Trim();

            if (keyword != "" || keyword != null)
                keyword += " {0}";

            db.Query("INSERT INTO tbl_codes SET _LOAD = '" + txtDesc.Text.Trim() +
                "', _CODE = '" + txtCode.Text.Trim() +
                "', _SRP = '" + nudSRP.Value.ToString() + 
                "', _CARRIER = '" + cmbNetwork.Text.Trim() + 
                "', _ACTUAL_CODE = '" + txtActualCode.Text.Trim() +
                "', _USSD_CODE ='" + txtUssd.Text.Trim() +
                "', _KEYWORD = '" + keyword + "'");

            txtDesc.Clear();
            txtActualCode.Clear();
            txtKeyword.Clear();
            txtUssd.Clear();
            txtCode.Clear();

            LoadDataGrid();
        }

        void dgvCodes_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 9)
            {
                if (MessageBox.Show("Are you sure you want to delete code \"" + dgvCodes.ValueString("_CODE", e.RowIndex) + "\" from \"" + dgvCodes.ValueString("_CARRIER", e.RowIndex) + "\"?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    db.Query("DELETE FROM tbl_codes WHERE _ID = '" + dgvCodes.ValueString("_ID", e.RowIndex) + "'");
                    LoadDataGrid();
                }
            }
            else if (e.ColumnIndex == 8)
            {
                form_edit_promo fep = new form_edit_promo(dgvCodes.ValueString("_ID", e.RowIndex));
                fep.ShowDialog();
                LoadDataGrid();
            }
        }

        void cmbNetwork_SelectedValueChanged(object sender, EventArgs e)
        {
            txtKeyword.Enabled = cmbNetwork.SelectedIndex != 2;
            txtUssd.Enabled = cmbNetwork.SelectedIndex == 2;
        }

        void LoadDataGrid()
        {
            Thread th = new Thread(new ThreadStart(delegate
            {
                if (dgvCodes.Rows.Count > 0)
                    this.Invoke((MethodInvoker)delegate
                    {
                        dgvCodes.Rows.Clear();
                    });

                try
                {
                    using (DataTable dt = db.QueryDT("SELECT * FROM tbl_codes ORDER BY _CARRIER, _SRP ASC"))
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            string ussd = dt.ValueString(6, i);
                            try
                            {
                                ussd = ussd.Remove(ussd.Length - 1);
                            }
                            catch { }

                            dgvCodes.Rows.Add(dt.ValueString(0, i),
                                dt.ValueString(1, i),
                                dt.ValueString(2, i),
                                dt.ValueString(3, i),
                                dt.ValueString(4, i),
                                dt.ValueString(5, i),
                                ussd,
                                dt.ValueString(7, i).Replace(" {0}", ""));
                        }
                    }
                }
                catch
                {
                    if (dgvCodes.Rows.Count > 0)
                        this.Invoke((MethodInvoker)delegate
                        {
                            dgvCodes.Rows.Clear();
                        });

                    using (DataTable dt = db.QueryDT("SELECT * FROM tbl_codes ORDER BY _CARRIER, _SRP ASC"))
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            string ussd = dt.ValueString(6, i);
                            try
                            {
                                ussd = ussd.Remove(ussd.Length - 1);
                            }
                            catch { }
                            this.Invoke((MethodInvoker)delegate
                            {
                                dgvCodes.Rows.Add(dt.ValueString(0, i),
                                    dt.ValueString(1, i),
                                    dt.ValueString(2, i),
                                    dt.ValueString(3, i),
                                    dt.ValueString(4, i),
                                    dt.ValueString(5, i),
                                    ussd,
                                    dt.ValueString(7, i).Replace(" {0}", ""));
                            });
                        }
                    }
                }
            }));
            th.Start();
        }
    }
}
