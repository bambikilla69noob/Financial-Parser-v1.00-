
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;
using System.Text.RegularExpressions;
using UglyToad.PdfPig.Core;
using System.Dynamic;
using UglyToad.PdfPig.DocumentLayoutAnalysis;
using Microsoft.VisualBasic;



string filepath = Console.ReadLine();

if (string.IsNullOrEmpty(filepath) || !File.Exists(filepath))
{
    Console.WriteLine("Invalid file path.");
    return;
}
int arraycounter = 0;
string pdfText = "";
int pageskip = 0;
int bandend = 0;
int bandstart = 0;
string bandtext = "";
int indexfinancialconslidated = pdfText.IndexOf("conolidated statement of financial position");
int indexprofitloss = 0;
int indexcashflow = 0;
int indexchangesinequity = 0;
int indexend = 0;
int startpoint = 0;
int indextofinddate = 0;
string regexpatterndictionary = "";
bool isQuarterly = false;
string datefinder="";
string datepattern="";
Match dateMatch=Match.Empty;
decimal extractedAmount = 0;
FianancialPoition FianancialPoitionvar = new FianancialPoition();
string month = "";
decimal year = 0;
int[] indexarray= new int[4];
Regex regexreplace;

// Synonyms dictionary
Dictionary<string, List<string>> financialpositionsynonyms = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase)
{
    //  Current Liabilities
    {"trade payables", new List<string> {"trade payables" } } ,

    {"short term borrowings", new List<string>

    {
    "short term borrowings","short-term borrowings net",
    "short term borrowings secured",
    "short term borrowings secured net",
    "short term borrowings from fianancial institutions net",
    "shor term borrowings from fianancial institutions",
    "short term borrowings from fianancial institutions secured net",
    "short term borrowings from fianancial institutions secured" } },

    {"immediate liablities", new List<string>

    {
        "cuurent portion of long term liablities",
        "cuurent portion of long term borrowings",
        "cuurent portion of long term borrowings secured" }
    },
    {"immediate lease", new List<string>

    {
        "current portion of lease liablities",
        "current portion of long term lease liablities" }
    },

    {"contract liablities", new List<string>

    { "contract liabilities",
      "advances from customers" } },

    //  Current Assets
    {"trade debts", new List<string> { "trade and other receivables", "trde debts" } },
    {"stores spares", new List<string>

    { "stores spares chemicals loose tools",
      "stores spares and chemicals",
      "stores spares and loose tools",
      "stores spare parts and loose tools",
      "stores spares parts and loose tools",
      "stores spare chemicals loose tools" }
    },
    {"stock in trade", new List<string> { "stock in trade" } },

    {"inventory", new List<string> { "inventories", "inventory" } },
    {"short term investments", new List<string> { "curreent portion of long term investments","short term investments" } },
    {"short term prepayments", new List<string>
    {
        "short term deposits and prepayments",
        "deposits and short term prepayments",
        "trade deposits and prepayments",
        "deposits and prepayments"
    } },
    {"current loans and advances", new List<string> { "loans and advances" } },
    // Non Current Assets
    {"property plant", new List<string> { "property plant and equipment", "fixed assets" } },
    {"long term investments", new List<string> { "long term investments" } },
    // Non Current Liabilities
    {" long term borrowings ", new List<string>
    {
        "long term borrowings",
        "long term borrowings net of current portion",
        "long term borrowings secured",
        "long term borrowings from fianancial institutions net of current portion",
        "long term borrowings from fianancial institutions",
        "long term borrowings from fianancial institutions secured net of current portion",
        "long term borrowings from fianancial institutions secured",
        "long term loans from financial institutions secured"
    } },
    {"deferred taxation", new List<string>
    {
        "deferred tax liability",
        "deferred income tax liability",
        "deferred taxation"
    } },
    // edge cases
    {"total receivables", new List<string>
    {
        "loans, advances, deposits, prepayments and other receivables",
        "trade and other payables"
    } },
};

