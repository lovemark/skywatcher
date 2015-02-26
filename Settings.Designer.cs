
namespace SkyWatcher
{
    partial class Settings
    {
        /// <summary>
        /// Designer variable used to keep track of non-visual components.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Disposes resources used by the form.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing) {
                if (components != null) {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// This method is required for Windows Forms designer support.
        /// Do not change the method contents inside the source code editor. The Forms designer might
        /// not be able to load this method if it was changed manually.
        /// </summary>
        private void InitializeComponent()
        {
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.monthCalendar1 = new System.Windows.Forms.MonthCalendar();
            this.label3 = new System.Windows.Forms.Label();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            //
            // textBox1
            //
            this.textBox1.Location = new System.Drawing.Point(128, 3);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(100, 20);
            this.textBox1.TabIndex = 0;
            this.textBox1.Text = "Altair";
            this.textBox1.MouseLeave += new System.EventHandler(this.TextBox1TextChanged);
            //
            // label1
            //
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(112, 23);
            this.label1.TabIndex = 1;
            this.label1.Text = "Star shown at launch";
            //
            // label2
            //
            this.label2.Location = new System.Drawing.Point(0, 32);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(112, 23);
            this.label2.TabIndex = 2;
            this.label2.Text = "Birth date";
            //
            // monthCalendar1
            //
            this.monthCalendar1.Location = new System.Drawing.Point(128, 24);
            this.monthCalendar1.MaxDate = new System.DateTime(2017, 12, 21, 0, 0, 0, 0);
            this.monthCalendar1.MaxSelectionCount = 3;
            this.monthCalendar1.MinDate = new System.DateTime(1790, 1, 1, 0, 0, 0, 0);
            this.monthCalendar1.Name = "monthCalendar1";
            this.monthCalendar1.ShowToday = false;
            this.monthCalendar1.ShowTodayCircle = false;
            this.monthCalendar1.TabIndex = 3;
            this.monthCalendar1.TitleBackColor = System.Drawing.Color.White;
            this.monthCalendar1.TrailingForeColor = System.Drawing.Color.Silver;
            this.monthCalendar1.DateChanged += new System.Windows.Forms.DateRangeEventHandler(this.MonthCalendar1DateChanged);
            //
            // label3
            //
            this.label3.Location = new System.Drawing.Point(0, 200);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(112, 23);
            this.label3.TabIndex = 4;
            this.label3.Text = "Your star sign";
            //
            // textBox2
            //
            this.textBox2.Location = new System.Drawing.Point(128, 200);
            this.textBox2.Name = "textBox2";
            this.textBox2.ReadOnly = true;
            this.textBox2.Size = new System.Drawing.Size(100, 20);
            this.textBox2.TabIndex = 5;
            this.textBox2.Text = "Aquarius";
            //
            // button1
            //
            this.button1.Location = new System.Drawing.Point(240, 200);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 6;
            this.button1.Text = "Go";
            this.button1.Click += new System.EventHandler(this.GoToStarSign);
            //
            // Settings
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(418, 262);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.monthCalendar1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.button1);
            this.Name = "Settings";
            this.Text = "Settings";
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.MonthCalendar monthCalendar1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button button1;
    }
}
