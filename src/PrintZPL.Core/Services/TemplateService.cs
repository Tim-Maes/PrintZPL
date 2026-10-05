using System.Text.RegularExpressions;

namespace PrintZPL.Core.Services;

public sealed class TemplateService : ITemplateService
{
    public string PopulateZplTemplate(Dictionary<string, string> data, string zplTemplate, string delimiter)
    {
        ArgumentNullException.ThrowIfNull(data);
        ArgumentNullException.ThrowIfNull(zplTemplate);
        ArgumentException.ThrowIfNullOrEmpty(delimiter);

        var zpl = zplTemplate;

        if (data.Any())
        {
            foreach (var item in data)
            {
                ArgumentException.ThrowIfNullOrEmpty(item.Key);
                string propName = item.Key;
                string propValue = item.Value;

                var placeholder = Regex.Escape(delimiter) + Regex.Escape(item.Key) + Regex.Escape(delimiter);

                zpl = Regex.Replace(zpl, placeholder, _ => item.Value ?? "", RegexOptions.IgnoreCase);
            }
        }

        return zpl;
    }
}

public interface ITemplateService
{
    string PopulateZplTemplate(Dictionary<string, string> data, string zplTemplate, string delimiter);
}
