namespace Forms
{
    partial class MainForm
    {
        /// <summary>
        /// Требуется переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Обязательный метод для поддержки конструктора - не изменяйте
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnNextTurn = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.lbUnskilled = new System.Windows.Forms.Label();
            this.lbSmelters = new System.Windows.Forms.Label();
            this.lbWariors = new System.Windows.Forms.Label();
            this.lbMiners = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.lbMashroomsFarming = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.lbMashrooms = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.lbHammers = new System.Windows.Forms.Label();
            this.lbSpears = new System.Windows.Forms.Label();
            this.lbPickaxes = new System.Windows.Forms.Label();
            this.lbSteels = new System.Windows.Forms.Label();
            this.lbIronOres = new System.Windows.Forms.Label();
            this.lbWoods = new System.Windows.Forms.Label();
            this.btnlbMashroomsFarmingPlus = new System.Windows.Forms.Button();
            this.btnlbMashroomsFarmingMinus = new System.Windows.Forms.Button();
            this.lbName = new System.Windows.Forms.Label();
            this.lbAdd = new System.Windows.Forms.Label();
            this.lbAvail = new System.Windows.Forms.Label();
            this.lbCrafting = new System.Windows.Forms.Label();
            this.lbIngrsReq = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.lbCraftAvail = new System.Windows.Forms.Label();
            this.SuspendLayout();
            //
            // btnNextTurn
            //
            this.btnNextTurn.Location = new System.Drawing.Point(455, 251);
            this.btnNextTurn.Name = "btnNextTurn";
            this.btnNextTurn.Size = new System.Drawing.Size(70, 36);
            this.btnNextTurn.TabIndex = 0;
            this.btnNextTurn.Text = "Сделать ход";
            this.btnNextTurn.UseVisualStyleBackColor = true;
            //
            // label1
            //
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(33, 327);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(42, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Народ:";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            //
            // label2
            //
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(33, 340);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(78, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Разнорабочих";
            //
            // label3
            //
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(33, 366);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(32, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Вояк";
            //
            // label4
            //
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(33, 353);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(71, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Литейщиков";
            //
            // label5
            //
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(33, 379);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(56, 13);
            this.label5.TabIndex = 8;
            this.label5.Text = "Шахтеров";
            //
            // lbUnskilled
            //
            this.lbUnskilled.AutoSize = true;
            this.lbUnskilled.Location = new System.Drawing.Point(160, 340);
            this.lbUnskilled.Name = "lbUnskilled";
            this.lbUnskilled.Size = new System.Drawing.Size(24, 13);
            this.lbUnskilled.TabIndex = 9;
            this.lbUnskilled.Text = "0/0";
            //
            // lbSmelters
            //
            this.lbSmelters.AutoSize = true;
            this.lbSmelters.Location = new System.Drawing.Point(160, 353);
            this.lbSmelters.Name = "lbSmelters";
            this.lbSmelters.Size = new System.Drawing.Size(24, 13);
            this.lbSmelters.TabIndex = 10;
            this.lbSmelters.Text = "0/0";
            //
            // lbWariors
            //
            this.lbWariors.AutoSize = true;
            this.lbWariors.Location = new System.Drawing.Point(160, 366);
            this.lbWariors.Name = "lbWariors";
            this.lbWariors.Size = new System.Drawing.Size(24, 13);
            this.lbWariors.TabIndex = 11;
            this.lbWariors.Text = "0/0";
            //
            // lbMiners
            //
            this.lbMiners.AutoSize = true;
            this.lbMiners.Location = new System.Drawing.Point(160, 379);
            this.lbMiners.Name = "lbMiners";
            this.lbMiners.Size = new System.Drawing.Size(24, 13);
            this.lbMiners.TabIndex = 12;
            this.lbMiners.Text = "0/0";
            //
            // label6
            //
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(33, 420);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(106, 13);
            this.label6.TabIndex = 13;
            this.label6.Text = "Посадка грибочков";
            //
            // label7
            //
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(33, 303);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(56, 13);
            this.label7.TabIndex = 14;
            this.label7.Text = "Free/Busy";
            //
            // lbMashroomsFarming
            //
            this.lbMashroomsFarming.AutoSize = true;
            this.lbMashroomsFarming.Location = new System.Drawing.Point(160, 420);
            this.lbMashroomsFarming.Name = "lbMashroomsFarming";
            this.lbMashroomsFarming.Size = new System.Drawing.Size(24, 13);
            this.lbMashroomsFarming.TabIndex = 15;
            this.lbMashroomsFarming.Text = "0/0";
            //
            // label8
            //
            this.label8.AutoSize = true;
            this.label8.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label8.Location = new System.Drawing.Point(359, 377);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(53, 15);
            this.label8.TabIndex = 16;
            this.label8.Text = "Молотов";
            //
            // label9
            //
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(359, 364);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(60, 13);
            this.label9.TabIndex = 17;
            this.label9.Text = "Грибочков";
            //
            // label10
            //
            this.label10.AutoSize = true;
            this.label10.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.label10.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label10.Location = new System.Drawing.Point(359, 352);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(59, 15);
            this.label10.TabIndex = 18;
            this.label10.Text = "Название";
            //
            // label11
            //
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(438, 352);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(41, 13);
            this.label11.TabIndex = 19;
            this.label11.Text = "Кол-во";
            //
            // lbMashrooms
            //
            this.lbMashrooms.AutoSize = true;
            this.lbMashrooms.Location = new System.Drawing.Point(452, 364);
            this.lbMashrooms.Name = "lbMashrooms";
            this.lbMashrooms.Size = new System.Drawing.Size(13, 13);
            this.lbMashrooms.TabIndex = 20;
            this.lbMashrooms.Text = "0";
            //
            // label12
            //
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(359, 390);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(44, 13);
            this.label12.TabIndex = 21;
            this.label12.Text = "Копьев";
            //
            // label13
            //
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(359, 403);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(38, 13);
            this.label13.TabIndex = 22;
            this.label13.Text = "Кирок";
            //
            // label14
            //
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(359, 416);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(37, 13);
            this.label14.TabIndex = 23;
            this.label14.Text = "Сталь";
            //
            // label15
            //
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(359, 444);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(66, 13);
            this.label15.TabIndex = 24;
            this.label15.Text = "Бревнышек";
            //
            // label16
            //
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(359, 429);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(48, 13);
            this.label16.TabIndex = 25;
            this.label16.Text = "Железа";
            //
            // lbHammers
            //
            this.lbHammers.AutoSize = true;
            this.lbHammers.Location = new System.Drawing.Point(452, 377);
            this.lbHammers.Name = "lbHammers";
            this.lbHammers.Size = new System.Drawing.Size(13, 13);
            this.lbHammers.TabIndex = 26;
            this.lbHammers.Text = "0";
            //
            // lbSpears
            //
            this.lbSpears.AutoSize = true;
            this.lbSpears.Location = new System.Drawing.Point(452, 390);
            this.lbSpears.Name = "lbSpears";
            this.lbSpears.Size = new System.Drawing.Size(13, 13);
            this.lbSpears.TabIndex = 27;
            this.lbSpears.Text = "0";
            //
            // lbPickaxes
            //
            this.lbPickaxes.AutoSize = true;
            this.lbPickaxes.Location = new System.Drawing.Point(452, 403);
            this.lbPickaxes.Name = "lbPickaxes";
            this.lbPickaxes.Size = new System.Drawing.Size(13, 13);
            this.lbPickaxes.TabIndex = 28;
            this.lbPickaxes.Text = "0";
            //
            // lbSteels
            //
            this.lbSteels.AutoSize = true;
            this.lbSteels.Location = new System.Drawing.Point(452, 416);
            this.lbSteels.Name = "lbSteels";
            this.lbSteels.Size = new System.Drawing.Size(13, 13);
            this.lbSteels.TabIndex = 29;
            this.lbSteels.Text = "0";
            //
            // lbIronOres
            //
            this.lbIronOres.AutoSize = true;
            this.lbIronOres.Location = new System.Drawing.Point(452, 429);
            this.lbIronOres.Name = "lbIronOres";
            this.lbIronOres.Size = new System.Drawing.Size(13, 13);
            this.lbIronOres.TabIndex = 30;
            this.lbIronOres.Text = "0";
            //
            // lbWoods
            //
            this.lbWoods.AutoSize = true;
            this.lbWoods.Location = new System.Drawing.Point(452, 444);
            this.lbWoods.Name = "lbWoods";
            this.lbWoods.Size = new System.Drawing.Size(13, 13);
            this.lbWoods.TabIndex = 31;
            this.lbWoods.Text = "0";
            //
            // btnlbMashroomsFarmingPlus
            //
            this.btnlbMashroomsFarmingPlus.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnlbMashroomsFarmingPlus.Location = new System.Drawing.Point(229, 340);
            this.btnlbMashroomsFarmingPlus.Name = "btnlbMashroomsFarmingPlus";
            this.btnlbMashroomsFarmingPlus.Size = new System.Drawing.Size(10, 13);
            this.btnlbMashroomsFarmingPlus.TabIndex = 32;
            this.btnlbMashroomsFarmingPlus.Text = "+";
            this.btnlbMashroomsFarmingPlus.UseVisualStyleBackColor = true;
            //
            // btnlbMashroomsFarmingMinus
            //
            this.btnlbMashroomsFarmingMinus.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnlbMashroomsFarmingMinus.Location = new System.Drawing.Point(333, 416);
            this.btnlbMashroomsFarmingMinus.Name = "btnlbMashroomsFarmingMinus";
            this.btnlbMashroomsFarmingMinus.Size = new System.Drawing.Size(10, 13);
            this.btnlbMashroomsFarmingMinus.TabIndex = 33;
            this.btnlbMashroomsFarmingMinus.Text = "-";
            this.btnlbMashroomsFarmingMinus.UseVisualStyleBackColor = true;
            //
            // lbName
            //
            this.lbName.AutoSize = true;
            this.lbName.Location = new System.Drawing.Point(50, 9);
            this.lbName.Name = "lbName";
            this.lbName.Size = new System.Drawing.Size(35, 13);
            this.lbName.TabIndex = 34;
            this.lbName.Text = "Name";
            //
            // lbAdd
            //
            this.lbAdd.AutoSize = true;
            this.lbAdd.Location = new System.Drawing.Point(12, 9);
            this.lbAdd.Name = "lbAdd";
            this.lbAdd.Size = new System.Drawing.Size(26, 13);
            this.lbAdd.TabIndex = 35;
            this.lbAdd.Text = "Add";
            //
            // lbAvail
            //
            this.lbAvail.AutoSize = true;
            this.lbAvail.Location = new System.Drawing.Point(146, 9);
            this.lbAvail.Name = "lbAvail";
            this.lbAvail.Size = new System.Drawing.Size(30, 13);
            this.lbAvail.TabIndex = 36;
            this.lbAvail.Text = "Avail";
            //
            // lbCrafting
            //
            this.lbCrafting.AutoSize = true;
            this.lbCrafting.Location = new System.Drawing.Point(277, 9);
            this.lbCrafting.Name = "lbCrafting";
            this.lbCrafting.Size = new System.Drawing.Size(43, 13);
            this.lbCrafting.TabIndex = 37;
            this.lbCrafting.Text = "Crafting";
            //
            // lbIngrsReq
            //
            this.lbIngrsReq.AutoSize = true;
            this.lbIngrsReq.Location = new System.Drawing.Point(484, 9);
            this.lbIngrsReq.Name = "lbIngrsReq";
            this.lbIngrsReq.Size = new System.Drawing.Size(50, 13);
            this.lbIngrsReq.TabIndex = 38;
            this.lbIngrsReq.Text = "IngrsReq";
            //
            // label17
            //
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(265, 444);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(54, 13);
            this.label17.TabIndex = 39;
            this.label17.Text = "Бобышек";
            //
            // lbCraftAvail
            //
            this.lbCraftAvail.AutoSize = true;
            this.lbCraftAvail.Location = new System.Drawing.Point(399, 9);
            this.lbCraftAvail.Name = "lbCraftAvail";
            this.lbCraftAvail.Size = new System.Drawing.Size(52, 13);
            this.lbCraftAvail.TabIndex = 40;
            this.lbCraftAvail.Text = "CraftAvail";
            //
            // MainForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(797, 598);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.lbCraftAvail);
            this.Controls.Add(this.label17);
            this.Controls.Add(this.lbIngrsReq);
            this.Controls.Add(this.lbCrafting);
            this.Controls.Add(this.lbAvail);
            this.Controls.Add(this.lbAdd);
            this.Controls.Add(this.lbName);
            this.Controls.Add(this.btnlbMashroomsFarmingMinus);
            this.Controls.Add(this.btnlbMashroomsFarmingPlus);
            this.Controls.Add(this.lbWoods);
            this.Controls.Add(this.lbIronOres);
            this.Controls.Add(this.lbSteels);
            this.Controls.Add(this.lbPickaxes);
            this.Controls.Add(this.lbSpears);
            this.Controls.Add(this.lbHammers);
            this.Controls.Add(this.label16);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.lbMashrooms);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.lbMashroomsFarming);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.lbMiners);
            this.Controls.Add(this.lbWariors);
            this.Controls.Add(this.lbSmelters);
            this.Controls.Add(this.lbUnskilled);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnNextTurn);
            this.Name = "MainForm";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnlbMashroomsFarmingPlus;
        private System.Windows.Forms.Button btnlbMashroomsFarmingMinus;
        public System.Windows.Forms.Label lbName;
        public System.Windows.Forms.Label lbAdd;
        public System.Windows.Forms.Label lbAvail;
        public System.Windows.Forms.Label lbCrafting;
        public System.Windows.Forms.Label lbIngrsReq;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lbUnskilled;
        private System.Windows.Forms.Label lbSmelters;
        private System.Windows.Forms.Label lbWariors;
        private System.Windows.Forms.Label lbMiners;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label lbMashroomsFarming;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label lbMashrooms;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label lbHammers;
        private System.Windows.Forms.Label lbSpears;
        private System.Windows.Forms.Label lbPickaxes;
        private System.Windows.Forms.Label lbSteels;
        private System.Windows.Forms.Label lbIronOres;
        private System.Windows.Forms.Label lbWoods;
        private System.Windows.Forms.Label label17;
        public System.Windows.Forms.Label lbCraftAvail;
        public System.Windows.Forms.Button btnNextTurn;

    }
}

