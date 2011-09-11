

namespace Duplicati.GUI.HelperControls {

	partial class dlgTestSearchSelection {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if( disposing && ( components != null ) ) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(dlgTestSearchSelection));
			this.ctxSelection = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.miInclude = new System.Windows.Forms.ToolStripMenuItem();
			this.miIncludeFileName = new System.Windows.Forms.ToolStripMenuItem();
			this.miIncludeFileExt = new System.Windows.Forms.ToolStripMenuItem();
			this.miIncludeFilePath = new System.Windows.Forms.ToolStripMenuItem();
			this.miExlude = new System.Windows.Forms.ToolStripMenuItem();
			this.miExcludeFileName = new System.Windows.Forms.ToolStripMenuItem();
			this.miExcludeFileExt = new System.Windows.Forms.ToolStripMenuItem();
			this.miExludeFilePath = new System.Windows.Forms.ToolStripMenuItem();
			this.imageList = new System.Windows.Forms.ImageList(this.components);
			this.label1 = new System.Windows.Forms.Label();
			this.btnClose = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.lvFilters = new System.Windows.Forms.ListView();
			this.ctxFilters = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.removeFilterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.scanWorker = new System.ComponentModel.BackgroundWorker();
			this.lbTotalSize = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.tbSearch = new System.Windows.Forms.TextBox();
			this.btnSearch = new System.Windows.Forms.PictureBox();
			this.btnClearSearch = new System.Windows.Forms.PictureBox();
			this.lvFiles = new System.Windows.Forms.ListView();
			this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.lbLoading = new System.Windows.Forms.Label();
			this.ctxSelection.SuspendLayout();
			this.ctxFilters.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.btnSearch)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.btnClearSearch)).BeginInit();
			this.SuspendLayout();
			// 
			// ctxSelection
			// 
			this.ctxSelection.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miInclude,
            this.miExlude});
			this.ctxSelection.Name = "ctxSelection";
			this.ctxSelection.Size = new System.Drawing.Size(110, 48);
			this.ctxSelection.Opened += new System.EventHandler(this.ctxSelection_Opened);
			// 
			// miInclude
			// 
			this.miInclude.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miIncludeFileName,
            this.miIncludeFileExt,
            this.miIncludeFilePath});
			this.miInclude.Name = "miInclude";
			this.miInclude.Size = new System.Drawing.Size(109, 22);
			this.miInclude.Text = "Include";
			// 
			// miIncludeFileName
			// 
			this.miIncludeFileName.Name = "miIncludeFileName";
			this.miIncludeFileName.Size = new System.Drawing.Size(179, 22);
			this.miIncludeFileName.Tag = "1";
			this.miIncludeFileName.Text = "Add filename filer";
			this.miIncludeFileName.Click += new System.EventHandler(this.miIncludeFileName_Click);
			// 
			// miIncludeFileExt
			// 
			this.miIncludeFileExt.Name = "miIncludeFileExt";
			this.miIncludeFileExt.Size = new System.Drawing.Size(179, 22);
			this.miIncludeFileExt.Text = "Add file extension filter";
			this.miIncludeFileExt.Click += new System.EventHandler(this.miIncludeFileExt_Click);
			// 
			// miIncludeFilePath
			// 
			this.miIncludeFilePath.Name = "miIncludeFilePath";
			this.miIncludeFilePath.Size = new System.Drawing.Size(179, 22);
			this.miIncludeFilePath.Text = "File path";
			// 
			// miExlude
			// 
			this.miExlude.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miExcludeFileName,
            this.miExcludeFileExt,
            this.miExludeFilePath});
			this.miExlude.Name = "miExlude";
			this.miExlude.Size = new System.Drawing.Size(109, 22);
			this.miExlude.Text = "Exlude";
			// 
			// miExcludeFileName
			// 
			this.miExcludeFileName.Name = "miExcludeFileName";
			this.miExcludeFileName.Size = new System.Drawing.Size(179, 22);
			this.miExcludeFileName.Text = "Add filename filter";
			this.miExcludeFileName.Click += new System.EventHandler(this.addFilenameFilterToolStripMenuItem_Click);
			// 
			// miExcludeFileExt
			// 
			this.miExcludeFileExt.Name = "miExcludeFileExt";
			this.miExcludeFileExt.Size = new System.Drawing.Size(179, 22);
			this.miExcludeFileExt.Text = "Add file extension filter";
			this.miExcludeFileExt.Click += new System.EventHandler(this.addFileExtensionFilterToolStripMenuItem1_Click);
			// 
			// miExludeFilePath
			// 
			this.miExludeFilePath.Name = "miExludeFilePath";
			this.miExludeFilePath.Size = new System.Drawing.Size(179, 22);
			this.miExludeFilePath.Text = "File Path";
			this.miExludeFilePath.Click += new System.EventHandler(this.addFilePathFilerToolStripMenuItem_Click);
			// 
			// imageList
			// 
			this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
			this.imageList.TransparentColor = System.Drawing.Color.Transparent;
			this.imageList.Images.SetKeyName(0, "Inclusion.ico");
			this.imageList.Images.SetKeyName(1, "Exclusion.ico");
			this.imageList.Images.SetKeyName(2, "icon_up_sort_arrow[1].png");
			this.imageList.Images.SetKeyName(3, "icon_down_sort_arrow[1].png");
			this.imageList.Images.SetKeyName(4, "sort_ascending[1].png");
			this.imageList.Images.SetKeyName(5, "sort_descending[1].png");
			this.imageList.Images.SetKeyName(6, "sort_up[1].png");
			this.imageList.Images.SetKeyName(7, "sort_down[1].png");
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(9, 272);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(37, 13);
			this.label1.TabIndex = 2;
			this.label1.Text = "Filters:";
			// 
			// btnClose
			// 
			this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnClose.Location = new System.Drawing.Point(397, 418);
			this.btnClose.Name = "btnClose";
			this.btnClose.Size = new System.Drawing.Size(75, 23);
			this.btnClose.TabIndex = 3;
			this.btnClose.Text = "Close";
			this.btnClose.UseVisualStyleBackColor = true;
			this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
			// 
			// label2
			// 
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label2.AutoSize = true;
			this.label2.ForeColor = System.Drawing.Color.DimGray;
			this.label2.Location = new System.Drawing.Point(12, 242);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(212, 13);
			this.label2.TabIndex = 4;
			this.label2.Text = "Use context menu to add and remove filters";
			// 
			// lvFilters
			// 
			this.lvFilters.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.lvFilters.ContextMenuStrip = this.ctxFilters;
			this.lvFilters.FullRowSelect = true;
			this.lvFilters.Location = new System.Drawing.Point(12, 289);
			this.lvFilters.Name = "lvFilters";
			this.lvFilters.Size = new System.Drawing.Size(461, 123);
			this.lvFilters.SmallImageList = this.imageList;
			this.lvFilters.TabIndex = 5;
			this.lvFilters.UseCompatibleStateImageBehavior = false;
			this.lvFilters.View = System.Windows.Forms.View.List;
			// 
			// ctxFilters
			// 
			this.ctxFilters.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.removeFilterToolStripMenuItem});
			this.ctxFilters.Name = "ctxFilters";
			this.ctxFilters.Size = new System.Drawing.Size(140, 26);
			// 
			// removeFilterToolStripMenuItem
			// 
			this.removeFilterToolStripMenuItem.Name = "removeFilterToolStripMenuItem";
			this.removeFilterToolStripMenuItem.Size = new System.Drawing.Size(139, 22);
			this.removeFilterToolStripMenuItem.Text = "Remove Filter";
			this.removeFilterToolStripMenuItem.Click += new System.EventHandler(this.removeFilterToolStripMenuItem_Click);
			// 
			// scanWorker
			// 
			this.scanWorker.WorkerSupportsCancellation = true;
			this.scanWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.scanWorker_DoWork);
			this.scanWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.scanWorker_RunWorkerCompleted);
			// 
			// lbTotalSize
			// 
			this.lbTotalSize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.lbTotalSize.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
			this.lbTotalSize.Location = new System.Drawing.Point(324, 242);
			this.lbTotalSize.Name = "lbTotalSize";
			this.lbTotalSize.Size = new System.Drawing.Size(148, 18);
			this.lbTotalSize.TabIndex = 11;
			this.lbTotalSize.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(12, 11);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(101, 13);
			this.label3.TabIndex = 12;
			this.label3.Text = "Files to be included:";
			// 
			// tbSearch
			// 
			this.tbSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.tbSearch.Location = new System.Drawing.Point(292, 4);
			this.tbSearch.Name = "tbSearch";
			this.tbSearch.Size = new System.Drawing.Size(156, 20);
			this.tbSearch.TabIndex = 13;
			this.tbSearch.TextChanged += new System.EventHandler(this.tbSearch_TextChanged);
			this.tbSearch.Leave += new System.EventHandler(this.tbSearch_Leave);
			// 
			// btnSearch
			// 
			this.btnSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnSearch.Image = ((System.Drawing.Image)(resources.GetObject("btnSearch.Image")));
			this.btnSearch.Location = new System.Drawing.Point(451, 3);
			this.btnSearch.Name = "btnSearch";
			this.btnSearch.Size = new System.Drawing.Size(25, 25);
			this.btnSearch.TabIndex = 14;
			this.btnSearch.TabStop = false;
			this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
			// 
			// btnClearSearch
			// 
			this.btnClearSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnClearSearch.BackColor = System.Drawing.SystemColors.Window;
			this.btnClearSearch.Image = ((System.Drawing.Image)(resources.GetObject("btnClearSearch.Image")));
			this.btnClearSearch.Location = new System.Drawing.Point(429, 6);
			this.btnClearSearch.Name = "btnClearSearch";
			this.btnClearSearch.Size = new System.Drawing.Size(16, 16);
			this.btnClearSearch.TabIndex = 15;
			this.btnClearSearch.TabStop = false;
			this.btnClearSearch.Visible = false;
			this.btnClearSearch.Click += new System.EventHandler(this.btnClearSearch_Click);
			// 
			// lvFiles
			// 
			this.lvFiles.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.lvFiles.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4});
			this.lvFiles.ContextMenuStrip = this.ctxSelection;
			this.lvFiles.FullRowSelect = true;
			this.lvFiles.Location = new System.Drawing.Point(15, 30);
			this.lvFiles.Name = "lvFiles";
			this.lvFiles.Size = new System.Drawing.Size(462, 205);
			this.lvFiles.SmallImageList = this.imageList;
			this.lvFiles.TabIndex = 16;
			this.lvFiles.UseCompatibleStateImageBehavior = false;
			this.lvFiles.View = System.Windows.Forms.View.Details;
			this.lvFiles.VirtualMode = true;
			this.lvFiles.CacheVirtualItems += new System.Windows.Forms.CacheVirtualItemsEventHandler(this.lvFiles2_CacheVirtualItems);
			this.lvFiles.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.lvFiles2_ColumnClick);
			this.lvFiles.RetrieveVirtualItem += new System.Windows.Forms.RetrieveVirtualItemEventHandler(this.lvFiles2_RetrieveVirtualItem);
			// 
			// columnHeader1
			// 
			this.columnHeader1.Text = "Filename";
			this.columnHeader1.Width = 116;
			// 
			// columnHeader2
			// 
			this.columnHeader2.Text = "Ext";
			this.columnHeader2.Width = 47;
			// 
			// columnHeader3
			// 
			this.columnHeader3.Text = "Size";
			this.columnHeader3.Width = 76;
			// 
			// columnHeader4
			// 
			this.columnHeader4.Text = "Path";
			this.columnHeader4.Width = 192;
			// 
			// lbLoading
			// 
			this.lbLoading.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.lbLoading.BackColor = System.Drawing.SystemColors.Window;
			this.lbLoading.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lbLoading.Location = new System.Drawing.Point(28, 98);
			this.lbLoading.Name = "lbLoading";
			this.lbLoading.Size = new System.Drawing.Size(435, 36);
			this.lbLoading.TabIndex = 17;
			this.lbLoading.Text = "Scaning files, please wait....";
			this.lbLoading.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// dlgTestSearchSelection
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(486, 453);
			this.Controls.Add(this.lbLoading);
			this.Controls.Add(this.lvFiles);
			this.Controls.Add(this.btnClearSearch);
			this.Controls.Add(this.btnSearch);
			this.Controls.Add(this.tbSearch);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.lvFilters);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.lbTotalSize);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.btnClose);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "dlgTestSearchSelection";
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Test Search Selection";
			this.Load += new System.EventHandler(this.dlgTestSearchSelection_Load);
			this.ctxSelection.ResumeLayout(false);
			this.ctxFilters.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.btnSearch)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.btnClearSearch)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ContextMenuStrip ctxSelection;
		private System.Windows.Forms.ToolStripMenuItem miInclude;
		private System.Windows.Forms.ToolStripMenuItem miIncludeFileName;
		private System.Windows.Forms.ToolStripMenuItem miIncludeFileExt;
		private System.Windows.Forms.ToolStripMenuItem miExlude;
		private System.Windows.Forms.ToolStripMenuItem miExcludeFileName;
		private System.Windows.Forms.ToolStripMenuItem miExcludeFileExt;
		private System.Windows.Forms.Button btnClose;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ImageList imageList;
		private System.Windows.Forms.ListView lvFilters;
		private System.ComponentModel.BackgroundWorker scanWorker;
		private System.Windows.Forms.ContextMenuStrip ctxFilters;
		private System.Windows.Forms.ToolStripMenuItem removeFilterToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem miExludeFilePath;
		private System.Windows.Forms.Label lbTotalSize;
		private System.Windows.Forms.ToolStripMenuItem miIncludeFilePath;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox tbSearch;
		private System.Windows.Forms.PictureBox btnSearch;
		private System.Windows.Forms.PictureBox btnClearSearch;
		private System.Windows.Forms.ListView lvFiles;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.ColumnHeader columnHeader3;
		private System.Windows.Forms.ColumnHeader columnHeader4;
		private System.Windows.Forms.Label lbLoading;
	}
}