namespace OOP
{
    partial class FormObj
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            FlpBottom = new FlowLayoutPanel();
            FlpTop = new FlowLayoutPanel();
            PnlContent = new Panel();
            SuspendLayout();
            // 
            // FlpBottom
            // 
            FlpBottom.AutoSize = true;
            FlpBottom.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            FlpBottom.Dock = DockStyle.Bottom;
            FlpBottom.FlowDirection = FlowDirection.RightToLeft;
            FlpBottom.Location = new Point(0, 673);
            FlpBottom.Name = "FlpBottom";
            FlpBottom.Size = new Size(702, 0);
            FlpBottom.TabIndex = 1;
            FlpBottom.WrapContents = false;
            // 
            // FlpTop
            // 
            FlpTop.AutoSize = true;
            FlpTop.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            FlpTop.Dock = DockStyle.Top;
            FlpTop.Location = new Point(0, 0);
            FlpTop.Name = "FlpTop";
            FlpTop.Size = new Size(702, 0);
            FlpTop.TabIndex = 2;
            FlpTop.WrapContents = false;
            // 
            // PnlContent
            // 
            PnlContent.AutoScroll = true;
            PnlContent.Dock = DockStyle.Fill;
            PnlContent.Location = new Point(0, 0);
            PnlContent.Name = "PnlContent";
            PnlContent.Size = new Size(702, 673);
            PnlContent.TabIndex = 3;
            // 
            // FormObj
            // 
            AutoScaleDimensions = new SizeF(9F, 21F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(702, 673);
            Controls.Add(PnlContent);
            Controls.Add(FlpTop);
            Controls.Add(FlpBottom);
            MinimumSize = new Size(500, 480);
            Name = "FormObj";
            Text = "FormInput";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private FlowLayoutPanel FlpBottom;
        //private TableLayoutPanel TlpContent;
        private FlowLayoutPanel FlpTop;
        private Panel PnlContent;
    }
}