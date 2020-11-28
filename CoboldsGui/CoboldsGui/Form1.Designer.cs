namespace CoboldsGui
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
            this.lbStock = new System.Windows.Forms.ListBox();
            this.lbCraft = new System.Windows.Forms.ListBox();
            this.lbQueue = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.btnCraft = new System.Windows.Forms.Button();
            this.btnUndo = new System.Windows.Forms.Button();
            this.btnNextTurn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lbStock
            // 
            this.lbStock.FormattingEnabled = true;
            this.lbStock.Location = new System.Drawing.Point(12, 57);
            this.lbStock.Name = "lbStock";
            this.lbStock.Size = new System.Drawing.Size(120, 95);
            this.lbStock.TabIndex = 0;
            // 
            // lbCraft
            // 
            this.lbCraft.FormattingEnabled = true;
            this.lbCraft.Location = new System.Drawing.Point(138, 57);
            this.lbCraft.Name = "lbCraft";
            this.lbCraft.Size = new System.Drawing.Size(120, 95);
            this.lbCraft.TabIndex = 1;
            // 
            // lbQueue
            // 
            this.lbQueue.FormattingEnabled = true;
            this.lbQueue.Location = new System.Drawing.Point(264, 57);
            this.lbQueue.Name = "lbQueue";
            this.lbQueue.Size = new System.Drawing.Size(120, 95);
            this.lbQueue.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 41);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Stock";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(135, 41);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Craft";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(261, 41);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(39, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Queue";
            // 
            // btnCraft
            // 
            this.btnCraft.Location = new System.Drawing.Point(162, 171);
            this.btnCraft.Name = "btnCraft";
            this.btnCraft.Size = new System.Drawing.Size(75, 23);
            this.btnCraft.TabIndex = 6;
            this.btnCraft.Text = "Craft";
            this.btnCraft.UseVisualStyleBackColor = true;
            // 
            // btnUndo
            // 
            this.btnUndo.Location = new System.Drawing.Point(284, 171);
            this.btnUndo.Name = "btnUndo";
            this.btnUndo.Size = new System.Drawing.Size(75, 23);
            this.btnUndo.TabIndex = 7;
            this.btnUndo.Text = "Undo";
            this.btnUndo.UseVisualStyleBackColor = true;
            // 
            // btnNextTurn
            // 
            this.btnNextTurn.Location = new System.Drawing.Point(412, 96);
            this.btnNextTurn.Name = "btnNextTurn";
            this.btnNextTurn.Size = new System.Drawing.Size(75, 23);
            this.btnNextTurn.TabIndex = 8;
            this.btnNextTurn.Text = "NextTurn";
            this.btnNextTurn.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(499, 239);
            this.Controls.Add(this.btnNextTurn);
            this.Controls.Add(this.btnUndo);
            this.Controls.Add(this.btnCraft);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lbQueue);
            this.Controls.Add(this.lbCraft);
            this.Controls.Add(this.lbStock);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        public System.Windows.Forms.ListBox lbStock;
        public System.Windows.Forms.ListBox lbCraft;
        public System.Windows.Forms.ListBox lbQueue;
        public System.Windows.Forms.Button btnCraft;
        public System.Windows.Forms.Button btnUndo;
        public System.Windows.Forms.Button btnNextTurn;
    }
}

