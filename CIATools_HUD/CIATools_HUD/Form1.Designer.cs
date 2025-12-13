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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            label1 = new Label();
            button2 = new Button();
            version = new Label();
            github_link = new LinkLabel();
            toolStrip1 = new ToolStrip();
            toolStripLabel1 = new ToolStripLabel();
            toolStripDropDownButton1 = new ToolStripDropDownButton();
            versionToolStripMenuItem = new ToolStripMenuItem();
            devDebugPATHToolStripMenuItem = new ToolStripMenuItem();
            toolStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 21.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.Location = new Point(128, 9);
            label1.Name = "label1";
            label1.Size = new Size(208, 50);
            label1.TabIndex = 0;
            label1.Text = "> CIATools";
            // 
            // button2
            // 
            button2.BackColor = Color.FromArgb(255, 255, 128);
            button2.FlatAppearance.BorderColor = Color.Yellow;
            button2.Font = new Font("Segoe UI", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            button2.Location = new Point(128, 195);
            button2.Margin = new Padding(3, 4, 3, 4);
            button2.Name = "button2";
            button2.Size = new Size(224, 91);
            button2.TabIndex = 3;
            button2.Text = "Build CIA";
            button2.UseMnemonic = false;
            button2.UseVisualStyleBackColor = false;
            button2.Click += button_build;
            // 
            // version
            // 
            version.AutoSize = true;
            version.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            version.Location = new Point(324, 31);
            version.Name = "version";
            version.Size = new Size(28, 23);
            version.TabIndex = 5;
            version.Text = "v2";
            // 
            // github_link
            // 
            github_link.AutoSize = true;
            github_link.Font = new Font("Segoe UI", 10.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            github_link.Location = new Point(212, 59);
            github_link.Name = "github_link";
            github_link.Size = new Size(70, 25);
            github_link.TabIndex = 6;
            github_link.TabStop = true;
            github_link.Text = "Github";
            github_link.LinkClicked += github_link_LinkClicked;
            // 
            // toolStrip1
            // 
            toolStrip1.Dock = DockStyle.Bottom;
            toolStrip1.ImageScalingSize = new Size(20, 20);
            toolStrip1.Items.AddRange(new ToolStripItem[] { toolStripLabel1, toolStripDropDownButton1 });
            toolStrip1.Location = new Point(0, 410);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Size = new Size(485, 27);
            toolStrip1.TabIndex = 8;
            toolStrip1.Text = "toolStrip1";
            // 
            // toolStripLabel1
            // 
            toolStripLabel1.Name = "toolStripLabel1";
            toolStripLabel1.Size = new Size(62, 24);
            toolStripLabel1.Text = "Settings";
            // 
            // toolStripDropDownButton1
            // 
            toolStripDropDownButton1.DisplayStyle = ToolStripItemDisplayStyle.Image;
            toolStripDropDownButton1.DropDownItems.AddRange(new ToolStripItem[] { versionToolStripMenuItem, devDebugPATHToolStripMenuItem });
            toolStripDropDownButton1.Image = (Image)resources.GetObject("toolStripDropDownButton1.Image");
            toolStripDropDownButton1.ImageTransparentColor = Color.Magenta;
            toolStripDropDownButton1.Name = "toolStripDropDownButton1";
            toolStripDropDownButton1.Size = new Size(34, 24);
            toolStripDropDownButton1.Text = "toolStripDropDownButton1";
            // 
            // versionToolStripMenuItem
            // 
            versionToolStripMenuItem.Name = "versionToolStripMenuItem";
            versionToolStripMenuItem.Size = new Size(204, 26);
            versionToolStripMenuItem.Text = "Version";
            versionToolStripMenuItem.Click += versionToolStripMenuItem_Click;
            // 
            // devDebugPATHToolStripMenuItem
            // 
            devDebugPATHToolStripMenuItem.Name = "devDebugPATHToolStripMenuItem";
            devDebugPATHToolStripMenuItem.Size = new Size(204, 26);
            devDebugPATHToolStripMenuItem.Text = "Dev debug PATH";
            devDebugPATHToolStripMenuItem.Click += devDebugPATHToolStripMenuItem_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.SlateBlue;
            ClientSize = new Size(485, 437);
            Controls.Add(toolStrip1);
            Controls.Add(github_link);
            Controls.Add(version);
            Controls.Add(button2);
            Controls.Add(label1);
            Margin = new Padding(3, 4, 3, 4);
            MaximumSize = new Size(503, 484);
            MinimumSize = new Size(503, 484);
            Name = "Form1";
            Text = "CIATools";
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Button button2;
        private Label version;
        private LinkLabel github_link;
        private ToolStrip toolStrip1;
        private ToolStripLabel toolStripLabel1;
        private ToolStripDropDownButton toolStripDropDownButton1;
        private ToolStripMenuItem versionToolStripMenuItem;
        private ToolStripMenuItem devDebugPATHToolStripMenuItem;
    }
}
