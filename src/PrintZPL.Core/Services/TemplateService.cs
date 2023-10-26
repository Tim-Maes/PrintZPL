using System.Reflection;
using System.Text.RegularExpressions;

namespace PrintZPL.Core.Services;

public sealed class TemplateService : ITemplateService
{
    public string PopulateZplTemplate(object dataModel, string zplTemplate, string delimiter)
    {
        string zpl = zplTemplate;
        Type modelType = dataModel.GetType();

        foreach (PropertyInfo prop in modelType.GetProperties())
        {
            string propName = prop.Name;
            object propValue = prop.GetValue(dataModel);

            string placeholder = Regex.Escape(delimiter) + propName + Regex.Escape(delimiter);

            zpl = Regex.Replace(zpl, placeholder, propValue?.ToString() ?? "", RegexOptions.IgnoreCase);
        }

        return zpl;
    }
}

public interface ITemplateService
{
    string PopulateZplTemplate(object dataModel, string zplTemplate, string delimiter);
}
