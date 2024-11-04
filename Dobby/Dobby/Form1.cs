using System;
using System.Drawing;
using System.Windows.Forms;
using Tesseract;
using Gma.System.MouseKeyHook;
using System.Runtime.InteropServices;

namespace Dobby
{
    public partial class Form1 : Form
    {
        private IKeyboardMouseEvents globalHook;

        public Form1()
        {
            InitializeComponent();
            Subscribe();
            this.WindowState = FormWindowState.Minimized;
            this.ShowInTaskbar = false;
            this.Visible = false;
        }


        private void Subscribe()
        {
            globalHook = Hook.GlobalEvents();
            globalHook.KeyDown += GlobalHook_KeyDown;
        }

        private void GlobalHook_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.Y)
            {
                e.Handled = true;
                StartCapture();
            }
        }

        private void Unsubscribe()
        {
            globalHook.KeyDown -= GlobalHook_KeyDown;
            globalHook.Dispose();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            Unsubscribe();
            notifyIcon1.Visible = false;
            notifyIcon1.Dispose();
            base.OnFormClosing(e);
        }


        private void StartCapture()
        {
            this.Hide(); // Ocultamos el formulario principal
            using (OverlayForm overlay = new OverlayForm())
            {
                if (overlay.ShowDialog() == DialogResult.OK)
                {
                    Rectangle area = overlay.SelectedArea;
                    if (area.Width > 0 && area.Height > 0)
                    {
                        Bitmap screenshot = new Bitmap(area.Width, area.Height);
                        using (Graphics g = Graphics.FromImage(screenshot))
                        {
                            g.CopyFromScreen(area.Location, Point.Empty, area.Size);
                        }
                        ProcessImage(screenshot);
                    }
                }
            }
            this.Show(); // Mostramos el formulario principal nuevamente
        }
        private void ProcessImage(Bitmap image)
        {
            try
            {
                string tessDataPath = @"./tessdata"; // Asegúrate de que este directorio exista y contenga los datos de Tesseract
                using (var engine = new TesseractEngine(tessDataPath, "spa", EngineMode.Default))
                {
                    using (var pix = ConvertBitmapToPix(image))
                    {
                        using (var page = engine.Process(pix))
                        {
                            string text = page.GetText();
                            Clipboard.SetText(text);
                            // Eliminamos el MessageBox para que sea silencioso
                            // MessageBox.Show("Texto copiado al portapapeles:\n" + text);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al procesar la imagen: " + ex.Message);
            }
        }

        private Pix ConvertBitmapToPix(Bitmap bitmap)
        {
            Pix pix;
            using (MemoryStream ms = new MemoryStream())
            {
                bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                ms.Position = 0;
                pix = Pix.LoadFromMemory(ms.ToArray());
            }
            return pix;
        }

        private void contextMenuStrip1_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Application.Exit();
        }
    }

}
