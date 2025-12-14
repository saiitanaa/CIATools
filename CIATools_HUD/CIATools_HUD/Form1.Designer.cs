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
            toolStripLabel2 = new ToolStripLabel();
            import_button = new Button();
            toolStripLabel3 = new ToolStripLabel();
            toolStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.BackColor = Color.Transparent;
            label1.Font = new Font("Segoe UI", 21.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.ForeColor = Color.White;
            label1.Location = new Point(112, 7);
            label1.Name = "label1";
            label1.Size = new Size(182, 40);
            label1.TabIndex = 0;
            label1.Text = ">_  CIATools";
            // 
            // button2
            // 
            button2.BackColor = Color.FromArgb(0, 192, 0);
            button2.FlatAppearance.BorderColor = Color.Yellow;
            button2.Font = new Font("Segoe UI", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            button2.Location = new Point(112, 114);
            button2.Name = "button2";
            button2.Size = new Size(196, 82);
            button2.TabIndex = 3;
            button2.Text = "Build CIA";
            button2.UseMnemonic = false;
            button2.UseVisualStyleBackColor = false;
            button2.Click += button_build;
            // 
            // version
            // 
            version.AutoSize = true;
            version.BackColor = Color.Transparent;
            version.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            version.ForeColor = SystemColors.ActiveCaption;
            version.Location = new Point(286, 25);
            version.Name = "version";
            version.Size = new Size(22, 17);
            version.TabIndex = 5;
            version.Text = "v4";
            // 
            // github_link
            // 
            github_link.AutoSize = true;
            github_link.BackColor = Color.Transparent;
            github_link.Font = new Font("Segoe UI", 10.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            github_link.LinkColor = Color.FromArgb(255, 128, 0);
            github_link.Location = new Point(193, 47);
            github_link.Name = "github_link";
            github_link.Size = new Size(57, 20);
            github_link.TabIndex = 6;
            github_link.TabStop = true;
            github_link.Text = "Github";
            github_link.LinkClicked += github_link_LinkClicked;
            // 
            // toolStrip1
            // 
            toolStrip1.BackgroundImage = (Image)resources.GetObject("toolStrip1.BackgroundImage");
            toolStrip1.Dock = DockStyle.Bottom;
            toolStrip1.GripStyle = ToolStripGripStyle.Hidden;
            toolStrip1.ImageScalingSize = new Size(20, 20);
            toolStrip1.Items.AddRange(new ToolStripItem[] { toolStripLabel1, toolStripDropDownButton1, toolStripLabel3, toolStripLabel2 });
            toolStrip1.Location = new Point(0, 307);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Size = new Size(426, 27);
            toolStrip1.TabIndex = 8;
            toolStrip1.Text = "toolStrip1";
            // 
            // toolStripLabel1
            // 
            toolStripLabel1.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            toolStripLabel1.ForeColor = Color.FromArgb(192, 192, 255);
            toolStripLabel1.Name = "toolStripLabel1";
            toolStripLabel1.Size = new Size(50, 24);
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
            versionToolStripMenuItem.Size = new Size(180, 22);
            versionToolStripMenuItem.Text = "Version";
            versionToolStripMenuItem.Click += versionToolStripMenuItem_Click;
            // 
            // dEVDEBUGToolStripMenuItem
            // 
            dEVDEBUGToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { rootPATHToolStripMenuItem, sTARTImportpyToolStripMenuItem, sTARTCompilepyToolStripMenuItem, sTARTDeletepyToolStripMenuItem, rESTORERootpathToolStripMenuItem, rESTOREFILEPATHToolStripMenuItem });
            dEVDEBUGToolStripMenuItem.Name = "dEVDEBUGToolStripMenuItem";
            dEVDEBUGToolStripMenuItem.Size = new Size(180, 22);
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
            helpToolStripMenuItem.Size = new Size(180, 22);
            helpToolStripMenuItem.Text = "Help";
            helpToolStripMenuItem.Click += helpToolStripMenuItem_Click;
            // 
            // toolStripLabel2
            // 
            toolStripLabel2.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            toolStripLabel2.ForeColor = Color.FromArgb(255, 128, 0);
            toolStripLabel2.Name = "toolStripLabel2";
            toolStripLabel2.Size = new Size(101, 24);
            toolStripLabel2.Text = "Open USER_FILES";
            toolStripLabel2.Click += toolStripLabel2_Click;
            // 
            // import_button
            // 
            import_button.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            import_button.Location = new Point(112, 202);
            import_button.Name = "import_button";
            import_button.Size = new Size(196, 31);
            import_button.TabIndex = 10;
            import_button.Text = "Import Files";
            import_button.UseVisualStyleBackColor = true;
            import_button.Click += import_button_Click;
            // 
            // toolStripLabel3
            // 
            toolStripLabel3.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            toolStripLabel3.ForeColor = Color.White;
            toolStripLabel3.Name = "toolStripLabel3";
            toolStripLabel3.Size = new Size(12, 24);
            toolStripLabel3.Text = "|";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.SlateBlue;
            BackgroundImage = (Image)resources.GetObject("$this.BackgroundImage");
            ClientSize = new Size(426, 334);
            Controls.Add(import_button);
            Controls.Add(toolStrip1);
            Controls.Add(github_link);
            Controls.Add(version);
            Controls.Add(button2);
            Controls.Add(label1);
            MaximumSize = new Size(442, 373);
            MinimumSize = new Size(442, 373);
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
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
        private ToolStripMenuItem helpToolStripMenuItem;
        private Button import_button;
        private ToolStripLabel toolStripLabel2;
        private ToolStripLabel toolStripLabel3;
    }
}
