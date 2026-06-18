/*

    DCSoft.Expression Numerical Expression Engine

 Nanjing Duchang Information Technology Co., Ltd. 2018 All Rights Reserved
 Company website: http://www.dcwriter.cn

 */
using System;
using System.Collections.Generic;
using System.Text;

namespace DCSoft.Expression
{
    /// <summary>
    /// Token object.
    /// </summary>
    [System.Runtime.InteropServices.ComVisible(false)]
    internal class DCToken
    {
        public CharType Type = CharType.None;
        public string Text = null;
        public StringBuilder _Str = new StringBuilder();
        public override string ToString()
        {
            return this.Type + "\t:" + this.Text;
        }
        //public int Level
        //{
        //    get
        //    {
        //        if (this.Text == "*"
        //            || this.Text == "/"
        //            || this.Text == "%")
        //        {
        //            return 3;
        //        }
        //        if (this.Text == "+" || this.Text == "-")
        //        {
        //            return 2;
        //        }
        //        if (this.Type == CharType.MathOperator)
        //        {
        //            return 1;
        //        }
        //        if (this.Type == CharType.Symbol || this.Type == CharType.StringConst)
        //        {
        //            return 0;
        //        }
        //        if (this.Type == CharType.Spliter)
        //        {
        //            return -1;
        //        }

        //        return 0;
        //    }
        //}
    }

    /// <summary>
    /// Token list.
    /// </summary>
    [System.Runtime.InteropServices.ComVisible(false)]
    internal class DCTokenList : List<DCToken>
    {
        public DCTokenList(string txt)
        {
            this.Parse(txt);
        }
        
