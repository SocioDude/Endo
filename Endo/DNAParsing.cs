using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Endo
{
    public class DNAParsing
    {
        public static Pattern ParsePattern(DNAStream stream, List<string> rna)
        {
            int level = 0;
            List<IPatternToken> tokens = new List<IPatternToken>();
            while (true)
            {
                string consts = Consts(stream);
                if (consts != "")
                {
                    tokens.Add(new PatternConstToken(consts));
                }

                switch (GetCommand(stream))
                {
                    case "IP":
                        tokens.Add(new SkipToken(Nat(stream)));
                        break;
                    case "IF":
                        stream.Take(1);
                        tokens.Add(new SearchToken(Consts(stream)));
                        break;
                    case "IIP":
                        level++;
                        tokens.Add(OpenToken.INSTANCE);
                        break;
                    case "IIC":
                    case "IIF":
                        if (level == 0)
                        {
                            return new Pattern(tokens);
                        }

                        level--;
                        tokens.Add(CloseToken.INSTANCE);
                        break;
                    case "III":
                        rna.Add(stream.Peek(7));
                        stream.Take(7);
                        break;
                    default:
                        throw new Exception();
                }
            }
        }

        public static Template ParseTemplate(DNAStream stream, List<string> rna)
        {
            List<ITemplateToken> tokens = new List<ITemplateToken>();

            while (true)
            {
                string consts = Consts(stream);
                if (consts != "")
                {
                    tokens.Add(new TemplateConstToken(consts));
                }

                switch (GetCommand(stream))
                {
                    case "IF":
                    case "IP":
                        tokens.Add(new ReferenceToken(Nat(stream), Nat(stream)));
                        break;
                    case "IIC":
                    case "IIF":
                        return new Template(tokens);
                    case "IIP":
                        tokens.Add(new LengthToken(Nat(stream)));
                        break;
                    case "III":
                        rna.Add(stream.Peek(7));
                        stream.Take(7);
                        break;
                    default:
                        throw new Exception();
                }
            }
        }
        
        public static int Nat(DNAStream stream)
        {
            long result = 0;
            long digit = 1;
            string current;
            while ((current = stream.Take(1)) != "P")
            {
                if (current == "C")
                {
                    result += digit;
                }

                digit *= 2;
            }

            if (result > int.MaxValue)
            {
                throw new Exception();
            }

            return (int)result;
        }

        public static string GetCommand(DNAStream stream)
        {
            try
            {
                if (stream[1] != 'I')
                {
                    return stream.Take(2);
                }
            }
            catch (IndexOutOfRangeException)
            {
                throw new FinishException();
            }

            return stream.Take(3);
        }

        public static string Consts(DNAStream stream)
        {
            StringBuilder builder = new StringBuilder();
            while (true)
            {
                switch (stream[0])
                {
                    case 'C':
                        stream.Take(1);
                        builder.Append('I');
                        break;
                    case 'F':
                        stream.Take(1);
                        builder.Append('C');
                        break;
                    case 'P':
                        stream.Take(1);
                        builder.Append('F');
                        break;
                    case 'I':
                        if (stream[1] != 'C')
                        {
                            return builder.ToString();
                        }

                        stream.Take(2);
                        builder.Append('P');
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
        }
    }
}
