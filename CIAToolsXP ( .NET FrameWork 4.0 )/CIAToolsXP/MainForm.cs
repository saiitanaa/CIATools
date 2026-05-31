/*
 * Created by SharpDevelop.
 * User: l-primoo
 * Date: 31/05/2026
 * Time: 23:20
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.IO;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;

namespace CIAToolsXP
{
	/// <summary>
	/// Description of MainForm.
	/// </summary>
	public partial class MainForm : Form
	{
		public MainForm()
		{
			InitializeComponent();
			try {
		        string iconPath = Path.Combine(Application.StartupPath, Path.Combine("Assets", "icon.ico"));
		        
		        if (File.Exists(iconPath)) {
		            this.Icon = new Icon(iconPath);
		        }
		    } catch (Exception) {

		    }
		
		    if (label1 != null) {
		        label1.Cursor = Cursors.Hand;
		    }

		}
		
		void Label1Click(object sender, EventArgs e)
		{
			Process.Start("https://github.com/saysaa/CIATools");
		}
		

		
		void Button1Click(object sender, EventArgs e)
		{
			string execute_py = Path.Combine(Application.StartupPath, "PYSCRIPT");
			Process.Start("cmd.exe", string.Format("/k cd /d \"{0}\" && python import.py", execute_py));
		}
		
		void Button2Click(object sender, EventArgs e)
		{
			using (OpenFileDialog fileDialog = new OpenFileDialog())
			{
			    fileDialog.Title = "Select files to import";
			    fileDialog.Multiselect = true;
			    fileDialog.Filter = "All files (*.*)|*.*";
			
			    if (fileDialog.ShowDialog() == DialogResult.OK)
			    {
			        string USER_path = Path.Combine(Application.StartupPath, "USER_FILES");
			        if (!Directory.Exists(USER_path))
			            Directory.CreateDirectory(USER_path);
			
			        foreach (string file in fileDialog.FileNames)
			        {
			            try
			            {
			                string destFile = Path.Combine(USER_path, Path.GetFileName(file));
			                File.Copy(file, destFile, true);
			            }
			            catch (Exception)
			            {
			                MessageBox.Show("Error importing!", "Import Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			            }
			        }
			
			        MessageBox.Show("Files imported successfully!", "Import Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
			    }
			}
		}
		
		void ToolStripButton1Click(object sender, EventArgs e)
		{
		    ContextMenuStrip menuDeroulant = new ContextMenuStrip();
		    ToolStripMenuItem user1 = new ToolStripMenuItem("Open USER_FILES");
		    ToolStripMenuItem user2 = new ToolStripMenuItem("Exit");
		    user1.Click += (s, args) => {
		    	string USER_FILES = Path.Combine(Application.StartupPath, "USER_FILES");
		    	Process.Start("explorer.exe", USER_FILES);
		    };
		    
		    user2.Click += (s, args) => {
		        Application.Exit();
		    };
		    menuDeroulant.Items.Add(user1);
		    menuDeroulant.Items.Add(new ToolStripSeparator());
		    menuDeroulant.Items.Add(user2);
				    ToolStripItem item = sender as ToolStripItem;
				    if (item != null)
				    {
				        ToolStrip parentStrip = item.Owner;
				        if (parentStrip != null) {
				            menuDeroulant.Show(parentStrip, new Point(parentStrip.Width - menuDeroulant.Width, parentStrip.Height));
				    }
			}
		}
	}
}
