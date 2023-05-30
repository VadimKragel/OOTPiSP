using System.Text.RegularExpressions;
using System.Text;
using System.Data;
using Serializers;

namespace OOTPiSP.TextFormat
{
    public enum TokenType
    {
        Whitespaces,
        OpenBrace,
        CloseBrace,
        OpenBracket,
        CloseBracket,
        Colon,
        Semicolon,
        Type,
        String,
        Number,
        Bool,
        Null,
        Unknown
    }

    public class Token
    {
        public TokenType Type { get; }
        public int StartIndex { get; }
        public int Length { get; }

        public Token(TokenType type, int startIndex, int length)
        {
            Type = type;
            StartIndex = startIndex;
            Length = length;
        }
    }

    public class LexerStateMachine
    {
        private static int[,] transitions = new int[31, 26]{
            // 0   1   2   3   4   5   6   7   8   9  10  11  12  13  14  15  16  17  18  19  20  21  22  23  24  25
            {  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0 },
            {  2,  3,  4,  5,  6,  7,  8,  9,  0, 12, 15,  0,  0, 18,  0,  0,  0,  0, 22,  0,  0, 26,  0,  0,  0,  0 },//1
            {  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0 },
            {  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0 },
            {  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0 },
            {  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0 },//5
            {  0,  0,  0,  0,  6,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0 },
            {  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0 },
            {  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0 },
            { 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10 },
            { 10, 10, 10, 10, 10, 10, 10, 10, 11, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10 },//10
            {  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0 },
            { 13, 13, 13, 13, 13, 13, 13, 13, 13, 13, 13, 13, 13, 13, 13, 13, 13, 13, 13, 13, 13, 13, 13, 13, 13, 13 },
            { 13, 13, 13, 13, 13, 13, 13, 13, 13, 14, 13, 13, 13, 13, 13, 13, 13, 13, 13, 13, 13, 13, 13, 13, 13, 13 },
            {  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0 },
            {  0,  0,  0,  0,  0,  0,  0,  0,  0,  0, 15, 16, 16,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0 },//15
            {  0,  0,  0,  0,  0,  0,  0,  0,  0,  0, 17,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0 },
            {  0,  0,  0,  0,  0,  0,  0,  0,  0,  0, 17,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0 },
            {  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0, 19,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0 },
            {  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0, 20,  0,  0,  0,  0,  0,  0,  0,  0,  0 },
            {  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0, 21,  0,  0,  0,  0,  0,  0,  0,  0,  0 },//20
            {  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0 },
            {  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0, 23,  0,  0,  0,  0,  0,  0 },
            {  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0, 24, 24,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0 },
            {  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0, 25,  0,  0,  0,  0,  0 },//25
            {  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0 },
            {  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0, 27,  0,  0,  0 },
            {  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0, 28, 28,  0,  0,  0,  0,  0,  0,  0,  0 },
            {  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0, 29,  0,  0 },
            {  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0, 30,  0,  0,  0,  0,  0 },
            {  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0 },//30
        };

        private static int GetCharType(char ch)
        {
            return ch switch
            {
                '{' => 0,
                '}' => 1,
                '[' => 2,
                ']' => 3,
                char c when "\t\n\f\r\x0B\u0020".Contains(c) => 4,
                ':' => 5,
                ';' => 6,
                '(' => 7,
                ')' => 8,
                '"' => 9,
                char c when c >= '0' && c <= '9' => 10,
                '.' => 11,
                ',' => 12,
                'n' => 13,
                'u' => 14,
                'U' => 15,
                'l' => 16,
                'L' => 17,
                char c when c == 't' || c == 'T' => 18,
                char c when c == 'r' || c == 'R' => 19,
                char c when c == 'e' || c == 'E' => 20,
                char c when c == 'f' || c == 'F' => 21,
                char c when c == 'a' || c == 'A' => 22,
                char c when c == 's' || c == 'S' => 23,
                _ => 24,
            };
        }

        public LexerStateMachine()
        {
        }
        private TokenType GetTokenType(int state)
        {
            return state switch
            {
                2 => TokenType.OpenBrace,
                3 => TokenType.CloseBrace,
                4 => TokenType.OpenBracket,
                5 => TokenType.CloseBracket,
                6 => TokenType.Whitespaces,
                7 => TokenType.Colon,
                8 => TokenType.Semicolon,
                11 => TokenType.Type,
                14 => TokenType.String,
                15 => TokenType.Number,
                17 => TokenType.Number,
                21 => TokenType.Null,
                25 => TokenType.Bool,
                30 => TokenType.Bool,
                _ => TokenType.Unknown,
            };
        }
        public Token Match(string str, int start = 0)
        {
            int length = 0;
            int state = 1;
            TokenType tokenType = TokenType.Unknown;
            while (start + length < str.Length)
            {
                state = transitions[state, GetCharType(str[start + length])];
                if (state == 0)
                    break;
                tokenType = GetTokenType(state);
                length++;

            }
            return new Token(tokenType, start, length);
        }
    }

    public class TxtReader
    {
        private int pos;
        public string Str { get; }
        private LexerStateMachine lsm;
        public TxtReader(string str)
        {
            lsm = new LexerStateMachine();
            Str = str;
            Reset();
        }

        public Token Read()
        {
            if (pos < Str.Length)
            {
                Token token = lsm.Match(Str, pos);
                if (token.Type != TokenType.Unknown)
                {
                    if (token.Length == 0)
                        throw new Exception("Token length is 0. Check the rules");
                    Console.WriteLine(token.Type + " " + Str.Substring(token.StartIndex, token.Length));
                    pos += token.Length;
                    return token;
                }
                else
                {
                    throw new Exception($"Unknown token type was detected at {pos}");
                }
            }
            else
            {
                return null;
            }
        }

        public void Reset()
        {
            pos = 0;
        }

        public static IEnumerable<Token> Tokenize(string str)
        {
            TxtReader reader = new TxtReader(str);
            Token token;
            while ((token = reader.Read()) != null)
                yield return token;
        }
    }
}