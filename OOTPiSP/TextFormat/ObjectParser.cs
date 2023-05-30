#nullable enable
using System.Collections;
using System.Reflection;

namespace OOTPiSP.TextFormat
{
    public class ObjectParser
    {
        private Type type;
        private TxtReader reader;
        private Queue<Token> tokens;
        public ObjectParser(TxtReader reader, Type type)
        {
            this.type = type;
            this.reader = reader;
            tokens = new Queue<Token>();
            Token token;
            while (true)
            {
                token = reader.Read();
                if (token == null)
                {
                    break;
                }
                if (token.Type != TokenType.Whitespaces)
                {
                    tokens.Enqueue(token);
                }
            }
        }

        public object? Parse()
        {
            return Parse(type);
        }

        private object? Parse(Type toType)
        {
            Dictionary<TokenType, Func<Token, Type, object?>> objectParsers = new Dictionary<TokenType, Func<Token, Type, object?>>
            {
                { TokenType.OpenBracket, GetArrayObject },
                { TokenType.OpenBrace, GetObject },
                { TokenType.Type, GetObjectWithType },
                { TokenType.String, GetStringObject },
                { TokenType.Number, GetNumberObject },
                { TokenType.Null, GetNullObject },
                { TokenType.Bool, GetBoolObject },
            };
            Token token = tokens.Dequeue();
            if (objectParsers.ContainsKey(token.Type))
            {
                return objectParsers[token.Type].Invoke(token, toType);
            }
            else
            {
                throw new Exception($"Invalid token {token}: {reader.Str.Substring(token.StartIndex, token.Length)}");
            }
        }

        private object? GetObject(Token token, Type toType)
        {
            object? instance = Activator.CreateInstance(toType);
            if (tokens.Peek().Type == TokenType.CloseBrace)
            {
                return instance;
            }
            string key;
            PropertyInfo? prop;
            object? value;
            Token readToken;
            while (true)
            {
                readToken = tokens.Dequeue();
                if (readToken.Type != TokenType.String)
                    new Exception($"Unexpected token {readToken.Type}: {reader.Str.Substring(token.StartIndex, token.Length)}");
                key = ExtractStringValue(readToken);
                prop = toType.GetProperty(key);
                if (prop == null || !prop.CanWrite)
                    continue;
                readToken = tokens.Dequeue();
                if (readToken.Type != TokenType.Colon)
                    new Exception($"Unexpected token {readToken.Type}: {reader.Str.Substring(token.StartIndex, token.Length)}");
                value = Parse(prop.PropertyType);
                prop.SetValue(instance, value);
                readToken = tokens.Dequeue();
                if (readToken.Type == TokenType.CloseBrace)
                {
                    return instance;
                }
                else if (readToken.Type != TokenType.Semicolon)
                {
                    throw new Exception($"Unexpected token {readToken.Type}: {reader.Str.Substring(token.StartIndex, token.Length)}");
                }
            }
        }

        private object? GetObjectWithType(Token token, Type toType)
        {
            Type? readType = null;
            try
            {
                string strType = reader.Str.Substring(token.StartIndex + 1, token.Length - 2);
                readType = Type.GetType(strType);
            }
            catch { }
            if (readType != null)
            {
                Token readToken = tokens.Dequeue();
                if (readToken.Type == TokenType.OpenBrace)
                {
                    return GetObject(readToken, readType);
                }
                else
                {
                    throw new Exception($"Unexpected token {readToken.Type}: {reader.Str.Substring(token.StartIndex, token.Length)}");
                }
            }
            else
            {
                throw new Exception($"Can't resolve object type: {reader.Str.Substring(token.StartIndex, token.Length)}");
            }
        }

