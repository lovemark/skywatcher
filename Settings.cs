
using System;
using System.Drawing;
using System.Windows.Forms;

namespace SkyWatcher
{
    /// <summary>
    /// The settings form.
    /// </summary>
    public partial class Settings : Form
    {
        public Settings()
        {
            //
            // The InitializeComponent() call is required for Windows Forms designer support.
            //
            InitializeComponent();
            InitializeButton2();
            
            //
            // TODO: Add constructor code after the InitializeComponent() call.
            //
            textBox1.Text = Settings1.Default.CurrentStar;
            monthCalendar1.MaxDate = DateTime.Now.Date;
            monthCalendar1.TodayDate = Settings1.Default.BirthDate;
        }
        void TextBox1TextChanged(object sender, EventArgs e)
        {
            Settings1.Default.CurrentStar = textBox1.Text;
        }
        void MonthCalendar1DateChanged(object sender, DateRangeEventArgs e)
        {
            int day = e.Start.Day;
            int month = e.Start.Month;
            double temp = month * 100 + day;
            temp -= 21;
            temp /= 100;
            temp -= 3;
            if (temp < 0) temp += 12;
            int result = (int)(Math.Truncate(temp));
            textBox2.Text = StarSigns[result];
            monthCalendar1.SetSelectionRange(e.Start, e.Start);
        }
        public string[] StarSigns = {"Aries","Taurus","Gemini","Cancer","Leo","Virgo","Libra","Scorpius","Sagittarius","Capricornus","Aquarius","Pisces"};
        public void GoToStarSign(object sender, EventArgs e) {
            SkyObjectLibrary.Search(textBox2.Text);
            Program.myForm = new MainForm(SkyObjectLibrary.GetItem(SkyObjectLibrary.last_index + 1).Name);
        }
        Button button2;
        void InitializeButton2() {
            button2 = new Button();
            button2.Text = "Dump";
            button2.Size = new Size(75, 23);
            button2.Location = new Point(button1.Left + 90, button1.Top);
            button2.Click += new EventHandler(Dump);
            Controls.Add(button2);
        }
        public void Dump(object sender, EventArgs e) {
            Form dumpInfo = new Form();
            dumpInfo.Text = "Application dump";
            dumpInfo.Icon = Program.myForm.Icon;
            dumpInfo.Size = new Size(300, 300);
            dumpInfo.Location = new Point(Left, Bottom);
            Label label = new Label();
            label.Text = string.Format("Sky X Position: {0}\r\nSky Y Position: {1}\r\n Added stars: {2}", Program.myForm.SkyPositionX, Program.myForm.SkyPositionY, Program.myForm.AddedStars);
            label.AutoSize = true;
            dumpInfo.Controls.Add(label);
            dumpInfo.Show();
        }
    }
}
