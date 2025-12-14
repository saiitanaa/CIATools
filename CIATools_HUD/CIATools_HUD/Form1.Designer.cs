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
            dEVDEBUGToolStripMenuItem = new ToolStripMenuItem();
            rootPATHToolStripMenuItem = new ToolStripMenuItem();
            sTARTImportpyToolStripMenuItem = new ToolStripMenuItem();
            sTARTCompilepyToolStripMenuItem = new ToolStripMenuItem();
            sTARTDeletepyToolStripMenuItem = new ToolStripMenuItem();
            rESTORERootpathToolStripMenuItem = new ToolStripMenuItem();
            rESTOREFILEPATHToolStripMenuItem = new ToolStripMenuItem();
            helpToolStripMenuItem = new ToolStripMenuItem();
            button1 = new Button();
            toolStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 21.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.Location = new Point(112, 7);
            label1.Name = "label1";
            label1.Size = new Size(162, 40);
            label1.TabIndex = 0;
            label1.Text = "> CIATools";
            // 
            // button2
            // 
            button2.BackColor = Color.FromArgb(0, 192, 0);
            button2.FlatAppearance.BorderColor = Color.Yellow;
            button2.Font = new Font("Segoe UI", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            button2.Location = new Point(112, 146);
            button2.Name = "button2";
            button2.Size = new Size(196, 68);
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
            version.Location = new Point(267, 25);
            version.Name = "version";
            version.Size = new Size(22, 17);
            version.TabIndex = 5;
            version.Text = "v3";
            // 
            // github_link
            // 
            github_link.AutoSize = true;
            github_link.Font = new Font("Segoe UI", 10.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            github_link.Location = new Point(179, 47);
            github_link.Name = "github_link";
            github_link.Size = new Size(57, 20);
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
            toolStrip1.Location = new Point(0, 307);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Size = new Size(426, 27);
            toolStrip1.TabIndex = 8;
            toolStrip1.Text = "toolStrip1";
            // 
            // toolStripLabel1
            // 
            toolStripLabel1.Name = "toolStripLabel1";
            toolStripLabel1.Size = new Size(49, 24);
            toolStripLabel1.Text = "Settings";
            // 
            // toolStripDropDownButton1
            // 
            toolStripDropDownButton1.DisplayStyle = ToolStripItemDisplayStyle.Image;
            toolStripDropDownButton1.DropDownItems.AddRange(new ToolStripItem[] { versionToolStripMenuItem, dEVDEBUGToolStripMenuItem, helpToolStripMenuItem });
            toolStripDropDownButton1.Image = (Image)resources.GetObject("toolStripDropDownButton1.Image");
            toolStripDropDownButton1.ImageTransparentColor = Color.Magenta;
            toolStripDropDownButton1.Name = "toolStripDropDownButton1";
            toolStripDropDownButton1.Size = new Size(33, 24);
            toolStripDropDownButton1.Text = "toolStripDropDownButton1";
            // 
            // versionToolStripMenuItem
            // 
            versionToolStripMenuItem.Name = "versionToolStripMenuItem";
            versionToolStripMenuItem.Size = new Size(116, 22);
            versionToolStripMenuItem.Text = "Version";
            versionToolStripMenuItem.Click += versionToolStripMenuItem_Click;
            // 
            // dEVDEBUGToolStripMenuItem
            // 
            dEVDEBUGToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { rootPATHToolStripMenuItem, sTARTImportpyToolStripMenuItem, sTARTCompilepyToolStripMenuItem, sTARTDeletepyToolStripMenuItem, rESTORERootpathToolStripMenuItem, rESTOREFILEPATHToolStripMenuItem });
            dEVDEBUGToolStripMenuItem.Name = "dEVDEBUGToolStripMenuItem";
            dEVDEBUGToolStripMenuItem.Size = new Size(116, 22);
            dEVDEBUGToolStripMenuItem.Text = "Settings";
            // 
            // rootPATHToolStripMenuItem
            // 
            rootPATHToolStripMenuItem.Name = "rootPATHToolStripMenuItem";
            rootPATHToolStripMenuItem.Size = new Size(195, 22);
            rootPATHToolStripMenuItem.Text = "Root PATH";
            rootPATHToolStripMenuItem.Click += rootPATHToolStripMenuItem_Click;
            // 
            // sTARTImportpyToolStripMenuItem
            // 
            sTARTImportpyToolStripMenuItem.Name = "sTARTImportpyToolStripMenuItem";
            sTARTImportpyToolStripMenuItem.Size = new Size(195, 22);
            sTARTImportpyToolStripMenuItem.Text = "START -> import.py";
            sTARTImportpyToolStripMenuItem.Click += sTARTImportpyToolStripMenuItem_Click;
            // 
            // sTARTCompilepyToolStripMenuItem
            // 
            sTARTCompilepyToolStripMenuItem.Name = "sTARTCompilepyToolStripMenuItem";
            sTARTCompilepyToolStripMenuItem.Size = new Size(195, 22);
            sTARTCompilepyToolStripMenuItem.Text = "START -> compile.py";
            sTARTCompilepyToolStripMenuItem.Click += sTARTCompilepyToolStripMenuItem_Click;
            // 
            // sTARTDeletepyToolStripMenuItem
            // 
            sTARTDeletepyToolStripMenuItem.Name = "sTARTDeletepyToolStripMenuItem";
            sTARTDeletepyToolStripMenuItem.Size = new Size(195, 22);
            sTARTDeletepyToolStripMenuItem.Text = "START -> delete.py";
            sTARTDeletepyToolStripMenuItem.Click += sTARTDeletepyToolStripMenuItem_Click;
            // 
            // rESTORERootpathToolStripMenuItem
            // 
            rESTORERootpathToolStripMenuItem.Name = "rESTORERootpathToolStripMenuItem";
            rESTORERootpathToolStripMenuItem.Size = new Size(195, 22);
            rESTORERootpathToolStripMenuItem.Text = "RESTORE -> root_path";
            rESTORERootpathToolStripMenuItem.Click += rESTORERootpathToolStripMenuItem_Click;
            // 
            // rESTOREFILEPATHToolStripMenuItem
            // 
            rESTOREFILEPATHToolStripMenuItem.Name = "rESTOREFILEPATHToolStripMenuItem";
            rESTOREFILEPATHToolStripMenuItem.Size = new Size(195, 22);
            rESTOREFILEPATHToolStripMenuItem.Text = "RESTORE -> FILE_PATH";
            rESTOREFILEPATHToolStripMenuItem.Click += rESTOREFILEPATHToolStripMenuItem_Click;
            // 
            // helpToolStripMenuItem
            // 
            helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            helpToolStripMenuItem.Size = new Size(116, 22);
            helpToolStripMenuItem.Text = "Help";
            helpToolStripMenuItem.Click += helpToolStripMenuItem_Click;
            // 
            // button1
            // 
            button1.Location = new Point(112, 220);
            button1.Name = "button1";
            button1.Size = new Size(196, 23);
            button1.TabIndex = 9;
            button1.Text = "Open USER_FILES";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click_1;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.SlateBlue;
            ClientSize = new Size(426, 334);
            Controls.Add(button1);
            Controls.Add(toolStrip1);
            Controls.Add(github_link);
            Controls.Add(version);
            Controls.Add(button2);
            Controls.Add(label1);
            MaximumSize = new Size(442, 373);
            MinimumSize = new Size(442, 373);
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
        private ToolStripMenuItem dEVDEBUGToolStripMenuItem;
        private ToolStripMenuItem rootPATHToolStripMenuItem;
        private ToolStripMenuItem sTARTImportpyToolStripMenuItem;
        private ToolStripMenuItem sTARTCompilepyToolStripMenuItem;
        private ToolStripMenuItem sTARTDeletepyToolStripMenuItem;
        private ToolStripMenuItem rESTORERootpathToolStripMenuItem;
        private ToolStripMenuItem rESTOREFILEPATHToolStripMenuItem;
        private Button button1;
        private ToolStripMenuItem helpToolStripMenuItem;
    }
}
