namespace MultiagentModelingSystem
{
    partial class TranspotTunnel
    {
        /// <summary>
        /// Обязательная переменная конструктора.
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
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.tunnelPictureBox = new System.Windows.Forms.PictureBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.файлToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.задатьКонфигурациюТоннеляToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.начатьМоделированиеToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.tunnelPictureBox)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tunnelPictureBox
            // 
            this.tunnelPictureBox.BackColor = System.Drawing.SystemColors.ControlDark;
            this.tunnelPictureBox.Location = new System.Drawing.Point(12, 31);
            this.tunnelPictureBox.Name = "tunnelPictureBox";
            this.tunnelPictureBox.Size = new System.Drawing.Size(1293, 500);
            this.tunnelPictureBox.TabIndex = 0;
            this.tunnelPictureBox.TabStop = false;
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.файлToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1317, 28);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // файлToolStripMenuItem
            // 
            this.файлToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.задатьКонфигурациюТоннеляToolStripMenuItem,
            this.начатьМоделированиеToolStripMenuItem});
            this.файлToolStripMenuItem.Name = "файлToolStripMenuItem";
            this.файлToolStripMenuItem.Size = new System.Drawing.Size(57, 24);
            this.файлToolStripMenuItem.Text = "Файл";
            // 
            // задатьКонфигурациюТоннеляToolStripMenuItem
            // 
            this.задатьКонфигурациюТоннеляToolStripMenuItem.Name = "задатьКонфигурациюТоннеляToolStripMenuItem";
            this.задатьКонфигурациюТоннеляToolStripMenuItem.Size = new System.Drawing.Size(250, 26);
            this.задатьКонфигурациюТоннеляToolStripMenuItem.Text = "Задать конфигурацию";
            this.задатьКонфигурациюТоннеляToolStripMenuItem.Click += new System.EventHandler(this.задатьКонфигурациюТоннеляToolStripMenuItem_Click);
            // 
            // начатьМоделированиеToolStripMenuItem
            // 
            this.начатьМоделированиеToolStripMenuItem.Name = "начатьМоделированиеToolStripMenuItem";
            this.начатьМоделированиеToolStripMenuItem.Size = new System.Drawing.Size(250, 26);
            this.начатьМоделированиеToolStripMenuItem.Text = "Начать моделирование";
            this.начатьМоделированиеToolStripMenuItem.Click += new System.EventHandler(this.начатьМоделированиеToolStripMenuItem_Click);
            // 
            // TranspotTunnel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1317, 596);
            this.Controls.Add(this.tunnelPictureBox);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "TranspotTunnel";
            this.Text = "Tansport tunnel";
            ((System.ComponentModel.ISupportInitialize)(this.tunnelPictureBox)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox tunnelPictureBox;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem файлToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem задатьКонфигурациюТоннеляToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem начатьМоделированиеToolStripMenuItem;
    }
}

