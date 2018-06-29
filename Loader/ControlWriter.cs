using System;
using System.IO;
using System.Windows.Forms;

public class ControlWriter : TextWriter
{
    private Control textbox;
    public ControlWriter(Control textbox)
    {
        this.textbox = textbox;
    }

    public override void Write(char value)
    {
        textbox.Suspend();  
        textbox.Text += value;
        textbox.Resume();
    }



    public override void Write(string value)
    {
        textbox.Suspend();
        value = value.Replace("\n", Environment.NewLine);
        textbox.Text += value;
        textbox.Resume();
    }



    public override System.Text.Encoding Encoding => System.Text.Encoding.Unicode;

}