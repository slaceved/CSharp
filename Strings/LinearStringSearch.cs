using System;
using System.Collections.Generic;
using Tracker;

namespace StringSearching
{
    public class NaiveStringSearch : Tracker<char>, IStringSearchAlgorithm
    {
        public IEnumerable<ISearchMatch> Search(string toFind, string toSearch)
        {
            #region Validate Parameters are not null
            if (toFind == null)
            {
                throw new ArgumentNullException(nameof(toFind));
            }

            if (toSearch == null)
            {
                throw new ArgumentNullException(nameof(toSearch));
            }
            #endregion

            if (toFind.Length <= 0 || toSearch.Length <= 0) yield break;
            for (var startIndex = 0; startIndex <= toSearch.Length - toFind.Length; startIndex++)
            {
                var matchCount = 0;

                while (Compare(toFind[matchCount], toSearch[startIndex + matchCount]) == 0)
                {
                    matchCount++;

                    if (toFind.Length != matchCount) continue;
                    yield return new StringSearchMatch(startIndex, matchCount);

                    startIndex += matchCount - 1;
                    break;
                }
            }
        }
    }

    internal class StringSearchMatch : ISearchMatch
    {
        public StringSearchMatch(int start, int length)
        {
            Start = start;
            Length = length;
        }
        public int Start
        {
            get;
        }

        public int Length
        {
            get;
        }
    }

    public interface IStringSearchAlgorithm
    {
        IEnumerable<ISearchMatch> Search(string toFind, string toSearch);
    }

    public interface ISearchMatch
    {
        int Start { get; }
        int Length { get; }
    }
}
