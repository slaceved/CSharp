#region copyright
//Contains software or other content adapted from Base32Codec.jar, (c) 2006 Bitzi Corporation
#endregion
#region BITZI license
//BITZI PUBLIC DOMAIN NOTICES
//UPDATED July 13, 2006
//When we publish our source code, we usually place it in the public domain, whenever possible, to allow the widest possible reuse and benefit.
//We try to include on most such released files a small notice such as:
//(PD) 2006 The Bitzi Corporation
//Please see file COPYING or http://bitzi.com/publicdomain for more info.
//For major standalone files, or as the "COPYING" file, we include a version of this longer explanation:
//
//(PD) 2006 The Bitzi Corporation
// 1. Authorship. This work and others bearing the above label were created by, or on behalf of, the Bitzi Corporation. 
// Often other public domain material by other authors is incorporated; this should be clear from notations in the source code. 
// If other non-public-domain code or libraries are included, this is done under those works' respective licenses.
//
// 2. Release. The Bitzi Corporation places its portion of these labeled works into the public domain, disclaiming all rights granted us by 
// copyright law. 
// Bitzi places no restrictions on your freedom to copy, use, redistribute and modify this work, though you should be aware of points (3), (4), 
// and (5) below.
//
// 3. Trademark Advisory. The Bitzi Corporation reserves all rights with regard to any of its trademarks which may appear herein, such as 
// "Bitzi", "Bitcollider", or "Bitpedia". Please take care that your uses of this work do not infringe on our trademarks or imply our endorsement. 
// For example, you should change labels and identifier strings in your derivative works where appropriate.
//
// 4. Licensed portions. Some code and libraries may be incorporated in this work in accordance with the licenses offered by their respective 
// rights holders. Further copying, use, redistribution and modification of these third-party portions remains subject to their original licenses.
//
// 5. Disclaimer. THIS SOFTWARE IS PROVIDED BY THE AUTHOR ``AS IS'' AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE 
// IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY 
// DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR 
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE 
// POSSIBILITY OF SUCH DAMAGE.
//
// Please see http://bitzi.com/publicdomain or write info@bitzi.com for more info.
//
// We hope you find our public-domain source code useful, but remember that we can provide absolutely no support or assurances about it; 
// your use is entirely at your own risk.
//Thank you.
//- Bitzi
//- March 3, 2001
//- updated July 13, 2006 (clarifying that sometimes our public-domain releases include portions that remain under original licenses) 
#endregion 

   using System;
   using System.Text;

    /// <summary>
    /// Base32Codec - encodes and decodes 'Canonical' Base32
    /// C# implementation of BITZI Base32Codec Java Lib
    /// </summary>
    internal class Base32Codec : BaseClass
    {
        private static readonly int[] Base32Lookup =
        {
            0xFF, 0xFF, 0x1A, 0x1B, 0x1C, 0x1D, 0x1E, 0x1F, // '0', '1', '2', '3', '4', '5', '6', '7'
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, // '8', '9', ':', ';', '<', '=', '>', '?'
            0xFF, 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, // '@', 'A', 'B', 'C', 'D', 'E', 'F', 'G'
            0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, // 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O'
            0x0F, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16, // 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W'
            0x17, 0x18, 0x19, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, // 'X', 'Y', 'Z', '[', '\', ']', '^', '_'
            0xFF, 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, // '`', 'a', 'b', 'c', 'd', 'e', 'f', 'g'
            0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, // 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o'
            0x0F, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16, // 'p', 'q', 'r', 's', 't', 'u', 'v', 'w'
            0x17, 0x18, 0x19, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF  // 'x', 'y', 'z', '{', '|', '}', '~', 'DEL'
        };

        private const string CBase32Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";

        /// <summary>
        ///  Encodes Byte Array as 'Canonical' Base32 string
        /// </summary>
        /// <param name="bytes">input bytes</param>
        /// <returns>encoded string</returns>
        public static string Encode(byte[] bytes)
        {
            int i = 0, index = 0, digit = 0;
            try
            {
                var base32 = new StringBuilder(((bytes.Length * 8) + 4) / 5);
                while (i < bytes.Length)
                {
                    int currByte = bytes[i] & 0xFF;
                    if (index > 3)
                    {
                        int nextByte;
                        if ((i + 1) < bytes.Length)
                            nextByte = bytes[i + 1] & 0xFF;
                        else
                            nextByte = 0;

                        //digit = currByte & (0xFF >>> index);
                        digit = currByte & (int)((uint)0xFF >> index);
                        index = (index + 5) % 8;
                        digit <<= index;
                        //digit |= nextByte >>> (8 - index);
                        digit |= (int)((uint)nextByte >> (8 - index));
                        i++;
                    }
                    else
                    {
                        //digit= (currByte >>> (8 - (index + 5))) & 0x1F;
                        digit = (int)(((uint)currByte >> (8 - (index + 5))) & 0x1F);
                        index = (index + 5)%8;
                        if (index == 0)
                            i++;
                    }
                    base32.Append(CBase32Chars[digit]);
                }
                return base32.ToString();
            }
            catch (OverflowException oex) { throw new ApplicationException(string.Format("Overflow exception occurred while encoding Base32 string. index={0} digit={1}", index, digit), oex); }
            catch (Exception ex) { throw new ApplicationException("Fatal error occurred while .", ex); }
        }

        /// <summary>
        /// Decodes 'Canonical' Base32 string back to Byte Array
        /// </summary>
        /// <param name="base32">string input</param>
        /// <returns>decoded Byte Array</returns>
        public static byte[] Decode(string base32)
        {
            int index = 0, offset = 0, digit = 0;
            try
            {
                switch (base32.Length % 8)
                {
                    case 1: //  5 bits in sub-block:  0 useful bits but 5 discarded
                    case 3: // 15 bits in sub-block:  8 useful bits but 7 discarded
                    case 6: // 30 bits in sub-block: 24 useful bits but 6 discarded
                        throw new ArgumentException("Non-canonical Base32 string length.", "base32");
                }

                var bytes = new byte[base32.Length * 5 / 8];
                int i;

                for (i = 0, index = 0, offset = 0; i < base32.Length; i++)
                {
                    int lookup = base32[i] - '0';
                    if (lookup < 0 || lookup >= Base32Lookup.Length)
                    {
                        throw new ArgumentException(
                            "Invalid character in Base32 string. Make sure the character is inside the lookup table.","base32");
                    }

                    digit = Base32Lookup[lookup];
                    if (digit == 0xFF)
                    {
                        throw new ArgumentException(
                            "Invalid character in Base32 string. Make sure the character is valid.", "base32");
                    }

                    int tmp;
                    byte[] tmpArray;
                    if (index <= 3)
                    {
                        index = (index + 5) % 8;
                        if (index == 0)
                        {
                            bytes[offset] |= (byte)digit;
                            offset++;
                            if (offset >= bytes.Length)
                                break;
                        }
                        else
                        {   
                            tmp = (digit << (8 - index));
                            tmpArray = BitConverter.GetBytes(tmp);
                            bytes[offset] |= tmpArray[0]; //bytes[offset] |=(byte)(digit << (8 - index))
                        }
                    }
                    else
                    {
                        index = (index + 5) % 8;
                        
                        tmp = (int)((uint)digit >> index);
                        tmpArray = BitConverter.GetBytes(tmp);
                        bytes[offset] |= tmpArray[0]; //bytes[offset] |= (byte)(digit >>> index);
                        offset++;
                        if (offset >= bytes.Length)
                        {
                            if ((digit & ~(0xFFFFFFFF << index)) != 0)
                            {
                                throw new ArgumentException("Non-canonical bits at the end of Base32 string.", "base32");
                            }
                            break;
                        }
                        tmp = (digit << (8 - index));
                        tmpArray = BitConverter.GetBytes(tmp);
                        bytes[offset] |= tmpArray[0];  //bytes[offset] |= (digit << (8 - index));
                    }
                }
                return bytes;
            }
            catch (OverflowException oex) { throw new ApplicationException(string.Format("Overflow exception occurred while decoding {0} Base32 string. index={1} offset={2} digit={3}", base32, index, offset, digit ), oex); }
            catch (Exception ex) { throw new ApplicationException("Fatal error occurred while .", ex); }
        }
    }