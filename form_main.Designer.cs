namespace SMS_Sender
{
    partial class form_main
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            this.lblStatus = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.lblRunStat = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lblGlobe = new System.Windows.Forms.Label();
            this.lblSmart = new System.Windows.Forms.Label();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.dgvHelp = new System.Windows.Forms.DataGridView();
            this._ID2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._SENDER2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._CONCERN = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._DATETIME2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._STATUS = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.menStrip_help = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.stripReply = new System.Windows.Forms.ToolStripMenuItem();
            this.changeStatusToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stripResolved = new System.Windows.Forms.ToolStripMenuItem();
            this.stripFailed = new System.Windows.Forms.ToolStripMenuItem();
            this.stripPending = new System.Windows.Forms.ToolStripMenuItem();
            this.dgvSMS = new System.Windows.Forms.DataGridView();
            this._ID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._SENDER = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._CARRIER = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._MESSAGE = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.@__REFNO = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._DATETIME = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.menuToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stripAddPromo = new System.Windows.Forms.ToolStripMenuItem();
            this.stripGenerate = new System.Windows.Forms.ToolStripMenuItem();
            this.toggleSimToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stripToggleGLOBE = new System.Windows.Forms.ToolStripMenuItem();
            this.stripToggleSMART = new System.Windows.Forms.ToolStripMenuItem();
            this.stripAdjust = new System.Windows.Forms.ToolStripMenuItem();
            this.stripPorts = new System.Windows.Forms.ToolStripMenuItem();
            this.stripStart = new System.Windows.Forms.ToolStripMenuItem();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvHelp)).BeginInit();
            this.menStrip_help.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSMS)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblStatus
            // 
            this.lblStatus.Location = new System.Drawing.Point(3, 79);
            this.lblStatus.Multiline = true;
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.lblStatus.Size = new System.Drawing.Size(188, 277);
            this.lblStatus.TabIndex = 8;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 200F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel3, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 24);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 537F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(784, 537);
            this.tableLayoutPanel1.TabIndex = 9;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 1;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Controls.Add(this.lblStatus, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel4, 0, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 2;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 76F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(194, 531);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.ColumnCount = 2;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 47.42268F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 52.57732F));
            this.tableLayoutPanel4.Controls.Add(this.lblRunStat, 0, 0);
            this.tableLayoutPanel4.Controls.Add(this.label3, 0, 0);
            this.tableLayoutPanel4.Controls.Add(this.label1, 0, 1);
            this.tableLayoutPanel4.Controls.Add(this.label2, 0, 2);
            this.tableLayoutPanel4.Controls.Add(this.lblGlobe, 1, 1);
            this.tableLayoutPanel4.Controls.Add(this.lblSmart, 1, 2);
            this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel4.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel4.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 3;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(194, 76);
            this.tableLayoutPanel4.TabIndex = 9;
            // 
            // lblRunStat
            // 
            this.lblRunStat.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblRunStat.AutoSize = true;
            this.lblRunStat.Location = new System.Drawing.Point(94, 6);
            this.lblRunStat.Name = "lblRunStat";
            this.lblRunStat.Size = new System.Drawing.Size(26, 13);
            this.lblRunStat.TabIndex = 7;
            this.lblRunStat.Text = "N/A";
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(41, 6);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(47, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "STATUS:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(5, 31);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(83, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "GLOBE ACTIVE:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 56);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(85, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "SMART ACTIVE:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblGlobe
            // 
            this.lblGlobe.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblGlobe.AutoSize = true;
            this.lblGlobe.Location = new System.Drawing.Point(94, 31);
            this.lblGlobe.Name = "lblGlobe";
            this.lblGlobe.Size = new System.Drawing.Size(26, 13);
            this.lblGlobe.TabIndex = 10;
            this.lblGlobe.Text = "N/A";
            // 
            // lblSmart
            // 
            this.lblSmart.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblSmart.AutoSize = true;
            this.lblSmart.Location = new System.Drawing.Point(94, 56);
            this.lblSmart.Name = "lblSmart";
            this.lblSmart.Size = new System.Drawing.Size(26, 13);
            this.lblSmart.TabIndex = 11;
            this.lblSmart.Text = "N/A";
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 1;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Controls.Add(this.dgvHelp, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.dgvSMS, 0, 0);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(203, 3);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 2;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 240F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(578, 531);
            this.tableLayoutPanel3.TabIndex = 1;
            // 
            // dgvHelp
            // 
            this.dgvHelp.AllowUserToAddRows = false;
            this.dgvHelp.AllowUserToDeleteRows = false;
            this.dgvHelp.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvHelp.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this._ID2,
            this._SENDER2,
            this._CONCERN,
            this._DATETIME2,
            this._STATUS});
            this.dgvHelp.ContextMenuStrip = this.menStrip_help;
            this.dgvHelp.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvHelp.Location = new System.Drawing.Point(3, 243);
            this.dgvHelp.Name = "dgvHelp";
            this.dgvHelp.ReadOnly = true;
            this.dgvHelp.RowHeadersVisible = false;
            this.dgvHelp.Size = new System.Drawing.Size(572, 285);
            this.dgvHelp.TabIndex = 9;
            // 
            // _ID2
            // 
            this._ID2.HeaderText = "ID";
            this._ID2.Name = "_ID2";
            this._ID2.ReadOnly = true;
            this._ID2.Visible = false;
            // 
            // _SENDER2
            // 
            this._SENDER2.HeaderText = "SENDER";
            this._SENDER2.Name = "_SENDER2";
            this._SENDER2.ReadOnly = true;
            // 
            // _CONCERN
            // 
            this._CONCERN.HeaderText = "CONCERN";
            this._CONCERN.Name = "_CONCERN";
            this._CONCERN.ReadOnly = true;
            // 
            // _DATETIME2
            // 
            this._DATETIME2.HeaderText = "DATE TIME";
            this._DATETIME2.Name = "_DATETIME2";
            this._DATETIME2.ReadOnly = true;
            // 
            // _STATUS
            // 
            this._STATUS.HeaderText = "STATUS";
            this._STATUS.Name = "_STATUS";
            this._STATUS.ReadOnly = true;
            // 
            // menStrip_help
            // 
            this.menStrip_help.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.stripReply,
            this.changeStatusToolStripMenuItem});
            this.menStrip_help.Name = "menStrip_help";
            this.menStrip_help.Size = new System.Drawing.Size(151, 48);
            // 
            // stripReply
            // 
            this.stripReply.Name = "stripReply";
            this.stripReply.Size = new System.Drawing.Size(150, 22);
            this.stripReply.Text = "Reply";
            // 
            // changeStatusToolStripMenuItem
            // 
            this.changeStatusToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.stripResolved,
            this.stripFailed,
            this.stripPending});
            this.changeStatusToolStripMenuItem.Name = "changeStatusToolStripMenuItem";
            this.changeStatusToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
            this.changeStatusToolStripMenuItem.Text = "Change Status";
            // 
            // stripResolved
            // 
            this.stripResolved.Name = "stripResolved";
            this.stripResolved.Size = new System.Drawing.Size(121, 22);
            this.stripResolved.Text = "Resolved";
            // 
            // stripFailed
            // 
            this.stripFailed.Name = "stripFailed";
            this.stripFailed.Size = new System.Drawing.Size(121, 22);
            this.stripFailed.Text = "Failed";
            // 
            // stripPending
            // 
            this.stripPending.Name = "stripPending";
            this.stripPending.Size = new System.Drawing.Size(121, 22);
            this.stripPending.Text = "Pending";
            // 
            // dgvSMS
            // 
            this.dgvSMS.AllowUserToAddRows = false;
            this.dgvSMS.AllowUserToDeleteRows = false;
            this.dgvSMS.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvSMS.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this._ID,
            this._SENDER,
            this._CARRIER,
            this._MESSAGE,
            this.@__REFNO,
            this._DATETIME});
            this.dgvSMS.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvSMS.Location = new System.Drawing.Point(3, 3);
            this.dgvSMS.Name = "dgvSMS";
            this.dgvSMS.ReadOnly = true;
            this.dgvSMS.RowHeadersVisible = false;
            this.dgvSMS.Size = new System.Drawing.Size(572, 234);
            this.dgvSMS.TabIndex = 8;
            // 
            // _ID
            // 
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this._ID.DefaultCellStyle = dataGridViewCellStyle1;
            this._ID.HeaderText = "ID";
            this._ID.Name = "_ID";
            this._ID.ReadOnly = true;
            this._ID.Visible = false;
            // 
            // _SENDER
            // 
            this._SENDER.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this._SENDER.DefaultCellStyle = dataGridViewCellStyle2;
            this._SENDER.HeaderText = "SENDER";
            this._SENDER.Name = "_SENDER";
            this._SENDER.ReadOnly = true;
            this._SENDER.Width = 73;
            // 
            // _CARRIER
            // 
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this._CARRIER.DefaultCellStyle = dataGridViewCellStyle3;
            this._CARRIER.HeaderText = "CARRIER";
            this._CARRIER.Name = "_CARRIER";
            this._CARRIER.ReadOnly = true;
            // 
            // _MESSAGE
            // 
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this._MESSAGE.DefaultCellStyle = dataGridViewCellStyle4;
            this._MESSAGE.HeaderText = "MESSAGE";
            this._MESSAGE.Name = "_MESSAGE";
            this._MESSAGE.ReadOnly = true;
            // 
            // __REFNO
            // 
            this.@__REFNO.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.@__REFNO.DefaultCellStyle = dataGridViewCellStyle5;
            this.@__REFNO.HeaderText = "REF NO.";
            this.@__REFNO.Name = "__REFNO";
            this.@__REFNO.ReadOnly = true;
            this.@__REFNO.Width = 74;
            // 
            // _DATETIME
            // 
            this._DATETIME.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle6.Format = "yyyy-MM-dd hh:mm tt";
            this._DATETIME.DefaultCellStyle = dataGridViewCellStyle6;
            this._DATETIME.HeaderText = "DATE TIME";
            this._DATETIME.Name = "_DATETIME";
            this._DATETIME.ReadOnly = true;
            this._DATETIME.Width = 86;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuToolStripMenuItem,
            this.stripStart});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(784, 24);
            this.menuStrip1.TabIndex = 10;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // menuToolStripMenuItem
            // 
            this.menuToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.stripAddPromo,
            this.stripGenerate,
            this.toggleSimToolStripMenuItem,
            this.stripAdjust,
            this.stripPorts});
            this.menuToolStripMenuItem.Name = "menuToolStripMenuItem";
            this.menuToolStripMenuItem.Size = new System.Drawing.Size(50, 20);
            this.menuToolStripMenuItem.Text = "Menu";
            // 
            // stripAddPromo
            // 
            this.stripAddPromo.Name = "stripAddPromo";
            this.stripAddPromo.Size = new System.Drawing.Size(209, 22);
            this.stripAddPromo.Text = "Add Load/Promo";
            // 
            // stripGenerate
            // 
            this.stripGenerate.Name = "stripGenerate";
            this.stripGenerate.Size = new System.Drawing.Size(209, 22);
            this.stripGenerate.Text = "Generate Activation Code";
            // 
            // toggleSimToolStripMenuItem
            // 
            this.toggleSimToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.stripToggleGLOBE,
            this.stripToggleSMART});
            this.toggleSimToolStripMenuItem.Name = "toggleSimToolStripMenuItem";
            this.toggleSimToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
            this.toggleSimToolStripMenuItem.Text = "Toggle Sim";
            // 
            // stripToggleGLOBE
            // 
            this.stripToggleGLOBE.Name = "stripToggleGLOBE";
            this.stripToggleGLOBE.Size = new System.Drawing.Size(180, 22);
            this.stripToggleGLOBE.Text = "Toggle GLOBE";
            // 
            // stripToggleSMART
            // 
            this.stripToggleSMART.Name = "stripToggleSMART";
            this.stripToggleSMART.Size = new System.Drawing.Size(180, 22);
            this.stripToggleSMART.Text = "Toggle SMART";
            // 
            // stripAdjust
            // 
            this.stripAdjust.Name = "stripAdjust";
            this.stripAdjust.Size = new System.Drawing.Size(209, 22);
            this.stripAdjust.Text = "Adjust Percentage";
            // 
            // stripPorts
            // 
            this.stripPorts.Name = "stripPorts";
            this.stripPorts.Size = new System.Drawing.Size(209, 22);
            this.stripPorts.Text = "Configure Ports";
            // 
            // stripStart
            // 
            this.stripStart.BackColor = System.Drawing.Color.PaleGreen;
            this.stripStart.Name = "stripStart";
            this.stripStart.Size = new System.Drawing.Size(72, 20);
            this.stripStart.Text = "Start/Stop";
            // 
            // form_main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 561);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.menuStrip1);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.MainMenuStrip = this.menuStrip1;
            this.MinimumSize = new System.Drawing.Size(800, 600);
            this.Name = "form_main";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MAIN";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.tableLayoutPanel4.ResumeLayout(false);
            this.tableLayoutPanel4.PerformLayout();
            this.tableLayoutPanel3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvHelp)).EndInit();
            this.menStrip_help.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvSMS)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox lblStatus;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem menuToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem stripAddPromo;
        private System.Windows.Forms.ToolStripMenuItem stripGenerate;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.DataGridView dgvHelp;
        private System.Windows.Forms.ContextMenuStrip menStrip_help;
        private System.Windows.Forms.ToolStripMenuItem stripReply;
        private System.Windows.Forms.ToolStripMenuItem changeStatusToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem stripResolved;
        private System.Windows.Forms.ToolStripMenuItem stripFailed;
        private System.Windows.Forms.ToolStripMenuItem stripPending;
        private System.Windows.Forms.DataGridView dgvSMS;
        private System.Windows.Forms.DataGridViewTextBoxColumn _ID;
        private System.Windows.Forms.DataGridViewTextBoxColumn _SENDER;
        private System.Windows.Forms.DataGridViewTextBoxColumn _CARRIER;
        private System.Windows.Forms.DataGridViewTextBoxColumn _MESSAGE;
        private System.Windows.Forms.DataGridViewTextBoxColumn __REFNO;
        private System.Windows.Forms.DataGridViewTextBoxColumn _DATETIME;
        private System.Windows.Forms.DataGridViewTextBoxColumn _ID2;
        private System.Windows.Forms.DataGridViewTextBoxColumn _SENDER2;
        private System.Windows.Forms.DataGridViewTextBoxColumn _CONCERN;
        private System.Windows.Forms.DataGridViewTextBoxColumn _DATETIME2;
        private System.Windows.Forms.DataGridViewTextBoxColumn _STATUS;
        private System.Windows.Forms.ToolStripMenuItem toggleSimToolStripMenuItem;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.Label lblRunStat;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ToolStripMenuItem stripStart;
        private System.Windows.Forms.ToolStripMenuItem stripAdjust;
        private System.Windows.Forms.ToolStripMenuItem stripPorts;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblGlobe;
        private System.Windows.Forms.Label lblSmart;
        private System.Windows.Forms.ToolStripMenuItem stripToggleSMART;
        private System.Windows.Forms.ToolStripMenuItem stripToggleGLOBE;
    }
}

