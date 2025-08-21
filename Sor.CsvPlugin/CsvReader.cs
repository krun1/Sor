using Microsoft.VisualBasic.FileIO;
using Sor.API;
using Sor.API.SimpleNodes;

namespace Sor.CsvPlugin;

public class CsvReader : IInputReader
{
    public string TypeKey => "csvFile";
    
    public IInputNode? GetNode(string input)
    {
        using (var csvParser = new TextFieldParser(input))
        {
            csvParser.SetDelimiters(",");
            csvParser.HasFieldsEnclosedInQuotes = true;
            var header = csvParser.ReadFields();
            var list = new List<IInputObject>();
            
            while (csvParser.EndOfData == false)
            {
                var line = new Dictionary<string, IInputNode?>();
                var fields = csvParser.ReadFields();
                
                for (var i = 0; i < fields.Length; i++)
                    line.Add(header[i], InputHelper.Wrap(fields[i]));
                list.Add(new ResultObject(line));
            }
            return new ResultArray(list);
        }
    }
}