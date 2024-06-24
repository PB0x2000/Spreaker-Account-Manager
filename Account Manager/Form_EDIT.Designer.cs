
namespace Account_Manager
{
    partial class Form_EDIT
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.richTextBox_DEBUG = new System.Windows.Forms.RichTextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button_GETLINKS = new System.Windows.Forms.Button();
            this.button_CHROME = new System.Windows.Forms.Button();
            this.button_Kill = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.panel4);
            this.panel1.Controls.Add(this.panel3);
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1057, 862);
            this.panel1.TabIndex = 0;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.button_Kill);
            this.panel2.Controls.Add(this.button_CHROME);
            this.panel2.Controls.Add(this.button_GETLINKS);
            this.panel2.Controls.Add(this.button1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1057, 103);
            this.panel2.TabIndex = 0;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.richTextBox_DEBUG);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel3.Location = new System.Drawing.Point(0, 567);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(1057, 295);
            this.panel3.TabIndex = 1;
            // 
            // panel4
            // 
            this.panel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel4.Location = new System.Drawing.Point(0, 103);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(1057, 464);
            this.panel4.TabIndex = 2;
            // 
            // richTextBox_DEBUG
            // 
            this.richTextBox_DEBUG.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.richTextBox_DEBUG.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richTextBox_DEBUG.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBox_DEBUG.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richTextBox_DEBUG.ForeColor = System.Drawing.SystemColors.MenuHighlight;
            this.richTextBox_DEBUG.Location = new System.Drawing.Point(0, 0);
            this.richTextBox_DEBUG.Name = "richTextBox_DEBUG";
            this.richTextBox_DEBUG.ReadOnly = true;
            this.richTextBox_DEBUG.Size = new System.Drawing.Size(1057, 295);
            this.richTextBox_DEBUG.TabIndex = 0;
            this.richTextBox_DEBUG.Text = "";
            // 
            // button1
            // 
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.ForeColor = System.Drawing.Color.Yellow;
            this.button1.Location = new System.Drawing.Point(12, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(120, 79);
            this.button1.TabIndex = 0;
            this.button1.Text = "Add ADS";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button_GETLINKS
            // 
            this.button_GETLINKS.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_GETLINKS.ForeColor = System.Drawing.Color.Yellow;
            this.button_GETLINKS.Location = new System.Drawing.Point(138, 12);
            this.button_GETLINKS.Name = "button_GETLINKS";
            this.button_GETLINKS.Size = new System.Drawing.Size(120, 79);
            this.button_GETLINKS.TabIndex = 1;
            this.button_GETLINKS.Text = "Get Links\r\n(Spreaker/Addict)";
            this.button_GETLINKS.UseVisualStyleBackColor = true;
            this.button_GETLINKS.Click += new System.EventHandler(this.button_GETLINKS_Click);
            // 
            // button_CHROME
            // 
            this.button_CHROME.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_CHROME.ForeColor = System.Drawing.Color.Yellow;
            this.button_CHROME.Location = new System.Drawing.Point(264, 12);
            this.button_CHROME.Name = "button_CHROME";
            this.button_CHROME.Size = new System.Drawing.Size(120, 79);
            this.button_CHROME.TabIndex = 2;
            this.button_CHROME.Text = "Open in Chrome";
            this.button_CHROME.UseVisualStyleBackColor = true;
            this.button_CHROME.Click += new System.EventHandler(this.button_CHROME_Click);
            // 
            // button_Kill
            // 
            this.button_Kill.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_Kill.ForeColor = System.Drawing.Color.Yellow;
            this.button_Kill.Location = new System.Drawing.Point(390, 12);
            this.button_Kill.Name = "button_Kill";
            this.button_Kill.Size = new System.Drawing.Size(120, 79);
            this.button_Kill.TabIndex = 3;
            this.button_Kill.Text = "Kill Chrome";
            this.button_Kill.UseVisualStyleBackColor = true;
            this.button_Kill.Click += new System.EventHandler(this.button_Kill_Click);
            // 
            // Form_EDIT
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(24)))), ((int)(((byte)(24)))));
            this.ClientSize = new System.Drawing.Size(1057, 862);
            this.Controls.Add(this.panel1);
            this.Name = "Form_EDIT";
            this.Text = "EDIT Account";
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.RichTextBox richTextBox_DEBUG;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button_GETLINKS;
        private System.Windows.Forms.Button button_CHROME;
        private System.Windows.Forms.Button button_Kill;
    }
}