using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Vocals.InternalClasses;

namespace Vocals {
    public partial class FormCommand : Form {
        public List<Actions> actionList { get; set; }
        public string commandString { get; set; }

        public bool answering { get; set; }
        public string answeringString { get; set; }

        public bool answeringSound { get; set; }
        public string answeringSoundPath { get; set; }

        public FormCommand() {
            InitializeComponent();
            actionList = new List<Actions>();
            commandString = "";

            answering = false;
            answeringString = "";

            listBox1.DataSource = actionList;
        }

        public FormCommand(Command c) {
            InitializeComponent();
            actionList = c.actionList;
            commandString = c.commandString;

            answering = c.answering;
            checkBox1.Checked = answering;

            answeringString = c.answeringString;
            richTextBox1.Text = answeringString;

            answeringSound = c.answeringSound;
            checkBox2.Checked = answeringSound;

            answeringSoundPath = c.answeringSoundPath;
            
            if (!string.IsNullOrEmpty(answeringSoundPath))
            {
                foreach(string soundPath in answeringSoundPath.Split(';'))
                {
                    dgvSounds.Rows.Add(new object[] { soundPath });
                }
            }

            listBox1.DataSource = actionList;
            textBox1.Text = commandString;
        }


        private void textBox1_TextChanged(object sender, EventArgs e) {
            this.commandString = textBox1.Text;
        }

        private void button3_Click(object sender, EventArgs e) {
            if (listBox1.SelectedItem != null) {
                actionList.RemoveAt(listBox1.SelectedIndex);
                listBox1.DataSource = null;
                listBox1.DataSource = actionList;
            }
        }

        private void button1_Click(object sender, EventArgs e) {
            FormAction newActionForm = new FormAction();
            newActionForm.ShowDialog();

            if (newActionForm.selectedType != "") {
                if (newActionForm.selectedType == "Key press" && newActionForm.selectedKey != Keys.None
                    || newActionForm.selectedType == "Timer" && newActionForm.selectedTimer != 0) {

                    Actions myNewAction = new Actions(newActionForm.selectedType, newActionForm.selectedKey, newActionForm.modifier, newActionForm.selectedTimer);
                    

                    actionList.Add(myNewAction);

                    listBox1.DataSource = null;
                    listBox1.DataSource = actionList;
                }
            }
        }

        private void button4_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void button5_Click(object sender, EventArgs e) {
            commandString = "";
            actionList.Clear();
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e) {
            Actions a = (Actions)listBox1.SelectedItem;
            if (a != null) {
                FormAction formEditAction = new FormAction(a);
                formEditAction.ShowDialog();

                a.keys = formEditAction.selectedKey;
                a.type = formEditAction.selectedType;
                a.keyModifier = formEditAction.modifier;
                a.timer = (float)formEditAction.selectedTimer;

                listBox1.DataSource = null;
                listBox1.DataSource = actionList;
            }
        }

        private void button6_Click(object sender, EventArgs e) {
            int selectedIndex = listBox1.SelectedIndex;
            if (selectedIndex > 0) {
                Actions actionToMoveDown = actionList.ElementAt(selectedIndex - 1);
                actionList.RemoveAt(selectedIndex - 1);
                actionList.Insert(selectedIndex, actionToMoveDown);

                listBox1.DataSource = null;
                listBox1.DataSource = actionList;
                listBox1.SelectedIndex = selectedIndex - 1;
            }
        }

        private void button7_Click(object sender, EventArgs e) {
            int selectedIndex = listBox1.SelectedIndex;
            if (selectedIndex < actionList.Count - 1) {
                Actions actionToMoveUp = actionList.ElementAt(selectedIndex + 1);
                actionList.RemoveAt(selectedIndex + 1);
                actionList.Insert(selectedIndex, actionToMoveUp);

                listBox1.DataSource = null;
                listBox1.DataSource = actionList;
                listBox1.SelectedIndex = selectedIndex + 1;
            }
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e) {
            answeringString = richTextBox1.Text;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e) {
            if (checkBox1.Checked) {
                checkBox2.Checked = false;
                answeringSound = false;
            }
            answering = checkBox1.Checked;
        }

        private void button9_Click(object sender, EventArgs e) {
            OpenFileDialog ofd = new OpenFileDialog();

            ofd.Filter = "Sound file (*.wav,*.mp3)|*.wav;*.mp3";
            ofd.Multiselect = true;

            if (ofd.ShowDialog() == DialogResult.OK && ofd.CheckPathExists) {
                foreach(string filename in ofd.FileNames)
                {
                    dgvSounds.Rows.Add(new object[] { filename });

                    if (answeringSoundPath.Length > 0)
                        answeringSoundPath += ";";
                    answeringSoundPath += filename;
                }
            }
           
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e) {
            if (checkBox2.Checked) {
                checkBox1.Checked = false;
                answering = false;
            }
            answeringSound = true;
        }

        private void dgvSounds_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            var senderGrid = (DataGridView)sender;

            if (senderGrid.Columns[e.ColumnIndex] is DataGridViewButtonColumn &&
                e.RowIndex >= 0)
            {
                int rowHandler = e.RowIndex;

                switch(senderGrid.Columns[e.ColumnIndex].Name)
                {
                    case "PlaySound":
                        string soundPath = senderGrid.Rows[rowHandler].Cells["SoundPath"].Value.ToString();
                        Utils.PlaySound(soundPath);
                        break;

                    case "RemoveSound":
                        if (MessageBox.Show("Are you sure you want to remove this Sound?", "Question", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            dgvSounds.Rows.RemoveAt(e.RowIndex);
                            answeringSoundPath = string.Empty;
                            foreach (DataGridViewRow r in dgvSounds.Rows)
                            {
                                if (answeringSoundPath.Length > 0)
                                    answeringSoundPath += ";";
                                answeringSoundPath += r.Cells["SoundPath"].Value.ToString();
                            }
                        }
                        break;
                }
            }
        }

        private void dgvSounds_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            var senderGrid = (DataGridView)sender;

            if (e.ColumnIndex >= 0 && e.RowIndex >= 0)
            {
                e.Paint(e.CellBounds, DataGridViewPaintParts.All);

                DataGridViewButtonCell cellButton = senderGrid.Rows[e.RowIndex].Cells[senderGrid.Columns[e.ColumnIndex].Name] as DataGridViewButtonCell;

                switch (senderGrid.Columns[e.ColumnIndex].Name)
                {
                    case "PlaySound":
                        e.Graphics.DrawIcon(Vocals.Properties.Resources.PlayIcon, e.CellBounds.Left + 2, e.CellBounds.Top);

                        senderGrid.Rows[e.RowIndex].Height = Vocals.Properties.Resources.PlayIcon.Height;
                        senderGrid.Columns[e.ColumnIndex].Width = Vocals.Properties.Resources.PlayIcon.Width + 4;

                        break;

                    case "RemoveSound":
                        e.Graphics.DrawIcon(Vocals.Properties.Resources.NoIcon, e.CellBounds.Left + 2, e.CellBounds.Top);

                        senderGrid.Rows[e.RowIndex].Height = Vocals.Properties.Resources.NoIcon.Height;
                        senderGrid.Columns[e.ColumnIndex].Width = Vocals.Properties.Resources.NoIcon.Width + 4;

                        break;
                }
                e.Handled = true;
            }
        }
    }
}
