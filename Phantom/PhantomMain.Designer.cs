namespace Phantom
{
    partial class PhantomMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PhantomMain));
            this.label1 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.openButton = new System.Windows.Forms.Button();
            this.antiDebug = new System.Windows.Forms.CheckBox();
            this.buildButton = new System.Windows.Forms.Button();
            this.selfDelete = new System.Windows.Forms.CheckBox();
            this.hidden = new System.Windows.Forms.CheckBox();
            this.antiVM = new System.Windows.Forms.CheckBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.optionsPage = new System.Windows.Forms.TabPage();
            this.uacBypass = new System.Windows.Forms.CheckBox();
            this.startup = new System.Windows.Forms.CheckBox();
            this.runas = new System.Windows.Forms.CheckBox();
            this.binderPage = new System.Windows.Forms.TabPage();
            this.removeFile = new System.Windows.Forms.Button();
            this.addFile = new System.Windows.Forms.Button();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.outputPage = new System.Windows.Forms.TabPage();
            this.listBox2 = new System.Windows.Forms.ListBox();
            this.tabControl1.SuspendLayout();
            this.optionsPage.SuspendLayout();
            this.binderPage.SuspendLayout();
            this.outputPage.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(14, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(112, 29);
            this.label1.TabIndex = 0;
            this.label1.Text = "File path:";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(19, 56);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(748, 31);
            this.textBox1.TabIndex = 1;
            // 
            // openButton
            // 
            this.openButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.openButton.Location = new System.Drawing.Point(776, 54);
            this.openButton.Name = "openButton";
            this.openButton.Size = new System.Drawing.Size(120, 40);
            this.openButton.TabIndex = 2;
            this.openButton.Text = "...";
            this.openButton.UseVisualStyleBackColor = true;
            this.openButton.Click += new System.EventHandler(this.openButton_Click);
            // 
            // antiDebug
            // 
            this.antiDebug.AutoSize = true;
            this.antiDebug.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.antiDebug.Location = new System.Drawing.Point(6, 6);
            this.antiDebug.Name = "antiDebug";
            this.antiDebug.Size = new System.Drawing.Size(163, 33);
            this.antiDebug.TabIndex = 6;
            this.antiDebug.Text = "Anti Debug";
            this.antiDebug.UseVisualStyleBackColor = true;
            // 
            // buildButton
            // 
            this.buildButton.Location = new System.Drawing.Point(18, 574);
            this.buildButton.Name = "buildButton";
            this.buildButton.Size = new System.Drawing.Size(880, 77);
            this.buildButton.TabIndex = 7;
            this.buildButton.Text = "Build";
            this.buildButton.UseVisualStyleBackColor = true;
            this.buildButton.Click += new System.EventHandler(this.buildButton_Click);
            // 
            // selfDelete
            // 
            this.selfDelete.AutoSize = true;
            this.selfDelete.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.selfDelete.Location = new System.Drawing.Point(6, 90);
            this.selfDelete.Name = "selfDelete";
            this.selfDelete.Size = new System.Drawing.Size(129, 33);
            this.selfDelete.TabIndex = 8;
            this.selfDelete.Text = "Melt file";
            this.selfDelete.UseVisualStyleBackColor = true;
            // 
            // hidden
            // 
            this.hidden.AutoSize = true;
            this.hidden.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.hidden.Location = new System.Drawing.Point(6, 129);
            this.hidden.Name = "hidden";
            this.hidden.Size = new System.Drawing.Size(187, 33);
            this.hidden.TabIndex = 9;
            this.hidden.Text = "Hide console";
            this.hidden.UseVisualStyleBackColor = true;
            // 
            // antiVM
            // 
            this.antiVM.AutoSize = true;
            this.antiVM.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.antiVM.Location = new System.Drawing.Point(6, 48);
            this.antiVM.Name = "antiVM";
            this.antiVM.Size = new System.Drawing.Size(126, 33);
            this.antiVM.TabIndex = 10;
            this.antiVM.Text = "Anti VM";
            this.antiVM.UseVisualStyleBackColor = true;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.optionsPage);
            this.tabControl1.Controls.Add(this.binderPage);
            this.tabControl1.Controls.Add(this.outputPage);
            this.tabControl1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabControl1.Location = new System.Drawing.Point(19, 104);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(877, 464);
            this.tabControl1.TabIndex = 11;
            // 
            // optionsPage
            // 
            this.optionsPage.Controls.Add(this.uacBypass);
            this.optionsPage.Controls.Add(this.startup);
            this.optionsPage.Controls.Add(this.runas);
            this.optionsPage.Controls.Add(this.hidden);
            this.optionsPage.Controls.Add(this.selfDelete);
            this.optionsPage.Controls.Add(this.antiDebug);
            this.optionsPage.Controls.Add(this.antiVM);
            this.optionsPage.Location = new System.Drawing.Point(8, 40);
            this.optionsPage.Name = "optionsPage";
            this.optionsPage.Padding = new System.Windows.Forms.Padding(3);
            this.optionsPage.Size = new System.Drawing.Size(861, 416);
            this.optionsPage.TabIndex = 0;
            this.optionsPage.Text = "Options";
            this.optionsPage.UseVisualStyleBackColor = true;
            // 
            // uacBypass
            // 
            this.uacBypass.AutoSize = true;
            this.uacBypass.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.uacBypass.Location = new System.Drawing.Point(6, 249);
            this.uacBypass.Name = "uacBypass";
            this.uacBypass.Size = new System.Drawing.Size(178, 33);
            this.uacBypass.TabIndex = 13;
            this.uacBypass.Text = "UAC Bypass";
            this.uacBypass.UseVisualStyleBackColor = true;
            this.uacBypass.CheckedChanged += new System.EventHandler(this.uacBypass_CheckedChanged);
            // 
            // startup
            // 
            this.startup.AutoSize = true;
            this.startup.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.startup.Location = new System.Drawing.Point(6, 210);
            this.startup.Name = "startup";
            this.startup.Size = new System.Drawing.Size(121, 33);
            this.startup.TabIndex = 12;
            this.startup.Text = "Startup";
            this.startup.UseVisualStyleBackColor = true;
            this.startup.CheckedChanged += new System.EventHandler(this.startup_CheckedChanged);
            // 
            // runas
            // 
            this.runas.AutoSize = true;
            this.runas.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.runas.Location = new System.Drawing.Point(6, 171);
            this.runas.Name = "runas";
            this.runas.Size = new System.Drawing.Size(191, 33);
            this.runas.TabIndex = 11;
            this.runas.Text = "Run as admin";
            this.runas.UseVisualStyleBackColor = true;
            // 
            // binderPage
            // 
            this.binderPage.Controls.Add(this.removeFile);
            this.binderPage.Controls.Add(this.addFile);
            this.binderPage.Controls.Add(this.listBox1);
            this.binderPage.Location = new System.Drawing.Point(8, 40);
            this.binderPage.Name = "binderPage";
            this.binderPage.Padding = new System.Windows.Forms.Padding(3);
            this.binderPage.Size = new System.Drawing.Size(861, 416);
            this.binderPage.TabIndex = 2;
            this.binderPage.Text = "Binder";
            this.binderPage.UseVisualStyleBackColor = true;
            // 
            // removeFile
            // 
            this.removeFile.Location = new System.Drawing.Point(195, 354);
            this.removeFile.Name = "removeFile";
            this.removeFile.Size = new System.Drawing.Size(186, 54);
            this.removeFile.TabIndex = 2;
            this.removeFile.Text = "Remove file";
            this.removeFile.UseVisualStyleBackColor = true;
            this.removeFile.Click += new System.EventHandler(this.removeFile_Click);
            // 
            // addFile
            // 
            this.addFile.Location = new System.Drawing.Point(3, 354);
            this.addFile.Name = "addFile";
            this.addFile.Size = new System.Drawing.Size(186, 54);
            this.addFile.TabIndex = 1;
            this.addFile.Text = "Add file";
            this.addFile.UseVisualStyleBackColor = true;
            this.addFile.Click += new System.EventHandler(this.addFile_Click);
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.ItemHeight = 26;
            this.listBox1.Location = new System.Drawing.Point(3, 8);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(849, 316);
            this.listBox1.TabIndex = 0;
            // 
            // outputPage
            // 
            this.outputPage.Controls.Add(this.listBox2);
            this.outputPage.Location = new System.Drawing.Point(8, 40);
            this.outputPage.Name = "outputPage";
            this.outputPage.Padding = new System.Windows.Forms.Padding(3);
            this.outputPage.Size = new System.Drawing.Size(861, 416);
            this.outputPage.TabIndex = 3;
            this.outputPage.Text = "Output";
            this.outputPage.UseVisualStyleBackColor = true;
            // 
            // listBox2
            // 
            this.listBox2.FormattingEnabled = true;
            this.listBox2.ItemHeight = 26;
            this.listBox2.Location = new System.Drawing.Point(6, 6);
            this.listBox2.Name = "listBox2";
            this.listBox2.Size = new System.Drawing.Size(849, 368);
            this.listBox2.TabIndex = 4;
            // 
            // PhantomMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(192F, 192F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(914, 669);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.buildButton);
            this.Controls.Add(this.openButton);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label1);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "PhantomMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Phantom";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.tabControl1.ResumeLayout(false);
            this.optionsPage.ResumeLayout(false);
            this.optionsPage.PerformLayout();
            this.binderPage.ResumeLayout(false);
            this.outputPage.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button openButton;
        private System.Windows.Forms.CheckBox antiDebug;
        private System.Windows.Forms.Button buildButton;
        private System.Windows.Forms.CheckBox selfDelete;
        private System.Windows.Forms.CheckBox hidden;
        private System.Windows.Forms.CheckBox antiVM;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage optionsPage;
        private System.Windows.Forms.TabPage binderPage;
        private System.Windows.Forms.TabPage outputPage;
        private System.Windows.Forms.Button addFile;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Button removeFile;
        private System.Windows.Forms.ListBox listBox2;
        private System.Windows.Forms.CheckBox runas;
        private System.Windows.Forms.CheckBox startup;
        private System.Windows.Forms.CheckBox uacBypass;
    }
}
