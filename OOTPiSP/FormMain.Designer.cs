namespace OOP
{
    partial class FormMain
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
            components = new System.ComponentModel.Container();
            splitContainer = new SplitContainer();
            LbObjects = new ListBox();
            CmSpace = new ContextMenuStrip(components);
            CmSpaceToolStripMenuItem1 = new ToolStripMenuItem();
            TvProps = new TreeView();
            CmItem = new ContextMenuStrip(components);
            CmItemToolStripMenuItem1 = new ToolStripMenuItem();
            CmItemToolStripMenuItem2 = new ToolStripMenuItem();
            CmItemToolStripMenuItem3 = new ToolStripSeparator();
            CmItemToolStripMenuItem4 = new ToolStripMenuItem();
            menuStrip1 = new MenuStrip();
            toolStripMenuItem1 = new ToolStripMenuItem();
            openMenuItem = new ToolStripMenuItem();
            saveMenuItem = new ToolStripMenuItem();
            fdOpen = new OpenFileDialog();
            fdSave = new SaveFileDialog();
            ((System.ComponentModel.ISupportInitialize)splitContainer).BeginInit();
            splitContainer.Panel1.SuspendLayout();
            splitContainer.Panel2.SuspendLayout();
            splitContainer.SuspendLayout();
            CmSpace.SuspendLayout();
            CmItem.SuspendLayout();
            menuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // splitContainer
            // 
            splitContainer.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            splitContainer.Location = new Point(5, 32);
            splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            splitContainer.Panel1.Controls.Add(LbObjects);
            splitContainer.Panel1MinSize = 200;
            // 
            // splitContainer.Panel2
            // 
            splitContainer.Panel2.Controls.Add(TvProps);
            splitContainer.Panel2MinSize = 200;
            splitContainer.Size = new Size(1078, 414);
            splitContainer.SplitterDistance = 350;
            splitContainer.TabIndex = 5;
            // 
            // LbObjects
            // 
            LbObjects.ContextMenuStrip = CmSpace;
            LbObjects.Dock = DockStyle.Fill;
            LbObjects.FormattingEnabled = true;
            LbObjects.IntegralHeight = false;
            LbObjects.ItemHeight = 21;
            LbObjects.Location = new Point(0, 0);
            LbObjects.Name = "LbObjects";
            LbObjects.Size = new Size(350, 414);
            LbObjects.TabIndex = 3;
            LbObjects.SelectedIndexChanged += LbObjects_SelectedIndexChanged;
            LbObjects.MouseDown += LbObjects_MouseDown;
            // 
            // CmSpace
            // 
            CmSpace.ImageScalingSize = new Size(20, 20);
            CmSpace.Items.AddRange(new ToolStripItem[] { CmSpaceToolStripMenuItem1 });
            CmSpace.Name = "contextMenuStrip1";
            CmSpace.RenderMode = ToolStripRenderMode.System;
            CmSpace.Size = new Size(139, 30);
            // 
            // CmSpaceToolStripMenuItem1
            // 
            CmSpaceToolStripMenuItem1.Name = "CmSpaceToolStripMenuItem1";
            CmSpaceToolStripMenuItem1.Size = new Size(138, 26);
            CmSpaceToolStripMenuItem1.Text = "Создать";
            CmSpaceToolStripMenuItem1.Click += CmSpaceToolStripMenuItemCreate_Click;
            // 
            // TvProps
            // 
            TvProps.Dock = DockStyle.Fill;
            TvProps.Font = new Font("Courier New", 12F, FontStyle.Regular, GraphicsUnit.Point);
            TvProps.Location = new Point(0, 0);
            TvProps.Name = "TvProps";
            TvProps.Size = new Size(724, 414);
            TvProps.TabIndex = 0;
            // 
            // CmItem
            // 
            CmItem.ImageScalingSize = new Size(20, 20);
            CmItem.Items.AddRange(new ToolStripItem[] { CmItemToolStripMenuItem1, CmItemToolStripMenuItem2, CmItemToolStripMenuItem3, CmItemToolStripMenuItem4 });
            CmItem.Name = "CmItem";
            CmItem.Size = new Size(152, 88);
            // 
            // CmItemToolStripMenuItem1
            // 
            CmItemToolStripMenuItem1.Name = "CmItemToolStripMenuItem1";
            CmItemToolStripMenuItem1.Size = new Size(151, 26);
            CmItemToolStripMenuItem1.Text = "Изменить";
            CmItemToolStripMenuItem1.Click += CmSpaceToolStripMenuItemEdit_Click;
            // 
            // CmItemToolStripMenuItem2
            // 
            CmItemToolStripMenuItem2.Name = "CmItemToolStripMenuItem2";
            CmItemToolStripMenuItem2.Size = new Size(151, 26);
            CmItemToolStripMenuItem2.Text = "Удалить";
            CmItemToolStripMenuItem2.Click += CmSpaceToolStripMenuItemDelete_Click;
            // 
            // CmItemToolStripMenuItem3
            // 
            CmItemToolStripMenuItem3.Name = "CmItemToolStripMenuItem3";
            CmItemToolStripMenuItem3.Size = new Size(148, 6);
            // 
            // CmItemToolStripMenuItem4
            // 
            CmItemToolStripMenuItem4.Name = "CmItemToolStripMenuItem4";
            CmItemToolStripMenuItem4.Size = new Size(151, 26);
            CmItemToolStripMenuItem4.Text = "Создать";
            CmItemToolStripMenuItem4.Click += CmSpaceToolStripMenuItemCreate_Click;
            // 
            // menuStrip1
            // 
            menuStrip1.BackColor = SystemColors.Window;
            menuStrip1.ImageScalingSize = new Size(20, 20);
            menuStrip1.Items.AddRange(new ToolStripItem[] { toolStripMenuItem1 });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Padding = new Padding(2, 2, 0, 2);
            menuStrip1.Size = new Size(1087, 29);
            menuStrip1.TabIndex = 6;
            menuStrip1.Text = "menuStrip1";
            // 
            // toolStripMenuItem1
            // 
            toolStripMenuItem1.DropDownItems.AddRange(new ToolStripItem[] { openMenuItem, saveMenuItem });
            toolStripMenuItem1.Name = "toolStripMenuItem1";
            toolStripMenuItem1.Size = new Size(61, 25);
            toolStripMenuItem1.Text = "Файл";
            // 
            // openMenuItem
            // 
            openMenuItem.Name = "openMenuItem";
            openMenuItem.ShortcutKeys = Keys.Control | Keys.O;
            openMenuItem.Size = new Size(225, 26);
            openMenuItem.Text = "Открыть";
            openMenuItem.Click += OpenMenuItem_Click;
            // 
            // saveMenuItem
            // 
            saveMenuItem.Name = "saveMenuItem";
            saveMenuItem.ShortcutKeys = Keys.Control | Keys.S;
            saveMenuItem.Size = new Size(225, 26);
            saveMenuItem.Text = "Сохранить";
            saveMenuItem.Click += SaveMenuItem_Click;
            // 
            // FormMain
            // 
            AutoScaleDimensions = new SizeF(9F, 21F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1087, 450);
            Controls.Add(menuStrip1);
            Controls.Add(splitContainer);
            MainMenuStrip = menuStrip1;
            MinimumSize = new Size(895, 480);
            Name = "FormMain";
            Text = "OOP";
            splitContainer.Panel1.ResumeLayout(false);
            splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer).EndInit();
            splitContainer.ResumeLayout(false);
            CmSpace.ResumeLayout(false);
            CmItem.ResumeLayout(false);
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private SplitContainer splitContainer;
        private ListBox LbObjects;
        private ContextMenuStrip CmSpace;
        private ToolStripMenuItem CmSpaceToolStripMenuItem1;
        private ContextMenuStrip CmItem;
        private ToolStripMenuItem CmItemToolStripMenuItem1;
        private ToolStripMenuItem CmItemToolStripMenuItem2;
        private ToolStripSeparator CmItemToolStripMenuItem3;
        private ToolStripMenuItem CmItemToolStripMenuItem4;
        private TreeView TvProps;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem toolStripMenuItem1;
        private ToolStripMenuItem openMenuItem;
        private ToolStripMenuItem saveMenuItem;
        private OpenFileDialog fdOpen;
        private SaveFileDialog fdSave;
    }
}