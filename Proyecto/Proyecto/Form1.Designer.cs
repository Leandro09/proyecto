﻿namespace Proyecto
{
    partial class Form1
    {
        /// <summary>
        /// Variable del diseñador requerida.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén utilizando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido del método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.button1 = new System.Windows.Forms.Button();
            this.hillillosBox1 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.quantumBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.GridResultados = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.GridResultados)).BeginInit();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(783, 39);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(100, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Iniciar Proceso";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // hillillosBox1
            // 
            this.hillillosBox1.Location = new System.Drawing.Point(23, 25);
            this.hillillosBox1.Name = "hillillosBox1";
            this.hillillosBox1.Size = new System.Drawing.Size(100, 20);
            this.hillillosBox1.TabIndex = 1;
            this.hillillosBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(139, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(83, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Número de hilos";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // quantumBox
            // 
            this.quantumBox.Location = new System.Drawing.Point(23, 54);
            this.quantumBox.Name = "quantumBox";
            this.quantumBox.Size = new System.Drawing.Size(100, 20);
            this.quantumBox.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(139, 61);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(50, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Quantum";
            this.label2.Click += new System.EventHandler(this.label2_Click_1);
            // 
            // GridResultados
            // 
            this.GridResultados.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.GridResultados.Location = new System.Drawing.Point(23, 101);
            this.GridResultados.Name = "GridResultados";
            this.GridResultados.Size = new System.Drawing.Size(860, 230);
            this.GridResultados.TabIndex = 6;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(895, 343);
            this.Controls.Add(this.GridResultados);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.quantumBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.hillillosBox1);
            this.Controls.Add(this.button1);
            this.Name = "Form1";
            this.Text = "Proyecto Arquitectura";
            ((System.ComponentModel.ISupportInitialize)(this.GridResultados)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox hillillosBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox quantumBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DataGridView GridResultados;
    }
}

