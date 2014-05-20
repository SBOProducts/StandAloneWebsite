using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Net;
using System.Drawing.Imaging;
using System.IO;
using System.Web;

namespace Customer.Helpers
{
    /// <summary>
    /// Provides crop, rotate, scale, brightness, caption, and save ability for an image
    /// </summary>
    /// The constructors can be from an image, file path, url, or file stream
    public class ImageManager
    {
        Image _image;
        Image _originalImage;
        public Image Image { get { return _image; } }
        public Int32 FontSize = 14;
        public Int32 Opacity = 30;
        public const string WATERMARK_SYMBOL = "Nathan Townsend";


        #region Image Sorting


        public static void SetImageSortFile(string ImageFolder, string[] paths)
        {
            string path = HttpContext.Current.Server.MapPath(string.Format("~/Files/{0}/sort.txt", ImageFolder));

            List<string> data = new List<string>();
            foreach (string url in paths)
                data.Add(Path.GetFileName(url));

            System.IO.File.WriteAllText(path, string.Join("|", data.ToArray()));
        }

        public static string[] GetImageUrls(string ImageFolder, string filter)
        {
            // get folder and make sure it's there
            string dir = HttpContext.Current.Server.MapPath("~/Files/" + ImageFolder);
            
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            // get the sort file
            string sortFile = EnsureSortFileExists(ImageFolder);
            
            string files = File.ReadAllText(sortFile);
            
            // replace the default 100 size with the requested size
            files = files.Replace("-100.", filter.Replace("*", ""));
            
            List<string> names = new List<string>();

            foreach (string name in files.Split('|'))
            {
                names.Add(string.Format("/Files/{0}/{1}", ImageFolder, name));
            }

            return names.ToArray();
        }


        /// <summary>
        /// Ensures a sort file exists creating one from the -100. filter if it doesn't exist
        /// </summary>
        /// <param name="ImageFolder"></param>
        /// <returns></returns>
        static string EnsureSortFileExists(string ImageFolder)
        {
            string dir = HttpContext.Current.Server.MapPath("~/Files/" + ImageFolder);
            string sortFile = Path.Combine(dir, "sort.txt");

            if (!File.Exists(sortFile))
            {
                List<string> files = new List<string>();
                foreach (string phyPath in Directory.GetFiles(dir, "*-100.*"))
                {
                    files.Add(Path.GetFileName(phyPath));
                }
                File.WriteAllText(sortFile, string.Join("|", files.ToArray()));
            }

            return sortFile;
        }

        

        #endregion


        #region Constructors

        public ImageManager(Image image)
        {
            _image = image;
            _originalImage = _image;
        }

        public ImageManager(string path)
        {
            _image = new Bitmap(path);
            _originalImage = _image;
        }

        public ImageManager(Stream imageStream)
        {
            _image = new Bitmap(imageStream);
            _originalImage = _image;
        }

        public ImageManager(Uri uri)
        {
            WebClient wc = new WebClient();
            _image = new Bitmap(wc.OpenRead(uri));
            _originalImage = _image;
        }

        #endregion


        #region Cropping

        /// <summary>
        /// Crops an image
        /// </summary>
        /// <param name="left"></param>
        /// <param name="top"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public void Crop(int left, int top, int width, int height)
        {
            Rectangle rec = new Rectangle(left, top, width, height);
            Crop(rec);
        }

        /// <summary>
        /// Crops an image
        /// </summary>
        /// <param name="rectangle"></param>
        /// <returns></returns>
        public void Crop(Rectangle rectangle)
        {
            Bitmap bmp = new Bitmap(rectangle.Width, rectangle.Height);

            Graphics g = Graphics.FromImage(bmp);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

            Rectangle dest = new Rectangle(0, 0, bmp.Width, bmp.Height);
            g.DrawImage(_image, dest, rectangle, GraphicsUnit.Pixel);

            _image = (Image)bmp.Clone();

            bmp.Dispose();
            g.Dispose();

        }

