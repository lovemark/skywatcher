using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SkyWatcher
{
    /// <summary>
    /// The main form of the application.
    /// </summary>
    public partial class MainForm : Form, IContainer
    {
        const string degreeChar = "\u00b0";
        const string nl = "\r\n";
        public int SkyPositionX = ((DateTime.Now.Month * 2) + 6) % 24;
        public int SkyPositionY = 30;
        public int AddedStars;
        public Star[] AddedStarsTable = new Star[0];
        public static bool OpenedSettingsForm;
        readonly int[,] linkages = {
            // Linkages for Andromeda
            {1,4},{1,16},{4,2},{2,3},{16,12},
            // Linkages for Antlia
            {30,23},{23,29},{29,25},
            // Linkages for Apus
            {35,36},{36,37},{37,34},
            // Linkages for Aquarius
            {49,64},{64,56},{56,65},{65,52},{52,51},{51,48},{48,46},{46,53},{53,54},{46,47},{47,57},{57,50}
        };
        public MainForm(double x, double y)
        {
            //
            // The InitializeComponent() call is required for Windows Forms designer support.
            //
            InitializeComponent();

            //
            // TODO: Add constructor code after the InitializeComponent() call.
            //
            SkyPositionX = (int)(x);
            SkyPositionY = (int)(y);
            InitialiseDataGridView();

            // Remove remainders for correct positioning
            double xrem = Math.IEEERemainder(SkyPositionX, 60);
            double yrem = Math.IEEERemainder(SkyPositionY, 10);
            if (xrem != 0) {
                SkyPositionX -= (int)(xrem);
                SkyPositionX += 60;
            }
            if (yrem != 0) {
                SkyPositionY -= (int)(yrem);
                SkyPositionY += 10;
            }
            label1.Text = (SkyPositionX / 60) + "h";
            label2.Text = ((SkyPositionX - 1) / 60) + "h";
            label3.Text = SkyPositionY + degreeChar;
            label4.Text = (SkyPositionY - 10) + degreeChar;
            label5.Text = (SkyPositionY - 20) + degreeChar;
            OpenSettingsForm();
        }
        public MainForm(string searchText) {
            InitializeComponent();
            Star result = (Star)(SkyObjectLibrary.Search(searchText));
            int x = (int)(result.RA * 60);
            int y = (int)(result.Dec);
            SkyPositionX = (int)(x);
            SkyPositionY = (int)(y);
            InitialiseDataGridView();

            // Remove remainders for correct positioning
            double xrem = Math.IEEERemainder(SkyPositionX, 60);
            double yrem = Math.IEEERemainder(SkyPositionY, 10);
            if (xrem != 0) {
                SkyPositionX -= (int)(xrem);
                SkyPositionX += 60;
            }
            if (yrem != 0) {
                SkyPositionY -= (int)(yrem);
                SkyPositionY += 10;
            }
            label1.Text = (SkyPositionX / 60) + "h";
            label2.Text = ((SkyPositionX - 1) / 60) + "h";
            label3.Text = SkyPositionY + degreeChar;
            label4.Text = (SkyPositionY - 10) + degreeChar;
            label5.Text = (SkyPositionY - 20) + degreeChar;
            OpenSettingsForm();
        }
        public void InitialiseDataGridView() {
            dataGridView1.RowCount = 3;
        }
        void DataGridView1CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            int minX = e.ColumnIndex * 300;
            int minY = e.RowIndex * 200;
            int maxX = (e.ColumnIndex + 1) * 300;
            int maxY = (e.RowIndex + 1) * 200;
            Invalidate(new Region(new Rectangle(minX, minY, maxX - minX, maxY - minY)));
            for (int i = 0; i < Star.totalstars; i++) {
                try {
                    Star currentStar = (Star)(SkyObjectLibrary.GetItem(i));
                    Point location = currentStar.GetLocation(SkyPositionX, SkyPositionY);
                    Point location2 = currentStar.GetLocation2(SkyPositionX, SkyPositionY);
                    if (((location.X >= minX) && (location.Y >= minY) && (location.X < maxX) && (location.Y < maxY)) || (location2.X >= minX) && (location2.X < maxX) && (location2.Y >= minY) && (location2.Y < minY)) {
                        AddedStars++;
                        Star[] temp = AddedStarsTable;
                        AddedStarsTable = new Star[AddedStarsTable.Length + 1];
                        Array.Copy(temp, AddedStarsTable, temp.Length);
                        AddedStarsTable[AddedStars - 1] = currentStar;
                        location.X -= 2;
                        location.Y -= 2;
                        Control controlToAdd = new Control(this, String.Empty);
                        controlToAdd.Location = location;
                        controlToAdd.BackColor = currentStar.CustomColour;
                        controlToAdd.Size = new Size(5, 5);
                        controlToAdd.Click += StarClicked;
                        controlToAdd.Name = currentStar.Name;
                        controlToAdd.TabIndex = Controls.Count;
                        if (currentStar.IsNamed) {
                            Label nameLabel = new Label();
                            nameLabel.Location = controlToAdd.Location;
                            nameLabel.Location.Offset(4, 4);
                            nameLabel.ForeColor = Color.White;
                            nameLabel.AutoSize = true;
                            nameLabel.Font = new Font("Microsoft Sans Serif", 12);
                            nameLabel.Text = currentStar.Name;
                            nameLabel.TabIndex = Controls.Count;
                            AddedStars++;
                            controlToAdd.TabIndex++;
                            Controls.Add(nameLabel);
                            Controls[Controls.Count - 1].BringToFront();
                        }
                        if (currentStar.Magnitude < 3) {
                            controlToAdd.Location.Offset(-1, -1);
                            controlToAdd.Size = Size.Add(controlToAdd.Size, new Size(2, 2));
                        }
                        Controls.Add(controlToAdd);
                        int index = Controls.Count - 1;
                        Controls[index].BringToFront();
                    }
                }
                catch {
                    if (SkyObjectLibrary.GetItem(i) is Group) {
                        // Custom handling code here
                    }
                    continue;
                }
            }
            Update();
        }
        void StartChangingLocation() {
            for (int i = 0; Controls.Count > 8; i++) {
                Controls.RemoveAt(0);
            }
            AddedStars = 0;
        }
        void Button1Click(object sender, EventArgs e) {
            string searchText = textBox1.Text;
            if (SkyObjectLibrary.Search(searchText).Name == "Invalid search") return;
            Star result = (Star)(SkyObjectLibrary.Search(searchText));
            int x = (int)(result.RA * 60);
            int y = (int)(result.Dec);
            if (y < -60) {
                y = -60;
            }
            SkyPositionX = (int)(x);
            SkyPositionY = (int)(y);
            StartChangingLocation();

            // Remove remainders for correct positioning
            double xrem = Math.IEEERemainder(SkyPositionX, 60);
            double yrem = Math.IEEERemainder(SkyPositionY, 10);
            if (xrem != 0) {
                SkyPositionX -= (int)(xrem);
                SkyPositionX += 60;
            }
            if (yrem != 0) {
                SkyPositionY -= (int)(yrem);
                SkyPositionY += 10;
            }
            label1.Text = (SkyPositionX / 60) + "h";
            label2.Text = ((SkyPositionX - 1) / 60) + "h";
            label3.Text = SkyPositionY + degreeChar;
            label4.Text = (SkyPositionY - 10) + degreeChar;
            label5.Text = (SkyPositionY - 20) + degreeChar;
            InitialiseDataGridView();
            MakeStars();
        }
        void StarClicked(object sender, EventArgs e) {
            Control source = (Control)(sender);
            Form starInfo = new Form();
            starInfo.Text = "Details of " + source.Name;
            starInfo.Icon = Icon;
            starInfo.Size = new Size(730, 170);
            starInfo.Location = Location;
            starInfo.Location.Offset(0, Height);
            Star target = (Star)(SkyObjectLibrary.Search(source.Name));
            Label label = new Label();
            label.AutoSize = true;
            string properties = string.Empty;
            switch (target.Properties) {
                case StarProperties.Double: properties = nl + "This star is a binary system."; break;
                case StarProperties.VariableMagnitude: properties = nl + "This star has variable magnitude."; break;
                case StarProperties.Double | StarProperties.VariableMagnitude: properties = nl + "This is a binary system where each of the members periodically eclipses the other."; break;
            }
            label.Text = target + "Constellation: " + target.GetConstellation() + nl + "Best month to see (considering observation at midnight): " + target.GetBestMonth() + properties;
            label.Font = new Font("Segoe UI", 18);
            starInfo.Controls.Add(label);
            starInfo.Show();
        }
        public void ChangeLocation(object sender, KeyEventArgs e) {
            Keys key = e.KeyCode;
            switch (key) {
                case Keys.Left: SkyPositionX += 60; break;
                case Keys.Right: SkyPositionX -= 60; break;
                case Keys.Up: SkyPositionY += 10; break;
                case Keys.Down: SkyPositionY -= 10; break;
                case Keys.Enter: button1.PerformClick(); break;
                default: return;
            }
            if (SkyPositionX < 0) SkyPositionX += 1440;
            if (SkyPositionX >= 1440) SkyPositionX -= 1440;
            StartChangingLocation();
            int middle_ra = (SkyPositionX - 1) / 60;
            if (middle_ra < 0) middle_ra += 24;
            label1.Text = (SkyPositionX / 60) + "h";
            label2.Text = middle_ra + "h";
            label3.Text = SkyPositionY + degreeChar;
            label4.Text = (SkyPositionY - 10) + degreeChar;
            label5.Text = (SkyPositionY - 20) + degreeChar;
            InitialiseDataGridView();
            MakeStars();
        }
        public void OpenSettingsForm() {
            if (!OpenedSettingsForm) {
                Settings form = new Settings();
                form.Show();
                OpenedSettingsForm = true;
            }
        }
        public void MakeStars() {
            for (int i = 0; i < 3; i++) {
                for (int j = 0; j < 2; j++) {
                    DataGridView1CellContentClick(dataGridView1, new DataGridViewCellEventArgs(j, i));
                }
            }
        }
        void LoadForm(object sender, EventArgs e) {
            MakeStars();
        }
        /*
         * Do not uncomment this code.
        PaintEventArgs lastPaintEvent;
        protected override void OnPaint(PaintEventArgs e)
        {
            lastPaintEventArgs = e;
            Pen defaultPen = new Pen(Color.White);
            for (int i = 0; i < Star.totalstars - 1; i++) {
                if (!(SkyObjectLibrary.GetItem(i) is Star)) {
                    continue;
                }
                for (int j = i + 1; j < Star.totalstars; j++) {
                    if (!(SkyObjectLibrary.GetItem(j) is Star)) {
                        continue;
                    }
                    if (IsOnLinkages(i, j)) {
                        Star sourceStar = (Star)(SkyObjectLibrary.GetItem(i));
                        Star destinationStar = (Star)(SkyObjectLibrary.GetItem(j));
                        Point src = sourceStar.GetLocation(SkyPositionX, SkyPositionY);
                        Point dst = sourceStar.GetLocation(SkyPositionX, SkyPositionY);
                        e.Graphics.DrawLine(defaultPen, src, dst);
                    }
                }
            }
        }
        */
        bool IsOnLinkages(int src, int dst) {
            bool return_value = false;
            for (int i = 0; i < (linkages.Length / 2); i++) {
                if ((linkages[i, 0] == src) && (linkages[i, 1] == dst)) {
                    return_value = true;
                }
                if ((linkages[i, 1] == dst) && (linkages[i, 1] == src)) {
                    return_value = true;
                }
            }
            return return_value;
        }
        
        public void Add(IComponent component)
        {
            if (!(component is Control)) {
                return;
            }
            Controls.Add((Control)(component));
        }
        
        public void Add(IComponent component, string name)
        {
            if (!(component is Control)) {
                return;
            }
            Controls.Add((Control)(component));
        }
        
        public void Remove(IComponent component)
        {
            if (!(component is Control)) {
                return;
            }
            Controls.Remove((Control)(component));
        }
        
        public ComponentCollection Components {
            get {
                Component[] result = new Component[Controls.Count];
                for (int i = 0; i < Controls.Count; i++) {
                    result[i] = Controls[i];
                }
                return new ComponentCollection(result);
            }
        }
        protected override void OnLoad(EventArgs e)
        {
            button1.PerformClick();
            button1.PerformClick();
        }
    }
}

