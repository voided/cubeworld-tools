using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CubeLib
{
    /// <summary>
    /// Various asset utilities.
    /// </summary>
    public static class AssetTools
    {
        static int[] offsetLookupTable = 
        {
            0x1092, 0x254F, 0x348, 0x14B40, 0x241A, 0x2676, 0x7F, 0x9, 0x250B,
            0x18A, 0x7B, 0x12E2, 0x7EBC, 0x5F23, 0x981, 0x11, 0x85BA, 0x0A566,
            0x1093, 0x0E, 0x2D266, 0x7C3, 0x0C16, 0x76D, 0x15D41, 0x12CD,
            0x25, 0x8F, 0x0DA2, 0x4C1B, 0x53F, 0x1B0, 0x14AFC, 0x23E0, 0x258C,
            0x4D1, 0x0D6A, 0x72F, 0x0BA8, 0x7C9, 0x0BA8, 0x131F, 0x0C75C7, 0x0D
        };

        /// <summary>
        /// Performs an inplace data scrambling on given data, in order to prepare it for usage as a Cube World asset.
        /// </summary>
        /// <param name="inputData">The input data.</param>
        public static void Scramble( byte[] inputData )
        {
            for ( int i = 0 ; i < inputData.Length ; i++ )
            {
                inputData[ i ] = ( byte )( -1 - inputData[ i ] );
            }

            for ( int currOff = 0 ; currOff < inputData.Length ; currOff++ )
            {
                int offset = ( currOff + offsetLookupTable[ currOff % 44 ] ) % inputData.Length;

                byte temp = inputData[ currOff ];
                inputData[ currOff ] = inputData[ offset ];
                inputData[ offset ] = temp;
            }
        }

        /// <summary>
        /// Performs an inplace data descrambling on the given data.
        /// </summary>
        /// <param name="inputData">The input data.</param>
        public static void Descramble( byte[] inputData )
        {
            for ( int currOff = inputData.Length - 1 ; currOff >= 0 ; currOff-- )
            {
                int offset = ( currOff + offsetLookupTable[ currOff % 44 ] ) % inputData.Length;

                byte temp = inputData[ currOff ];
                inputData[ currOff ] = inputData[ offset ];
                inputData[ offset ] = temp;
            }

            for ( int i = 0 ; i < inputData.Length ; i++ )
            {
                inputData[ i ] = ( byte )( -1 - inputData[ i ] );
            }
        }
    }
}