        /// <summary>
        /// Crops an image
        /// </summary>
        /// <param name="option"></param>
        /// <returns></returns>
        public void Crop(CroppingOptions option)
        {
            Rectangle rec = new Rectangle(0, 0, _image.Width, _image.Height);
            Int32 wf = Convert.ToInt32(_image.Width / 3);
            int hf = Convert.ToInt32(_image.Height / 3);

            switch (option)
            {
                case CroppingOptions.ColLeft:
                    rec = new Rectangle(0, 0, wf, _image.Height);
                    break;
                case CroppingOptions.ColMiddle:
                    rec = new Rectangle(wf, 0, wf, _image.Height);
                    break;
                case CroppingOptions.ColRight:
                    rec = new Rectangle(_image.Width - wf, 0, wf, _image.Height);
                    break;
                case CroppingOptions.RowBottom:
                    rec = new Rectangle(0, _image.Height - hf, _image.Width, hf);
                    break;
                case CroppingOptions.RowMiddle:
                    rec = new Rectangle(0, hf, _image.Width, hf);
                    break;
                case CroppingOptions.RowTop:
                    rec = new Rectangle(0, 0, _image.Width, hf);
                    break;
            }

            Crop(rec);
        }


        public enum CroppingOptions
        {
            RowTop,
            RowMiddle,
            RowBottom,
            ColLeft,
            ColMiddle,
            ColRight
        }

        #endregion


        #region Brightness & Contrast

        /// <summary>
        /// Adjusts the brightness from pure black to pure white and all in between
        /// </summary>
        /// <param name="level">Zero is pure black, 100 is pure white</param>
        public void AlterBrightness(int level)
        {
            // avoid issues with going outside percentage bounds
            if (level < 0) level = 0;
            if (level > 100) level = 100;


            Graphics graphics = Graphics.FromImage(_image);

            if (level == 50)
            {
                // do nothing
                return;
            }
            else if (level < 50)
            {
                // make it darker
                // Work out how much darker
                int conversion = 250 - (5 * level);
                Pen pDark = new Pen(Color.FromArgb(conversion, 0, 0, 0), _image.Width * 2);
                graphics.DrawLine(pDark, 0, 0, _image.Width, _image.Height);
            }
            else if (level > 50)
            {
                // mark it lighter
                // Work out how much lighter.
                int conversion = (5 * (level - 50));
                Pen pLight = new Pen(Color.FromArgb(conversion, 255, 255, 255), _image.Width * 2);
                graphics.DrawLine(pLight, 0, 0, _image.Width, _image.Height);
            }

            graphics.Save();
            graphics.Dispose();

        }


        public void ConvertToGrayScale()
        {
            //create a blank bitmap the same size as original
            //Bitmap newBitmap = new Bitmap(original.Width, original.Height);
            Bitmap newBitmap = new Bitmap(_image);
            Bitmap original = new Bitmap(_image);

            //get a graphics object from the new image
            Graphics g = Graphics.FromImage(newBitmap);

            //create the grayscale ColorMatrix
            ColorMatrix colorMatrix = new ColorMatrix(new float[][]{
                 new float[] {.3f, .3f, .3f, 0, 0},
                 new float[] {.59f, .59f, .59f, 0, 0},
                 new float[] {.11f, .11f, .11f, 0, 0},
                 new float[] {0, 0, 0, 1, 0},
                 new float[] {0, 0, 0, 0, 1}
            });

            //create some image attributes
            ImageAttributes attributes = new ImageAttributes();

            //set the color matrix attribute
            attributes.SetColorMatrix(colorMatrix);

            //draw the original image on the new image
            //using the grayscale color matrix
            g.DrawImage(original, new Rectangle(0, 0, original.Width, original.Height),
               0, 0, original.Width, original.Height, GraphicsUnit.Pixel, attributes);

            //dispose the Graphics object
            g.Dispose();

            //return the image
            _image = (Image)newBitmap.Clone();
            newBitmap.Dispose();
        }


