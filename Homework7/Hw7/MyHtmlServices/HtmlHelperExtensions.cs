using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.VisualBasic.CompilerServices;

namespace Hw7.MyHtmlServices;

public static class HtmlHelperExtensions
{
    public static IHtmlContent MyEditorForModel(this IHtmlHelper helper)
    {
        var properties = helper.ViewData.ModelMetadata.ModelType.GetProperties();
        var content = new HtmlContentBuilder();
        foreach (var property in properties)
        {
            var value = helper.ViewData.Model is null ? String.Empty : property.GetValue(helper.ViewData.Model);
            content.AppendHtmlLine(
                "<div>" +
                MakeLabelString(property) +
                MakeInputField(property, value) +
                MakeSpan(property, helper.ViewData.Model) +
                "</div>");
        }
        return content;
    }

    private static string MakeLabelString(PropertyInfo prop)
    {
        var builder = new StringBuilder();
        builder.Append("<label for=\"" + prop.Name + "\">");
        var attribute = prop.GetCustomAttribute<DisplayAttribute>();
        if (attribute is null)
            builder.Append(Regex.Replace(prop.Name, "([A-Z])", " $1", RegexOptions.Compiled).Trim());
        else
            builder.Append(attribute.Name);
        builder.Append("</label><br>");
        return builder.ToString();
    }

    private static string MakeInputField(PropertyInfo prop, object? value)
    {
        if (prop.PropertyType.IsEnum)
        {
            var builder = new StringBuilder();
            builder.Append($"<select value=\"{value}\">");
            foreach (var field in Enum.GetValues(prop.PropertyType))
                builder.Append($"<option>{field}</option>");
            builder.Append("</select>");
            return builder.ToString();
        }
        else
        {
            var valStr = value == String.Empty ? String.Empty : $" value=\"{value}\"";
            if(prop.PropertyType == typeof(int))
                return $"<input id=\"{prop.Name}\" type=\"number\" {valStr}/>";
            else
                return $"<input id=\"{prop.Name}\" type=\"text\" {valStr}/>";
        }
    }

    private static string MakeSpan(PropertyInfo prop, object? model)
    {
        if (model is null)
            return String.Empty;
        foreach (var attribute in prop.GetCustomAttributes<ValidationAttribute>())
            if (!attribute.IsValid(prop.GetValue(model)))
                return "<span>" + attribute.ErrorMessage! + "</span>";
        return String.Empty;
    }
} 