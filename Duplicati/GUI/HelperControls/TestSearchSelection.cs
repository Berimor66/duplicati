﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Duplicati.Library.Utility;
using System.Collections;

namespace Duplicati.GUI.HelperControls {
    public partial class TestSearchSelection : Form {

        private class AbortException : Exception { }
        
        private class ResultItem {
            public string FileName;
            public string Ext;
            public string Path;
            public long Size;

            public ResultItem(string path)
            {
                this.FileName = System.IO.Path.GetFileNameWithoutExtension(path);
                this.Path = System.IO.Path.GetDirectoryName(path);
                this.Size = new FileInfo(path).Length;
                this.Ext = System.IO.Path.GetExtension(path);
                if (!string.IsNullOrEmpty(this.Ext) && this.Ext.StartsWith("."))
                    this.Ext = this.Ext.Substring(1);
            }

            public override int GetHashCode() {
                return FileName.GetHashCode() + Ext.GetHashCode() + Path.GetHashCode() + (int)Size;
            }
        }

        enum ResultItemSortBy { FileName=0, Ext=1, Size=2, Path=3 }

        class ResultItemComparer : IComparer<ResultItem> {
            private ResultItemSortBy _sortBy;
            private bool _ascending = false;

            public ResultItemComparer() {
                _sortBy = ResultItemSortBy.Size;
            }
            public ResultItemComparer(ResultItemSortBy sortBy, bool ascending) {
                _sortBy = sortBy;
                //_col = column;
                _ascending = ascending;
            }

            #region IComparer<ListViewItem> Members

            private string GetAttrib(ResultItem itm, ResultItemSortBy prm) {
                if( prm == ResultItemSortBy.FileName )
                    return itm.FileName;
                else if( prm == ResultItemSortBy.Ext )
                    return itm.Ext;
                else if( prm == ResultItemSortBy.Path )
                    return itm.Path;

                else return string.Empty;
            }

            public int Compare(ResultItem a, ResultItem b) {

                switch( _sortBy ) {
                    case ResultItemSortBy.FileName:
                    case ResultItemSortBy.Ext:
                    case ResultItemSortBy.Path:
                        string strA = GetAttrib(_ascending ? a : b, _sortBy);
                        string strB = GetAttrib(_ascending ? b : a, _sortBy);
                        
                        return String.Compare(strA, strB);

                    case ResultItemSortBy.Size:
                        long iA = (_ascending ? a : b).Size;
                        long iB = (_ascending ? b : a).Size;
                        
                        return Convert.ToInt32(iA - iB);
                }

                return 0;
            }

            #endregion
        }

        const int PAGE_SIZE = 800;
        const int SORT_ASC_IMAGEINDEX = 2;

        int currentSortingColumn = 0;
        bool sortingOrderAsc = true;

        private string[] m_paths;
        private List<KeyValuePair<bool, string>> m_filters;
        private string m_dynamicFilters;

        public string[] Paths { get { return m_paths; } set { m_paths = value; } }
        public List<KeyValuePair<bool, string>> Filters { get { return m_filters; } set { m_filters = value; } }
        public string DynamicFilters { get { return m_dynamicFilters; } set { m_dynamicFilters = value; } }

        List<ResultItem> _results = new List<ResultItem>();
        List<ResultItem> _subResults = null;
        bool _updating = false;

        private ListViewItem[] _cache;

        public TestSearchSelection() {
            InitializeComponent();
        }

        private void dlgTestSearchSelection_Load(object sender, EventArgs e) {

            currentSortingColumn = 2;
            lvFiles.Columns[currentSortingColumn].ImageIndex = SORT_ASC_IMAGEINDEX + ( sortingOrderAsc ? 0 : 1 );

            foreach (KeyValuePair<bool, string> f in Filters)
                lvFilters.Items.Add(new ListViewItem(f.Value, f.Key ? 0 : 1));

            StartScanThread();
        }

