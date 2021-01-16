namespace SMS_Sender
{
    partial class form_adjust
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.btnAdjust = new System.Windows.Forms.Button();
            this.nudTransfer = new System.Windows.Forms.NumericUpDown();
            this.nudLoad = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.nudTransfer)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudLoad)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 36);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(49, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Loading";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 64);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(76, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Transfer Load";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(161, 36);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(16, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "%";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(161, 64);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(16, 13);
            this.label4.TabIndex = 5;
            this.label4.Text = "%";
            // 
            // btnAdjust
            // 
            this.btnAdjust.Location = new System.Drawing.Point(56, 102);
            this.btnAdjust.Name = "btnAdjust";
            this.btnAdjust.Size = new System.Drawing.Size(75, 23);
            this.btnAdjust.TabIndex = 6;
            this.btnAdjust.Text = "Adjust";
            this.btnAdjust.UseVisualStyleBackColor = true;
            // 
            // nudTransfer
            // 
            this.nudTransfer.DecimalPlaces = 1;
            this.nudTransfer.Location = new System.Drawing.Point(94, 62);
            this.nudTransfer.Name = "nudTransfer";
            this.nudTransfer.Size = new System.Drawing.Size(61, 22);
            this.nudTransfer.TabIndex = 2;
            this.nudTransfer.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.nudTransfer.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // nudLoad
            // 
            this.nudLoad.DecimalPlaces = 1;
            this.nudLoad.Location = new System.Drawing.Point(94, 34);
            this.nudLoad.Name = "nudLoad";
            this.nudLoad.Size = new System.Drawing.Size(61, 22);
            this.nudLoad.TabIndex = 0;
            this.nudLoad.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.nudLoad.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // form_adjust
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(189, 137);
            this.Controls.Add(this.btnAdjust);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.nudTransfer);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.nudLoad);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "form_adjust";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Adjust";
            ((System.ComponentModel.ISupportInitialize)(this.nudTransfer)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudLoad)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnAdjust;
        private System.Windows.Forms.NumericUpDown nudTransfer;
        private System.Windows.Forms.NumericUpDown nudLoad;
    }
}