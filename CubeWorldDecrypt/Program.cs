using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

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

            // inplace descramble
            Descramble( fileData );

            File.WriteAllBytes( outputFile, fileData );
            Console.WriteLine( "Decrypted file written to {0}", outputFile );
        }

        static int[] offsetLookupTable = 
        {
            0x1092, 0x254F, 0x348, 0x14B40, 0x241A, 0x2676, 0x7F, 0x9, 0x250B,
            0x18A, 0x7B, 0x12E2, 0x7EBC, 0x5F23, 0x981, 0x11, 0x85BA, 0x0A566,
            0x1093, 0x0E, 0x2D266, 0x7C3, 0x0C16, 0x76D, 0x15D41, 0x12CD,
            0x25, 0x8F, 0x0DA2, 0x4C1B, 0x53F, 0x1B0, 0x14AFC, 0x23E0, 0x258C,
            0x4D1, 0x0D6A, 0x72F, 0x0BA8, 0x7C9, 0x0BA8, 0x131F, 0x0C75C7, 0x0D
        };

        static void Descramble( byte[] input )
        {
            for ( int currOff = input.Length - 1 ; currOff >= 0 ; currOff-- )
            {
                int offset = ( currOff + offsetLookupTable[ currOff % 44 ] ) % input.Length;

                byte temp = input[ currOff ];
                input[ currOff ] = input[ offset ];
                input[ offset ] = temp;
            }

            for ( int i = 0 ; i < input.Length ; i++ )
            {
                input[ i ] = ( byte )( -1 - input[ i ] );
            }
        }

        static void Scramble( byte[] input )
        {
            for ( int i = 0 ; i < input.Length ; i++ )
            {
                input[ i ] = ( byte )( -1 - input[ i ] );
            }

            for ( int currOff = 0 ; currOff < input.Length ; currOff++ )
            {
                int offset = ( currOff + offsetLookupTable[ currOff % 44 ] ) % input.Length;

                byte temp = input[ currOff ];
                input[ currOff ] = input[ offset ];
                input[ offset ] = temp;
            }
        }

        /*
            int __thiscall DecryptValueBlob(int *valueBlob)
            {
                signed int len; // esi@1
                signed int v2; // ebx@2
                int valueBlob2; // edi@3
                unsigned int v4; // edx@3
                bool dataProcessed; // sf@3
                char v6; // bl@3
                int result; // eax@4
                int i; // esi@4
                int v9; // edx@5
                signed int len2; // [sp+4h] [bp-4h]@1

                len = valueBlob[1] - *valueBlob - 1;
                len2 = len;
                if ( len >= 0 )
                {
                    v2 = 44;
                    do
                    {
                        valueBlob2 = *valueBlob;
                        v4 = (len + cryptTable[len % v2]) % (unsigned int)(valueBlob[1] - *valueBlob);
                        len = len2 - 1;
                        dataProcessed = len2 - 1 < 0;
                        v6 = *(_BYTE *)(*valueBlob + len2 - 1 + 1);
                        len2 = len;
                        *(_BYTE *)(*valueBlob + len + 1) = *(_BYTE *)(v4 + *valueBlob);
                        *(_BYTE *)(v4 + valueBlob2) = v6;
                        v2 = 44;
                    }
                    while ( !dataProcessed );
                }
                result = valueBlob[1] - *valueBlob;
                for ( i = 0; i < result; result = valueBlob[1] - *valueBlob )
                {
                    v9 = *valueBlob + i++;
                    *(_BYTE *)v9 = -1 - *(_BYTE *)v9;
                }
                return result;
            }
        */
    }
}