        #endregion


        #region Rotation

        /// <summary>
        /// Rotates the image in the direction and amount specified
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="amount"></param>
        public void Rotate(RotationDirections direction, RotationAmount amount)
        {
            int angle = (int)direction * (int)amount;

            //create an empty Bitmap image
            Bitmap bmp;

            if (Math.Abs(angle) == 90 || Math.Abs(angle) == 270)
                bmp = new Bitmap(_image.Height, _image.Width);
            else
                bmp = new Bitmap(_image.Width, _image.Height);

            //turn the Bitmap into a Graphics object
            Graphics gfx = Graphics.FromImage(bmp);

            //now we set the rotation point to the center of our image
            gfx.TranslateTransform((float)bmp.Width / 2, (float)bmp.Height / 2);

            //now rotate the image
            gfx.RotateTransform((float)angle);
            if (Math.Abs(angle) == 90 || Math.Abs(angle) == 270)
                gfx.TranslateTransform(-(float)bmp.Height / 2, -(float)bmp.Width / 2);
            else
                gfx.TranslateTransform(-(float)bmp.Width / 2, -(float)bmp.Height / 2);

            //set the InterpolationMode to HighQualityBicubic so to ensure a high
            //quality image once it is transformed to the specified size
            gfx.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

            //now draw our new image onto the graphics object
            gfx.DrawImage(_image, new Point(0, 0));

            //dispose of our Graphics object
            gfx.Dispose();

            //return the image
            _image = (Image)bmp.Clone();

            bmp.Dispose();
        }

        /// <summary>
        /// Rotates the image 90 degrees in the direction specified
        /// </summary>
        /// <param name="direction"></param>
        public void Rotate(RotationDirections direction)
        {
            Rotate(direction, RotationAmount._90);
        }

        public enum RotationDirections
        {
            Clockwise = 1,
            CounterClockwise = -1
        }

        public enum RotationAmount
        {
            _90 = 90,
            _180 = 180,
            _270 = 270
        }



        #endregion


        #region Caption

        public void AddInfo(DateTime taken, string feedName, Color backgrounColor)
        {

            Bitmap bmp = new Bitmap(_image.Width, _image.Height + 60);
            Graphics gfx = Graphics.FromImage(bmp);

            // make the background black
            Rectangle fullsize = new Rectangle(0, 0, bmp.Width, bmp.Height);
            gfx.FillRectangle(new Pen(backgrounColor).Brush, fullsize);

            // draw lapse cam
            Font lc = new Font(FontFamily.GenericSansSerif, 12);
            gfx.DrawString("LapseCam.com", lc, Brushes.Yellow, 5, _image.Height + 5);

            // draw feed name and data / time
            Font fn = new Font("Arial", 9);
            string name = string.Format("{0} : {1}", feedName, taken.ToString()); // taken.ToString("yyyy:MM:DD HH:MM"));
            gfx.DrawString(name, fn, Brushes.White, 5, _image.Height + lc.Height + 10);

            //set the InterpolationMode to HighQualityBicubic so to ensure a high
            //quality image once it is transformed to the specified size
            gfx.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

            //now draw our new image onto the graphics object
            gfx.DrawImage(_image, new Point(0, 0));

            //dispose of our Graphics object
            gfx.Dispose();

            //return the image
            _image = (Image)bmp.Clone();

            bmp.Dispose();

        }

