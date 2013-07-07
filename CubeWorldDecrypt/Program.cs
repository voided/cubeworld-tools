using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using CubeLib;

namespace CubeWorldDecrypt
{
    class Program
    {
        static void Main( string[] args )
        {
            if ( args.Length == 0 )
            {
                Console.WriteLine( "Usage: cubeworlddecrypt <file>" );
                return;
            }

            string fileName = args[ 0 ];
            string outputFile = fileName + ".out";

            if ( !File.Exists( fileName ) )
            {
                Console.WriteLine( "File doesn't exist" );
                return;
            }

            byte[] fileData = File.ReadAllBytes( args[ 0 ] );

            AssetTools.Descramble( fileData );

            File.WriteAllBytes( outputFile, fileData );
            Console.WriteLine( "Decrypted file written to {0}", outputFile );
        }


    }
}
