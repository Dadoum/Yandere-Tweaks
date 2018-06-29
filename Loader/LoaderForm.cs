using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
public class LoaderForm : Form
{
    bool showed = false;

    public LoaderForm()
    {
        InitializeComponent();
        var t = new ModLoader();
        t.Program();
    }

    public void OnClose(object sender, FormClosingEventArgs e) {
        ModLoader.FastClose();
        Process.GetCurrentProcess().Kill();
    }

    public ProgressBar GetProgressBar() {
        return progress;
    }

    public void OnClick(Object sender, EventArgs eventArgs) {
        if (!showed) {
            this.ClientSize = new System.Drawing.Size(684, 401);
            showDetails.Text = "Hide Details";
        }
        else {
            this.ClientSize = new System.Drawing.Size(684, 70);
            showDetails.Text = "Show Details";
        }
        showed = !showed;
    }

    #region Partial class

    public static ProgressBar progress = new ProgressBar();
    public static TextBox textBox1 = new TextBox();
    public static Button showDetails = new Button();

    private void InitializeComponent()
    {
        Form.CheckForIllegalCrossThreadCalls = false;

        textBox1.ReadOnly = true;
        textBox1.Multiline = true;
        textBox1.Size = new Size(682, 330);
        textBox1.Location = new Point(1, 70);
        textBox1.SelectionStart = textBox1.Text.Length;
        textBox1.ScrollToCaret();
        textBox1.Refresh();
        this.Controls.Add(textBox1);

        progress.Style = ProgressBarStyle.Continuous;
        progress.BackColor = Color.LightGray;
        progress.Minimum = 0;
        progress.Size = new Size(662, 30);
        progress.Location = new Point(10, 15);
        this.Controls.Add(progress);

        showDetails.FlatStyle = FlatStyle.Standard;
        showDetails.Text = "Show Details";
        showDetails.Size = new Size(90, 20);
        showDetails.Location = new Point(292, 47);
        showDetails.Click += new EventHandler(OnClick);
        this.Controls.Add(showDetails);

        this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(684, 70);
        this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnClose);
        this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
        this.Name = "Loader";
        this.Text = "Yandere Loader";
        this.ResumeLayout(false);
        this.PerformLayout();
        this.Icon = null;
        this.ShowIcon = false;
    }

    #endregion
}