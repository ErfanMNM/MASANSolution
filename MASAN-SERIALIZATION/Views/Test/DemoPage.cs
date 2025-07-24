using Newtonsoft.Json;
using Sunny.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZXing;
using ZXing.Common;
using ZXing.QrCode;

namespace MASAN_SERIALIZATION.Views.Test
{
    public partial class DemoPage : UIPage
    {
        private FlowLayoutPanel flowLayoutPanel;
        private Button btnLoadTemplate;
        private Button btnAddLabel;
        private Button btnPrint;
        private LabelTemplate currentTemplate;

        public DemoPage()
        {
            InitializeComponent();
        }


        private void BtnLoadTemplate_Click(object sender, EventArgs e)
        {
            using (var openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "JSON files (*.json)|*.json";
                openFileDialog.Title = "Select Label Template";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        string json = File.ReadAllText(openFileDialog.FileName);
                        currentTemplate = JsonConvert.DeserializeObject<LabelTemplate>(json);

                        MessageBox.Show($"Template loaded successfully!\nCanvas size: {currentTemplate.TemplateInfo.CanvasWidth}x{currentTemplate.TemplateInfo.CanvasHeight}",
                            "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        btnAddLabel.Enabled = true;
                        btnPrint.Enabled = true;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error loading template: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void BtnAddLabel_Click(object sender, EventArgs e)
        {
            if (currentTemplate == null) return;

            // Create sample data
            var sampleData = new Dictionary<string, string>();

            // Populate with sample data based on required fields
            if (currentTemplate.RequiredDataFields != null)
            {
                foreach (var field in currentTemplate.RequiredDataFields)
                {
                    sampleData[field.FieldName] = $"Sample {field.FieldName} #{flowLayoutPanel.Controls.Count + 1}";
                }
            }

            // Create and render label
            var labelPanel = new LabelPanel();
            labelPanel.LoadTemplate(currentTemplate);
            labelPanel.BindData(sampleData);
            labelPanel.Margin = new Padding(5);

            flowLayoutPanel.Controls.Add(labelPanel);
        }

        private void BtnPrint_Click(object sender, EventArgs e)
        {
            if (flowLayoutPanel.Controls.Count == 0)
            {
                MessageBox.Show("No labels to print!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Create print preview
            using (var printPreview = new PrintPreviewDialog())
            {
                var printDoc = new System.Drawing.Printing.PrintDocument();
                printDoc.PrintPage += (s, ev) =>
                {
                    int x = 50, y = 50;
                    int maxHeight = 0;

                    foreach (LabelPanel label in flowLayoutPanel.Controls)
                    {
                        if (x + label.Width > ev.PageBounds.Width - 50)
                        {
                            x = 50;
                            y += maxHeight + 20;
                            maxHeight = 0;
                        }

                        using (var bitmap = label.ExportToImage())
                        {
                            ev.Graphics.DrawImage(bitmap, x, y);
                        }

                        maxHeight = Math.Max(maxHeight, label.Height);
                        x += label.Width + 20;
                    }
                };

                printPreview.Document = printDoc;
                printPreview.ShowDialog();
            }
        }
    }

    // Classes để deserialize JSON
    public class LabelTemplate
    {
        public TemplateInfo TemplateInfo { get; set; }
        public List<TextElement> StaticElements { get; set; }
        public List<TextElement> DynamicElements { get; set; }
        public List<BarcodeElement> BarcodeElements { get; set; }
        public List<ShapeElement> ShapeElements { get; set; }
        public List<ImageElement> ImageElements { get; set; }
        public List<DataField> RequiredDataFields { get; set; }
    }

    public class TemplateInfo
    {
        public string Name { get; set; }
        public string Version { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CanvasWidth { get; set; }
        public int CanvasHeight { get; set; }
        public string Unit { get; set; }
    }

    public class ElementBase
    {
        public string ElementId { get; set; }
        public Rectangle Bounds { get; set; }
        public int ZIndex { get; set; }
    }

    public class Rectangle
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }

    public class TextElement : ElementBase
    {
        public string Text { get; set; }
        public string FontFamily { get; set; }
        public int FontSize { get; set; }
        public string FontColor { get; set; }
        public string TextAlign { get; set; }
        public string BindingField { get; set; }
        public string DefaultValue { get; set; }
    }

    public class BarcodeElement : ElementBase
    {
        public string BarcodeType { get; set; }
        public string Data { get; set; }
        public bool ShowText { get; set; }
        public string BindingField { get; set; }
    }

    public class ShapeElement : ElementBase
    {
        public string ShapeType { get; set; }
        public string FillColor { get; set; }
        public string BorderColor { get; set; }
        public int BorderWidth { get; set; }
    }

    public class ImageElement : ElementBase
    {
        public string ImageType { get; set; }
        public string ImagePath { get; set; }
        public string BindingField { get; set; }
    }

    public class DataField
    {
        public string FieldName { get; set; }
        public string FieldType { get; set; }
        public string DefaultValue { get; set; }
    }

    // Label Renderer Panel
    public class LabelPanel : Panel
    {
        private LabelTemplate template;
        private Dictionary<string, string> dataBinding;
        private List<Control> renderedElements = new List<Control>();

        public LabelPanel()
        {
            this.BackColor = Color.White;
            this.BorderStyle = BorderStyle.FixedSingle;
            this.DoubleBuffered = true;
        }

        public void LoadTemplate(LabelTemplate labelTemplate)
        {
            this.template = labelTemplate;
            if (template?.TemplateInfo != null)
            {
                this.Width = template.TemplateInfo.CanvasWidth;
                this.Height = template.TemplateInfo.CanvasHeight;
            }
        }

        public void BindData(Dictionary<string, string> data)
        {
            this.dataBinding = data ?? new Dictionary<string, string>();
            RenderLabel();
        }

        public void RenderLabel()
        {
            // Clear existing elements
            foreach (var control in renderedElements)
            {
                this.Controls.Remove(control);
                control.Dispose();
            }
            renderedElements.Clear();

            if (template == null) return;

            // Collect all elements and sort by Z-index
            var allElements = new List<(object element, int zIndex)>();

            if (template.ShapeElements != null)
                allElements.AddRange(template.ShapeElements.Select(e => ((object)e, e.ZIndex)));

            if (template.StaticElements != null)
                allElements.AddRange(template.StaticElements.Select(e => ((object)e, e.ZIndex)));

            if (template.DynamicElements != null)
                allElements.AddRange(template.DynamicElements.Select(e => ((object)e, e.ZIndex)));

            if (template.BarcodeElements != null)
                allElements.AddRange(template.BarcodeElements.Select(e => ((object)e, e.ZIndex)));

            if (template.ImageElements != null)
                allElements.AddRange(template.ImageElements.Select(e => ((object)e, e.ZIndex)));

            // Sort by Z-index and render
            foreach (var item in allElements.OrderBy(e => e.zIndex))
            {
                Control control = null;

                switch (item.element)
                {
                    case TextElement textElement:
                        control = RenderTextElement(textElement);
                        break;
                    case BarcodeElement barcodeElement:
                        control = RenderBarcodeElement(barcodeElement);
                        break;
                    case ShapeElement shapeElement:
                        control = RenderShapeElement(shapeElement);
                        break;
                    case ImageElement imageElement:
                        control = RenderImageElement(imageElement);
                        break;
                }

                if (control != null)
                {
                    renderedElements.Add(control);
                    this.Controls.Add(control);
                }
            }
        }

        private Control RenderTextElement(TextElement element)
        {
            var label = new Label();
            label.Name = element.ElementId;
            label.Location = new Point(element.Bounds.X, element.Bounds.Y);
            label.Size = new Size(element.Bounds.Width, element.Bounds.Height);

            // Get text value
            string text = element.Text;
            if (!string.IsNullOrEmpty(element.BindingField) && dataBinding.ContainsKey(element.BindingField))
            {
                text = dataBinding[element.BindingField];
            }
            label.Text = text;

            // Apply font
            try
            {
                label.Font = new Font(element.FontFamily ?? "Arial", element.FontSize > 0 ? element.FontSize : 12);
            }
            catch
            {
                label.Font = new Font("Arial", 12);
            }

            // Apply color
            try
            {
                label.ForeColor = ColorTranslator.FromHtml(element.FontColor ?? "#000000");
            }
            catch
            {
                label.ForeColor = Color.Black;
            }

            // Apply text alignment
            switch (element.TextAlign?.ToLower())
            {
                case "center":
                    label.TextAlign = ContentAlignment.MiddleCenter;
                    break;
                case "right":
                    label.TextAlign = ContentAlignment.MiddleRight;
                    break;
                default:
                    label.TextAlign = ContentAlignment.MiddleLeft;
                    break;
            }

            label.AutoSize = false;
            label.BackColor = Color.Transparent;

            return label;
        }

        private Control RenderBarcodeElement(BarcodeElement element)
        {
            var panel = new Panel();
            panel.Name = element.ElementId;
            panel.Location = new Point(element.Bounds.X, element.Bounds.Y);
            panel.Size = new Size(element.Bounds.Width, element.Bounds.Height);
            panel.BackColor = Color.White;
            panel.BorderStyle = BorderStyle.FixedSingle;

            // Get barcode data
            string data = element.Data;
            if (!string.IsNullOrEmpty(element.BindingField) && dataBinding.ContainsKey(element.BindingField))
            {
                data = dataBinding[element.BindingField];
            }

            var pictureBox = new PictureBox();
            pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox.Dock = DockStyle.Fill;

            try
            {
                if (element.BarcodeType == "CODE128")
                {
                    var writer = new BarcodeWriter
                    {
                        Format = BarcodeFormat.CODE_128,
                        Options = new EncodingOptions
                        {
                            Height = element.ShowText ? element.Bounds.Height - 20 : element.Bounds.Height,
                            Width = element.Bounds.Width,
                            Margin = 0
                        }
                    };
                    pictureBox.Image = writer.Write(data);

                    if (element.ShowText)
                    {
                        var label = new Label();
                        label.Text = data;
                        label.Dock = DockStyle.Bottom;
                        label.Height = 20;
                        label.TextAlign = ContentAlignment.MiddleCenter;
                        label.Font = new Font("Courier New", 8);
                        panel.Controls.Add(label);
                    }
                }
                else if (element.BarcodeType == "QRCODE")
                {
                    var writer = new BarcodeWriter
                    {
                        Format = BarcodeFormat.QR_CODE,
                        Options = new QrCodeEncodingOptions
                        {
                            Height = element.Bounds.Height,
                            Width = element.Bounds.Width,
                            Margin = 0
                        }
                    };
                    pictureBox.Image = writer.Write(data);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error generating barcode: {ex.Message}");
                // Nếu không có ZXing, hiển thị placeholder
                pictureBox.Image = CreatePlaceholderBarcode(element.Bounds.Width, element.Bounds.Height, element.BarcodeType);
            }

            panel.Controls.Add(pictureBox);
            return panel;
        }

        private Control RenderShapeElement(ShapeElement element)
        {
            var panel = new ShapePanel(element);
            panel.Name = element.ElementId;
            panel.Location = new Point(element.Bounds.X, element.Bounds.Y);
            panel.Size = new Size(element.Bounds.Width, element.Bounds.Height);
            return panel;
        }

        private Control RenderImageElement(ImageElement element)
        {
            var pictureBox = new PictureBox();
            pictureBox.Name = element.ElementId;
            pictureBox.Location = new Point(element.Bounds.X, element.Bounds.Y);
            pictureBox.Size = new Size(element.Bounds.Width, element.Bounds.Height);
            pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox.BackColor = Color.LightGray;
            pictureBox.BorderStyle = BorderStyle.FixedSingle;

            // Load image if path exists
            string imagePath = element.ImagePath;
            if (!string.IsNullOrEmpty(element.BindingField) && dataBinding.ContainsKey(element.BindingField))
            {
                imagePath = dataBinding[element.BindingField];
            }

            if (!string.IsNullOrEmpty(imagePath) && File.Exists(imagePath))
            {
                try
                {
                    pictureBox.Image = Image.FromFile(imagePath);
                }
                catch { }
            }
            else
            {
                // Show placeholder
                pictureBox.Paint += (s, e) =>
                {
                    string text = element.ImageType == "logo" ? "LOGO" : "IMAGE";
                    using (var font = new Font("Arial", 12))
                    {
                        var textSize = e.Graphics.MeasureString(text, font);
                        var x = (pictureBox.Width - textSize.Width) / 2;
                        var y = (pictureBox.Height - textSize.Height) / 2;
                        e.Graphics.DrawString(text, font, Brushes.Gray, x, y);
                    }
                };
            }

            return pictureBox;
        }

        private Bitmap CreatePlaceholderBarcode(int width, int height, string type)
        {
            var bitmap = new Bitmap(width, height);
            using (var g = Graphics.FromImage(bitmap))
            {
                g.Clear(Color.White);

                if (type == "CODE128")
                {
                    // Draw fake barcode lines
                    int x = 10;
                    Random rand = new Random();
                    while (x < width - 10)
                    {
                        int barWidth = rand.Next(2, 5);
                        g.FillRectangle(Brushes.Black, x, 10, barWidth, height - 30);
                        x += barWidth + rand.Next(2, 4);
                    }
                }
                else if (type == "QRCODE")
                {
                    // Draw QR pattern
                    int cellSize = 4;
                    Random rand = new Random();
                    for (int y = 0; y < height; y += cellSize)
                    {
                        for (int x = 0; x < width; x += cellSize)
                        {
                            if (rand.Next(2) == 1)
                                g.FillRectangle(Brushes.Black, x, y, cellSize, cellSize);
                        }
                    }
                }
            }
            return bitmap;
        }

        // Export to image
        public Bitmap ExportToImage()
        {
            var bitmap = new Bitmap(this.Width, this.Height);
            this.DrawToBitmap(bitmap, new System.Drawing.Rectangle(0, 0, this.Width, this.Height));
            return bitmap;
        }
    }

    // Custom panel for shapes
    public class ShapePanel : Panel
    {
        private ShapeElement shapeElement;

        public ShapePanel(ShapeElement element)
        {
            this.shapeElement = element;
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.DoubleBuffer, true);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            Color fillColor = Color.LightGray;
            Color borderColor = Color.Gray;

            try
            {
                fillColor = ColorTranslator.FromHtml(shapeElement.FillColor ?? "#e0e0e0");
                borderColor = ColorTranslator.FromHtml(shapeElement.BorderColor ?? "#999999");
            }
            catch { }

            using (var brush = new SolidBrush(fillColor))
            using (var pen = new Pen(borderColor, shapeElement.BorderWidth))
            {
                var rect = new System.Drawing.Rectangle(0, 0, this.Width - 1, this.Height - 1);

                switch (shapeElement.ShapeType?.ToLower())
                {
                    case "circle":
                        g.FillEllipse(brush, rect);
                        g.DrawEllipse(pen, rect);
                        break;
                    case "line":
                        g.DrawLine(pen, 0, this.Height / 2, this.Width, this.Height / 2);
                        break;
                    default: // rectangle
                        g.FillRectangle(brush, rect);
                        g.DrawRectangle(pen, rect);
                        break;
                }
            }
        }
    }
}
