// Decompiled with JetBrains decompiler
// Type: TakeScreenShot.ScreenshotApp
// Assembly: TakeScreenShot, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E5A75721-56D5-4985-9077-A2998E44E1D2
// Assembly location: C:\Users\NEUROEQUILIBRIUM\Desktop\Take Screen Shot\TakeScreenShot.exe

using Microsoft.VisualBasic;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace TakeScreenShot
{
  public class ScreenshotApp : Form
  {
    private Panel leftPanel;
    private FlowLayoutPanel flowPanel;
    private PictureBox screenshotPreview;
    private List<Rectangle> rectangles = new List<Rectangle>();
    private TextBox folderNameTextBox;
    private TextBox screenshotNameTextBox;
    private Button takeScreenshotButton;
    private Button saveScreenshotButton;
    private Button resetButton;
    private Button addRectangleButton;
    private Button nextButton;
    private Label screenshotCounterLabel;
    private Rectangle? selectedRectangle = new Rectangle?();
    private Point dragStart;
    private Dictionary<Rectangle, string> rectangleNames = new Dictionary<Rectangle, string>();
    private const string SaveFilePath = "rectangles.json";
    private int screenshotCounter = 1;
    private int rectangleIndex = -1;
    private IContainer components = (IContainer) null;

    private void ScreenshotApp_Load(object sender, EventArgs e)
    {
    }

    public ScreenshotApp()
    {
      this.InitializeComponent();
      this.Text = "Screenshot Application";
      this.WindowState = FormWindowState.Maximized;
      this.Opacity = 0.9;
      FlowLayoutPanel flowLayoutPanel = new FlowLayoutPanel();
      flowLayoutPanel.Dock = DockStyle.Fill;
      flowLayoutPanel.FlowDirection = FlowDirection.TopDown;
      flowLayoutPanel.AutoSize = true;
      this.flowPanel = flowLayoutPanel;
      Panel panel = new Panel();
      panel.Width = 250;
      panel.Dock = DockStyle.Left;
      panel.BackColor = Color.LightGray;
      this.leftPanel = panel;
      PictureBox pictureBox = new PictureBox();
      pictureBox.Dock = DockStyle.Fill;
      pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
      this.screenshotPreview = pictureBox;
      TextBox textBox = new TextBox();
      textBox.Text = "Enter folder name...";
      this.folderNameTextBox = textBox;
      this.folderNameTextBox.Size = new Size(200, 50);
      this.folderNameTextBox.Font = new Font(this.folderNameTextBox.Font.FontFamily, 15f);
      Label label = new Label();
      label.Text = string.Format("Screenshot #: {0}", (object) this.screenshotCounter);
      label.AutoSize = true;
      this.screenshotCounterLabel = label;
      this.screenshotCounterLabel.Font = new Font(this.screenshotCounterLabel.Font.FontFamily, 15f);
      this.folderNameTextBox.GotFocus += (EventHandler) ((s, e) =>
      {
        if (!(this.folderNameTextBox.Text == "Enter folder name..."))
          return;
        this.folderNameTextBox.Text = "";
      });
      this.folderNameTextBox.LostFocus += (EventHandler) ((s, e) =>
      {
        if (!string.IsNullOrWhiteSpace(this.folderNameTextBox.Text))
          return;
        this.folderNameTextBox.Text = "Enter folder name...";
      });
      Button button1 = new Button();
      button1.Text = "Save Screenshot";
      this.saveScreenshotButton = button1;
      this.saveScreenshotButton.Size = new Size(200, 50);
      this.saveScreenshotButton.Font = new Font(this.saveScreenshotButton.Font.FontFamily, 15f);
      this.saveScreenshotButton.Focus();
      Button button2 = new Button();
      button2.Text = "Reset";
      this.resetButton = button2;
      this.resetButton.Size = new Size(200, 50);
      this.resetButton.Font = new Font(this.resetButton.Font.FontFamily, 15f);
      Button button3 = new Button();
      button3.Text = "Add Rectangle";
      this.addRectangleButton = button3;
      this.addRectangleButton.Size = new Size(200, 50);
      this.addRectangleButton.Font = new Font(this.addRectangleButton.Font.FontFamily, 15f);
      Button button4 = new Button();
      button4.Text = "Next";
      this.nextButton = button4;
      this.nextButton.Size = new Size(200, 50);
      this.nextButton.Font = new Font(this.nextButton.Font.FontFamily, 15f);
      this.saveScreenshotButton.Click += new EventHandler(this.TakeScreenshot);
      this.saveScreenshotButton.Click += new EventHandler(this.SaveScreenshot);
      this.resetButton.Click += new EventHandler(this.ResetApplication);
      this.addRectangleButton.Click += new EventHandler(this.AddRectangle);
      this.nextButton.Click += new EventHandler(this.NextScreenshot);
      this.flowPanel.Controls.Add((Control) this.folderNameTextBox);
      this.flowPanel.Controls.Add((Control) this.saveScreenshotButton);
      this.flowPanel.Controls.Add((Control) this.resetButton);
      this.flowPanel.Controls.Add((Control) this.addRectangleButton);
      this.flowPanel.Controls.Add((Control) this.nextButton);
      this.flowPanel.Controls.Add((Control) this.screenshotCounterLabel);
      this.leftPanel.Controls.Add((Control) this.flowPanel);
      this.Controls.Add((Control) this.leftPanel);
      this.Controls.Add((Control) this.screenshotPreview);
      this.screenshotPreview.MouseDown += new MouseEventHandler(this.StartDragging);
      this.screenshotPreview.MouseMove += new MouseEventHandler(this.DragRectangle);
      this.screenshotPreview.MouseUp += new MouseEventHandler(this.StopDragging);
      this.screenshotPreview.Paint += new PaintEventHandler(this.DrawRectangles);
      this.screenshotPreview.MouseDoubleClick += new MouseEventHandler(this.EditRectangle);
      this.flowPanel.MouseMove += new MouseEventHandler(this.PanelMouseMove);
      this.flowPanel.MouseLeave += new EventHandler(this.PanelMouseLeave);
      this.folderNameTextBox.TextChanged += new EventHandler(this.FolderNameChanged);
      this.LoadRectanglesFromFile();
    }

    private void FolderNameChanged(object sender, EventArgs e)
    {
      this.screenshotCounter = 1;
      this.screenshotCounterLabel.Text = string.Format("Screenshot #: {0}", (object) this.screenshotCounter);
    }

    private void PanelMouseLeave(object sender, EventArgs e)
    {
      if (this.flowPanel.ClientRectangle.Contains(this.flowPanel.PointToClient(Cursor.Position)) || this.Opacity != 1.0)
        return;
      this.Opacity = 0.9;
    }

    private void PanelMouseMove(object sender, MouseEventArgs e)
    {
      if (this.Opacity == 1.0)
        return;
      this.Opacity = 1.0;
    }

    private void TakeScreenshot(object sender, EventArgs e)
    {
      this.WindowState = FormWindowState.Minimized;
      Thread.Sleep(500);
      Rectangle bounds = Screen.PrimaryScreen.Bounds;
      int width = bounds.Width;
      bounds = Screen.PrimaryScreen.Bounds;
      int height = bounds.Height;
      Bitmap bitmap = new Bitmap(width, height);
      using (Graphics graphics = Graphics.FromImage((Image) bitmap))
        graphics.CopyFromScreen(Point.Empty, Point.Empty, bitmap.Size);
      this.WindowState = FormWindowState.Maximized;
      this.screenshotPreview.Image = (Image) bitmap;
      this.Opacity = 1.0;
      this.screenshotPreview.Invalidate();
    }

    private void AddRectangle(object sender, EventArgs e)
    {
      Rectangle key = new Rectangle(500, 500, 400, 400);
      this.rectangles.Add(key);
      this.rectangleNames[key] = "NewRectangle";
      this.screenshotPreview.Invalidate();
    }

    private void NextScreenshot(object sender, EventArgs e)
    {
      ++this.screenshotCounter;
      this.screenshotCounterLabel.Text = string.Format("Screenshot #: {0}", (object) this.screenshotCounter);
      this.screenshotPreview.Image = (Image) null;
      this.screenshotPreview.Invalidate();
      this.Opacity = 0.9;
    }

    private void SaveScreenshot(object sender, EventArgs e)
    {
      if (this.screenshotPreview.Image == null)
      {
        int num1 = (int) MessageBox.Show("No screenshot available!");
      }
      else if (this.folderNameTextBox.Text.Equals("Enter folder name..."))
      {
        int num2 = (int) MessageBox.Show("Please enter folder name!");
      }
      else
      {
        string str1 = ("screenshot\\" + this.folderNameTextBox.Text).Trim();
        if (string.IsNullOrEmpty(str1))
        {
          int num3 = (int) MessageBox.Show("Please enter a folder name.");
        }
        else
        {
          Directory.CreateDirectory(str1);
          Bitmap bitmap1 = new Bitmap(this.screenshotPreview.Image);
          Bitmap bitmap2 = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
          foreach (Rectangle rectangle in this.rectangles)
          {
            string str2 = this.rectangleNames.ContainsKey(rectangle) ? this.rectangleNames[rectangle] : "Rectangle";
            string filename = Path.Combine(str1, str2 + string.Format("_{0}.jpg", (object) this.screenshotCounter));
            Point screen = this.PointToScreen(rectangle.Location);
            Rectangle rect = new Rectangle(screen, rectangle.Size);
            using (Bitmap bitmap3 = bitmap1.Clone(rect, bitmap1.PixelFormat))
            {
              bitmap3.Save(filename);
              using (Graphics graphics = Graphics.FromImage((Image) bitmap2))
                graphics.DrawImage((Image) bitmap3, screen);
            }
          }
          this.screenshotPreview.Image = (Image) bitmap2;
          int num4 = (int) MessageBox.Show("Screenshots saved successfully!");
        }
      }
    }

    private void ResetApplication(object sender, EventArgs e)
    {
      if (MessageBox.Show("Do you want to reset?", "Reset Data", MessageBoxButtons.YesNo) != DialogResult.Yes)
        return;
      this.folderNameTextBox.Clear();
      this.screenshotPreview.Image = (Image) null;
      this.rectangles.Clear();
      this.rectangleNames.Clear();
      this.screenshotCounterLabel.Text = string.Format("Screenshot #: {0}", (object) this.screenshotCounter);
      this.screenshotPreview.Invalidate();
    }

    private void StartDragging(object sender, MouseEventArgs e)
    {
      this.rectangleIndex = -1;
      int num = 0;
      foreach (Rectangle rectangle in this.rectangles)
      {
        if (rectangle.Contains(e.Location))
        {
          this.selectedRectangle = new Rectangle?(rectangle);
          this.dragStart = e.Location;
          this.rectangleIndex = num;
          break;
        }
        ++num;
      }
    }

    private void DragRectangle(object sender, MouseEventArgs e)
    {
      if (!this.selectedRectangle.HasValue || e.Button != MouseButtons.Left)
        return;
      this.leftPanel.Width = 0;
      this.Opacity = 0.3;
      int num1 = e.X - this.dragStart.X;
      int num2 = e.Y - this.dragStart.Y;
      Rectangle key = selectedRectangle.Value;
      ref Rectangle local = ref key;
      int x = this.selectedRectangle.Value.X + num1;
      Rectangle rectangle = this.selectedRectangle.Value;
      int y = rectangle.Y + num2;
      rectangle = this.selectedRectangle.Value;
      int width = rectangle.Width;
      rectangle = this.selectedRectangle.Value;
      int height = rectangle.Height;
      local = new Rectangle(x, y, width, height);
      this.rectangles[this.rectangleIndex] = key;
      string rectangleName = this.rectangleNames[this.selectedRectangle.Value];
      this.rectangleNames.Remove(this.selectedRectangle.Value);
      this.rectangleNames[key] = rectangleName;
      this.dragStart = e.Location;
      this.selectedRectangle = new Rectangle?(key);
      this.screenshotPreview.Invalidate();
    }

    private void DrawRectangles(object sender, PaintEventArgs e)
    {
      using (Pen pen = new Pen(Color.Red, 2f))
      {
        foreach (Rectangle rectangle in this.rectangles)
        {
          e.Graphics.DrawRectangle(pen, rectangle);
          if (this.rectangleNames.ContainsKey(rectangle))
          {
            string s = string.Format("{0} ({1}x{2})", (object) this.rectangleNames[rectangle], (object) rectangle.Width, (object) rectangle.Height);
            e.Graphics.DrawString(s, SystemFonts.DefaultFont, Brushes.Black, (PointF) rectangle.Location);
          }
        }
      }
    }

    private void StopDragging(object sender, MouseEventArgs e)
    {
      this.selectedRectangle = new Rectangle?();
      this.Opacity = 0.9;
      this.leftPanel.Width = 250;
      this.SaveRectanglesToFile();
    }

    private void EditRectangle(object sender, MouseEventArgs e)
    {
      foreach (Rectangle rectangle in this.rectangles)
      {
        if (rectangle.Contains(e.Location))
        {
          string str1 = Interaction.InputBox("Enter rectangle name:", "Rectangle Name", this.rectangleNames[rectangle]);
          string str2 = Interaction.InputBox("Enter width and height (format: width,height):", "Rectangle Size", string.Format("{0},{1}", (object) rectangle.Width, (object) rectangle.Height));
          if (!string.IsNullOrEmpty(str1))
            this.rectangleNames[rectangle] = str1;
          string[] strArray = str2.Split(',');
          int result1;
          int result2;
          if (strArray.Length == 2 && int.TryParse(strArray[0], out result1) && int.TryParse(strArray[1], out result2))
          {
            Rectangle key = new Rectangle(rectangle.X, rectangle.Y, result1, result2);
            this.rectangles[this.rectangles.IndexOf(rectangle)] = key;
            this.rectangleNames.Remove(rectangle);
            this.rectangleNames[key] = str1;
          }
          this.screenshotPreview.Invalidate();
          return;
        }
      }
      this.SaveRectanglesToFile();
    }

    private void SaveRectanglesToFile()
    {
      Dictionary<Rectangle, string> dictionary = new Dictionary<Rectangle, string>();
      foreach (Rectangle rectangle in this.rectangles)
        dictionary.Add(rectangle, this.rectangleNames[rectangle]);
      File.WriteAllText("rectangles.json", JsonConvert.SerializeObject((object) dictionary, Formatting.Indented));
    }

    private void LoadRectanglesFromFile()
    {
      if (!File.Exists("rectangles.json"))
        return;
      Dictionary<Rectangle, string> dictionary = JsonConvert.DeserializeObject<Dictionary<Rectangle, string>>(File.ReadAllText("rectangles.json"));
      this.rectangles.Clear();
      this.rectangleNames.Clear();
      foreach (KeyValuePair<Rectangle, string> keyValuePair in dictionary)
      {
        this.rectangles.Add(keyValuePair.Key);
        this.rectangleNames[keyValuePair.Key] = keyValuePair.Value;
      }
      this.screenshotPreview.Invalidate();
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      this.SuspendLayout();
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.ClientSize = new Size(800, 450);
      this.Name = nameof (ScreenshotApp);
      this.Text = "Form1";
      this.Load += new EventHandler(this.ScreenshotApp_Load);
      this.ResumeLayout(false);
    }
  }
}
