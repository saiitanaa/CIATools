using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;


namespace CIATools_HUD
{

    public partial class Form1 : Form
    {
        private string rootFolder;

        public Form1()
        {
            InitializeComponent();
            rootFolder = RootFolderPath();
        }

        string RootFolderPath()
        {
            DirectoryInfo dir = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);

            while (dir != null)
            {
                string marker = Path.Combine(dir.FullName, "root_path");
                if (File.Exists(marker))
                    return dir.FullName;

                dir = dir.Parent;
            }

            return null;
        }

        private void button_build(object sender, EventArgs e)
        {
            string execute_py = Path.Combine(rootFolder, "script_import");
            Process.Start("cmd.exe", $"/c cd /d \"{execute_py}\" && python import.py");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Root Path: " + rootFolder, "Dev debug Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void github_link_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("cmd.exe", $"/c start https://github.com/saysaa/CIATools");
        }

        private void version_button_Click(object sender, EventArgs e)
        {
            MessageBox.Show("CIATools HUD\nVersion 2\n\nCIABUILDER\nBuild-2\n\nLauncher\nVersion 0.2.1", "Version Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }



        // Settings Options
        private void versionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("CIATools HUD\nVersion 3\n\nCIABUILDER\nBuild-2\n\nLauncher\nVersion 0.1.0", "Version Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void rootPATHToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Root Path: " + rootFolder, "Dev debug Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void sTARTImportpyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string execute_py = Path.Combine(rootFolder, "script_import");
            Process.Start("cmd.exe", $"/c cd /d \"{execute_py}\" && python import.py");
        }

        private void sTARTCompilepyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string execute_py = Path.Combine(rootFolder, "script_import");
            Process.Start("cmd.exe", $"/c cd /d \"{execute_py}\" && python compile.py");
        }

        private void sTARTDeletepyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string execute_py = Path.Combine(rootFolder, "script_import");
            Process.Start("cmd.exe", $"/c cd /d \"{execute_py}\" && python delete.py");
        }

        private void rESTORERootpathToolStripMenuItem_Click(object sender, EventArgs e)
        {
            File.Create(Path.Combine(rootFolder, "root_path")).Close();
        }

        private void rESTOREFILEPATHToolStripMenuItem_Click(object sender, EventArgs e)
        {
            File.Create(Path.Combine(rootFolder, "USER_FILES", "FILE_PATH")).Close();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            string USER_path = Path.Combine(rootFolder, "USER_FILES");
            Process.Start("explorer.exe", USER_path);
        }

        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Where should I put the files needed for compilation?\nPlease click the \"Open USER_FILES\" button and drag all your files into it (your original files will never be deleted!).\n\nAny other problems?\nVisit the GitHub page and read the documentation, or create an issue.", "Help", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
