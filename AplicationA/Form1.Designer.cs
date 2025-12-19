namespace AplicationA
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
            this.label1 = new System.Windows.Forms.Label();
            this.txtResnameContainer = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnCreateContainer = new System.Windows.Forms.Button();
            this.BtnUpdateContainer = new System.Windows.Forms.Button();
            this.btnUpdateContent = new System.Windows.Forms.Button();
            this.btnCreateContent = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.txtResnameContentInstance = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.lbProdutos = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(514, 36);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(137, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "Criar Nova Promoção";
            // 
            // txtResnameContainer
            // 
            this.txtResnameContainer.Location = new System.Drawing.Point(473, 96);
            this.txtResnameContainer.Name = "txtResnameContainer";
            this.txtResnameContainer.Size = new System.Drawing.Size(176, 22);
            this.txtResnameContainer.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(368, 102);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(89, 16);
            this.label2.TabIndex = 2;
            this.label2.Text = "Novo produto";
            // 
            // btnCreateContainer
            // 
            this.btnCreateContainer.Location = new System.Drawing.Point(674, 62);
            this.btnCreateContainer.Name = "btnCreateContainer";
            this.btnCreateContainer.Size = new System.Drawing.Size(75, 23);
            this.btnCreateContainer.TabIndex = 3;
            this.btnCreateContainer.Text = "Novo";
            this.btnCreateContainer.UseVisualStyleBackColor = true;
            this.btnCreateContainer.Click += new System.EventHandler(this.btnCreateContainer_Click);
            // 
            // BtnUpdateContainer
            // 
            this.BtnUpdateContainer.Location = new System.Drawing.Point(674, 123);
            this.BtnUpdateContainer.Name = "BtnUpdateContainer";
            this.BtnUpdateContainer.Size = new System.Drawing.Size(75, 23);
            this.BtnUpdateContainer.TabIndex = 4;
            this.BtnUpdateContainer.Text = "Alterar";
            this.BtnUpdateContainer.UseVisualStyleBackColor = true;
            this.BtnUpdateContainer.Click += new System.EventHandler(this.BtnUpdateContainer_Click);
            // 
            // btnUpdateContent
            // 
            this.btnUpdateContent.Location = new System.Drawing.Point(674, 232);
            this.btnUpdateContent.Name = "btnUpdateContent";
            this.btnUpdateContent.Size = new System.Drawing.Size(75, 23);
            this.btnUpdateContent.TabIndex = 8;
            this.btnUpdateContent.Text = "Alterar";
            this.btnUpdateContent.UseVisualStyleBackColor = true;
            this.btnUpdateContent.Click += new System.EventHandler(this.btnUpdateContent_Click);
            // 
            // btnCreateContent
            // 
            this.btnCreateContent.Location = new System.Drawing.Point(674, 180);
            this.btnCreateContent.Name = "btnCreateContent";
            this.btnCreateContent.Size = new System.Drawing.Size(75, 23);
            this.btnCreateContent.TabIndex = 7;
            this.btnCreateContent.Text = "Novo";
            this.btnCreateContent.UseVisualStyleBackColor = true;
            this.btnCreateContent.Click += new System.EventHandler(this.btnCreateContent_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(368, 205);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(106, 16);
            this.label3.TabIndex = 6;
            this.label3.Text = "Nova Promoção";
            // 
            // txtResnameContentInstance
            // 
            this.txtResnameContentInstance.Location = new System.Drawing.Point(489, 202);
            this.txtResnameContentInstance.Name = "txtResnameContentInstance";
            this.txtResnameContentInstance.Size = new System.Drawing.Size(162, 22);
            this.txtResnameContentInstance.TabIndex = 5;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(81, 36);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(61, 16);
            this.label4.TabIndex = 9;
            this.label4.Text = "Produtos";
            // 
            // lbProdutos
            // 
            this.lbProdutos.FormattingEnabled = true;
            this.lbProdutos.ItemHeight = 16;
            this.lbProdutos.Location = new System.Drawing.Point(12, 83);
            this.lbProdutos.Name = "lbProdutos";
            this.lbProdutos.Size = new System.Drawing.Size(267, 180);
            this.lbProdutos.TabIndex = 10;
            this.lbProdutos.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lbProdutos_MouseDoubleClick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.lbProdutos);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.btnUpdateContent);
            this.Controls.Add(this.btnCreateContent);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtResnameContentInstance);
            this.Controls.Add(this.BtnUpdateContainer);
            this.Controls.Add(this.btnCreateContainer);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtResnameContainer);
            this.Controls.Add(this.label1);
            this.Name = "Form1";
            this.Text = "Gestor Promoções";
            this.Shown += new System.EventHandler(this.Form1_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtResnameContainer;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnCreateContainer;
        private System.Windows.Forms.Button BtnUpdateContainer;
        private System.Windows.Forms.Button btnUpdateContent;
        private System.Windows.Forms.Button btnCreateContent;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtResnameContentInstance;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ListBox lbProdutos;
    }
}

