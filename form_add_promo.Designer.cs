namespace SMS_Sender
{
    partial class form_add_promo
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtDesc = new System.Windows.Forms.TextBox();
            this.txtActualCode = new System.Windows.Forms.TextBox();
            this.txtUssd = new System.Windows.Forms.TextBox();
            this.txtKeyword = new System.Windows.Forms.TextBox();
            this.cmbNetwork = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.nudSRP = new System.Windows.Forms.NumericUpDown();
            this.btnAdd = new System.Windows.Forms.Button();
            this.txtCode = new System.Windows.Forms.TextBox();
            this.dgvCodes = new System.Windows.Forms.DataGridView();
            this._ID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._LOAD = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._CODE = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._SRP = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._CARRIER = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._ACTUAL_CODE = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._USSD_CODE = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._KEYWORD = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudSRP)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCodes)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 29.52489F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 70.47511F));
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.dgvCodes, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(884, 261);
            this.tableLayoutPanel1.TabIndex = 10;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 180F));
            this.tableLayoutPanel2.Controls.Add(this.label7, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.label6, 0, 6);
            this.tableLayoutPanel2.Controls.Add(this.label5, 0, 5);
            this.tableLayoutPanel2.Controls.Add(this.label4, 0, 4);
            this.tableLayoutPanel2.Controls.Add(this.label3, 0, 3);
            this.tableLayoutPanel2.Controls.Add(this.label2, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.txtDesc, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.txtActualCode, 1, 4);
            this.tableLayoutPanel2.Controls.Add(this.txtUssd, 1, 5);
            this.tableLayoutPanel2.Controls.Add(this.txtKeyword, 1, 6);
            this.tableLayoutPanel2.Controls.Add(this.cmbNetwork, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.nudSRP, 1, 3);
            this.tableLayoutPanel2.Controls.Add(this.btnAdd, 1, 7);
            this.tableLayoutPanel2.Controls.Add(this.txtCode, 1, 2);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 8;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(255, 255);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label7.Location = new System.Drawing.Point(3, 56);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(69, 28);
            this.label7.TabIndex = 14;
            this.label7.Text = "Code";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label6.Location = new System.Drawing.Point(3, 168);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(69, 28);
            this.label6.TabIndex = 11;
            this.label6.Text = "Keyword";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label5.Location = new System.Drawing.Point(3, 140);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(69, 28);
            this.label5.TabIndex = 10;
            this.label5.Text = "USSD Code";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label4.Location = new System.Drawing.Point(3, 112);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(69, 28);
            this.label4.TabIndex = 9;
            this.label4.Text = "Actual Code";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label3.Location = new System.Drawing.Point(3, 84);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(69, 28);
            this.label3.TabIndex = 8;
            this.label3.Text = "SRP";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Location = new System.Drawing.Point(3, 28);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(69, 28);
            this.label2.TabIndex = 7;
            this.label2.Text = "Description";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtDesc
            // 
            this.txtDesc.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtDesc.Location = new System.Drawing.Point(78, 31);
            this.txtDesc.Name = "txtDesc";
            this.txtDesc.Size = new System.Drawing.Size(174, 22);
            this.txtDesc.TabIndex = 1;
            // 
            // txtActualCode
            // 
            this.txtActualCode.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtActualCode.Location = new System.Drawing.Point(78, 115);
            this.txtActualCode.Name = "txtActualCode";
            this.txtActualCode.Size = new System.Drawing.Size(174, 22);
            this.txtActualCode.TabIndex = 4;
            // 
            // txtUssd
            // 
            this.txtUssd.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtUssd.Location = new System.Drawing.Point(78, 143);
            this.txtUssd.Name = "txtUssd";
            this.txtUssd.Size = new System.Drawing.Size(174, 22);
            this.txtUssd.TabIndex = 5;
            // 
            // txtKeyword
            // 
            this.txtKeyword.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtKeyword.Location = new System.Drawing.Point(78, 171);
            this.txtKeyword.Name = "txtKeyword";
            this.txtKeyword.Size = new System.Drawing.Size(174, 22);
            this.txtKeyword.TabIndex = 6;
            // 
            // cmbNetwork
            // 
            this.cmbNetwork.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cmbNetwork.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbNetwork.FormattingEnabled = true;
            this.cmbNetwork.Items.AddRange(new object[] {
            "Smart",
            "TNT",
            "Globe/TM",
            "PLDT",
            "CIGNAL"});
            this.cmbNetwork.Location = new System.Drawing.Point(78, 3);
            this.cmbNetwork.Name = "cmbNetwork";
            this.cmbNetwork.Size = new System.Drawing.Size(174, 21);
            this.cmbNetwork.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(69, 28);
            this.label1.TabIndex = 6;
            this.label1.Text = "Network";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // nudSRP
            // 
            this.nudSRP.Increment = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.nudSRP.Location = new System.Drawing.Point(78, 87);
            this.nudSRP.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.nudSRP.Name = "nudSRP";
            this.nudSRP.Size = new System.Drawing.Size(57, 22);
            this.nudSRP.TabIndex = 3;
            this.nudSRP.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.nudSRP.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // btnAdd
            // 
            this.btnAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAdd.Location = new System.Drawing.Point(177, 199);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(75, 23);
            this.btnAdd.TabIndex = 7;
            this.btnAdd.Text = "ADD";
            this.btnAdd.UseVisualStyleBackColor = true;
            // 
            // txtCode
            // 
            this.txtCode.Location = new System.Drawing.Point(78, 59);
            this.txtCode.Name = "txtCode";
            this.txtCode.Size = new System.Drawing.Size(174, 22);
            this.txtCode.TabIndex = 2;
            // 
            // dgvCodes
            // 
            this.dgvCodes.AllowUserToAddRows = false;
            this.dgvCodes.AllowUserToDeleteRows = false;
            this.dgvCodes.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dgvCodes.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvCodes.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this._ID,
            this._LOAD,
            this._CODE,
            this._SRP,
            this._CARRIER,
            this._ACTUAL_CODE,
            this._USSD_CODE,
            this._KEYWORD});
            this.dgvCodes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvCodes.Location = new System.Drawing.Point(264, 3);
            this.dgvCodes.Name = "dgvCodes";
            this.dgvCodes.ReadOnly = true;
            this.dgvCodes.RowHeadersVisible = false;
            this.dgvCodes.Size = new System.Drawing.Size(617, 255);
            this.dgvCodes.TabIndex = 7;
            // 
            // _ID
            // 
            this._ID.HeaderText = "ID";
            this._ID.Name = "_ID";
            this._ID.ReadOnly = true;
            this._ID.Visible = false;
            // 
            // _LOAD
            // 
            this._LOAD.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this._LOAD.HeaderText = "DESCRIPTION";
            this._LOAD.Name = "_LOAD";
            this._LOAD.ReadOnly = true;
            this._LOAD.Width = 101;
            // 
            // _CODE
            // 
            this._CODE.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this._CODE.HeaderText = "CODE";
            this._CODE.Name = "_CODE";
            this._CODE.ReadOnly = true;
            this._CODE.Width = 62;
            // 
            // _SRP
            // 
            this._SRP.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this._SRP.HeaderText = "SRP";
            this._SRP.Name = "_SRP";
            this._SRP.ReadOnly = true;
            this._SRP.Width = 51;
            // 
            // _CARRIER
            // 
            this._CARRIER.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this._CARRIER.HeaderText = "CARRIER";
            this._CARRIER.Name = "_CARRIER";
            this._CARRIER.ReadOnly = true;
            this._CARRIER.Width = 76;
            // 
            // _ACTUAL_CODE
            // 
            this._ACTUAL_CODE.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this._ACTUAL_CODE.HeaderText = "ACTUAL CODE";
            this._ACTUAL_CODE.Name = "_ACTUAL_CODE";
            this._ACTUAL_CODE.ReadOnly = true;
            this._ACTUAL_CODE.Width = 96;
            // 
            // _USSD_CODE
            // 
            this._USSD_CODE.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this._USSD_CODE.HeaderText = "USSD CODE";
            this._USSD_CODE.Name = "_USSD_CODE";
            this._USSD_CODE.ReadOnly = true;
            this._USSD_CODE.Width = 86;
            // 
            // _KEYWORD
            // 
            this._KEYWORD.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this._KEYWORD.HeaderText = "KEYWORD";
            this._KEYWORD.Name = "_KEYWORD";
            this._KEYWORD.ReadOnly = true;
            this._KEYWORD.Width = 84;
            // 
            // form_add_promo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(884, 261);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "form_add_promo";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Add Promo";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudSRP)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCodes)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.DataGridView dgvCodes;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtDesc;
        private System.Windows.Forms.TextBox txtActualCode;
        private System.Windows.Forms.TextBox txtUssd;
        private System.Windows.Forms.TextBox txtKeyword;
        private System.Windows.Forms.ComboBox cmbNetwork;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown nudSRP;
        private System.Windows.Forms.DataGridViewTextBoxColumn _ID;
        private System.Windows.Forms.DataGridViewTextBoxColumn _LOAD;
        private System.Windows.Forms.DataGridViewTextBoxColumn _CODE;
        private System.Windows.Forms.DataGridViewTextBoxColumn _SRP;
        private System.Windows.Forms.DataGridViewTextBoxColumn _CARRIER;
        private System.Windows.Forms.DataGridViewTextBoxColumn _ACTUAL_CODE;
        private System.Windows.Forms.DataGridViewTextBoxColumn _USSD_CODE;
        private System.Windows.Forms.DataGridViewTextBoxColumn _KEYWORD;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtCode;
    }
}