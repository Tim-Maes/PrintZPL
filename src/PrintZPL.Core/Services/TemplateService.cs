using System.Reflection;
using System.Text.RegularExpressions;

namespace PrintZPL.Core.Services;

public sealed class TemplateService : ITemplateService
{
    public string PopulateZplTemplate(Dictionary<string, string> data, string zplTemplate, string delimiter)
    {
        string zpl = zplTemplate;

        if (data.Any())
        {
            foreach (var item in data)
            {
                string propName = item.Key;
                string propValue = item.Value;

                string placeholder = Regex.Escape(delimiter) + propName + Regex.Escape(delimiter);

                zpl = Regex.Replace(zpl, placeholder, propValue ?? "", RegexOptions.IgnoreCase);
            }
        }

        return zpl;
    }
}

public interface ITemplateService
{
    string PopulateZplTemplate(Dictionary<string, string> data, string zplTemplate, string delimiter);
}
