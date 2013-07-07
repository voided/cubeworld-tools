using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using CubeLib;

namespace AssetBrowser
{
    public partial class MainForm : Form
    {
        AssetDatabase currentDb;


        public MainForm()
        {
            InitializeComponent();
        }


        void PopulateAssetRoot( DataFile file )
        {
            if ( currentDb != null )
            {
                currentDb.Dispose();
            }

            assetListView.BeginUpdate();

            assetListView.Items.Clear();

            currentDb = new AssetDatabase( file.FilePath );

            foreach ( var asset in currentDb.GetAssets() )
            {
                var lvi = new ListViewItem
                {
                    Text = asset.FileName,
                };
                lvi.Tag = asset;

                lvi.SubItems.Add( BytesToString( asset.Size ) );

                assetListView.Items.Add( lvi );
            }

            assetListView.EndUpdate();
        }


        private void openToolStripMenuItem_Click( object sender, EventArgs e )
        {
            var guessedPath = Path.Combine( Environment.GetFolderPath( Environment.SpecialFolder.ProgramFilesX86 ), "Cube World" );

            FolderBrowserDialog fbd = new FolderBrowserDialog
            {
                Description = "Select Cube World install directory",

                ShowNewFolderButton = false,
                SelectedPath = guessedPath,
            };

            var result = fbd.ShowDialog( this );

            if ( result != DialogResult.OK )
                return;

            var path = fbd.SelectedPath;

            if ( !File.Exists( Path.Combine( path, "cube.exe" ) ) )
            {
                MessageBox.Show( this, "You have selected an invalid directory", "Invalid directory", MessageBoxButtons.OK, MessageBoxIcon.Error );
                return;
            }

            var dirInfo = new DirectoryInfo( path );
            foreach ( var fileInfo in dirInfo.EnumerateFiles( "*.db", SearchOption.TopDirectoryOnly ) )
            {
                var dataFile = new DataFile
                {
                    FilePath = fileInfo.FullName,
                };

                dataFileListBox.Items.Add( dataFile );
            }

            // display our controls
            panel1.Visible = true;
        }

        private void exitToolStripMenuItem_Click( object sender, EventArgs e )
        {
            this.Close();
        }

        private void dataFileListBox_SelectedIndexChanged( object sender, EventArgs e )
        {
            var file = dataFileListBox.SelectedItem as DataFile;

            if ( file == null )
                return;

            PopulateAssetRoot( file );
        }

        private void contextMenuStrip1_Opening( object sender, CancelEventArgs e )
        {
            if ( assetListView.SelectedItems.Count == 0 )
            {
                e.Cancel = true;
                return;
            }

            var selectedItem = assetListView.SelectedItems[ 0 ];

            fileNameMenuItem.Text = string.Format( "[{0}]", selectedItem.Text );
        }

        private void extractToolStripMenuItem_Click( object sender, EventArgs e )
        {
            var selectedItem = assetListView.SelectedItems[ 0 ];

            var sfd = new SaveFileDialog
            {
                Title = string.Format( "Extract {0}...", selectedItem.Text ),

                CheckPathExists = true,
                FileName = selectedItem.Text,
                
                Filter = "All Files (*.*)|*.*",
            };

            var result = sfd.ShowDialog( this );

            if ( result != DialogResult.OK )
                return;

            byte[] fileData = currentDb.GetAssetData( selectedItem.Tag as Asset );
            File.WriteAllBytes( sfd.FileName, fileData );
        }

        static String BytesToString( long byteCount )
        {
            string[] suf = { "B", "KB", "MB", "GB", "TB", "PB", "EB" }; //Longs run out around EB
            if ( byteCount == 0 )
                return "0 " + suf[ 0 ];
            long bytes = Math.Abs( byteCount );
            int place = Convert.ToInt32( Math.Floor( Math.Log( bytes, 1024 ) ) );
            double num = Math.Round( bytes / Math.Pow( 1024, place ), 1 );
            return string.Format( "{0} {1}", Math.Sign( byteCount ) * num, suf[ place ] );
        }
    }

    class DataFile
    {
        public string FilePath { get; set; }

        public override string ToString()
        {
            return Path.GetFileNameWithoutExtension( FilePath );
        }
    }
}
