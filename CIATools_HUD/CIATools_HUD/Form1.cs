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
            Process.Start("cmd.exe", $"/k cd /d \"{execute_py}\" && python import.py");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Root Path: " + rootFolder, "Dev debug Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void github_link_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("cmd.exe", $"/c start https://github.com/saysaa/CIATools");
        }
    }
}
