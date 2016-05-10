using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Proyecto
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            tarea prueba = new tarea();
            //Variable y cast para obtener el número de hilillos y el quantum.
            int numero_hilillos;
            int quantum;
            if (hillillosBox1.Text != null && !string.IsNullOrWhiteSpace(hillillosBox1.Text))
            {
                if (quantumBox.Text != null && !string.IsNullOrWhiteSpace(quantumBox.Text))
                {
                    numero_hilillos = Convert.ToInt32(hillillosBox1.Text);
                    quantum = Convert.ToInt32(quantumBox.Text);
                    //numero_hilillos = int.Parse(textBox1.Text);

                    prueba.setHilillos(numero_hilillos);
                    prueba.setQuantum(quantum);
                    prueba.inicializarEstructuras();
                    prueba.leeArchivos();
                    
                }
                else
                {
                    MessageBox.Show("Por favor introduzca un número para el quantum", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                }
            }
            else
            {
                MessageBox.Show("Por favor introduzca un número de hilillos entre 1 y 12", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
            

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click_1(object sender, EventArgs e)
        {

        }
    }
}
