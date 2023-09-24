using System;
namespace CosmosExplorer.Infrastructure.Extensions;

public static class StringExtensions
{
    public static string RemoveSpecialCharacters(this string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return input;
        }

        return input.Replace('\u2018', '\'').Replace('\u2019', '\'').Replace('\u201c', '\"').Replace('\u201d', '\"');
    }
}

