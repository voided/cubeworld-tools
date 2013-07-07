using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using System.IO;

namespace CubeLib
{
    /// <summary>
    /// Represents information about a
    /// </summary>
    public class Asset
    {
        public string FileName { get; set; }
        public int Size { get; set; }

        internal AssetDatabase parentDb;
    }

    /// <summary>
    /// Represents an SQLite asset database which contains the assets of cubeworld.
    /// </summary>
    public class AssetDatabase : IDisposable
    {
        SQLiteConnection connection;


        public AssetDatabase( string path )
        {
            string connString = string.Format( "Data Source={0};", path );

            connection = new SQLiteConnection( connString );
            connection.Open();
        }


        public Asset[] GetAssets()
        {
            var fileList = new List<Asset>();

            using ( var cmd = connection.CreateCommand() )
            {
                cmd.CommandText = "SELECT `key`, LENGTH( `value` ) FROM `blobs`";

                using ( var dataReader = cmd.ExecuteReader() )
                {
                    while ( dataReader.Read() )
                    {
                        var asset = new Asset
                        {
                            parentDb = this,

                            FileName = dataReader.GetString( 0 ),
                            Size = dataReader.GetInt32( 1 ),
                        };

                        fileList.Add( asset );
                    }
                }
            }

            return fileList.ToArray();
        }


        public byte[] GetAssetData( Asset asset )
        {
            if ( asset == null )
                throw new ArgumentNullException( "asset" );

            if ( asset.parentDb != this )
                throw new ArgumentException( "Asset does not belong to this database" );

            using ( var cmd = connection.CreateCommand() )
            {
                cmd.CommandText = "SELECT `value` FROM `blobs` WHERE `key` = @keyName";
                cmd.Parameters.AddWithValue( "@keyName", asset.FileName );

                byte[] valueData = ( byte[] )cmd.ExecuteScalar();

                AssetTools.Descramble( valueData );

                return valueData;
            }
        }

        public void DeleteAsset( Asset asset )
        {
            if ( asset == null )
                throw new ArgumentNullException( "asset" );

            if ( asset.parentDb != this )
                throw new ArgumentException( "Asset does not belong to this database" );

            using ( var cmd = connection.CreateCommand() )
            {
                cmd.CommandText = "DELETE FROM `blobs` WHERE `key` = @keyName";
                cmd.Parameters.AddWithValue( "@keyName", asset.FileName );

                int affectedRows = cmd.ExecuteNonQuery();
                if ( affectedRows != 1 )
                {
                    throw new InvalidOperationException( string.Format( "Unable to delete asset \"{0}\"", asset.FileName ) );
                }
            }
        }
        public Asset InsertAsset( string fileName, byte[] data, bool replace = true )
        {
            if ( string.IsNullOrWhiteSpace( fileName ) )
                throw new ArgumentNullException( "fileName" );

            if ( data == null )
                throw new ArgumentNullException( "data" );

            // scramble the data so cubeworld can unscramble it when loading
            AssetTools.Scramble( data );

            using ( var cmd = connection.CreateCommand() )
            {
                cmd.CommandText = "INSERT " + ( replace ? "OR REPLACE " : "" ) + "INTO `blobs` ( `key`, `value` ) VALUES ( @keyName, @data )";
                cmd.Parameters.AddWithValue( "@keyName", fileName );
                cmd.Parameters.AddWithValue( "@data", data );

                int affectedRows = cmd.ExecuteNonQuery();

                if ( affectedRows != 1 )
                {
                    throw new InvalidOperationException( string.Format( "Unable to insert asset \"{0}\"", fileName ) );
                }

                return new Asset
                {
                    parentDb = this,

                    FileName = fileName,
                    Size = data.Length,
                };
            }
        }

        public void Dispose()
        {
            connection.Dispose();
        }

    }
}
