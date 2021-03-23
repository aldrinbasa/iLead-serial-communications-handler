namespace SMS_Sender
{
    partial class form_generate
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.dgvCode = new System.Windows.Forms.DataGridView();
            this._ID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._CODE = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._TYPE = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._USER = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._USED = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbFor = new System.Windows.Forms.ComboBox();
            this.btnGenerate = new System.Windows.Forms.Button();
            this.chkHide = new System.Windows.Forms.CheckBox();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCode)).BeginInit();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.dgvCode, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(464, 561);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // dgvCode
            // 
            this.dgvCode.AllowUserToAddRows = false;
            this.dgvCode.AllowUserToDeleteRows = false;
            this.dgvCode.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgvCode.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvCode.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this._ID,
            this._CODE,
            this._TYPE,
            this._USER,
            this._USED});
            this.dgvCode.Location = new System.Drawing.Point(3, 43);
            this.dgvCode.Name = "dgvCode";
            this.dgvCode.ReadOnly = true;
            this.dgvCode.RowHeadersVisible = false;
            this.dgvCode.Size = new System.Drawing.Size(458, 515);
            this.dgvCode.TabIndex = 8;
            // 
            // _ID
            // 
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this._ID.DefaultCellStyle = dataGridViewCellStyle4;
            this._ID.HeaderText = "ID";
            this._ID.Name = "_ID";
            this._ID.ReadOnly = true;
            this._ID.Visible = false;
            this._ID.Width = 24;
            // 
            // _CODE
            // 
            this._CODE.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this._CODE.DefaultCellStyle = dataGridViewCellStyle5;
            this._CODE.HeaderText = "CODE";
            this._CODE.Name = "_CODE";
            this._CODE.ReadOnly = true;
            this._CODE.Width = 62;
            // 
            // _TYPE
            // 
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this._TYPE.DefaultCellStyle = dataGridViewCellStyle6;
            this._TYPE.HeaderText = "TYPE";
            this._TYPE.Name = "_TYPE";
            this._TYPE.ReadOnly = true;
            this._TYPE.Width = 55;
            // 
            // _USER
            // 
            this._USER.HeaderText = "USER";
            this._USER.Name = "_USER";
            this._USER.ReadOnly = true;
            this._USER.Width = 59;
            // 
            // _USED
            // 
            this._USED.HeaderText = "USED";
            this._USED.Name = "_USED";
            this._USED.ReadOnly = true;
            this._USED.Width = 60;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 4;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 33F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 262F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 77F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.cmbFor, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.btnGenerate, 2, 0);
            this.tableLayoutPanel2.Controls.Add(this.chkHide, 3, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(458, 34);
            this.tableLayoutPanel2.TabIndex = 9;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 3);
            this.label1.Margin = new System.Windows.Forms.Padding(3, 3, 0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "FOR";
            // 
            // cmbFor
            // 
            this.cmbFor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cmbFor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbFor.FormattingEnabled = true;
            this.cmbFor.Items.AddRange(new object[] {
            "DISTRIBUTOR",
            "DEALER",
            "MOBILE",
            "CITY",
            "PROVINCIAL"});
            this.cmbFor.Location = new System.Drawing.Point(36, 3);
            this.cmbFor.Name = "cmbFor";
            this.cmbFor.Size = new System.Drawing.Size(256, 21);
            this.cmbFor.TabIndex = 1;
            // 
            // btnGenerate
            // 
            this.btnGenerate.Location = new System.Drawing.Point(298, 3);
            this.btnGenerate.Name = "btnGenerate";
            this.btnGenerate.Size = new System.Drawing.Size(71, 23);
            this.btnGenerate.TabIndex = 2;
            this.btnGenerate.Text = "GENERATE";
            this.btnGenerate.UseVisualStyleBackColor = true;
            // 
            // chkHide
            // 
            this.chkHide.AutoSize = true;
            this.chkHide.Checked = true;
            this.chkHide.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkHide.Location = new System.Drawing.Point(375, 3);
            this.chkHide.Name = "chkHide";
            this.chkHide.Size = new System.Drawing.Size(79, 17);
            this.chkHide.TabIndex = 3;
            this.chkHide.Text = "Hide Used";
            this.chkHide.UseVisualStyleBackColor = true;
            // 
            // form_generate
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(464, 561);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "form_generate";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Generate";
            this.tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvCode)).EndInit();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.DataGridView dgvCode;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbFor;
        private System.Windows.Forms.Button btnGenerate;
        private System.Windows.Forms.CheckBox chkHide;
        private System.Windows.Forms.DataGridViewTextBoxColumn _ID;
        private System.Windows.Forms.DataGridViewTextBoxColumn _CODE;
        private System.Windows.Forms.DataGridViewTextBoxColumn _TYPE;
        private System.Windows.Forms.DataGridViewTextBoxColumn _USER;
        private System.Windows.Forms.DataGridViewTextBoxColumn _USED;
    }
}