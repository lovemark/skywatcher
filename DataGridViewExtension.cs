
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SkyWatcher
{
    /// <summary>
    /// An extension for the <see cref="System.Windows.Forms.DataGridView"/> class.
    /// </summary>
    public class DataGridViewExtension : DataGridView
    {
        readonly MainForm _owner;
        public DataGridViewExtension(MainForm owner)
        {
            _owner = owner;
            Timer timer = new Timer((IContainer)(_owner));
            timer.Interval = 17;
            timer.Tick += TimerMethod;
        }
        
        void TimerMethod(object sender, EventArgs e) {
            OnPaint(new PaintEventArgs(CreateGraphics(), new Rectangle(Location, Size)));
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if ((double)(_owner.SkyPositionY) < 30 && (double)(_owner.SkyPositionY) > 0) {
                e.Graphics.DrawLine(new Pen(Color.White, 5), 0, _owner.SkyPositionX * 20, Width, _owner.SkyPositionX * 20);
            }
        }
    }
}
