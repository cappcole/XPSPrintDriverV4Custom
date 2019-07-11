namespace EclaimsPrint
{
    partial class SubmitForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SubmitForm));
            this.SubNumTextbox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.SubmitXPS = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.buttonCanel = new System.Windows.Forms.Button();
            this.ChangeButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // SubNumTextbox
            // 
            this.SubNumTextbox.Enabled = false;
            this.SubNumTextbox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SubNumTextbox.Location = new System.Drawing.Point(152, 78);
            this.SubNumTextbox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.SubNumTextbox.Name = "SubNumTextbox";
            this.SubNumTextbox.Size = new System.Drawing.Size(172, 28);
            this.SubNumTextbox.TabIndex = 0;
            this.SubNumTextbox.TextChanged += new System.EventHandler(this.SubNumTextbox_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(8, 78);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(134, 25);
            this.label1.TabIndex = 3;
            this.label1.Text = "SubNumber";
            // 
            // SubmitXPS
            // 
            this.SubmitXPS.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.SubmitXPS.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SubmitXPS.ForeColor = System.Drawing.Color.Black;
            this.SubmitXPS.Location = new System.Drawing.Point(152, 165);
            this.SubmitXPS.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.SubmitXPS.Name = "SubmitXPS";
            this.SubmitXPS.Size = new System.Drawing.Size(172, 48);
            this.SubmitXPS.TabIndex = 7;
            this.SubmitXPS.Text = "&Submit Claim";
            this.SubmitXPS.UseVisualStyleBackColor = false;
            this.SubmitXPS.Click += new System.EventHandler(this.SubmitXPS_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // buttonCanel
            // 
            this.buttonCanel.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.buttonCanel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCanel.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonCanel.ForeColor = System.Drawing.Color.Black;
            this.buttonCanel.Location = new System.Drawing.Point(152, 282);
            this.buttonCanel.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.buttonCanel.Name = "buttonCanel";
            this.buttonCanel.Size = new System.Drawing.Size(172, 48);
            this.buttonCanel.TabIndex = 8;
            this.buttonCanel.Text = "&Cancel";
            this.buttonCanel.UseVisualStyleBackColor = false;
            this.buttonCanel.Click += new System.EventHandler(this.buttonCanel_Click);
            // 
            // ChangeButton
            // 
            this.ChangeButton.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.ChangeButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ChangeButton.ForeColor = System.Drawing.Color.Black;
            this.ChangeButton.Location = new System.Drawing.Point(152, 223);
            this.ChangeButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.ChangeButton.Name = "ChangeButton";
            this.ChangeButton.Size = new System.Drawing.Size(172, 48);
            this.ChangeButton.TabIndex = 9;
            this.ChangeButton.Text = "&Change";
            this.ChangeButton.UseVisualStyleBackColor = false;
            this.ChangeButton.Click += new System.EventHandler(this.ChangeButton_Click);
            // 
            // SubmitForm
            // 
            this.AcceptButton = this.SubmitXPS;
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.CancelButton = this.buttonCanel;
            this.ClientSize = new System.Drawing.Size(478, 344);
            this.Controls.Add(this.ChangeButton);
            this.Controls.Add(this.buttonCanel);
            this.Controls.Add(this.SubmitXPS);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.SubNumTextbox);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SubmitForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "IAP E-Claim Submit";
            this.Load += new System.EventHandler(this.SubmitForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox SubNumTextbox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.Button SubmitXPS;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button buttonCanel;
        private System.Windows.Forms.Button ChangeButton;
    }
}

