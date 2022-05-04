/*
 *  Simple Classic Theme, a basic utility to bring back classic theme to 
 *  newer versions of the Windows operating system.
 *  Copyright (C) 2022 Anis Errais
 *
 *  This program is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with this program. If not, see <https://www.gnu.org/licenses/>.
 *
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Win32;

namespace SimpleClassicTheme.Forms
{
	public partial class OptionsForm : Form
	{
		public class UpdateModeItem
		{
			public string Text { get; set; }
			public UpdateMode Value { get; set; }

			public override string ToString()
			{
				return Text;
			}
		}

		public OptionsForm()
		{
			InitializeComponent();
		}

		private void OptionsForm_Load(object sender, EventArgs e)
		{
            comboBoxUpdates.Items.AddRange(new []
			{ 
				new UpdateModeItem { Text = "Automatic", Value = UpdateMode.Automatic },
				new UpdateModeItem { Text = "Ask on startup", Value = UpdateMode.AskOnStartup },
				new UpdateModeItem { Text = "Manual", Value = UpdateMode.Manual },
			});
			comboBoxUpdates.SelectedIndex = 0;
			numericUpDownTaskbarDelay.Value = SCT.Configuration.TaskbarDelay;
			checkBox1.Checked = SCT.Configuration.TaskbarType != TaskbarType.None;
			checkBox2.Checked = SCT.Configuration.BetaUpdates;
			taskbarTypeSelector1.Enabled = numericUpDownTaskbarDelay.Enabled = label2.Enabled = checkBox1.Checked;
		}

		private void buttonOK_Click(object sender, EventArgs e)
		{
			buttonApply.PerformClick();
			buttonCancel.PerformClick();
		}

		private void buttonCancel_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void buttonApply_Click(object sender, EventArgs e)
		{
			SCT.Configuration.UpdateMode = (UpdateMode)comboBoxUpdates.SelectedItem;
			SCT.Configuration.TaskbarType = checkBox1.Checked ? taskbarTypeSelector1.SelectedItem : TaskbarType.None;
			SCT.Configuration.TaskbarDelay = (int)numericUpDownTaskbarDelay.Value;
			SCT.Configuration.BetaUpdates = checkBox2.Checked;
			buttonApply.Enabled = false;
		}

		private void valueChangedEvent(object sender, EventArgs e)
		{
			buttonApply.Enabled = true;
		}

		private void numericUpDownTaskbarDelay_ValueChanged(object sender, EventArgs e)
		{
			valueChangedEvent(sender, e);
		}

		private void checkBox1_CheckedChanged(object sender, EventArgs e)
		{
			taskbarTypeSelector1.Enabled = numericUpDownTaskbarDelay.Enabled = label2.Enabled = checkBox1.Checked;
		}
	}
}
