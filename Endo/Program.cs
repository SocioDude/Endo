using System;
using System.Collections.Generic;
using System.IO;

namespace Endo
{
    class Program
    {
        static void Main(string[] args)
        {
            //string dna = "(?[ICFP]![2]PFCI) PPPPICFPPPPFCI";
            //string dna = "IIPIFFCFPICIPFCPICPFCIICIICPPPPICFPPPPFCI";
            string dna = File.ReadAllText(@"C:\Users\hbayd\source\repos\Endo\Endo\endo.dna");
            DNAStream stream = new DNAStream(dna);
            List<string> rna = new List<string>();

            try
            {
                while (true)
                {
                    Pattern p = DNAParsing.ParsePattern(stream, rna);
                    Template t = DNAParsing.ParseTemplate(stream, rna);
                    List<string> environment = p.Match(stream);
                    t.Replace(stream, environment);
                    ;
                }
            }
            catch (FinishException)
            {
                ;
            }
        }
    }
}
