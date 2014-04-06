using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QConverterV2
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                DoWork();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            /*
            finally
            {
                Console.WriteLine("finally block");
            }
            */

            Console.WriteLine("Change output file Converted.txt to Converted.QIF before importing into Quicken.");
            Console.WriteLine("Validate Compnay Names exactly match those already in Quicken.\n\n");

            Console.WriteLine("Enter any key to exit");
            Console.ReadKey();
        }

        static void DoWork()
        {
            int transCount;
            int transRecordStart;
            string transDate;
            string quantity, companyName, pricePerShare, commission;
            string transType;
            bool buy;

            //DateTime outDate;

            string[] lines = System.IO.File.ReadAllLines(@"C:\Users\Dave\Documents\QConverter\Motif.csv");
            Console.WriteLine("Number of input lines: {0}", lines.Length);

            transCount = (lines.Length - 4) / 8;
            Console.WriteLine("Number of transactions to process: {0}", transCount);

            // Console.WriteLine("Content of file: \n\n");

            using (System.IO.StreamWriter outFile = new System.IO.StreamWriter(@"C:\Users\Dave\Documents\QConverter\Converted.txt"))
            {
                /*
                foreach (string line in lines)
                {
                    outFile.WriteLine(line);
                }
                */

                // *** Build Header
                outFile.WriteLine("!Account");
                outFile.WriteLine("NMotif Investing");
                outFile.WriteLine("TInvst");
                outFile.WriteLine("^");

                // *** Get Date for use in each transaction
                // transDate = lines[1].Substring(6, lines[1].IndexOf("\"", 1) - 1 - 6);
                transDate = lines[1].Substring(lines[1].IndexOf(":") + 2, 12);
                DateTime outDate = Convert.ToDateTime(transDate);
                //outDate = new DateTime(int.Parse(transDate.Substring(transDate.Length - 4)), 2, int.Parse(transDate.Substring(transDate.IndexOf(" ") + 1, 2)));

                //outFile.WriteLine("**{0}**", outDate.ToShortDateString());
                Console.WriteLine("**{0}**", outDate.ToShortDateString());


                // ** Loop through transactions
                for (int i = 0; i < transCount; i++)
                {
                    buy = true;
                    transType = null;
                    
                    transRecordStart = 3 + (i * 8);

                    // One transaction
                    outFile.WriteLine("!Type:Invst");
                    outFile.WriteLine("D{0}", outDate.ToShortDateString());
                    //outFile.WriteLine("NBuy");
                    
                    // Transaction type - "Buy" or "Sell"
                    if (lines[transRecordStart].Contains("Buy") == true)
                    {
                        outFile.WriteLine("NBuy");
                        transType = "Purchased";
                        buy = true;
                    }
                    else if (lines[transRecordStart].Contains("Sell") == true)
                    {
                        outFile.WriteLine("NSell");
                        transType = "Sold";
                        buy = false;
                    }

                    // Stock name
                    companyName = lines[transRecordStart].Substring(0, lines[transRecordStart].IndexOf(","));
                    if (companyName.Substring(companyName.Length - 1, 1) == ".")
                    {
                        companyName = companyName.Substring(0, companyName.Length - 1);
                    }
                    outFile.WriteLine("Y{0}", companyName);

                    // Total
                    //Console.WriteLine("{0}, {1}", lines[3].IndexOf("(") + 1, lines[3].Length - lines[3].IndexOf("(") - 3);
                    if (buy)
                    {
                        outFile.WriteLine("T{0}", lines[transRecordStart].Substring(lines[transRecordStart].IndexOf("(") + 2, lines[transRecordStart].Length - lines[transRecordStart].IndexOf("(") - 3));
                    }
                    else
                    {
                        outFile.WriteLine("T{0}", lines[transRecordStart].Substring(lines[transRecordStart].LastIndexOf("$") + 1, lines[transRecordStart].Length - lines[transRecordStart].LastIndexOf("$") - 1));
                    }
                    // Price per share
                    //   Assumes space after price
                    pricePerShare = lines[transRecordStart].Substring(lines[transRecordStart].IndexOf("$") + 1, lines[transRecordStart].IndexOf(",", lines[transRecordStart].IndexOf("$")) - lines[transRecordStart].IndexOf("$") - 2);
                    outFile.WriteLine("I{0}", pricePerShare);

                    // Quantity
                    if (buy)
                    {
                        quantity = lines[transRecordStart].Substring(lines[transRecordStart].IndexOf(",Buy,") + 5, lines[transRecordStart].IndexOf("$") - lines[transRecordStart].IndexOf(",Buy,") - 6);
                        outFile.WriteLine("Q{0}", quantity);
                    }
                    else
                    {
                        quantity = lines[transRecordStart].Substring(lines[transRecordStart].IndexOf("-") + 1, lines[transRecordStart].IndexOf("$") - lines[transRecordStart].IndexOf("-") - 2);
                        outFile.WriteLine("Q{0}", quantity);
                    }

                    // Commission
                    //commission = lines[transRecordStart + 5].Substring(lines[transRecordStart + 5].IndexOf("(") + 2, lines[transRecordStart + 5].Length - lines[transRecordStart + 5].IndexOf("(") - 3);
                    commission = lines[transRecordStart + 5].Substring(lines[transRecordStart + 5].IndexOf("$") + 1, lines[transRecordStart + 5].Length - lines[transRecordStart + 5].IndexOf("$") - 2);
                    outFile.WriteLine("O{0}", commission);

                    // Memo
                    outFile.WriteLine("M{0} {1} shares of {2} at ${3} each muinus ${4} commission", transType, quantity, companyName, pricePerShare, commission);

                    outFile.WriteLine("^");
                }
            }
        }
    }
}
