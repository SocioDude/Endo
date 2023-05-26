using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Endo
{
    public class Pattern
    {
        public readonly IReadOnlyList<IPatternToken> Tokens;

        public Pattern(List<IPatternToken> tokens)
        {
            Tokens = tokens;
        }

        public override string ToString()
        {
            return string.Join("", Tokens);
        }

        public List<string> Match(DNAStream stream)
        {
            int i = 0;
            List<string> environment = new List<string>();
            List<int> c = new List<int>();

            foreach (var token in Tokens)
            {
                if (!token.Match(stream, environment, ref i, c))
                {
                    return null;
                }
            }

            stream.Take(i);
            return environment;
        }
    }

    public interface IPatternToken 
    {
        public bool Match(DNAStream stream, List<string> environment, ref int i, List<int> c);
    }

    public class PatternConstToken : IPatternToken
    {
        public readonly string S;

        public PatternConstToken(string s)
        {
            S = s;
        }

        public bool Match(DNAStream stream, List<string> environment, ref int i, List<int> c)
        {
            string dnaData = stream.Substring(i, S.Length);
            if (dnaData == S)
            {
                i += S.Length;
                return true;
            }

            return false;
        }

        public override string ToString()
        {
            return S;
        }
    }

    public class SkipToken : IPatternToken
    {
        public readonly int N;

        public SkipToken(int n)
        {
            N = n;
        }

        public bool Match(DNAStream stream, List<string> environment, ref int i, List<int> c)
        {
            i += N;
            return i <= stream.Length;
        }

        public override string ToString()
        {
            return $"![{N}]";
        }
    }

    public class SearchToken : IPatternToken
    {
        public readonly string S;

        public SearchToken(string s)
        {
            S = s;
        }

        public bool Match(DNAStream stream, List<string> environment, ref int i, List<int> c)
        {
            int location = stream.IndexOf(S, i);
            if (location == -1)
            {
                return false;
            }

            i = location + S.Length;
            return true;
        }

        public override string ToString()
        {
            return $"?[{S}]";
        }
    }

    public class OpenToken : IPatternToken
    {
        public static readonly OpenToken INSTANCE = new OpenToken();

        private OpenToken() { }

        public bool Match(DNAStream stream, List<string> environment, ref int i, List<int> c)
        {
            c.Add(i);
            return true;
        }

        public override string ToString()
        {
            return "(";
        }
    }

    public class CloseToken : IPatternToken
    {
        public static readonly CloseToken INSTANCE = new CloseToken();

        private CloseToken() { }

        public bool Match(DNAStream stream, List<string> environment, ref int i, List<int> c)
        {
            environment.Add(stream.Substring(c.Last(), i));
            c.RemoveAt(c.Count - 1);
            return true;
        }

        public override string ToString()
        {
            return ")";
        }
    }
}
