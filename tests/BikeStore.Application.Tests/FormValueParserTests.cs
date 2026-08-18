using System.Globalization;
using BikeStore.Web.ModelBinding;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace BikeStore.Application.Tests;

public sealed class FormValueParserTests
{
    [Theory]
    [InlineData("1250,50", "1250.50")]
    [InlineData("1250.50", "1250.50")]
    [InlineData("1.250,50", "1250.50")]
    [InlineData("1,250.50", "1250.50")]
    [InlineData("0,01", "0.01")]
    [InlineData("9 999 999,99", "9999999.99")]
    public void TryDecimal_AcceptsCommaAndPointFormats(string text, string expectedText)
    {
        var form = CreateForm("Price", text);

        var parsed = FormValueParser.TryDecimal(form, "Price", out var value);

        Assert.True(parsed);
        Assert.Equal(decimal.Parse(expectedText, CultureInfo.InvariantCulture), value);
    }

    [Theory]
    [InlineData("")]
    [InlineData("precio")]
    [InlineData("12,34,56")]
    public void TryDecimal_RejectsInvalidValues(string text)
    {
        var form = CreateForm("Price", text);

        Assert.False(FormValueParser.TryDecimal(form, "Price", out _));
    }

    private static IFormCollection CreateForm(string key, string value)
        => new FormCollection(new Dictionary<string, StringValues>
        {
            [key] = new StringValues(value)
        });
}
