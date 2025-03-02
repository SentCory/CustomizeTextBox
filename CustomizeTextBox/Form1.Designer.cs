namespace CustomizeTextBox
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            textBox1 = new TextBox();
            SuspendLayout();
            // 
            // textBox1
            // 
            textBox1.BackColor = Color.Black;
            textBox1.Cursor = Cursors.IBeam;
            textBox1.ForeColor = Color.WhiteSmoke;
            textBox1.ImeMode = ImeMode.On;
            textBox1.Location = new Point(351, 351);
            textBox1.Name = "textBox1";
            textBox1.Padding = new Padding(8);
            textBox1.Size = new Size(298, 33);
            textBox1.TabIndex = 0;
            textBox1.Text = "TextTest";
            textBox1.UnderlineColor = Color.FromArgb(255, 255, 192);
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1068, 592);
            Controls.Add(textBox1);
            Name = "Form1";
            Text = "Form1";
            ResumeLayout(false);
        }

        #endregion

        private TextBox textBox1;
    }
}
