using System;
using System.Collections.Generic;

public class KaprekarNumber
{
    public List<int> Data { get; private set; }
    public int Min { get; set; }
    public int Max { get; set; }

    public void PrintKaprekarNumber()
    {
        if (Min <= 0 || Max <= 0 || Min > Max) return;
        Data = new List<int>();
        for (var idx = Min; idx <= Max; idx++)
        {
            var squar = (long)Math.Pow(idx, 2);
            var squarString = squar.ToString();

            //for one digital,prefix with "0"
            if (squar <= 9)
                squarString = $"0{squarString}";

            var midIdx = squarString.Length / 2;
            var leftSubstring = squarString.Substring(0, midIdx);
            var rightSubstring = squarString.Substring(midIdx);

            var leftSubInt = long.Parse(leftSubstring);
            var rightSubInt = long.Parse(rightSubstring);

            if (leftSubInt + rightSubInt == idx)
            {
                Data.Add(idx);
            }
        }
    }
}