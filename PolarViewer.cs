using System;
using System.Drawing;
using System.Windows.Forms;

namespace SkyWatcher {
    public sealed class PolarViewer : Form {
        bool isSouth;
        public PolarViewer() {
            Text = "SkyWatcher Polar Viewer";
            Size = new Size(600, 600);
            
            MessageBox.Show("Click in the center of the Polar Viewer Window to switch between North and South poles.");
            
            VisibleChanged += delegate(object sender, EventArgs e) {
                if (Visible) {
                    MakeStars();
                }
            };
            MouseClick += delegate(object sender, MouseEventArgs e) {
                if (e.X >= 290 && e.Y >= 290 && e.X < 310 && e.Y < 310) {
                    isSouth = !isSouth;
                    MakeStars();
                }
            };
        }
        public void MakeStars() {
            const double TAU = 2 * Math.PI;
            for (int i = Controls.Count - 1; i >= 0; i++) {
                Controls.RemoveAt(i);
            }
            for (int j = 0; j < Star.totalstars; j++) {
                try {
                    Star result = (Star)(SkyObjectLibrary.GetItem(j));
                    if (Math.Abs(result.Dec) < 50 || (isSouth ^ (result.Dec / Math.Abs(result.Dec) == 1))) {
                        if (isSouth) {
                            double x = -300 * Math.Cos(0.261799387799149 * result.RA) + 300;
                            double y = 300 * Math.Sin(0.261799387799149 * result.RA) + 300;
                        } else {
                            double x = -300 * Math.Cos(TAU - (0.261799387799149 * result.RA)) + 300;
                            double y = 300 * Math.Sin(TAU - (0.261799387799149 * result.RA)) + 300;
                    Star result = SkyObjectLibrary.GetItem(j);
                    if (Math.Abs(j.Dec) < 50 || (isSouth ^ (j.Dec / Math.Abs(j.Dec) == 1))) {
                        if (isSouth) {
                            double x = -300 * Math.Cos(0.261799387799149 * j.RA) + 300;
                            double y = 300 * Math.Sin(0.261799387799149 * j.RA) + 300;
                        } else {
                            double x = -300 * Math.Cos(TAU - (0.261799387799149 * j.RA)) + 300;
                            double y = 300 * Math.Sin(TAU - (0.261799387799149 * j.RA)) + 300;
                        }
                    }
                } catch {
                    continue;
                }
            }
        }
        
    }
}
<<<<<<< HEAD
=======

>>>>>>> origin/gh-pages