        private int _Position = -1;
        public DCToken Current
        {
            get
            {
                if (_Position >= 0 && _Position < this.Count)
                {
                    return this[_Position];
                }
                return null;
            }
        }
        public DCToken NextItem
        {
            get
            {
                if (_Position >= 0 && this._Position < this.Count - 1)
                {
                    return this[_Position + 1];
                }
                return null;
            }
        }
        public void ResetPosition()
        {
            this._Position = -1;
        }
        public bool MoveNext()
        {
            this._Position++;
            if (this._Position >= 0 && this._Position < this.Count)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Parses the expression text into tokens.
        /// </summary>
        /// <returns></returns>
        public void Parse(string text)
        {
            this.Clear();
            if (text == null || text.Length == 0)
            {
                return;
            }

            DCToken currentToken = null;
            StringMode sm = StringMode.None;

            for (int position = 0; position < text.Length; position++)
            {
                char c = text[position];
                if (sm == StringMode.SingleQuotes || sm == StringMode.DoubleQuotes)
                {
                    // Inside a string definition
                    if (c == '\\')
                    {
                        // Start of escape sequence
                        const string EigthDigs = "01234567";
                        const string HexDigs = "0123456789ABCDEF";
                        char nextC = NextChar(text, position);
                        if (EigthDigs.IndexOf(nextC) >= 0)
                        {
                            // Three-digit octal number
                            string v = NextChars(text, position, 3);
                            if (v != null && v.Length == 3)
                            {
                                position += 3;
                                int num = ParseNumber(v, EigthDigs);
                                currentToken._Str.Append((char)num);
                            }
                            else
                            {
                                throw new System.Exception("Insufficient length: " + text);
                            }
                        }
                        else if (nextC == 'x')
                        {
                            // Two hex digits
                            position++;
                            string v = NextChars(text, position, 2);
                            if (v != null && v.Length == 2)
                            {
                                v = v.ToUpper();
                                position += 2;
                                int num = ParseNumber(v, HexDigs);
                                currentToken._Str.Append((char)num);
                            }
                                 else
                                {
                                    throw new System.Exception("Insufficient length: " + text);
                                }
                            }
                            else if (nextC == 'a')
                        {
                            currentToken._Str.Append('\a');
                        }
                        else if (nextC == 'b')
                        {
                            currentToken._Str.Append('\b');
                        }
                        else if (nextC == 'n')
                        {
                            currentToken._Str.Append('\n');
                        }
                        else if (nextC == 'r')
                        {
                            currentToken._Str.Append('\r');
                        }
                        else if (nextC == 'v')
                        {
                            currentToken._Str.Append('\v');
                        }
                        else if (nextC == '"')
                        {
                            currentToken._Str.Append('"');
                        }
                        else if (nextC == '\'')
                        {
                            currentToken._Str.Append('\'');
                        }
                        else
                        {
                            throw new System.Exception("Unsupported escape: " + nextC);
                        }
                    }
                    else if (sm == StringMode.SingleQuotes && c == '\'')
                    {
                        // End single-quoted string definition
                        currentToken._Str.Append(c);
                        currentToken = null;
                        sm = StringMode.None;
                    }
                    else if (sm == StringMode.DoubleQuotes && c == '"')
                    {
                        // End double-quoted string definition
                        currentToken._Str.Append(c);
                        currentToken = null;
                        sm = StringMode.None;
                    }
                    else
                    {
                        // Add normal character
                        currentToken._Str.Append(c);
                    }
                }
                else
                {
                    // Not inside a string definition
                    if (c == '\'')
                    {
                        // Start single-quoted string definition
                        currentToken = new DCToken();
                        currentToken.Type = CharType.StringConst;
                        currentToken._Str.Append(c);
                        sm = StringMode.SingleQuotes;
                        this.Add(currentToken);
                    }
                    else if (c == '"')
                    {
                        // Start double-quoted string definition
                        currentToken = new DCToken();
                        currentToken.Type = CharType.StringConst;
                        currentToken._Str.Append(c);
                        sm = StringMode.DoubleQuotes;
                        this.Add(currentToken);
                    }
                    else if (char.IsWhiteSpace(c))
                    {
                        // Skip whitespace characters
                        currentToken = null;
                        for (; position < text.Length; position++)
                        {
                            if (char.IsWhiteSpace(text[position]) == false)
                            {
                                position--;
                                break;
                            }
                        }
                    }
                    else
                    {
                        // Regular character
                        if (c == '(')
                        {
                            currentToken = new DCToken();
                            currentToken._Str.Append(c);
                            currentToken.Type = CharType.CurLeft;
                            this.Add(currentToken);
                            currentToken = null;
                        }
                        else if (c == ')')
                        {
                            currentToken = new DCToken();
                            currentToken._Str.Append(c);
                            currentToken.Type = CharType.CurRight;
                            this.Add(currentToken);
                            currentToken = null;
                        }
                        else
                        {
                            CharType ct = GetChartType(c);

                            if (currentToken == null || currentToken.Type != ct)
                            {
                                currentToken = new DCToken();
                                currentToken.Type = ct;
                                this.Add(currentToken);
                            }
                            currentToken._Str.Append(c);
                        }
                    }
                }
            }//for
            if (currentToken != null && currentToken._Str.Length > 0)
            {
                if (this.Contains(currentToken) == false)
                {
                    this.Add(currentToken);
                }
            }
            foreach (var item in this)
            {
                if (item._Str != null)
                {
                    item.Text = item._Str.ToString();
                    item._Str = null;
                }
            }
        }

        /// <summary>
        /// Converts text to a number.
        /// </summary>
        /// <param name="txt">The text.</param>
        /// <param name="digs">Digit characters.</param>
        /// <returns>The converted number.</returns>
        private int ParseNumber(string txt, string digs)
        {
            int v = 0;
            for (int iCount = 0; iCount < txt.Length; iCount++)
            {
                int i = digs.IndexOf(txt[iCount]);
                if (i < 0)
                {
                    throw new System.InvalidCastException(digs + ":" + txt);
                }
                v = v * digs.Length + i;
            }
            return v;
        }
        /// <summary>
        /// Gets the next character.
        /// </summary>
        /// <returns></returns>
        private char NextChar(string text, int position)
        {
            if (position < text.Length - 1)
            {
                return text[position + 1];
            }
            return char.MinValue;
        }

        private string NextChars(string text, int position, int len)
        {
            if (position < text.Length - len)
            {
                string v = text.Substring(position, len);
                return v;
            }
            return null;
        }


        /// <summary>
        /// Gets the character type.
        /// </summary>
        /// <param name="c">The character.</param>
        /// <param name="isVB">Whether it is VB syntax.</param>
        /// <returns>The character type.</returns>
        private CharType GetChartType(char c)
        {
            if (c == '$')
            {
                return CharType.Symbol;
            }

            if (c == '(')
            {
                return CharType.CurLeft;
            }
            if (c == ')')
            {
                return CharType.CurRight;
            }
            // For compatibility, square brackets are not supported.
            //if (c == '[')
            //{
            //    return CharType.SquLeft;
            //}
            //if( c == ']')
            //{
            //    return CharType.SquRight;
            //}
            if (c == ',')
            {
                return CharType.Spliter;
            }
            if (c == '+' 
                || c == '-'
                || c == '*'
                || c == '/'
                || c =='%'
                || c == '\\')
            {
                return CharType.MathOperator;
            }
            if ( c == '&' || c == '^' || c == '|'  || c == '='
                || c == '>' || c == '<')
            {
                return CharType.LogicOperator;
            }
            else if (char.IsWhiteSpace(c))
            {
                return CharType.Whitespace;
            }
            if (c == ':'
                || c == '!'
                || c == '.'
                || char.IsLetterOrDigit(c)
                || char.IsSymbol(c)
                || c == '[' || c == ']')
            {
                return CharType.Symbol;
            }
            return CharType.Symbol;
        }
    }


    internal enum StringMode
    {
        /// <summary>
        /// None.
        /// </summary>
        None,
        /// <summary>
        /// Single-quoted string.
        /// </summary>
        SingleQuotes,
        /// <summary>
        /// Double-quoted string.
        /// </summary>
        DoubleQuotes
    }

    internal enum CharType
    {
        None,
        /// <summary>
        /// Identifier.
        /// </summary>
        Symbol,
        /// <summary>
        /// Math operator.
        /// </summary>
        MathOperator,
        /// <summary>
        /// Logic operator.
        /// </summary>
        LogicOperator,
        /// <summary>
        /// Left parenthesis.
        /// </summary>
        CurLeft,
        /// <summary>
        /// Right parenthesis.
        /// </summary>
        CurRight,
        /// <summary>
        /// Left square bracket.
        /// </summary>
        SquLeft,
        /// <summary>
        /// Right square bracket.
        /// </summary>
        SquRight,
        /// <summary>
        /// Whitespace.
        /// </summary>
        Whitespace,
        /// <summary>
        /// Separator.
        /// </summary>
        Spliter,
        /// <summary>
        /// String constant.
        /// </summary>
        StringConst
    }

}