        ListViewItem _CreateListViewItem(ResultItem ri) {
            var lvi = new ListViewItem(ri.FileName);		
            lvi.SubItems.Add(ri.Ext);
            lvi.SubItems.Add(Utility.FormatSizeString(ri.Size)).Tag = ri.Size;
            lvi.SubItems.Add(ri.Path);
            lvi.Tag = ri.GetHashCode();

            return lvi;
        }


        void UpdateTotalStat(List<ResultItem> items) {
            long sum = 0;

            foreach (ResultItem r in items)
                sum += r.Size;

            lbTotalSize.Text = string.Format(Strings.TestSearchSelection.TotalSizeLabel, items.Count, Utility.FormatSizeString(sum));		
        }

        void LoadFileView() {
            lvFiles.VirtualListSize = _results.Count;
            _cache = new ListViewItem[_results.Count];

            UpdateTotalStat(_results);

            ResizeColumns();
        }

        private void UpdateFileView() {
            if( _updating )
                return;

            _updating = true;
            try {
                string str = tbSearch.Text;

                lvFiles.BeginUpdate();
                try {
                    ClearFileCache();
                    
                    if( !string.IsNullOrEmpty(str) ) {
                        _subResults = new List<ResultItem>();
                        foreach (ResultItem r in _results)
                            if (r.FileName.Contains(str) || r.Ext.Contains(str) || r.Path.Contains(str))
                                _subResults.Add(r);

                        _cache = new ListViewItem[_subResults.Count];
                        lvFiles.VirtualListSize = _subResults.Count;

                        UpdateTotalStat(_subResults);
                        
                        if( !btnClearSearch.Visible )
                            btnClearSearch.Visible = true;

                    } else {
                        _subResults = null;
                        lvFiles.VirtualListSize = _results.Count;
                        
                        btnClearSearch.Visible = false;

                        UpdateTotalStat(_results);
                    }
                } finally {
                    lvFiles.EndUpdate();
                }

            } finally {
                _updating = false;
            }
        }

        private void DoTestSearchSelection() {

            List<KeyValuePair<bool, string>> dyn_filters = new List<KeyValuePair<bool, string>>(Filters);
            if (!string.IsNullOrEmpty(DynamicFilters))
                dyn_filters.AddRange(Library.Utility.FilenameFilter.DecodeFilter(DynamicFilters));
            
            FilenameFilter fn = new FilenameFilter(dyn_filters);

            try
            {
                foreach (string scanDir in Paths)
                {
                    if (!scanWorker.CancellationPending)
                        Library.Utility.Utility.EnumerateFileSystemEntries(scanDir, fn, AddFileToList_Callback);
                }
            }
            catch (AbortException)
            {
            }
        }

        void AddFileToList_Callback(string rootpath, string path, Duplicati.Library.Utility.Utility.EnumeratedFileStatus status)
        {

            if (scanWorker.CancellationPending)
                throw new AbortException();

            if (status == Utility.EnumeratedFileStatus.File)
            {
                _results.Add(new ResultItem(path));
            }
            else if (status == Utility.EnumeratedFileStatus.Folder)
            {
                //Should be displayed in tree so the user can exclude with right click?
            }
        }


        // Update Search Selection upon changed Filter

        //TODO: Does not work, because the folder that contains a file could be excluded,
        // causing files to be excluded even though they match no filter...
        //
        //This should probably not be used, because it can accidentially show wrong
        // results if some subtle aspect of the filter process is not done 100% correct
        //

        /*private void UpdateSearchSelection()
        {
            FilenameFilter fn = new FilenameFilter(Filters);

            List<ResultItem> bin = new List<ResultItem>();

            foreach( ResultItem ri in _results ) {
                string fileName = string.Format("{0}.{1}", ri.FileName, ri.Ext);

                if( !fn.ShouldInclude(ri.Path, Path.Combine(ri.Path, fileName) ) )
                    bin.Add(ri);
            }

            foreach( ResultItem ri in bin )
                _results.Remove(ri);
        }*/

