using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CleanDesk
{
    public partial class SettingsWindow : Form
    {
        public Settings settings;

        public SettingsWindow()
        {
            InitializeComponent();
        }

        public static bool Execute(Settings settings)
        {
            using (var dialog = new SettingsWindow())
            {
                dialog.settings = settings;
                dialog.numericUpDown1.Value = settings.DirtyLimit;

                foreach (var cat in settings.Categories)
                    dialog.listView1.Items.Add(new ListViewItem(new[] { cat.Name, cat.Filter }));

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    settings.DirtyLimit = (int) dialog.numericUpDown1.Value;
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            panel1.Hide();
            settings.Categories.Add(FileCategory.Create(textBox2.Text, textBox1.Text));
            listView1.Items.Add(new ListViewItem(new[] { textBox1.Text, textBox2.Text }));
        }

        private void button5_Click(object sender, EventArgs e)
        {
            panel1.Hide();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Text = "Example";
            textBox2.Text = ".xxx .xxxx";
            panel1.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                if (MessageBox.Show("Are you sure?", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    foreach (ListViewItem item in listView1.SelectedItems)
                    {
                        settings.Categories.Delete(item.Text);
                        item.Remove();
                    }
                }
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            settings.Categories.Default();
            listView1.Items.Clear();
            foreach (var cat in settings.Categories)
                listView1.Items.Add(new ListViewItem(new[] { cat.Name, cat.Filter }));
        }
    }
}