        private object GetArrayObject(Token token, Type toType)
        {
            Type? elemType = toType.IsGenericType ? toType.GetGenericArguments()[0] : toType.GetElementType();
            if ((toType.IsArray || type.GetInterface(nameof(IEnumerable)) != null) && elemType != null)
            {
                if (tokens.Peek().Type == TokenType.CloseBracket)
                {
                    return Array.CreateInstance(elemType, 0);
                }
                List<object?> tmp = new List<object?>();
                while (true)
                {
                    tmp.Add(Parse(elemType));
                    Token readToken = tokens.Dequeue();
                    if (readToken.Type == TokenType.CloseBracket)
                    {
                        if (type.IsArray)
                        {
                            Array arrayObj = Array.CreateInstance(elemType, tmp.Count);
                            for (int i = 0; i < tmp.Count; i++)
                            {
                                arrayObj.SetValue(tmp[i], i);
                            }
                            return arrayObj;
                        }
                        else
                        {
                            object? instance = Activator.CreateInstance(typeof(List<>).MakeGenericType(elemType));
                            MethodInfo? methodAdd;
                            if (instance != null && (methodAdd = instance.GetType().GetMethod("Add")) != null)
                            {
                                for (int i = 0; i < tmp.Count; i++)
                                {
                                    methodAdd.Invoke(instance, new object?[] { tmp[i] });
                                }
                                return instance;
                            }
                            else
                            {
                                throw new Exception("ArrayObject instance was not created");
                            }
                        }

                    }
                    else if (readToken.Type != TokenType.Semicolon)
                    {
                        throw new Exception($"Unexpected token {readToken.Type}: {reader.Str.Substring(token.StartIndex, token.Length)}");
                    }
                }
            }
            else
            {
                throw new Exception($"Invalid cast type: Array -> {toType}");
            }
        }

        private object GetStringObject(Token token, Type toType)
        {
            Dictionary<Type, Func<string, object>> stringConverters = new Dictionary<Type, Func<string, object>>
            {
                {typeof(string), (value) => value },
                {typeof(object), (value) => value },
                {typeof(char[]), (value) => value.ToCharArray() },
            };
            if (stringConverters.ContainsKey(toType))
            {
                return stringConverters[toType].Invoke(ExtractStringValue(token));
            }
            else
            {
                throw new Exception($"Invalid cast type: String -> {toType}");
            }

        }

        private object GetNumberObject(Token token, Type toType)
        {
            Type? underlyingType;
            if ((underlyingType = Nullable.GetUnderlyingType(toType)) != null)
            {
                toType = underlyingType;
            }
            try
            {
                if (toType.IsEnum)
                {
                    return Enum.ToObject(toType, Convert.ChangeType(reader.Str.Substring(token.StartIndex, token.Length), typeof(int)));
                }
                return Convert.ChangeType(reader.Str.Substring(token.StartIndex, token.Length), toType);
            }
            catch
            {
                throw new Exception($"Invalid cast type: Number -> {toType}");
            }
        }

        private object? GetNullObject(Token token, Type toType)
        {
            if (Nullable.GetUnderlyingType(toType) != null || type.IsClass)
            {
                return null;
            }
            else
            {
                throw new Exception($"Invalid cast type: Null -> {toType}");
            }

        }

        private object GetBoolObject(Token token, Type toType)
        {
            Dictionary<Type, Func<bool, object>> boolConverters = new Dictionary<Type, Func<bool, object>>
            {
                {typeof(bool), (value) => value },
                {typeof(object), (value) => value },
            };
            if (boolConverters.ContainsKey(toType))
            {
                return boolConverters[toType].Invoke(Convert.ToBoolean(reader.Str.Substring(token.StartIndex, token.Length)));
            }
            else
            {
                throw new Exception($"Invalid cast type: String -> {toType}");
            }
        }

        private string ExtractStringValue(Token token)
        {
            if (token.Type != TokenType.String || token.Length < 2)
                throw new Exception($"Invalid token {token.Type}: {reader.Str.Substring(token.StartIndex, token.Length)}");
            return Uri.UnescapeDataString(reader.Str.Substring(token.StartIndex + 1, token.Length - 2));
        }
    }
}
