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
            this.btnSubscrever = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.cbProdutos = new System.Windows.Forms.ListBox();
            this.rtbPromocoes = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // btnSubscrever
            // 
            this.btnSubscrever.Location = new System.Drawing.Point(220, 75);
            this.btnSubscrever.Name = "btnSubscrever";
            this.btnSubscrever.Size = new System.Drawing.Size(102, 45);
            this.btnSubscrever.TabIndex = 0;
            this.btnSubscrever.Text = "Ativar Subscrição";
            this.btnSubscrever.UseVisualStyleBackColor = true;
            this.btnSubscrever.Click += new System.EventHandler(this.btnSubscrever_Click);
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(217, 36);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(139, 16);
            this.lblStatus.TabIndex = 2;
            this.lblStatus.Text = "Status: Desconectado";
            this.lblStatus.Click += new System.EventHandler(this.lblStatus_Click);
            // 
            // cbProdutos
            // 
            this.cbProdutos.FormattingEnabled = true;
            this.cbProdutos.ItemHeight = 16;
            this.cbProdutos.Location = new System.Drawing.Point(13, 22);
            this.cbProdutos.Name = "cbProdutos";
            this.cbProdutos.Size = new System.Drawing.Size(169, 404);
            this.cbProdutos.TabIndex = 3;
            this.cbProdutos.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            // 
            // rtbPromocoes
            // 
            this.rtbPromocoes.Location = new System.Drawing.Point(368, 221);
            this.rtbPromocoes.Name = "rtbPromocoes";
            this.rtbPromocoes.Size = new System.Drawing.Size(100, 96);
            this.rtbPromocoes.TabIndex = 4;
            this.rtbPromocoes.Text = "";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.rtbPromocoes);
            this.Controls.Add(this.cbProdutos);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.btnSubscrever);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Button btnSubscrever;
        private Label lblStatus;
        private ListBox cbProdutos;
        private RichTextBox rtbPromocoes;
    }
}

