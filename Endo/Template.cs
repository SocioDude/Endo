using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Endo
{
    public class Template
    {
        public readonly IReadOnlyList<ITemplateToken> Tokens;

        public Template(List<ITemplateToken> tokens)
        {
            Tokens = tokens;
        }

        public override string ToString()
        {
            return string.Join("", Tokens);
        }

        public void Replace(DNAStream stream, List<string> environment)
        {
            StringBuilder r = new StringBuilder();
            foreach (var token in Tokens)
            {
                token.Replace(r, environment);
            }

            stream.Prepend(r);
        }
    }

    public interface ITemplateToken 
    {
        public void Replace(StringBuilder r, List<string> environment);
    }

    public class TemplateConstToken : ITemplateToken
    {
        public readonly string S;

        public TemplateConstToken(string s)
        {
            S = s;
        }

        public void Replace(StringBuilder r, List<string> environment)
        {
            r.Append(S);
        }

        public override string ToString()
        {
            return S;
        }
    }

    public class ReferenceToken : ITemplateToken
    {
        public readonly int L;
        public readonly int N;

        public ReferenceToken(int l, int n)
        {
            L = l;
            N = n;
        }

        public void Replace(StringBuilder r, List<string> environment)
        {
            if (N >= environment.Count)
            {
                return;
            }

            foreach (char c in environment[N])
            {
                Quote(c, L, r);
            }
        }

        public void Quote(char c, int level, StringBuilder r)
        {
            if (level == 0)
            {
                r.Append(c);
                return;
            }

            switch (c)
            {
                case 'I':
                    Quote('C', level - 1, r);
                    break;
                case 'C':
                    Quote('F', level - 1, r);
                    break;
                case 'F':
                    Quote('P', level - 1, r);
                    break;
                case 'P':
                    Quote('I', level - 1, r);
                    Quote('C', level - 1, r);
                    break;
                default:
                    throw new Exception();
            }
        }

        public override string ToString()
        {
            return $"{N}[{L}]";
        }
    }

    public class LengthToken : ITemplateToken
    {
        public readonly int N;

        public LengthToken(int n)
        {
            N = n;
        }

        public void Replace(StringBuilder r, List<string> environment)
        {
            if (N >= environment.Count)
            {
                r.Append("P");
                return;
            }

            AsNat(environment[N].Length, r);
        }

        public void AsNat(int n, StringBuilder r)
        {
            int current = n;
            while (current != 0)
            {
                if (n % 2 == 0)
                {
                    r.Append('I');
                }
                else
                {
                    r.Append('C');
                }

                current /= 2;
            }

            r.Append('P');
        }

        public override string ToString()
        {
            return $"|{N}|";
        }
    }
}