/*
{ "trade payables", "trade payables" },
{ "short term borrowings", "short term borrowings" },
{ "short-term borrowings net", "short term borrowings"},
{ "short term borrowings secured", "short term borrowings"},
{ "short term borrowings secured net", "short term borrowings"},
{ "short-term borrowings from fianancial institutions net", "short term borrowings"},
{ "short-term borrowings from fianancial institutions", "short term borrowings"},
{ "short-term borrowings from fianancial institutions secured net", "short term borrowings"},
{ "short-term borrowings from fianancial institutions secured", "short term borrowings"},

{"cuurent portion of long term liablities", "immediate liablities" },
{"cuurent portion of long term borrowings", "immediate liablities" },
{"cuurent portion of long term borrowings secured", "immediate liablities" },

{"current portion of lease liablities", "immediate lease" },
{"current portion of long term lease liablities", "immediate lease" },


{ "contract liabilities", "contract liablities"},
{ "advances from customers", "contract liablities"},
//  Current Assets
{ "trade and other receivables", "trade debts" },
{ "trade receivables", "trade debts" },

{ "stores spares chemicals loose tools", "stores spares" },
{ "stores spares and chemicals", "stores spares" },
{ "stores spares and loose tools", "stores spares" },
{ "stores spare parts and loose tools", "stores spares" },
{ "stores spares parts and loose tools", "stores spares" },
{ "stores spare chemicals loose tools", "stores spares" },

{ "stock in trade", "stock in trade" },
{ "inventories", "stock in trade" },
{ "inventory", "stock in trade" },

{ " curreent portion of long term investments", "short term investments" },

{"short term deposits and prepayments", "short term prepayments" },
{"deposits and short term prepayments", "short term prepayments" } ,
{"trade deposits and prepayments", "short term prepayments"   },
{"deposits and prepayments", "short term prepayments"   },
{"loans and advances", "current loans and advances" },


// Edge Cases Current assets
{"loans, advances, deposits, prepayments and other receivables", "total receivables"   },
{"trade and other payables","total receivables"   },

// Non Current Assets
{ "property plant and equipment", "property plant" },
{ "fixed assets", "property plant" },

{ "long term investments", "long term investments" },

// Non Current Liabilities
{ "long term borrowings", " non liablities" },
{ "long term borrowings net of current portion", " non liablities" },
{ "long term borrowings secured", " non liablities" },
{ "long term borrowings from fianancial institutions net of current portion", " non liablities" },
{ "long term borrowings from fianancial institutions", " non liablities" },
{ "long term borrowings from fianancial institutions secured net of current portion", " non liablities" },
{ "long term borrowings from fianancial institutions secured", " non liablities" },
{"long term loans from financial institutions secured " , " non liablities" },

{"deferred tax liability" , "deferred taxation" },
{"deferred income tax liability" , "deferred taxation" },
{"deferred taxation","deferred taxation" }
};*/
Dictionary<string, Action<FianancialPoition, decimal>> financialpositionrecordsetter = new Dictionary<string, Action<FianancialPoition, decimal>>()
{   // Liabilities
    { "trade payables", (record, value) => record.TradePayables = value },
    { "short term borrowings", (record, value) => record.ShortTermBorrowings = value },
    { "liablities", (record, value) => record.Liablities = value },
    { "current liablities", (record, value) => record.CurentLiablities = value },
    { "immediate liablities", (record, value) => record.ImmediateLiablities = value },
    { "immediate lease", (record, value) => record.ImmediateLease = value },
    { "contract liablities", (record, value) => record.ContractLiablities = value },
     {"long term borrowings",(record,value)=> record.LongTermBorrowings=value },
      {"current loans and advances" , (record,value)=>record.CurrentLoansAndAdvances = value },
       {"deferred taxation",(record,value)=> record.DeferredTaxation=value },
    // Assets
    { "property plant", (record, value) => record.PropertyPlant = value },
    { "stores spares", (record, value) => record.StoresSpares = value },
    { "stock in trade", (record, value) => record.StockInTrade = value },
    { "trade debts", (record, value) => record.TradeDebts = value },
    { "current assets", (record, value) => record.CurrentAssets = value },
    { "non current assets", (record, value) => record.NonCurrentAssets = value },
    { "long term investments", (record, value) => record.LongTermInvestments = value },
    { "short term investments", (record, value) => record.ShortermInvestments = value },
    { "other recievables", (record, value) => record.OtherRecievables = value },
    { "deposits", (record, value) => record.Deposits = value },
    {"inventory",(record,value)=>record.Inventory=value },
    {"short term prepayments",(record,value)=>record.ShortTermPrepayments=value },  
    { "total receivables",(record,value)=>record.TotalRecievables=value}
}
;

