namespace CIATools_HUD
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            label1 = new Label();
            button2 = new Button();
            button1 = new Button();
            version = new Label();
            github_link = new LinkLabel();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 21.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.Location = new Point(127, 9);
            label1.Name = "label1";
            label1.Size = new Size(162, 40);
            label1.TabIndex = 0;
            label1.Text = "> CIATools";
            // 
            // button2
            // 
            button2.BackColor = Color.FromArgb(255, 255, 128);
            button2.FlatAppearance.BorderColor = Color.Yellow;
            button2.Font = new Font("Segoe UI", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            button2.Location = new Point(104, 211);
            button2.Name = "button2";
            button2.Size = new Size(212, 68);
            button2.TabIndex = 3;
            button2.Text = "Build CIA";
            button2.UseMnemonic = false;
            button2.UseVisualStyleBackColor = false;
            button2.Click += button_build;
            // 
            // button1
            // 
            button1.Location = new Point(12, 432);
            button1.Name = "button1";
            button1.Size = new Size(75, 23);
            button1.TabIndex = 4;
            button1.Text = "Dev debug";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // version
            // 
            version.AutoSize = true;
            version.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            version.Location = new Point(236, 49);
            version.Name = "version";
            version.Size = new Size(30, 17);
            version.TabIndex = 5;
            version.Text = "v1.0";
            // 
            // github_link
            // 
            github_link.AutoSize = true;
            github_link.Location = new Point(187, 51);
            github_link.Name = "github_link";
            github_link.Size = new Size(43, 15);
            github_link.TabIndex = 6;
            github_link.TabStop = true;
            github_link.Text = "Github";
            github_link.LinkClicked += github_link_LinkClicked;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Silver;
            ClientSize = new Size(424, 467);
            Controls.Add(github_link);
            Controls.Add(version);
            Controls.Add(button1);
            Controls.Add(button2);
            Controls.Add(label1);
            Name = "Form1";
            Text = "CIATools";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Button button2;
        private Button button1;
        private Label version;
        private LinkLabel github_link;
    }
}