        private void ResizeColumns() {
            //lvFiles.AutoResizeColumn(1, ColumnHeaderAutoResizeStyle.ColumnContent);
            //lvFiles.AutoResizeColumn(2, ColumnHeaderAutoResizeStyle.ColumnContent);
            lvFiles.AutoResizeColumn(3, ColumnHeaderAutoResizeStyle.ColumnContent);
        }

        private ResultItem GetResultItem(int index) {
            if( _subResults == null ) {
                if( index < _results.Count )
                    return _results[index];

            } else if( index < _subResults.Count )
                return _subResults[index];

            return null;
        }
        private List<ResultItem> GetResultList() {
            if( _subResults == null )
                return _results;
            else return _subResults;
        }
        private void ClearFileCache() {
            for( int i = 0;i < _cache.Length;i++ )
                _cache[i] = null;
        }
        private void SortView(ResultItemSortBy sortBy, bool sortDirectionAsc) {
            ClearFileCache();

            GetResultList().Sort(new ResultItemComparer(sortBy, sortDirectionAsc));

            lvFiles.Invalidate();
        }

        bool ExtractLastDirectory(StringBuilder path, StringBuilder dest) {
            if( path.Length == 0 )
                return false;

            int i = path.Length;

            while( --i > -1 ) {
                if( path[i] == '\\' ) {
                    if( i != path.Length - 1 && i > 3 )
                        break;
                    else continue;
                }

                dest.Insert(0, path[i]);
            }
            i = i > -1 ? i : 0;

            path.Remove(i, path.Length - i);

            return true;
        }
        void AddFilter(bool include, string filter) {
            Filters.Add(new KeyValuePair<bool, string>(include, filter));
            lvFilters.Items.Add(filter, include ? 0 : 1);
        }
        string GetFileFilterStr(string filename, string ext) {
            return string.Format(@".*\\{0}.{1}", filename, ext);
        }
        void _AddSelectedFilter(bool include, bool anyFileName) {
            if( lvFiles.SelectedIndices.Count > 0 ) {
                foreach( int index in lvFiles.SelectedIndices ) {
                    ResultItem itm = GetResultItem(index);
                    string filter = GetFileFilterStr(!anyFileName ? itm.FileName : "*", itm.Ext);

                    AddFilter(include, filter);
                }

                StartScanThread();
            }
        }
        
        // Exclude FileName
        private void addFilenameFilterToolStripMenuItem_Click(object sender, EventArgs e) {
            _AddSelectedFilter(false, false);
            
        }

        // Exclude File Ext
        private void addFileExtensionFilterToolStripMenuItem1_Click(object sender, EventArgs e) {
            _AddSelectedFilter(false, true);			
        }

        // Exlude File Path
        private void ctxSelection_Opened(object sender, EventArgs e) {
            if( lvFiles.SelectedIndices.Count > 0 ) {
                ResultItem itm = GetResultItem(lvFiles.SelectedIndices[0]);
                // Set Include names
                string fileName = string.Format(Strings.TestSearchSelection.FileName, GetFileFilterStr(itm.FileName, itm.Ext));
                string fileExt = string.Format(Strings.TestSearchSelection.FileType, GetFileFilterStr("*", itm.Ext));
                miIncludeFileName.Text = fileName;
                miIncludeFileExt.Text = fileExt;
                miExcludeFileName.Text = fileName;
                miExcludeFileExt.Text = fileExt; 
                
                // Add Path items
                StringBuilder completePath = new StringBuilder(itm.Path);
                StringBuilder path = new StringBuilder(completePath.Length );
                miExludeFilePath.DropDownItems.Clear();

                while( ExtractLastDirectory(completePath, path) ) {
                    string s = ( completePath.Length > 0 ) ? string.Format(@".*\\{0}\\", path) : path.ToString();
                    
                    miIncludeFilePath.DropDownItems.Add(s, null, FilterFilePath_Click).Tag = true;
                    miExludeFilePath.DropDownItems.Add(s, null, FilterFilePath_Click).Tag = false;

                    path.Insert(0, @"\\");
                }

            }
        }

