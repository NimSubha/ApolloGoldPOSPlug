using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.IO;

namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch
{
    public partial class frmOrderSketch:frmTouchBase
    {
        #region Variable
        DataTable dtImage = new DataTable();
        decimal linenum = 0;
        public DataTable dtUploadSketch = new DataTable();
        DataTable dtDownload = new DataTable();
        bool isEdit = false;
        bool isClear = false;
        string suniqueNo = string.Empty;
        public string sketchName = string.Empty;

        bool bPurchSketch = false;
        #endregion

        /// <summary>
        /// Created by : Ripan Hossain
        /// Created on : 12/04/2013
        /// Modified by :
        /// Modified on : 
        /// Purpose :
        /// </summary>
        public frmOrderSketch()
        {
            InitializeComponent();
        }


        public frmOrderSketch(string sSketchName)
        {
            InitializeComponent();
            sketchName = Convert.ToString(sSketchName);
            bPurchSketch = true;

            if(sketchName != "")
            {
                Bitmap bitmap = new Bitmap(Convert.ToString(sSketchName));
                picItem.Image = bitmap;
            }
        }
        public frmOrderSketch(string sSketchName, int i)
        {
            InitializeComponent();
            sketchName = Convert.ToString(sSketchName);
            bPurchSketch = true;

            if(sketchName != "" && i == 1)
            {
                if(!string.IsNullOrEmpty(sSketchName))
                {
                    if(File.Exists(sSketchName))
                    {
                        System.Drawing.Image image = System.Drawing.Image.FromFile(Convert.ToString(sSketchName));
                        int width = image.Width;
                        int height = image.Height;
                        //decimal aspectRatio = width > height ? decimal.divide(width, height) : decimal.divide(height, width);
                        int fileSize = (int)new System.IO.FileInfo(Convert.ToString(sSketchName)).Length;

                        picItem.Image = ScaleImage(image, picItem.Width, picItem.Height);

                        this.ShowDialog();
                    }
                    else
                    {
                        MessageBox.Show("No image uploaded");
                    }
                }
            }
        }

        public static Image ScaleImage(Image image, int maxWidth, int maxHeight)
        {
            var ratioX = (double)maxWidth / image.Width;
            var ratioY = (double)maxHeight / image.Height;
            var ratio = Math.Min(ratioX, ratioY);

            var newWidth = (int)(image.Width * ratio);
            var newHeight = (int)(image.Height * ratio);

            var newImage = new Bitmap(newWidth, newHeight);

            using(var graphics = Graphics.FromImage(newImage))
                graphics.DrawImage(image, 0, 0, newWidth, newHeight);

            return newImage;
        }
        /// <summary>
        /// Created by : Ripan Hossain
        /// Created on : 12/04/2013 
        /// Modified by :
        /// Modified on : 
        /// Purpose :
        /// </summary>
        /// <param name="_dt"></param>
        /// <param name="_unique"></param>
        /// <param name="_dtDown"></param>
        /// <param name="_line"></param>
        public frmOrderSketch(DataTable _dt, string _unique, DataTable _dtDown, decimal _line)
        {
            InitializeComponent();
            if(_dtDown != null && _dtDown.Rows.Count > 0)
            {
                foreach(DataRow drImage in _dtDown.Rows)
                {
                    sketchName = Convert.ToString(drImage[2]);
                    if(sketchName != "")
                    {
                        if(File.Exists(sketchName))
                        {
                            Bitmap bitmap = new Bitmap(Convert.ToString(drImage["SKETCH"]));
                            picItem.SizeMode = PictureBoxSizeMode.StretchImage;
                            picItem.Image = bitmap;
                            break;
                        }
                    }
                }
            }
            dtImage = _dt;
            suniqueNo = _unique;
            dtDownload = _dtDown;

            linenum = _line;
        }

        /// <summary>
        /// Created by : Ripan Hossain
        /// Created on : 12/04/2013
        /// Modified by :
        /// Modified on : 
        /// Purpose :
        /// </summary>
        /// <param name="_dt"></param>
        /// <param name="_line"></param>
        public frmOrderSketch(DataTable _dt, decimal _line, string _breadCrumb)
        {
            InitializeComponent();
            btnClear.Enabled = false;
            btnSearch.Enabled = false;
            btnSearch.Enabled = false;
            btnSubmit.Enabled = false;
            DataRow[] drView = _dt.Select("LINENUM=" + _line);

            lblBreadCrumbs.Text = _breadCrumb + " > " + " Line no : " + " " + Convert.ToInt16(_line) + " > " + " Sketch";
            if(drView.Count() > 0)
            {
                if(!string.IsNullOrEmpty(Convert.ToString(drView[0]["SKETCH"])))
                {
                    Byte[] pictureData = (byte[])(drView[0]["SKETCH"]);


                    if(pictureData.Length > 0)
                    {
                        Image i = LSRetailPosis.ButtonGrid.GUIHelper.GetBitmap(pictureData);
                        if(i != null)
                        {
                            Bitmap bitmap = new Bitmap(i);

                            PictureBox imageControl = new PictureBox();
                            imageControl.Height = 433;
                            imageControl.Width = 749;

                            if(i.Height > imageControl.Height && i.Width > imageControl.Width || (i.Height > imageControl.Height || i.Width > imageControl.Width))
                                picItem.SizeMode = PictureBoxSizeMode.StretchImage;
                            else
                                picItem.SizeMode = PictureBoxSizeMode.Normal;

                            imageControl.Image = i;
                            picItem.Image = i;

                        }
                    }
                }

                this.ShowDialog();
            }
            else
            {
                using(LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("No Sketch Found.", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                }
                this.Close();
            }
        }

        /// <summary>
        /// Created by : Ripan Hossain
        /// Created on : 12/04/2013
        /// Modified by :
        /// Modified on : 
        /// Purpose : sketch/image searching for sketch/image upload
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSearch_Click(object sender, EventArgs e)
        {
            this.openFileDiolog.Filter = "Image Files (*.jpg, *.jpeg, *.jpe, *.jfif)|*.jpg;*.jpeg;*.jpe;*.jfif;";
            //"Images (*.BMP;*.JPG;*.GIF,*.PNG,*.TIFF)|*.BMP;*.JPG;*.GIF;*.PNG;*.TIFF|" +
            //"All files (*.*)|*.*";

            this.openFileDiolog.Multiselect = true;
            this.openFileDiolog.Title = "Select Photos";

            DialogResult dr = this.openFileDiolog.ShowDialog();
            if(dr == System.Windows.Forms.DialogResult.OK)
            {

                foreach(String file in openFileDiolog.FileNames)
                {
                    try
                    {
                        PictureBox imageControl = new PictureBox();
                        imageControl.Height = 433;
                        imageControl.Width = 749;
                        sketchName = openFileDiolog.FileName;

                        Bitmap myBitmap = new Bitmap(file);
                        if(myBitmap.Height > imageControl.Height && myBitmap.Width > imageControl.Width || (myBitmap.Height > imageControl.Height || myBitmap.Width > imageControl.Width))
                            picItem.SizeMode = PictureBoxSizeMode.StretchImage;
                        else
                            picItem.SizeMode = PictureBoxSizeMode.Normal;

                        imageControl.Image = myBitmap;
                        picItem.Image = myBitmap;
                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.Message);
                    }
                }
            }
        }

        /// <summary>
        /// Created by : Ripan Hossain
        /// Created on : 12/04/2013
        /// Modified by :
        /// Modified on : 
        /// Purpose : image clear from image control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClear_Click(object sender, EventArgs e)
        {
            picItem.Image = null;
            sketchName = "";
        }

        /// <summary>
        /// Created by : Ripan Hossain
        /// Created on : 12/04/2013
        /// Modified by :
        /// Modified on : 
        /// Purpose : page close
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClose_Click(object sender, EventArgs e)
        {
            if(picItem.Image != null)
                btnSubmit_Click(sender, e);
            else
                this.Close();
        }

        /// <summary>
        /// Created by : Ripan Hossain
        /// Created on : 12/04/2013
        /// Modified by :
        /// Modified on : 
        /// Purpose : sketch image submit against a perticular item
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSubmit_Click(object sender, EventArgs e)
        {
            if(bPurchSketch)
            {
                this.Close();
            }
            else
            {
                if(isEdit == false)
                {
                    dtUploadSketch = new DataTable();
                    dtUploadSketch.Columns.Add("UNIQUEID", typeof(string));
                    dtUploadSketch.Columns.Add("LINENUM", typeof(decimal));
                    dtUploadSketch.Columns.Add("SKETCH", typeof(string));

                    DataRow dr = dtUploadSketch.NewRow();
                    dr["UNIQUEID"] = Convert.ToString(suniqueNo);
                    dr["LINENUM"] = Convert.ToDecimal(linenum);

                    if(!string.IsNullOrEmpty(sketchName))
                    {
                        System.Drawing.Image image = System.Drawing.Image.FromFile(Convert.ToString(sketchName));
                        int width = image.Width;
                        int height = image.Height;
                        //decimal aspectRatio = width > height ? decimal.divide(width, height) : decimal.divide(height, width);
                        int fileSize = (int)new System.IO.FileInfo(Convert.ToString(sketchName)).Length;
                    }

                    dr["SKETCH"] = Convert.ToString(sketchName); //GetByteArray(

                    dtUploadSketch.Rows.Add(dr);
                }
                else
                {
                    if(dtDownload != null && dtDownload.Rows.Count > 0)
                    {
                        foreach(DataRow drSk in dtDownload.Rows)
                        {
                            Bitmap bitmap = new Bitmap(Convert.ToString(drSk[2]));
                            picItem.Image = bitmap;
                            break;
                        }
                    }
                }
                isClear = false;
            }

            this.Close();
        }
    }
}
