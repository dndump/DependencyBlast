namespace DBlast
{
    partial class Main
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.btnBrowseInput = new System.Windows.Forms.Button();
            this.txtInput = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnGenerate = new System.Windows.Forms.Button();
            this.chkAutoSendEmails = new System.Windows.Forms.CheckBox();
            this.clstOwners = new System.Windows.Forms.CheckedListBox();
            this.lblCreateFor = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnBrowseInput
            // 
            this.btnBrowseInput.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBrowseInput.Location = new System.Drawing.Point(265, 19);
            this.btnBrowseInput.Margin = new System.Windows.Forms.Padding(2);
            this.btnBrowseInput.Name = "btnBrowseInput";
            this.btnBrowseInput.Size = new System.Drawing.Size(76, 24);
            this.btnBrowseInput.TabIndex = 0;
            this.btnBrowseInput.TabStop = false;
            this.btnBrowseInput.Text = "Browse...";
            this.btnBrowseInput.UseVisualStyleBackColor = true;
            this.btnBrowseInput.Click += new System.EventHandler(this.btnBrowseInput_Click);
            // 
            // txtInput
            // 
            this.txtInput.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtInput.Location = new System.Drawing.Point(11, 22);
            this.txtInput.Margin = new System.Windows.Forms.Padding(2);
            this.txtInput.Name = "txtInput";
            this.txtInput.Size = new System.Drawing.Size(246, 21);
            this.txtInput.TabIndex = 2;
            this.txtInput.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Tahoma", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(11, 9);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(195, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Read dependencies from this Excel file:";
            // 
            // btnGenerate
            // 
            this.btnGenerate.Enabled = false;
            this.btnGenerate.Location = new System.Drawing.Point(11, 374);
            this.btnGenerate.Name = "btnGenerate";
            this.btnGenerate.Size = new System.Drawing.Size(330, 27);
            this.btnGenerate.TabIndex = 1;
            this.btnGenerate.Text = "Do it.";
            this.btnGenerate.UseVisualStyleBackColor = true;
            this.btnGenerate.Click += new System.EventHandler(this.btnGenerate_Click);
            // 
            // chkAutoSendEmails
            // 
            this.chkAutoSendEmails.AutoSize = true;
            this.chkAutoSendEmails.Location = new System.Drawing.Point(11, 346);
            this.chkAutoSendEmails.Name = "chkAutoSendEmails";
            this.chkAutoSendEmails.Size = new System.Drawing.Size(148, 17);
            this.chkAutoSendEmails.TabIndex = 4;
            this.chkAutoSendEmails.Text = "Automatically send emails";
            this.chkAutoSendEmails.UseVisualStyleBackColor = true;
            this.chkAutoSendEmails.CheckedChanged += new System.EventHandler(this.chkAutoSendEmails_CheckedChanged);
            // 
            // clstOwners
            // 
            this.clstOwners.CheckOnClick = true;
            this.clstOwners.FormattingEnabled = true;
            this.clstOwners.Location = new System.Drawing.Point(11, 80);
            this.clstOwners.Name = "clstOwners";
            this.clstOwners.Size = new System.Drawing.Size(330, 260);
            this.clstOwners.Sorted = true;
            this.clstOwners.TabIndex = 5;
            this.clstOwners.MouseDown += new System.Windows.Forms.MouseEventHandler(this.clstOwners_MouseDown);
            // 
            // lblCreateFor
            // 
            this.lblCreateFor.AutoSize = true;
            this.lblCreateFor.Location = new System.Drawing.Point(8, 64);
            this.lblCreateFor.Name = "lblCreateFor";
            this.lblCreateFor.Size = new System.Drawing.Size(214, 13);
            this.lblCreateFor.TabIndex = 6;
            this.lblCreateFor.Text = "Create notification emails for these people:";
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(351, 413);
            this.Controls.Add(this.lblCreateFor);
            this.Controls.Add(this.clstOwners);
            this.Controls.Add(this.chkAutoSendEmails);
            this.Controls.Add(this.btnGenerate);
            this.Controls.Add(this.btnBrowseInput);
            this.Controls.Add(this.txtInput);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Main";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Dependency Blast";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnBrowseInput;
        private System.Windows.Forms.TextBox txtInput;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnGenerate;
        private System.Windows.Forms.CheckBox chkAutoSendEmails;
        private System.Windows.Forms.CheckedListBox clstOwners;
        private System.Windows.Forms.Label lblCreateFor;

    }
}

