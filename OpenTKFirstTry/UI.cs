using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenTKFirstTry
{
    public partial class UI : Form
    {
        MainWindow Main;
        

        public UI()
        {
            InitializeComponent();
            TypeOfCalc.SelectedIndex = 0;
            TypeOfParallel.SelectedIndex = 0;
            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 0;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (Main!= null && Main.isRunning == true)
                Main.exit_flag = true;
            Application.Exit();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (TypeOfCalc.SelectedIndex == 2 && TypeOfParallel.SelectedIndex == 2)
            {
                MessageBox.Show("This type of parallelism is unavalible");
            }
            else
            {
                button1.Enabled = false;
                button3.Enabled = false;
                Main = new MainWindow((int)NumOfObj.Value, TypeOfCalc.SelectedIndex, TypeOfParallel.SelectedIndex, comboBox1.SelectedIndex, (int)numericUpDown1.Value, 0, comboBox2.SelectedIndex, null);
                Main.mem += Clear;
                Main.Run(60);
            }
        }

        private void Clear()
        {
            Main.mem -= Clear;
            Main = null;
            button1.Enabled = true;
            button3.Enabled = true;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if(openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string FileName = openFileDialog1.FileName;
                button1.Enabled = false;
                button3.Enabled = false;
                Main = new MainWindow((int)NumOfObj.Value, TypeOfCalc.SelectedIndex, TypeOfParallel.SelectedIndex, comboBox1.SelectedIndex, (int)numericUpDown1.Value,  1, comboBox1.SelectedIndex, FileName);
                Main.mem += Clear;
                Main.Run(60);
            }
            else
            {
                MessageBox.Show("Something went wrong with reading of that file");
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            MessageBox.Show("When Main Window is active press:   " +
                "F - to fill objects with color;   " +
                "L - to see only the vertices;   " +
                "M - for default display mode ;   " +
                "Z - for middle zoom;   " +
                "W - for zooming out;   " +
                "B - for zooming in;   " +
                "S - for default zoom;   ");
        }
      
    }
}
