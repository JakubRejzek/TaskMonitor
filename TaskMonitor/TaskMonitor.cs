using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TaskMonitor
{
	public partial class TaskMonitor : Form
	{
		public bool lockedMonitoring = false;
		public string lastMonitoredProcess = "";
		BindingList<MonitorableProcess> processList {  get; set; }
		public TaskMonitor()
		{
			InitializeComponent();
			processList = MonitorableProcess.GetMonitorableProcesses();
			dataGridView1.DataSource = processList;
			dataGridView1.Columns[0].MinimumWidth = 80;
			dataGridView1.Columns[1].MinimumWidth = 80;
			dataGridView1.Columns[2].Width = 50;
			dataGridView1.Columns[3].Width = 50;
		}
		public void RefreshTable()
		{
			dataGridView1.Refresh();
			label5.Text = lastMonitoredProcess;
		}

		private void TaskMonitor_Load(object sender, EventArgs e)
		{
			button1_Click(sender, e);
		}

		private void button1_Click(object sender, EventArgs e)
		{
			RefreshTable();
		}

		private void SelectionChange(object sender, EventArgs e)
		{
			if(dataGridView1.CurrentCell != null)
			{
				MonitorableProcess selectedProcess = processList[dataGridView1.CurrentCell.RowIndex];
				if (selectedProcess.terminationDate != null)
				{
					numericUpDown1.Enabled = false;
					numericUpDown2.Enabled = false;
				}
				else
				{
					numericUpDown1.Enabled = true;
					numericUpDown2.Enabled = true;
				}
			}
		}

		private void button2_Click(object sender, EventArgs e)
		{
			if (!lockedMonitoring)
			{
				MonitorableProcess selectedProcess = processList[dataGridView1.CurrentCell.RowIndex];
				if (numericUpDown2.Value >= numericUpDown1.Value)
				{
					numericUpDown1.Value = (decimal)(numericUpDown2.Value > 10 ? 10 : 0.5);
				}
				selectedProcess.StartMonitoring(
					DateTime.Now.AddMinutes((double)numericUpDown2.Value),
					(numericUpDown1.Value),
					this);
				numericUpDown1.Enabled = false;
				numericUpDown2.Enabled = false;
				lockedMonitoring = true;
			}
			else
			{
				MessageBox.Show("You are already monitoring a process. Monitoring multiple processes at the same time can create unwanted behaviour");
			}
		}

		private void label4_Click(object sender, EventArgs e)
		{

		}
	}
}
