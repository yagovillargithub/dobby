using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Dobby
{
    public partial class OverlayForm : Form
    {
        private Point startPoint;
        private Rectangle selectionRectangle;
        private bool isDrawing;

        public Rectangle SelectedArea { get; private set; }

        public OverlayForm()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            this.BackColor = Color.Gray;
            this.Opacity = 0.5;
            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.None;
            this.TopMost = true;
            this.Cursor = Cursors.Cross;
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            isDrawing = true;
            startPoint = e.Location;
            selectionRectangle = new Rectangle(e.Location, new Size());
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (isDrawing)
            {
                int width = e.X - startPoint.X;
                int height = e.Y - startPoint.Y;
                selectionRectangle = new Rectangle(startPoint.X, startPoint.Y, width, height);
                Invalidate();
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            isDrawing = false;
            SelectedArea = selectionRectangle;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (selectionRectangle != null && selectionRectangle.Width != 0 && selectionRectangle.Height != 0)
            {
                using (Pen pen = new Pen(Color.Red, 2))
                {
                    e.Graphics.DrawRectangle(pen, selectionRectangle);
                }
            }
        }
    }

}
