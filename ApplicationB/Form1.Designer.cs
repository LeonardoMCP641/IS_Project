using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Schema;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
namespace ApplicationB
{
    partial class Form1
    {

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lstProdutos = new System.Windows.Forms.ListBox();
            this.btnSubscrever = new System.Windows.Forms.Button();
            this.btnAtualizar = new System.Windows.Forms.Button();
            this.lstHistorico = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btndessubscrever = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lstProdutos
            // 
            this.lstProdutos.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.lstProdutos.FormattingEnabled = true;
            this.lstProdutos.ItemHeight = 16;
            this.lstProdutos.Location = new System.Drawing.Point(49, 50);
            this.lstProdutos.Name = "lstProdutos";
            this.lstProdutos.Size = new System.Drawing.Size(251, 388);
            this.lstProdutos.TabIndex = 0;
            // 
            // btnSubscrever
            // 
            this.btnSubscrever.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.btnSubscrever.Location = new System.Drawing.Point(345, 50);
            this.btnSubscrever.Name = "btnSubscrever";
            this.btnSubscrever.Size = new System.Drawing.Size(100, 48);
            this.btnSubscrever.TabIndex = 1;
            this.btnSubscrever.Text = "Subscrever";
            this.btnSubscrever.UseVisualStyleBackColor = false;
            this.btnSubscrever.Click += new System.EventHandler(this.btnSubscrever_Click);
            // 
            // btnAtualizar
            // 
            this.btnAtualizar.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.btnAtualizar.Location = new System.Drawing.Point(116, 7);
            this.btnAtualizar.Name = "btnAtualizar";
            this.btnAtualizar.Size = new System.Drawing.Size(108, 33);
            this.btnAtualizar.TabIndex = 2;
            this.btnAtualizar.Text = "Ver Produtos";
            this.btnAtualizar.UseVisualStyleBackColor = false;
            this.btnAtualizar.Click += new System.EventHandler(this.btnAtualizar_Click);
            // 
            // lstHistorico
            // 
            this.lstHistorico.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.lstHistorico.FormattingEnabled = true;
            this.lstHistorico.ItemHeight = 16;
            this.lstHistorico.Location = new System.Drawing.Point(495, 50);
            this.lstHistorico.Name = "lstHistorico";
            this.lstHistorico.Size = new System.Drawing.Size(265, 388);
            this.lstHistorico.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.label1.Location = new System.Drawing.Point(539, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(178, 16);
            this.label1.TabIndex = 4;
            this.label1.Text = "Promoções em Tempo Real";
            // 
            // btndessubscrever
            // 
            this.btndessubscrever.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.btndessubscrever.Location = new System.Drawing.Point(345, 104);
            this.btndessubscrever.Name = "btndessubscrever";
            this.btndessubscrever.Size = new System.Drawing.Size(100, 48);
            this.btndessubscrever.TabIndex = 5;
            this.btndessubscrever.Text = "Não Subscrever";
            this.btndessubscrever.UseVisualStyleBackColor = false;
            this.btndessubscrever.Click += new System.EventHandler(this.btndessubscrever_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.btndessubscrever);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lstHistorico);
            this.Controls.Add(this.btnAtualizar);
            this.Controls.Add(this.btnSubscrever);
            this.Controls.Add(this.lstProdutos);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }


        #endregion

        private ListBox lstProdutos;
        private Button btnSubscrever;
        private Button btnAtualizar;
        private ListBox lstHistorico;
        private Label label1;
        private Button btndessubscrever;
    }
}

