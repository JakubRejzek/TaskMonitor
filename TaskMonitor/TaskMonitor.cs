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

		List<MonitorableProcess> processList {  get; set; }
		DataTable dt = new DataTable();
		public TaskMonitor()
		{
			InitializeComponent();
			dt.Columns.Add("Process name", typeof(string));
			dt.Columns.Add("Termination date", typeof(DateTime));
			dt.Columns.Add("Monitored", typeof(bool));
			dt.Columns.Add("Dead", typeof(bool));
			dataGridView1.DataSource = dt;
			dataGridView1.Columns[0].MinimumWidth = 80;
			dataGridView1.Columns[1].MinimumWidth = 80;
			dataGridView1.Columns[2].Width = 50;
			dataGridView1.Columns[3].Width = 50;
		}
		public void RefreshTable()
		{
			dt.Rows.Clear();
			foreach (MonitorableProcess process in processList)
			{
				dt.Rows.Add(
					process.processName,
					process.terminationDate,
					process.monitored,
					process.dead);
			}
		}

		private void TaskMonitor_Load(object sender, EventArgs e)
		{
			processList = MonitorableProcess.GetMonitorableProcesses();
			button1_Click(sender, e);
		}

		private void button1_Click(object sender, EventArgs e)
		{
			processList = MonitorableProcess.GetMonitorableProcesses();
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
			MonitorableProcess selectedProcess = processList[dataGridView1.CurrentCell.RowIndex];
			if(numericUpDown2.Value >= numericUpDown1.Value)
			{
				numericUpDown1.Value = (decimal)(numericUpDown2.Value > 10 ? 10 : 0.5);
			}
			selectedProcess.StartMonitoring(
				DateTime.Now.AddMinutes((double)numericUpDown2.Value),
				(numericUpDown1.Value),
				this);
			numericUpDown1.Enabled = false;
			numericUpDown2.Enabled = false;
		}
	}
}