using (PdfDocument document = PdfDocument.Open(filepath))
{
    foreach (Page page in document.GetPages())
    {  
        
            pdfText += page.Text;



    }

}
// Normalisation
pdfText = Regex.Replace(pdfText,@"(?<=\d),(?=\d)", "");
pdfText = Regex.Replace(pdfText, @"(?<=[A-Za-z]),(?=\s?[A-Za-z])", "");
pdfText = pdfText.Replace("-", " ");
pdfText = pdfText.ToLowerInvariant();
pdfText = Regex.Replace(pdfText, @"\s+", " ");
foreach (var dict in financialpositionsynonyms)
{
     regexpatterndictionary = string.Join("|", dict.Value.Select(Regex.Escape));
    regexreplace = new Regex(@"\b(" + regexpatterndictionary + @")\b", RegexOptions.IgnorePatternWhitespace);
    pdfText = regexreplace.Replace(pdfText, dict.Key);



}




startpoint = pdfText.IndexOf("consolidated statement");

if (startpoint > 0)
{
    indexfinancialconslidated = pdfText.IndexOf("consolidated statement of financial position");
     Console.WriteLine("Found consolidated statement  financial position at ");
    indexprofitloss = pdfText.IndexOf("consolidated statement of profit or loss");
    Console.WriteLine("Found consolidated statement proft at " + indexprofitloss);

    indexcashflow = pdfText.IndexOf("consolidated statement of cash flows");
    Console.WriteLine("Found consolidated statement cash flows at" + indexcashflow);
    indexchangesinequity = pdfText.IndexOf("consolidated statement of changes in equity");
    Console.WriteLine("Found consolidated statement changes in equity at " + indexchangesinequity);
    indexend = pdfText.Length;
    
}
else if (startpoint < 0)
{    
    startpoint = pdfText.IndexOf("statement of");
    indexfinancialconslidated = pdfText.IndexOf("statement of financial position");
    indexprofitloss = pdfText.IndexOf("statement of profit or loss");
    indexcashflow = pdfText.IndexOf("statement of cash flows");
    indexchangesinequity = pdfText.IndexOf("statement of changes in equity");
    indexend = pdfText.Length;
    
}
Console.WriteLine(pdfText);

indextofinddate =pdfText.IndexOf("for the year ended");
 isQuarterly = false;
if (indextofinddate < 0)
{
    indextofinddate = pdfText.IndexOf("for the quarter ended");
    isQuarterly = true;
}
 datepattern = @"(january|february|march|april|may|june|july|august|september|october|november|december)\s+\d{1,2},\s+(\d{4})";
 datefinder = pdfText.Substring(indextofinddate, 200);
 dateMatch = Regex.Match(datefinder, datepattern);
if (dateMatch.Success)
{
    month = dateMatch.Groups[1].Value;
   year = decimal.Parse(dateMatch.Groups[2].Value);
    
}

