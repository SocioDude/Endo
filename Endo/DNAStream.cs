using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Endo
{
    public class DNAStream
    {
        private string Data;
        private int CurrentIndex;

        public int Length => Data.Length - CurrentIndex;

        public DNAStream(string data)
        {
            Data = data;
            CurrentIndex = 0;
        }

        public char this[int index]
        {
            get 
            {
                return Data[CurrentIndex + index]; 
            }
        }

        public string Take(int n)
        {
            if (CurrentIndex + n > Data.Length)
            {
                throw new FinishException();
            }

            string result = Data.Substring(CurrentIndex, n);
            CurrentIndex += n;
            return result;
        }

        public void Prepend(StringBuilder r)
        {
            r.Append(Data.Substring(CurrentIndex));
            Data = r.ToString();
            CurrentIndex = 0;
        }

        // Gets the stuff without incrementing.
        public string Peek(int n)
        {
            return Data.Substring(CurrentIndex, Math.Min(n, Length));
        }

        public int IndexOf(string s, int start)
        {
            return Data.IndexOf(s, CurrentIndex + start) - CurrentIndex;
        }

        public string Substring(int start, int length)
        {
            return Data.Substring(CurrentIndex + start, Math.Min(Length - start, length));
        }

        public DNAStream Replace(string r)
        {
            return new DNAStream(r + Data.Substring(CurrentIndex));
        }
    }
}