        public void AddWatermark()
        {

            Bitmap bmp = new Bitmap(_image.Width, _image.Height);
            Graphics gfx = Graphics.FromImage(bmp);

            // make the background black
            Rectangle fullsize = new Rectangle(0, 0, bmp.Width, bmp.Height);
            gfx.FillRectangle(new Pen(Color.Black).Brush, fullsize);

            //set the InterpolationMode to HighQualityBicubic so to ensure a high
            //quality image once it is transformed to the specified size
            gfx.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

            //now draw our new image onto the graphics object
            gfx.DrawImage(_image, new Point(0, 0));

            // draw watermark
            SolidBrush colorBrush = new SolidBrush(Color.FromArgb(Opacity, 255, 255, 255));
            Font lc = new Font(FontFamily.GenericSansSerif, FontSize);
            StringFormat stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.LineAlignment = StringAlignment.Near;
            gfx.DrawString(WATERMARK_SYMBOL, lc, colorBrush, fullsize, stringFormat);


            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.LineAlignment = StringAlignment.Far;
            gfx.DrawString(WATERMARK_SYMBOL, lc, colorBrush, fullsize, stringFormat);

            //dispose of our Graphics object
            gfx.Dispose();

            //return the image
            _image = (Image)bmp.Clone();

            bmp.Dispose();
        }

        #endregion


        #region Scaling

        public void ScaleImage(int maxSize)
        {
            ScaleImage(maxSize, false);
        }

        public void ScaleImage(int maxSize, bool makeSquareImage)
        {
            ScaleImage(maxSize, makeSquareImage, Color.White);
        }

        public void ScaleImage(int maxSize, bool makeSquareImage, Color backgroundColor)
        {
            float w = _image.Width;
            float h = _image.Height;

            // don't enlarge images
            if (WillEnlarge(w, h, maxSize))
                return;

            // calculate the the new sizes
            float nW = (w > h ? maxSize : (w / h * maxSize));
            float nH = (h > w ? maxSize : (h / w * maxSize));

            Bitmap bmp;
            Graphics gfx;

            if (makeSquareImage)
            {
                bmp = new Bitmap(maxSize, maxSize);
                gfx = Graphics.FromImage(bmp);
                gfx.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                float x = nW == maxSize ? 0 : ((maxSize - nW) / 2);
                float y = nH == maxSize ? 0 : ((maxSize - nH) / 2);
                Rectangle fullsize = new Rectangle(0, 0, maxSize, maxSize);
                Pen p = new Pen(backgroundColor);
                gfx.FillRectangle(p.Brush, fullsize);

                gfx.DrawImage(_image, x, y, nW, nH);
            }
            else
            {
                bmp = new Bitmap((int)nW, (int)nH);
                gfx = Graphics.FromImage(bmp);
                gfx.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                gfx.DrawImage(_image, 0, 0, nW, nH);
            }

            _image = (Image)bmp.Clone();

            bmp.Dispose();
            gfx.Dispose();
        }

        /// <summary>
        /// determines if an enlargement will occur
        /// </summary>
        /// <param name="w"></param>
        /// <param name="h"></param>
        /// <param name="maxSize"></param>
        /// <returns></returns>
        bool WillEnlarge(float w, float h, int maxSize)
        {
            // if the larger size is less than the desired size then don't enrlarge the image
            if (w >= h && w <= maxSize)
                return true;
            if (h >= w && h <= maxSize)
                return true;

            return false;
        }

        #endregion


        /// <summary>
        /// Saves the image in jpeg format
        /// </summary>
        /// <param name="path"></param>
        public void Save(string path)
        {
            ImageFormat format;
            
            string ext = Path.GetExtension(path);
            switch (ext.ToLower())
            {
                // "image/jpeg", "image/gif", "image/png", "image/bmp", "image/x-windows-bmp"
                case ".bmp":
                    format = ImageFormat.Bmp;
                    break;
                case ".gif":
                    format = ImageFormat.Gif;
                    break;
                case ".jpg": 
                case ".jpeg":
                    format = ImageFormat.Jpeg;
                    break;
                case ".png":
                    format = ImageFormat.Png;
                    break;
                default:
                    format = ImageFormat.Jpeg;
                    break;
            }

            _image.Save(path, format);
        }



        /// <summary>
        /// Restores the image to it's original form
        /// </summary>
        public void RestoreOriginalImage()
        {
            _image = _originalImage;
        }

    }
}
