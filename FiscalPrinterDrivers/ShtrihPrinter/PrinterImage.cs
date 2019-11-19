/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace Microsoft.Dynamics.Retail.FiscalPrinter.ShtrihPrinter
{

    /// <summary>
    /// class for working with image file
    /// </summary>
    internal static class PrinterImageFileHelper
    {
        private const string imageExt = ".bmp";

        /// <summary>
        /// Constructs full path to image file
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static string GetFullFileName(string filename)
        {
            string extension = Path.GetExtension(filename) ?? string.Empty;

            if (!extension.Equals(imageExt, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception(Resources.Translate(Resources.OnlyBMPFileCanBeLoaded));
            }

            if (!Path.IsPathRooted(filename))
            {
                string folder = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                filename = Path.Combine(folder, filename);
            }

            return filename;
        }
        
        /// <summary>
        /// Loads image from file
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static Bitmap LoadImageFromFile(string filename)
        {
            if (!File.Exists(filename))
            {
                throw new Exception(Resources.Translate(Resources.ImageFileDoesNotExist));
            }

            return (Bitmap)System.Drawing.Image.FromFile(filename, true);
        }

    }
    
    /// <summary>
    /// Class for printer image validating
    /// </summary>
    internal static class PrinterImageValidator
    {
        private const int PixelsPerSymbol = 8;
        private const int MaxSymbolsPerLine = 40;
        private const int MaxWidth = PixelsPerSymbol * MaxSymbolsPerLine;
        private const int MaxHeight = 1200;

        /// <summary>
        /// Validates the image and throws <see cref="FiscalPrinterException"/> if it is not valid.
        /// </summary>
        /// <param name="image">A <see cref="Bitmap"/> image object.</param>
        /// <remarks>
        /// Throws <see cref="FiscalPrinterException"/> if the provided image is not valid.
        /// </remarks>
        public static void ValidateImage(Bitmap image)
        {
            if (image.Height > MaxHeight || image.Width > MaxWidth)
            {
                throw new Exception(Resources.Translate(Resources.ImageIsTooBig, (object)MaxWidth, MaxHeight));
            }
        }
    }

    /// <summary>
    /// Class represents image for the fiscal printer.
    /// </summary>
    internal sealed class PrinterImage
    {
        private Microsoft.Dynamics.Retail.FiscalPrinter.ShtrihPrinter.Image imageConfig;
        private string filename = null;
        private int imageHeight = 0;

        private void LoadAndValidateImage()
        {
            if (filename == null)
            {
                var imagepath = PrinterImageFileHelper.GetFullFileName(imageConfig.Path);

                using (var bitmap = PrinterImageFileHelper.LoadImageFromFile(imagepath))
                {
                    PrinterImageValidator.ValidateImage(bitmap);
                    imageHeight = bitmap.Height;
                }

                filename = imagepath;
            }
        }

        /// <summary>
        /// Full path of the printer image.
        /// </summary>
        /// <remarks>
        /// This is a cached filename. First time it loads the image and checks if it is valid for the printer.
        /// </remarks>
        public string FileName
        {
            get
            {
                LoadAndValidateImage();
                return filename;
            }
        }

        /// <summary>
        /// Start line for the image.
        /// </summary>
        public int StartLine
        {
            get
            {
                LoadAndValidateImage();
                return Math.Min(imageConfig.StartLine, imageHeight);
            }
        }

        /// <summary>
        /// End line for the image.
        /// </summary>
        public int EndLine
        {
            get
            {
                LoadAndValidateImage();
                return Math.Min(imageConfig.EndLine, imageHeight);
            }
        }

        /// <summary>
        /// Shows if the image should be centered on the receipt.
        /// </summary>
        public bool Center
        {
            get
            {
                return imageConfig.Center;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="imageConfig"> object representing the image in the config file.</param>
        public PrinterImage(Image imageConfig)
        {
            this.imageConfig = imageConfig;
        }

    }

    /// <summary>
    /// Class for operating with the printer image.
    /// </summary>
    internal sealed class PrinterImageController
    {
        private string currentPrinterId = null;
        private string defaultImage = null;
        private IShtrihMServiceFunctions printerService;
        private Dictionary<string, PrinterImage> images = new Dictionary<string, PrinterImage>();

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="printerService">Printer service functions implementation.</param>
        /// <param name="printerConfig">Printer configuration object.</param>
        public PrinterImageController(IShtrihMServiceFunctions printerService, PrinterConfiguration printerConfig)
        {
            this.printerService = printerService;
            this.defaultImage = printerConfig.ImageList.DefaultImageId;

            foreach (Image imageConfig in printerConfig.ImageList)
            {
                images[imageConfig.Id] = new PrinterImage(imageConfig);
            }
        }

        /// <summary>
        /// Loads the image to the printer.
        /// </summary>
        /// <param name="id">Image ID</param>
        private void LoadImageToPrinter(string id)
        {
            if (id != currentPrinterId)
            {
                var image = images[id];
                printerService.LoadImageToPrinter(image.FileName, image.Center);
                currentPrinterId = id;
            }
        }

        /// <summary>
        /// Loads default image to the printer.
        /// </summary>
        public void LoadDefaultImage()
        {
            if (!string.IsNullOrWhiteSpace(defaultImage))
            {
                LoadImageToPrinter(defaultImage);
            }
        }

        /// <summary>
        /// Gets the image and loads it to the printer if necessary.
        /// </summary>
        /// <param name="id">Image ID</param>
        /// <returns>Printer image object.</returns>
        public PrinterImage GetPrinterImage(string id)
        {
            LoadImageToPrinter(id);
            return images[id];
        }
    }
}
