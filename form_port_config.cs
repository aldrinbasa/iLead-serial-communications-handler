using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SMS_Sender
{
    public partial class form_port_config : Form
    {
        DatabaseHandler db = new DatabaseHandler();
        List<string> coms = new List<string>();

        public form_port_config()
        {
            InitializeComponent();

            btnTest1.Click += test_Click;
            btnTest2.Click += test_Click;
            btnTest3.Click += test_Click;
            btnTest4.Click += test_Click;
            btnTest5.Click += test_Click;
            btnTest6.Click += test_Click;
            btnTest7.Click += test_Click;
            btnTest8.Click += test_Click;
            btnApply.Click += btnApply_Click;

            loadData();
            initPorts();
            initCMB();
        }

        void btnApply_Click(object sender, EventArgs e)
        {
            db.Query("UPDATE tbl_config SET _VALUE = '" + cmb1.Text + "' WHERE _CONFIG = 'PORT_1'");
            db.Query("UPDATE tbl_config SET _VALUE = '" + cmb2.Text + "' WHERE _CONFIG = 'PORT_2'");
            db.Query("UPDATE tbl_config SET _VALUE = '" + cmb3.Text + "' WHERE _CONFIG = 'PORT_3'");
            db.Query("UPDATE tbl_config SET _VALUE = '" + cmb4.Text + "' WHERE _CONFIG = 'PORT_4'");
            db.Query("UPDATE tbl_config SET _VALUE = '" + cmb5.Text + "' WHERE _CONFIG = 'PORT_5'");
            db.Query("UPDATE tbl_config SET _VALUE = '" + cmb6.Text + "' WHERE _CONFIG = 'PORT_6'");
            db.Query("UPDATE tbl_config SET _VALUE = '" + cmb7.Text + "' WHERE _CONFIG = 'PORT_7'");
            db.Query("UPDATE tbl_config SET _VALUE = '" + cmb8.Text + "' WHERE _CONFIG = 'PORT_8'");
            MessageBox.Show("Successfully Updated!");
            this.Close();
        }

        void test_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            Button btn = sender as Button;
            var nbr = int.Parse(btn.Name[btn.Name.Length - 1].ToString());
            var port = "";

            if (nbr == 1)
                port = cmb1.Text;
            else if (nbr == 2)
                port = cmb2.Text;
            else if (nbr == 3)
                port = cmb3.Text;
            else if (nbr == 4)
                port = cmb4.Text;
            else if (nbr == 5)
                port = cmb5.Text;
            else if (nbr == 6)
                port = cmb6.Text;
            else if (nbr == 7)
                port = cmb7.Text;
            else if (nbr == 8)
                port = cmb8.Text;

            try
            {
                using (SerialPort sp = new SerialPort(port, 115200))
                {
                    sp.Open();
                    sp.Write("AT+CNMI=1,2,0,0,0\r");
                    Thread.Sleep(100);
                    sp.NewLine = Environment.NewLine;
                    sp.Write("AT\r\n");
                    Thread.Sleep(100);
                    sp.Write("AT+CNUM\r");
                    Thread.Sleep(100);
                    sp.Write("AT\r\n");
                    Thread.Sleep(100);
                    sp.Write("AT+CMGF=1\r\n");
                    Thread.Sleep(100);
                    sp.Write("AT+CMGL=\"ALL\"\r\n");
                    Thread.Sleep(100);
                    string existing = sp.ReadExisting();
                    MessageBox.Show(existing.Split(':')[1].Split('"')[3]);
                    sp.Close();
                    sp.Dispose();
                }
            }
            catch (Exception ex)
            {
                if (!ex.Message.Contains("bounds"))
                    MessageBox.Show(ex.Message);
                else
                    MessageBox.Show("PORT EMPTY");
            }
        }

        void loadData()
        {
            var tmpDat = db.QueryDT("SELECT * FROM tbl_config");
            cmb1.Text = tmpDat.ValueString("_VALUE", 4);
            cmb2.Text = tmpDat.ValueString("_VALUE", 5);
            cmb3.Text = tmpDat.ValueString("_VALUE", 6);
            cmb4.Text = tmpDat.ValueString("_VALUE", 7);
            cmb5.Text = tmpDat.ValueString("_VALUE", 8);
            cmb6.Text = tmpDat.ValueString("_VALUE", 9);
            cmb7.Text = tmpDat.ValueString("_VALUE", 10);
            cmb8.Text = tmpDat.ValueString("_VALUE", 11);
        }

        void initCMB()
        {
            for (int i = 0; i < coms.Count; i++)
            {
                cmb1.Items.Add(coms[i]);
                cmb2.Items.Add(coms[i]);
                cmb3.Items.Add(coms[i]);
                cmb4.Items.Add(coms[i]);
                cmb5.Items.Add(coms[i]);
                cmb6.Items.Add(coms[i]);
                cmb7.Items.Add(coms[i]);
                cmb8.Items.Add(coms[i]);
            }
        }

        //INITALIZE ALL PORTS
        void initPorts()
        {
            ManagementObjectCollection ManObjReturn;
            ManagementObjectSearcher ManObjSearch;
            ManObjSearch = new ManagementObjectSearcher("Select * from Win32_PnPEntity WHERE Name LIKE '%XR21V1414 USB UART Ch%'");
            ManObjReturn = ManObjSearch.Get();
            List<string> tempList = new List<string>();

            foreach (ManagementObject ManObj in ManObjReturn)
            {
                //int s = ManObj.Properties.Count;
                //foreach (PropertyData d in ManObj.Properties)
                //{
                //    MessageBox.Show(d.Name);
                //}
                tempList.Add(ManObj["Name"].ToString());
                //try
                //{
                //    if (ManObj["Name"].ToString().Contains("XR21V1414 USB UART Ch"))
                //    {
                //        tempList.Add(ManObj["Name"].ToString());
                //    }
                //}
                //catch(Exception e)
                //{
                //    MessageBox.Show(e.Message);
                //}
            }

            tempList.Sort();

            for (int i = 0; i < tempList.Count; i++)
            {
                var temp = tempList[i].ToString().Split(' ');
                coms.Add(temp[temp.Length - 1].Replace("(", "").Replace(")", ""));
            }
        }
    }
}
