using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Reflection;
using System.IO;
using C1.Win.C1Tile;
using C1.C1Pdf;

namespace Image_Gallery
{
    public partial class ImageGallery : Form
    {
        // Controls
        private SplitContainer splitContainer = new SplitContainer();
        private TableLayoutPanel tableLayout = new TableLayoutPanel();
        private Panel panel = new Panel();
        private TextBox searchBox = new TextBox();
        private Panel searchButton = new Panel();
        private PictureBox search = new PictureBox();
        private PictureBox exportPdf = new PictureBox();
        private PictureBox exportFile = new PictureBox();
        private C1TileControl tileControl = new C1TileControl();
        private Group group = new Group();
        private C1PdfDocument pdf = new C1PdfDocument();
        private StatusStrip statusStrip = new StatusStrip();
        private ToolStripProgressBar progressBar = new ToolStripProgressBar();

        DataFetcher datafetch = new DataFetcher(); 
        List<ImageItem> imagesList; 
        int checkedItems = 0;

        public ImageGallery()
        {
            InitializeComponent();
        }

        private void ImageGallery_Load(object sender, EventArgs e)
        {
            // Format Form
            MaximumSize = new Size(810, 810);
            MaximizeBox = false;
            ShowIcon = false;
            Size = new Size(780, 700);
            StartPosition = FormStartPosition.CenterParent;
            Text = "Image Gallery";

            // Format SplitContainer
            splitContainer.Dock = DockStyle.Fill;
            splitContainer.Margin = new Padding(2);
            splitContainer.Orientation = Orientation.Horizontal;
            splitContainer.SplitterDistance = 40;
            splitContainer.FixedPanel = FixedPanel.Panel1;
            splitContainer.IsSplitterFixed = true;

            // Format TableLayoutPanel
            tableLayout.ColumnCount = 3;
            tableLayout.Dock = DockStyle.Fill;
            tableLayout.Location = new Point(0, 0);
            tableLayout.RowCount = 1;
            tableLayout.Size = new Size(800, 40);
            tableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 37.5F));
            tableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 37.5F));

            // Format Panel
            panel.Location = new Point(477, 0);
            panel.Size = new Size(287, 40);
            panel.Dock = DockStyle.Fill;
            panel.Paint += Panel_Paint;

            // Format SearchBox
            searchBox.Name = "_searchBox";
            searchBox.BorderStyle = BorderStyle.None;
            searchBox.Dock = DockStyle.Fill;
            searchBox.Location = new Point(16, 9);
            searchBox.Size = new Size(244, 16);
            searchBox.Text = "Search Image";
            searchBox.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Right);

            // Format SearchButton
            searchButton.Location = new Point(479, 12);
            searchButton.Margin = new Padding(2, 12, 45, 12);
            searchButton.Size = new Size(40, 16);
            searchButton.TabIndex = 1;

            // Format Search PictureBox
            search.Name = "_search";
            search.Dock = DockStyle.Left;
            search.Location = new Point(0, 0);
            search.Margin = new Padding(0, 0, 0, 0);
            search.Size = new Size(40, 16);
            search.SizeMode = PictureBoxSizeMode.Zoom;
            search.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Right);
            var outPutDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase);
            var pathSearch = Path.Combine(outPutDirectory, "Data\\search.jpg");
            string rel_pathSearch = new Uri(pathSearch).LocalPath;
            search.Image = Image.FromFile(rel_pathSearch);
            search.Click += Search_Click;

            // Format ExportPdf PictureBox
            exportPdf.Name = "_exportImage";
            exportPdf.Location = new Point(29, 3);
            exportPdf.Size = new Size(135, 29);
            exportPdf.SizeMode = PictureBoxSizeMode.StretchImage;
            exportPdf.TabIndex = 2;
            var pathPdf = Path.Combine(outPutDirectory, "Data\\pdf.jpg");
            string rel_pathPdf = new Uri(pathPdf).LocalPath;
            exportPdf.Image = Image.FromFile(rel_pathPdf);
            exportPdf.Click += ExportPdf_Click;
            exportPdf.Paint += ExportPdf_Paint;
            exportPdf.Visible = false;

            // Format ExportFile PictureBox
            exportFile.Name = "_exportFile";
            exportFile.Location = new Point(180, 3);
            exportFile.Size = new Size(135, 29);
            exportFile.SizeMode = PictureBoxSizeMode.StretchImage;
            exportFile.TabIndex = 2;
            var pathFile = Path.Combine(outPutDirectory, "Data\\download.png");
            string rel_pathFile = new Uri(pathFile).LocalPath;
            exportFile.Image = Image.FromFile(rel_pathFile);
            exportFile.Click += ExportFile_Click;
            exportFile.Paint += ExportFile_Paint;
            exportFile.Visible = false;

            // Format C1TileControl
            tileControl.Name = "_imageTileControl";
            tileControl.AllowRearranging = true;
            tileControl.AllowChecking = true;
            tileControl.CellHeight = 78;
            tileControl.CellSpacing = 11;
            tileControl.CellWidth = 78;
            tileControl.Dock = DockStyle.Fill;
            tileControl.Size = new Size(764, 718);
            tileControl.SurfacePadding = new Padding(12, 4, 12, 4);
            tileControl.SwipeDistance = 20;
            tileControl.SwipeRearrangeDistance = 98;
            tileControl.TileChecked += TileControl_TileChecked;
            tileControl.TileUnchecked += TileControl_TileUnchecked;
            tileControl.Paint += TileControl_Paint;
            tileControl.Groups.Add(group);

            // Format StatusStrip
            statusStrip.Dock = DockStyle.Bottom;
            statusStrip.Visible = false;

            // Format ProgressBar
            progressBar.Style = ProgressBarStyle.Marquee;
            statusStrip.Items.Add(progressBar);

            // Add controls to the form
            searchButton.Controls.Add(search);
            panel.Controls.Add(searchBox);
            tableLayout.Controls.Add(panel, 1, 0);
            tableLayout.Controls.Add(searchButton, 2, 0);
            splitContainer.Panel1.Controls.Add(tableLayout);
            splitContainer.Panel2.Controls.Add(exportPdf);
            splitContainer.Panel2.Controls.Add(exportFile);
            splitContainer.Panel2.Controls.Add(tileControl);
            Controls.Add(splitContainer);
            Controls.Add(statusStrip);
        }

        private void ExportFile_Paint(object sender, PaintEventArgs e)
        {
            Rectangle r = new Rectangle(exportPdf.Location.X, exportPdf.Location.Y, exportPdf.Width, exportPdf.Height);
            r.X -= 29;
            r.Y -= 3;
            r.Width--;
            r.Height--;
            Pen p = new Pen(Color.LightGray);
            e.Graphics.DrawRectangle(p, r);
            e.Graphics.DrawLine(p, new Point(0, 43), new Point(this.Width, 43));
        }


        private void ExportFile_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog saveFile = new FolderBrowserDialog();
            int count = 1;
            
            if (saveFile.ShowDialog() == DialogResult.OK)
            {
                foreach (Tile tile in tileControl.Groups[0].Tiles)
                {
                    if (tile.Checked)
                    {
                        string fileName = saveFile.SelectedPath + "/" + searchBox.Text + count + "-" +  DateTime.Now.ToString("yyyyMMddTHHmmss");
                        Image image = tile.Image;
                        if (image.RawFormat.Equals(ImageFormat.Jpeg))
                            tile.Image.Save(fileName + ".jpg", ImageFormat.Jpeg);
                        else if (image.RawFormat.Equals(ImageFormat.Bmp))
                            tile.Image.Save(fileName + ".bmp", ImageFormat.Bmp);
                        else if (image.RawFormat.Equals(ImageFormat.Gif))
                            tile.Image.Save(fileName + ".gif", ImageFormat.Gif);
                        else if (image.RawFormat.Equals(ImageFormat.Icon))
                            tile.Image.Save(fileName + ".ico", ImageFormat.Icon);
                        else if (image.RawFormat.Equals(ImageFormat.Png))
                            tile.Image.Save(fileName + ".png", ImageFormat.Png);

                        count++;
                    }
                }
            }

        }

        private void AddTiles(List<ImageItem> imageList)
        {
            tileControl.Groups[0].Tiles.Clear();

            foreach (var imageitem in imageList)
            {
                Tile tile = new Tile(); 
                tile.HorizontalSize = 2; 
                tile.VerticalSize = 2;
                tileControl.Groups[0].Tiles.Add(tile);

                Image img = Image.FromStream(new MemoryStream(imageitem.Base64));

                Template tl = new Template(); 
                ImageElement ie = new ImageElement(); 
                ie.ImageLayout = ForeImageLayout.Stretch; 
                tl.Elements.Add(ie); 
                tile.Template = tl; 
                tile.Image = img;
            }
        }

        private void TileControl_Paint(object sender, PaintEventArgs e)
        {
            Pen p = new Pen(Color.LightGray); 
            e.Graphics.DrawLine(p, 0, 43, 800, 43);
        }

        private void TileControl_TileUnchecked(object sender, TileEventArgs e)
        {
            checkedItems--; 
            exportPdf.Visible = checkedItems > 0;
            exportFile.Visible = checkedItems > 0;
        }

        private void TileControl_TileChecked(object sender, TileEventArgs e)
        {
            checkedItems++; 
            exportPdf.Visible = true;
            exportFile.Visible = true;
        }

        private void ExportPdf_Paint(object sender, PaintEventArgs e)
        {
            Rectangle r = new Rectangle(exportPdf.Location.X, exportPdf.Location.Y, exportPdf.Width, exportPdf.Height);             
            r.X -= 29; 
            r.Y -= 3; 
            r.Width--; 
            r.Height--; 
            Pen p = new Pen(Color.LightGray); 
            e.Graphics.DrawRectangle(p, r); 
            e.Graphics.DrawLine(p, new Point(0, 43), new Point(this.Width, 43));
        }

        private void ExportPdf_Click(object sender, EventArgs e)
        {
            List<Image> images = new List<Image>(); 
            foreach (Tile tile in tileControl.Groups[0].Tiles)
            {
                if (tile.Checked)
                {
                    images.Add(tile.Image);
                }
            }
            ConvertToPdf(images); 
            SaveFileDialog saveFile = new SaveFileDialog(); 
            saveFile.DefaultExt = "pdf"; 
            saveFile.Filter = "PDF files (*.pdf)|*.pdf*"; 
            if (saveFile.ShowDialog() == DialogResult.OK) 
            { 
                pdf.Save(saveFile.FileName); 
            }
        }
        private void ConvertToPdf(List<Image> images)
        {
            RectangleF rect = pdf.PageRectangle; 
            bool firstPage = true; 
            foreach (var selectedimg in images)
            {
                if (!firstPage) 
                { 
                    pdf.NewPage(); 
                }
                firstPage = false;

                rect.Inflate(-72, -72); 
                pdf.DrawImage(selectedimg, rect);

            }
        }

        private async void Search_Click(object sender, EventArgs e)
        {
            statusStrip.Visible = true; 
            imagesList = await datafetch.GetImageData(searchBox.Text); 
            AddTiles(imagesList); 
            statusStrip.Visible = false;
        }

        private void Panel_Paint(object sender, PaintEventArgs e)
        {
            Rectangle r = searchBox.Bounds; 
            r.Inflate(3, 3); 
            Pen p = new Pen(Color.LightGray); 
            e.Graphics.DrawRectangle(p, r);
        }
    }
}