        // Include fileName
        private void miIncludeFileName_Click(object sender, EventArgs e) {
            _AddSelectedFilter(true, false);
        }
        
        // Include File Ext
        private void miIncludeFileExt_Click(object sender, EventArgs e) {
            _AddSelectedFilter(true, true);
        }


        void StartScanThread() {
            lbLoading.Visible = true;
            tbSearch.Enabled = false;
            lvFiles.Enabled = false;
            lvFilters.Enabled = false;
            scanWorker.RunWorkerAsync();
        }
        private void scanWorker_DoWork(object sender, DoWorkEventArgs e) {
            //if( _results.Count == 0 )
                DoTestSearchSelection();
            //else UpdateSearchSelection();

            if (scanWorker.CancellationPending)
                e.Cancel = true;
        }
        private void scanWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            if (e.Error == null && !e.Cancelled)
            {
                lbLoading.Visible = false;
                tbSearch.Enabled = true;
                lvFiles.Enabled = true;
                lvFilters.Enabled = true;

                LoadFileView();
                SortView((ResultItemSortBy)currentSortingColumn, true);
            }
            else
            {
                //Should report the error to the user somehow
            }
        }

        private void FilterFilePath_Click(object sender, EventArgs e) {
            ToolStripItem itm = sender as ToolStripItem;
            AddFilter((bool)itm.Tag, itm.Text);

            StartScanThread();
        }
        private void lvFiles_ColumnClick(object sender, ColumnClickEventArgs e) {
            if( currentSortingColumn != e.Column )
                sortingOrderAsc = true;
            else sortingOrderAsc = !sortingOrderAsc;

            if( currentSortingColumn != -1 ) {
                lvFiles.Columns[currentSortingColumn].ImageIndex = -1;
                lvFiles.Columns[currentSortingColumn].TextAlign = HorizontalAlignment.Left;
            }

            lvFiles.Columns[e.Column].ImageIndex = SORT_ASC_IMAGEINDEX + ( sortingOrderAsc ? 0 : 1 );

            SortView((ResultItemSortBy)e.Column, sortingOrderAsc);

            currentSortingColumn = e.Column;
        }

        private void removeFilterToolStripMenuItem_Click(object sender, EventArgs e) {
            if( lvFilters.SelectedItems.Count > 0 ) {

                for(int i = lvFilters.SelectedItems.Count-1; i > -1; i--) {
                    Filters.RemoveAt(lvFilters.SelectedItems[i].Index);
                    lvFilters.Items.RemoveAt(lvFilters.SelectedItems[i].Index);
                }

                StartScanThread();
            }
        }
        private void addFilePathFilerToolStripMenuItem_Click(object sender, EventArgs e) {

        }
        private void btnClose_Click(object sender, EventArgs e) {
            scanWorker.CancelAsync();
            Close();
        }

        private void lvFiles_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e) {

            ListViewItem itm = e.ItemIndex < _cache.Length ? _cache[e.ItemIndex] : null;
            if( itm == null ) {
                ResultItem ri = GetResultItem(e.ItemIndex);
                if( ri != null )
                    itm = _CreateListViewItem(ri);
            }
            e.Item = itm;
        }
        private void lvFiles_CacheVirtualItems(object sender, CacheVirtualItemsEventArgs e) {

            if( _cache.Length > 0 ) {
                
                int length = e.EndIndex - e.StartIndex + 1;

                for( int i = e.StartIndex;i < length;i++ ) {
                    if( _cache[i] == null )
                        _cache[i] = _CreateListViewItem(GetResultItem(i));
                }
            }
        }
        private void tbSearch_TextChanged(object sender, EventArgs e) {
            UpdateFileView();
        }
        private void btnClearSearch_Click(object sender, EventArgs e) {
            tbSearch.Text = string.Empty;
            _subResults = null;
            LoadFileView();
            btnClearSearch.Visible = false;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }



    }
}
