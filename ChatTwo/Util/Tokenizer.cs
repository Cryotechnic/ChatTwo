﻿using System.Text.RegularExpressions;

namespace ChatTwo.Util;

// Modified from: https://jack-vanlightly.com/blog/2016/2/24/a-more-efficient-regex-tokenizer
public static class Tokenizer
{
    public enum TokenType
    {
        CloseParenthesis,
        Comma,
        Dot,
        QuestionMark,
        ExclamationMark,
        Semicolon,
        Whitespace,
        Equals,
        OpenParenthesis,
        UrlString,
        StringValue,
        Leftover,
        SequenceTerminator
    }

    public class Token(TokenType tokenType, string value)
    {
        public Token(TokenType tokenType) : this(tokenType, string.Empty) { }

        public TokenType TokenType { get; } = tokenType;
        public string Value { get; } = value;
    }

    public static class PrecedenceBasedRegexTokenizer
    {
        private static readonly List<TokenDefinition> TokenDefinitions;

        static PrecedenceBasedRegexTokenizer()
        {
            TokenDefinitions =
            [
                new TokenDefinition(TokenType.CloseParenthesis, "\\)", 1),
                new TokenDefinition(TokenType.Comma, ",", 1),
                new TokenDefinition(TokenType.Dot, "\\.", 1),
                new TokenDefinition(TokenType.QuestionMark, "\\?", 1),
                new TokenDefinition(TokenType.ExclamationMark, "!", 1),
                new TokenDefinition(TokenType.Semicolon, ";", 1),
                new TokenDefinition(TokenType.Whitespace, "\\s", 1),
                new TokenDefinition(TokenType.Equals, "=", 1),
                new TokenDefinition(TokenType.OpenParenthesis, "\\(", 1),
                new TokenDefinition(TokenType.UrlString, URLRegex, 1),
                new TokenDefinition(TokenType.StringValue, "\\p{IsBasicLatin}", 2),
                new TokenDefinition(TokenType.Leftover, ".", 3)
            ];
        }

        public static IEnumerable<Token> Tokenize(string lqlText)
        {
            var tokenMatches = FindTokenMatches(lqlText);

            var groupedByIndex = tokenMatches.GroupBy(x => x.StartIndex)
                .OrderBy(x => x.Key)
                .ToList();

            TokenMatch? lastMatch = null;
            foreach (var t in groupedByIndex)
            {
                var bestMatch = t.OrderBy(x => x.Precedence).First();
                if (lastMatch != null && bestMatch.StartIndex < lastMatch.EndIndex)
                    continue;

                yield return new Token(bestMatch.TokenType, bestMatch.Value);

                lastMatch = bestMatch;
            }

            yield return new Token(TokenType.SequenceTerminator);
        }

        private static List<TokenMatch> FindTokenMatches(string lqlText)
        {
            var tokenMatches = new List<TokenMatch>();

            foreach (var tokenDefinition in TokenDefinitions)
                tokenMatches.AddRange(tokenDefinition.FindMatches(lqlText).ToList());

            return tokenMatches;
        }
    }

    private class TokenDefinition
    {
        private readonly TokenType Type;
        private readonly int Precedence;
        private readonly Regex Regex;

        public TokenDefinition(TokenType returnsToken, string regexPattern, int precedence)
        {
            Type = returnsToken;
            Precedence = precedence;
            Regex = new Regex(regexPattern, RegexOptions.IgnoreCase|RegexOptions.Compiled);
        }

        public TokenDefinition(TokenType returnsToken, Regex regex, int precedence)
        {
            Type = returnsToken;
            Precedence = precedence;
            Regex = regex;
        }

        public IEnumerable<TokenMatch> FindMatches(string inputString)
        {
            var matches = Regex.Matches(inputString);
            for(var i = 0; i < matches.Count; i++)
            {
                yield return new TokenMatch
                {
                    StartIndex = matches[i].Index,
                    EndIndex = matches[i].Index + matches[i].Length,
                    TokenType = Type,
                    Value = matches[i].Value,
                    Precedence = Precedence
                };
            }
        }
    }

    private class TokenMatch
    {
        public TokenType TokenType { get; set; }
        public string Value { get; set; }
        public int StartIndex { get; set; }
        public int EndIndex { get; set; }
        public int Precedence { get; set; }
    }

    /// <summary>
    /// URLRegex returns a regex object that matches URLs like:
    /// - https://example.com
    /// - http://example.com
    /// - www.example.com
    /// - https://sub.example.com
    /// - example.com
    /// - sub.example.com
    ///
    /// It matches URLs with www. or https:// prefix, and also matches URLs
    /// without a prefix on specific TLDs.
    /// </summary>
    private static Regex URLRegex = new(
        @"(?<URL>((https?:\/\/|www\.)[a-z0-9-]+(\.[a-z0-9-]+)*|([a-z0-9-]+(\.[a-z0-9-]+)*\.(com|net|org|co|io|app)))(:[\d]{1,5})?(\/[^\s]*)?)",
        RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture
    );
}