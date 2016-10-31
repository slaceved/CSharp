	using System;
	using System.Security.Cryptography;
	using System.Text;

	/// <summary>
	/// Random Alpha-Numeric Password Generator
	/// </summary>
	public class PasswordGenerator
	{
		private const int CDefaultMinimum = 6;
		private const int CDefaultMaximum = 10;
		private const int CUBoundDigit    = 61;
		private readonly RNGCryptoServiceProvider    rng;
		private int 			minSize;
		private int 			maxSize;

	    private readonly char[] pwdCharArray = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789`~!@#$%^&*()-_=+[]{}\\|;:'\",<.>/?".ToCharArray();        
		
		/// <summary>
		/// Password Generator by default does not allow Consecutive or Repeat characters, Excludes Symbols and zero(0), capital O, one(1), and lower case l
		/// To over ride the default exclusion set set the Exclusions property to null or string.Empty
		/// </summary>
		public PasswordGenerator() {
			this.Minimum 					= CDefaultMinimum;
			this.Maximum 					= CDefaultMaximum;
			this.ConsecutiveCharacters 		= false;
			this.RepeatCharacters 			= false;
			this.ExcludeSymbols             = true;
            this.Exclusions = "0O1l`~!@#$%^&*()-_=+[]{}\\|;:'\",<.>/?";

			rng = new RNGCryptoServiceProvider();
		}		
		/// <summary>
		/// This function generates a Random Number in between an upper and lower bound numbers
		/// </summary>
		/// <param name="lBound">Lower Bound</param>
		/// <param name="uBound">Upper Bound</param>
		/// <returns>A Randomly generated number</returns>
		protected int GetCryptographicRandomNumber(int lBound, int uBound){   
			// Assumes lBound >= 0 && lBound < uBound returns an int >= lBound and < uBound
			uint urndnum;   
			var rndnum = new Byte[4];   
			if (lBound == uBound-1) {
				// test for degenerate case where only lBound can be returned   
				return lBound;
			}
                                                              
			uint xcludeRndBase = (uint.MaxValue - (uint.MaxValue%(uint)(uBound-lBound)));   
			do {      
				rng.GetBytes(rndnum);      
				urndnum = BitConverter.ToUInt32(rndnum,0);      
			} while (urndnum >= xcludeRndBase);   
            
			return (int)(urndnum % (uBound-lBound)) + lBound;
		}

        /// <summary>
        /// This function generates random characters which makes up the autogenerated password
        /// </summary>
        /// <returns>Returns a random character</returns>
		protected char GetRandomCharacter() {            
			int upperBound = pwdCharArray.GetUpperBound(0);

			if ( this.ExcludeSymbols ){
				upperBound = CUBoundDigit;
			}

			int randomCharPosition = GetCryptographicRandomNumber(pwdCharArray.GetLowerBound(0), upperBound);
			char randomChar = pwdCharArray[randomCharPosition];

			return randomChar;
		}

        /// <summary>
        /// This function implements a logic to generate a random set of numbers and charactes which is used as
        /// a password when a new user is created
        /// </summary>
        /// <returns>Random set of numbers and characters</returns>
		public string Generate() {
			// Pick random length between minimum and maximum   
			int pwdLength = GetCryptographicRandomNumber(this.Minimum, this.Maximum);

            var pwdBuffer = new StringBuilder { Capacity = this.Maximum };

            char lastCharacter = '\n';

			for ( int i = 0; i < pwdLength; i++ ){
				char nextCharacter = GetRandomCharacter();	// Generate random characters

				if ( false == this.ConsecutiveCharacters ) {
					while ( lastCharacter == nextCharacter ) {
						nextCharacter = GetRandomCharacter();
					}
				}

				if ( false == this.RepeatCharacters ) {
					string temp = pwdBuffer.ToString();
					int duplicateIndex = temp.IndexOf(nextCharacter);
					while ( -1 != duplicateIndex ) {
						nextCharacter = GetRandomCharacter();
						duplicateIndex = temp.IndexOf(nextCharacter);
					}
				}

				if ( null != this.Exclusions ) {
					while ( -1 != this.Exclusions.IndexOf(nextCharacter) ) {
						nextCharacter = GetRandomCharacter();
					}
				}

				pwdBuffer.Append(nextCharacter);
				lastCharacter = nextCharacter;
			}

			return pwdBuffer.ToString();
		}

        /// <summary>
        /// Validates password input for rules
        /// </summary>
        /// <param name="password">string</param>
        /// <returns>true/false</returns>
        public bool Validate(string password)
        {
            int iCount;

            if (password.Length < minSize || password.Length > maxSize)
                return (false);

            // check for Consecutive characters
            if (!ConsecutiveCharacters) // cannot have consecutive characters
            {
                for (iCount = 0; iCount < password.Length - 1; iCount++)
                {
                    if (password[iCount] == password[iCount + 1])
                        return (false);
                }
            }

            if (!RepeatCharacters) // cannot have repeating characters
            {
                for (iCount = 0; iCount < password.Length; iCount++)
                {
                    int index = password.IndexOf(password[iCount]);
                    while (index != -1)
                    {
                        if (index != iCount)
                            return (false);
                        index = password.IndexOf(password[iCount]);
                    }
                }
            }

            if (Exclusions != null)	// cannot have characters from exclusion string
            {
                for (iCount = 0; iCount < password.Length; iCount++)
                {
                    if (Exclusions.IndexOf(password[iCount]) != -1)
                        return (false);
                }
            }

            if (ExcludeSymbols) // cannot contain 'symbols'
            {
                for (iCount = CUBoundDigit; iCount < pwdCharArray.GetUpperBound(0); iCount++)
                {
                    if (password.IndexOf(pwdCharArray[iCount]) != -1)
                        return (false);
                }
            }

            return (true);
        } 


        #region Properties
        /// <summary>
	    /// A property that stores a set of exclusions
	    /// </summary>
	    public string Exclusions { get; set; }

	    /// <summary>
        /// Property that store the minimum size of password
        /// </summary>
		public int Minimum
		{
			get { return this.minSize; }
			set	{ this.minSize = value;
				if ( CDefaultMinimum > this.minSize ){
					this.minSize = CDefaultMinimum; } }
		}

        /// <summary>
        /// Property that store Default maximum size of password
        /// </summary>
		public int Maximum{
			get { return this.maxSize; }
			set	{ this.maxSize = value;
				if ( this.minSize >= this.maxSize ){
					this.maxSize = CDefaultMaximum; } }
		}

	    /// <summary>
	    ///  A property that stores a excluded symbols
	    /// </summary>
	    public bool ExcludeSymbols { get; set; }

	    /// <summary>
	    /// A property that stores repeating characters
	    /// </summary>
	    public bool RepeatCharacters { get; set; }

	    /// <summary>
	    /// A property that stores Consecutive characters
	    /// </summary>
	    public bool ConsecutiveCharacters { get; set; }
        #endregion
    }