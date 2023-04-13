using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WIA;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;




namespace Fingerprint_Recognition_Project
{
    public partial class Form4 : Form
    {
        public Form4()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ListScanners();

            // Set start output folder TMP
            textBox1.Text = Path.GetTempPath();
            // Set JPEG as default
            comboBox1.SelectedIndex = 1;

        }

        private void ListScanners()
        {
            // Clear the ListBox.
            listBox1.Items.Clear();

            // Create a DeviceManager instance
            var deviceManager = new DeviceManager();

            // Loop through the list of devices and add the name to the listbox
            for (int i = 1; i <= deviceManager.DeviceInfos.Count; i++)
            {
                // Add the device only if it's a scanner
                if (deviceManager.DeviceInfos[i].Type != WiaDeviceType.ScannerDeviceType)
                {
                    continue;
                }

                // Add the Scanner device to the listbox (the entire DeviceInfos object)
                // Important: we store an object of type scanner (which ToString method returns the name of the scanner)
                listBox1.Items.Add(
                    new Scanner(deviceManager.DeviceInfos[i])
                );
            }
        }


        private void TriggerScan()
        {
            Console.WriteLine("Image succesfully scanned");
        }

        public void StartScanning()
        {
            Scanner device = null;

            this.Invoke(new MethodInvoker(delegate ()
            {
                device = listBox1.SelectedItem as Scanner;
            }));

            if (device == null)
            {
                MessageBox.Show("You need to select first an scanner device from the list",
                                "Warning",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else if (String.IsNullOrEmpty(textBox2.Text))
            {
                MessageBox.Show("Provide a filename",
                                "Warning",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            ImageFile image = new ImageFile();
            string imageExtension = "";

            this.Invoke(new MethodInvoker(delegate ()
            {
                switch (comboBox1.SelectedIndex)
                {
                    case 0:
                        image = device.ScanImage(WIA.FormatID.wiaFormatPNG);
                        imageExtension = ".png";
                        break;
                    case 1:
                        image = device.ScanImage(WIA.FormatID.wiaFormatJPEG);
                        imageExtension = ".jpeg";
                        break;
                    case 2:
                        image = device.ScanImage(WIA.FormatID.wiaFormatBMP);
                        imageExtension = ".bmp";
                        break;
                    case 3:
                        image = device.ScanImage(WIA.FormatID.wiaFormatGIF);
                        imageExtension = ".gif";
                        break;
                    case 4:
                        image = device.ScanImage(WIA.FormatID.wiaFormatTIFF);
                        imageExtension = ".tiff";
                        break;
                }
            }));


            // Save the image
            var path = Path.Combine(textBox1.Text, textBox2.Text + imageExtension);

            if (File.Exists(path))
            {
                File.Delete(path);
            }
            
            image.SaveFile(path);

            pictureBox1.Image = new Bitmap(path);

        }

        private void button2_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderDlg = new FolderBrowserDialog();
            folderDlg.ShowNewFolderButton = true;
            DialogResult result = folderDlg.ShowDialog();

            if (result == DialogResult.OK)
            {
                textBox1.Text = folderDlg.SelectedPath;
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            Task.Factory.StartNew(StartScanning).ContinueWith(result => TriggerScan());
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // Задайте значение контраста
            if (pictureBox1.Image != null)
            {
                Bitmap bmp = new Bitmap(pictureBox1.Image);
                for (int x = 0; x < bmp.Width; x++)
                {
                    for (int y = 0; y < bmp.Height; y++)
                    {
                        Color pixelColor = bmp.GetPixel(x, y);
                        int grayScale = (int)((pixelColor.R * 0.3) + (pixelColor.G * 0.59) + (pixelColor.B * 0.11));
                        Color newColor = Color.FromArgb(grayScale, grayScale, grayScale);
                        bmp.SetPixel(x, y, newColor);
                    }
                }
                pictureBox1.Image = bmp;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            // Задайте значение контраста
            float contrast = 1.5f; // 1 - без изменений, меньше 1 - уменьшение контраста, больше 1 - увеличение контраста

            Bitmap bmp = new Bitmap(pictureBox1.Image);
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            System.Drawing.Imaging.BitmapData bmpData = bmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, bmp.PixelFormat);

            int bytesPerPixel = System.Drawing.Bitmap.GetPixelFormatSize(bmp.PixelFormat) / 8;
            int heightInPixels = bmpData.Height;
            int widthInBytes = bmpData.Width * bytesPerPixel;
            byte[] pixels = new byte[bmpData.Stride * heightInPixels];

            System.Runtime.InteropServices.Marshal.Copy(bmpData.Scan0, pixels, 0, pixels.Length);

            for (int y = 0; y < heightInPixels; y++)
            {
                int currentLine = y * bmpData.Stride;
                for (int x = 0; x < widthInBytes; x = x + bytesPerPixel)
                {
                    double blue = pixels[currentLine + x];
                    double green = pixels[currentLine + x + 1];
                    double red = pixels[currentLine + x + 2];

                    blue = ((((blue / 255.0) - 0.5) * contrast) + 0.5) * 255.0;
                    green = ((((green / 255.0) - 0.5) * contrast) + 0.5) * 255.0;
                    red = ((((red / 255.0) - 0.5) * contrast) + 0.5) * 255.0;

                    blue = (blue > 255) ? 255 : (blue < 0) ? 0 : blue;
                    green = (green > 255) ? 255 : (green < 0) ? 0 : green;
                    red = (red > 255) ? 255 : (red < 0) ? 0 : red;

                    pixels[currentLine + x] = (byte)blue;
                    pixels[currentLine + x + 1] = (byte)green;
                    pixels[currentLine + x + 2] = (byte)red;
                }
            }

            System.Runtime.InteropServices.Marshal.Copy(pixels, 0, bmpData.Scan0, pixels.Length);
            bmp.UnlockBits(bmpData);

            pictureBox1.Image = bmp;

        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "JPEG Image|*.jpg|Bitmap Image|*.bmp|PNG Image|*.png";
                saveFileDialog.Title = "Save an Image File";
                saveFileDialog.ShowDialog();

                if (saveFileDialog.FileName != "")
                {
                    System.IO.FileStream fs = (System.IO.FileStream)saveFileDialog.OpenFile();
                    switch (saveFileDialog.FilterIndex)
                    {
                        case 1:
                            pictureBox1.Image.Save(fs, System.Drawing.Imaging.ImageFormat.Jpeg);
                            break;

                        case 2:
                            pictureBox1.Image.Save(fs, System.Drawing.Imaging.ImageFormat.Bmp);
                            break;

                        case 3:
                            pictureBox1.Image.Save(fs, System.Drawing.Imaging.ImageFormat.Png);
                            break;
                    }
                    fs.Close();
                }
            }
        }   
    }
}