// initialisation
 indexarray = [ indexfinancialconslidated, indexprofitloss, indexcashflow, indexchangesinequity ];
Array.Sort(indexarray);


// Logic to extract sections
for (arraycounter = 0; arraycounter <= indexarray.Length - 1; arraycounter++)
{
    if (arraycounter == indexarray.Length - 1)
    {
        bandend = pdfText.Length;
    }
    else { bandend = indexarray[arraycounter + 1]; }
    bandstart = indexarray[arraycounter];
    int length = bandend - bandstart;
    bandtext = pdfText.Substring(bandstart, length);
    /*
    if (indexarray[arraycounter] == indexfinancialconslidated)
    {
        Console.WriteLine(bandtext);
        Console.WriteLine(indexarray[arraycounter]);
        Console.WriteLine(indexarray[arraycounter+1]);

    }*/

    switch (indexarray[arraycounter])
    { case int _value when _value == indexfinancialconslidated:
            {
                foreach (var dict in financialpositionsynonyms)
                {
                    extractedAmount = ExtractValueAfterKeyword(bandtext, dict.Key);

                    if ( financialpositionrecordsetter.TryGetValue(dict.Key, out var setter))
                    {
                        
                        financialpositionrecordsetter[dict.Key](FianancialPoitionvar, extractedAmount);
                    }
                    else
                    { Console.WriteLine(dict.Key);

                    }
                }

                break;
                
            }
    }
}
/*
Console.WriteLine(FianancialPoitionvar.PropertyPlant);
Console.WriteLine(month);
Console.WriteLine(year);
Console.WriteLine(FianancialPoitionvar.LongTermInvestments);
*/

    decimal ExtractValueAfterKeyword(string text, string keyword)
    {
        Regex pattern = new Regex(Regex.Escape(keyword) + @"\s+\d{1,2}?\s+(\(?\d{3,12}\)?)", RegexOptions.IgnorePatternWhitespace);
        Match match = pattern.Match(text);
        if (match.Success)
        {

            Console.WriteLine("extracted " + keyword);
            string valueString = match.Groups[1].Value;
            bool isNegative = valueString.StartsWith("(") && valueString.EndsWith(")");
            valueString = valueString.Replace("(", "").Replace(")", "").Trim();
            if (decimal.TryParse(valueString, out decimal value))
            {

                return isNegative ? -value : value;
            }

        }

        Console.WriteLine("no extraction " + keyword);
        return 0;
    }

     

          
    
    
        
     


public record FianancialPoition() 
{
    // Liabilities
    public decimal TradePayables { get; set; } = 0;
    public decimal ShortTermBorrowings { get; set; } = 0;
    public decimal Liablities { get; set; } = 0;
    public decimal CurentLiablities { get; set; } = 0;
    public decimal ImmediateLiablities { get; set; } = 0;
    public decimal ImmediateLease { get; set; } = 0;
    public decimal ContractLiablities { get; set; } = 0;
    public decimal LongTermBorrowings { get; set; } = 0;
    public decimal DeferredTaxation { get; set; } = 0;  
    // Assets
    public decimal PropertyPlant { get; set; } = 0;
    public decimal StoresSpares { get; set; } = 0;
    public decimal StockInTrade { get; set; }  = 0;
    public decimal TradeDebts { get; set; } = 0;
    public decimal CurrentAssets { get; set; } = 0;
    public decimal NonCurrentAssets { get; set; }  = 0;
    public decimal LongTermInvestments { get; set; } = 0;
    public decimal ShortermInvestments { get; set; } = 0;
    public decimal OtherRecievables { get; set; } = 0;
    public decimal Deposits { get; set; } =0;
    public decimal Inventory { get; set; } = 0;
     public decimal ShortTermPrepayments { get; set; } = 0;
    public decimal CurrentLoansAndAdvances { get; set; } = 0;
    public decimal TotalRecievables { get; set; } = 0;
}
    

   

  
    
