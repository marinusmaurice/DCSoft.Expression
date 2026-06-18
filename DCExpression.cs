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
    /// Expression object; the top-level API type.
    /// </summary>
    [System.Runtime.InteropServices.ComVisible(false)]
    public class DCExpression
    {
        //private static Dictionary<string, DCExpression> _Instances 
        //    = new Dictionary<string, DCExpression>();
        ///// <summary>
        ///// Creates an object using the cache.
        ///// </summary>
        ///// <param name="text">Expression text</param>
        ///// <returns>The created object</returns>
        //public static DCExpression CreateUseBuffer(string text)
        //{
        //    if (text == null || text.Length == 0)
        //    {
        //        throw new ArgumentNullException("text");
        //    }
        //    if (_Instances.ContainsKey(text))
        //    {
        //        return _Instances[text];
        //    }
        //    DCExpression exp = new DCExpression(text);
        //    _Instances[text] = exp;
        //    return exp;
        //}
        ///// <summary>
        ///// Clears the internal cache.
        ///// </summary>
        //public static void ClearBuffer()
        //{
        //    _Instances.Clear();
        //}

        /// <summary>
        /// Initializes the object.
        /// </summary>
        public DCExpression()
        {
        }
        /// <summary>
        /// Initializes the object.
        /// </summary>
        /// <param name="txt">Expression text</param>
        public DCExpression(string txt)
        {
            this.Parse(txt);
        }

        private string _Text = null;
        /// <summary>
        /// Raw expression text.
        /// </summary>
        public string Text
        {
            get
            {
                return _Text;
            }
        }

        private DCExpressionItem _RootItem = null;
        /// <summary>
        /// Root expression item.
        /// </summary>
        public DCExpressionItem RootItem
        {
            get
            {
                return _RootItem;
            }
        }
        /// <summary>
        /// Evaluates the expression.
        /// </summary>
        /// <param name="context">The context object.</param>
        /// <returns>The evaluation result.</returns>
        public object Eval(IDCExpressionContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            if (this._RootItem == null)
            {
                throw new NullReferenceException("this.RootItem");
            }
            object result = this._RootItem.Eval(context);
            if (result is Array)
            {
                Array arr = (Array)result;
                if (arr.Length > 0)
                {
                    return arr.GetValue(0);
                }
                else
                {
                    return null;
                }
            }
            return result;
        }
        /// <summary>
        /// Converts to debug text.
        /// </summary>
        /// <returns></returns>
        public string ToDebugString()
        {
            if (this._RootItem == null)
            {
                return "";
            }
            StringBuilder str = new StringBuilder();
            str.AppendLine("Text:" + this.Text);
            str.AppendLine("------------------");
            this._RootItem.ToDebugString(0, str);
            return str.ToString();
        }
        /// <summary>
        /// Parses the expression.
        /// </summary>
        /// <param name="text"></param>
        public void Parse(string text)
        {
            this._Text = text;
            DCTokenList tokens = new DCTokenList(text);
            this._RootItem = new DCGroupExpressionItem();
            ParseItem(this._RootItem, tokens);
        }
        /// <summary>
        /// Parses expression items.
        /// </summary>
        /// <param name="rootItem"></param>
        /// <param name="tokens"></param>
        private void ParseItem(DCExpressionItem rootItem, DCTokenList tokens)
        {
            List<DCExpressionItem> items = new List<DCExpressionItem>();
            while (tokens.MoveNext())
            {
                DCExpressionItem newItem = null;
                DCToken token = tokens.Current;
                if (token.Type == CharType.Symbol)
                {
                    // Fix based on keywords And Or.
                    if (string.Equals(token.Text, "And", StringComparison.CurrentCultureIgnoreCase))
                    {
                        token.Type = CharType.MathOperator;
                        token.Text = "&&";
                    }
                    else if (string.Equals(token.Text, "or", StringComparison.CurrentCultureIgnoreCase))
                    {
                        token.Type = CharType.MathOperator;
                        token.Text = "||";
                    }
                }
                if (token.Type == CharType.Symbol)
                {
                    // Identifier
                    DCToken next = tokens.NextItem;
                    if (next != null && next.Type == CharType.CurLeft)
                    {
                        // Function call
                        tokens.MoveNext();
                        DCFunctionExpressionItem func = new DCFunctionExpressionItem();
                        func.Name = token.Text;
                        ParseItem(func, tokens);
                        newItem = func;
                        newItem.Priority = 0;
                    }
                    else
                    {
                        if (string.Compare(token.Text, "true", true) == 0)
                        {
                            // Boolean constant
                            newItem = new DCConstExpressionItem(true, DCValueType.Boolean);
                        }
                        else if (string.Compare(token.Text, "false", true) == 0)
                        {
                            // Boolean constant
                            newItem = new DCConstExpressionItem(false, DCValueType.Boolean);
                        }
                        else
                        {
                            double dbl = 0;
                            if (double.TryParse(token.Text, out dbl))
                            {
                                // Numeric constant
                                newItem = new DCConstExpressionItem(dbl, DCValueType.Number);
                            }
                            else
                            {
                                // Referenced variable
                                DCVariableExpressionItem var = new DCVariableExpressionItem();
                                var.Name = token.Text;
                                newItem = var;
                            }
                        }
                        newItem.Priority = 0;
                    }
                }
                else if (token.Type == CharType.StringConst)
                {
                    // String constant
                    var strV = token.Text;
                    if (strV != null && strV.Length >= 2)
                    {
                        if (strV[0] == '\'' || strV[0] == '"')
                        {
                            strV = strV.Substring(1, strV.Length - 2);
                        }
                    }
                    newItem = new DCConstExpressionItem(strV, DCValueType.String);
                }
                else if (token.Type == CharType.CurLeft)
                {
                    // Left parenthesis, perform grouping
                    DCGroupExpressionItem group = new DCGroupExpressionItem();
                    newItem = group;
                    newItem.Priority = 0;
                    ParseItem(group, tokens);
                }
                else if (token.Type == CharType.Spliter
                    || token.Type == CharType.CurRight)
                {
                    // Separator or right parenthesis, exit grouping
                    if (items == null || items.Count == 0)
                    {
                        throw new System.Exception("Invalid item group: " + this.Text);
                    }
                    if (items != null && items.Count > 0)
                    {
                        DCExpressionItem item = CollpaseItems(items);
                        rootItem.AddSubItem(item);
                    }
                    items = new List<DCExpressionItem>();
                    if (token.Type == CharType.CurRight)
                    {
                        //Exit grouping
                        break;
                    }
                    else
                    {
                        continue;
                    }
                }
                else if (token.Type == CharType.MathOperator
                    || token.Type == CharType.LogicOperator)
                {
                    // Operator

                    DCOperatorExpressionItem math = new DCOperatorExpressionItem();
                    math.Text = token.Text;
                    switch (token.Text)
                    {
                        case "+":
                            math.Operator = DCOperatorType.Plus;
                            math.Priority = 1;
                            break;
                        case "-":
                            math.Operator = DCOperatorType.Minus;
                            math.Priority = 1;
                            break;
                        case "*":
                            math.Operator = DCOperatorType.Multi;
                            math.Priority = 2;
                            break;
                        case "/":
                            math.Operator = DCOperatorType.Division;
                            math.Priority = 2;
                            break;
                        case ">":
                            math.Operator = DCOperatorType.Bigger;
                            math.Priority = 0;
                            math.IsLogicExpression = true;
                            break;
                        case ">=":
                            math.Operator = DCOperatorType.BiggerOrEqual;
                            math.Priority = 0;
                            math.IsLogicExpression = true;
                            break;
                        case "=":
                            math.Operator = DCOperatorType.Equal;
                            math.Priority = 0;
                            math.IsLogicExpression = true;
                            break;
                        case "==":
                            math.Operator = DCOperatorType.Equal;
                            math.Priority = 0;
                            math.IsLogicExpression = true;
                            break;
                        case "<":
                            math.Operator = DCOperatorType.Less;
                            math.Priority = 0;
                            math.IsLogicExpression = true;
                            break;
                        case "<=":
                            math.Operator = DCOperatorType.LessOrEqual;
                            math.Priority = 0;
                            math.IsLogicExpression = true;
                            break;
                        case "%":
                            math.Operator = DCOperatorType.Mod;
                            math.Priority = 2;
                            break;
                        case "||":
                            math.Operator = DCOperatorType.Or;
                            math.Priority = -1;
                            math.IsLogicExpression = true;
                            break;
                        case "&&":
                            math.Operator = DCOperatorType.And;
                            math.Priority = -1;
                            math.IsLogicExpression = true;
                            break;
                        case "!=":
                            math.Operator = DCOperatorType.Unequal;
                            math.Priority = 0;
                            math.IsLogicExpression = true;
                            break;
                        default:
                            throw new NotSupportedException("Invalid operator: " + token.Text);
                    }
                    newItem = math;
                }
                else
                {
                    throw new NotSupportedException(token.Type + ":" + token.Text);
                }
                items.Add(newItem);
            }//while
            DCExpressionItem item2 = CollpaseItems(items);
            if (item2 != null)
            {
                rootItem.AddSubItem(item2);
            }
        }

        /// <summary>
        /// Collapses the expression item list.
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        private DCExpressionItem CollpaseItems(List<DCExpressionItem> items)
        {
            if (items.Count == 1)
            {
                return items[0];
            }
            if (items.Count == 0)
            {
                return null;
            }
            // Special handling of subtraction
            for (int iCount = 0; iCount < items.Count; iCount++)
            {
                if (items[iCount] is DCOperatorExpressionItem && iCount < items.Count - 1)
                {
                    DCOperatorExpressionItem item = (DCOperatorExpressionItem)items[iCount];
                    if (item.Operator == DCOperatorType.Minus)
                    {
                        bool isNegative = false;
                        if (iCount == 0)
                        {
                            //  At the first position, must be a negation operation.
                            isNegative = true;
                        }
                        else
                        {
                            DCExpressionItem preItem = items[iCount - 1];
                            if (preItem is DCOperatorExpressionItem)
                            {
                                DCOperatorExpressionItem preOper = (DCOperatorExpressionItem)preItem;
                                if (preOper.IsLogicExpression)
                                {
                                    // The preceding element is a logic expression, it is a negation operation.
                                    isNegative = true;
                                }
                            }
                            else if (preItem is DCGroupExpressionItem)
                            {
                                // The preceding is a group expression item, it is a negation operation.
                                isNegative = true;
                            }
                        }
                        if (isNegative)
                        {
                            // Convert subtraction to negation
                            item.Operator = DCOperatorType.Negative;
                            item.Rigth = items[iCount + 1];
                            items.RemoveAt(iCount + 1);
                            item.Priority = 0;
                            item.Collapsed = true;
                            //iCount--;
                        }
                    }//if
                }//if
            }//for
            // Multiple iterations here, each promotes the highest-priority operator item.
            // Generally only one operator item remains at the end.
            while (items.Count > 1)
            {
                DCOperatorExpressionItem maxPriorityItem = null;
                int maxIndex = -1;
                int len = items.Count;
                // Find the operator with the highest priority.
                for (int iCount = 0; iCount < len; iCount++)
                {
                    DCExpressionItem item = items[iCount];
                    if (item.Collapsed == false && item is DCOperatorExpressionItem)
                    {
                        if (maxPriorityItem == null || maxPriorityItem.Priority < item.Priority)
                        {
                            maxPriorityItem = (DCOperatorExpressionItem)item;
                            maxIndex = iCount;
                        }
                    }
                }//for
                if (maxPriorityItem == null)
                {
                    // No operator found, exit loop.
                    break;
                }

                if (maxIndex < items.Count - 1)
                {
                    // Absorb the right item
                    maxPriorityItem.Rigth = items[maxIndex + 1];
                    items.RemoveAt(maxIndex + 1);
                }
                if (maxIndex > 0)
                {
                    // Absorb the left item
                    maxPriorityItem.Left = items[maxIndex - 1];
                    items.RemoveAt(maxIndex - 1);
                }
                //if (maxPriorityItem.Operator == DCOperatorType.Minus
                //    && maxPriorityItem.Left == null
                //    && maxPriorityItem.Rigth != null)
                //{
                //    // Convert subtraction to negation
                //    maxPriorityItem.Operator = DCOperatorType.Negative;
                //}
                // Mark operator as already processed.
                maxPriorityItem.Collapsed = true;
            }//while
            if (items.Count > 0)
            {
                return items[0];
            }
            return null;
        }

        /// <summary>
        /// Clones the object.
        /// </summary>
        /// <returns>The clone.</returns>
        public DCExpression Clone()
        {
            DCExpression result = (DCExpression)this.MemberwiseClone();
            if (this._RootItem != null)
            {
                result._RootItem = this._RootItem.Clone();
            }
            return result;
        }

        internal static bool ToBoolean(object obj, bool defaultValue = false)
        {
            if (obj == null || DBNull.Value.Equals(obj))
            {
                return defaultValue;
            }
            if (obj is bool)
            {
                return (bool)obj;
            }
            if (obj is string)
            {
                if (string.Compare((string)obj, "true", true) == 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            if (obj is float)
            {
                float v = (float)obj;
                if (float.IsNaN(v) || v == 0)
                {
                    return false;
                }
                return true;
            }
            if (obj is double)
            {
                double v = (double)obj;
                if (double.IsNaN(v) || v == 0)
                {
                    return false;
                }
                return true;
            }
            if (obj is int)
            {
                int v = (int)obj;
                if (v == NaN || v == 0)
                {
                    return false;
                }
                return true;
            }
            if (obj.GetType().IsValueType)
            {
                try
                {
                    int v = Convert.ToInt32(obj);
                    if (v == NaN || v == 0)
                    {
                        return false;
                    }
                    return true;
                }
                catch (System.Exception ext)
                {
                    return defaultValue;
                }
            }
            if (typeof(Array).IsInstanceOfType(obj))
            {
                Array arr = (Array)obj;
                if (arr.Length > 0)
                {
                    object v2 = arr.GetValue(0);
                    return ToBoolean(v2, defaultValue);
                }
            }
            return defaultValue;
        }

        /// <summary>
        /// Represents a non-numeric value.
        /// </summary>
        internal const int NaN = 2147439148;
        /// <summary>
        /// Converts an object to a string.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        internal static string ToString(object obj)
        {
            if (obj == null || DBNull.Value.Equals(obj))
            {
                return null;
            }
            if (obj.GetType().IsArray)
            {
                StringBuilder str = new StringBuilder();
                foreach (object item in ((System.Collections.IEnumerable)obj))
                {
                    if (item != null)
                    {
                        string v = ToString(item);
                        if (string.IsNullOrEmpty(v) == false)
                        {
                            str.Append(v);
                        }
                    }
                }
                return str.ToString();
            }
            return Convert.ToString(obj);
        }
        /// <summary>
        /// Converts an object to a double.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        internal static double ToDouble(object obj, double defaultValue = 0)
        {
            if (obj is float)
            {
                if (float.IsNaN((float)obj))
                {
                    return defaultValue;
                }
                else
                {
                    return (double)obj;
                }
            }
            if (obj is double)
            {
                if (double.IsNaN((double)obj))
                {
                    return defaultValue;
                }
                else
                {
                    return (double)obj;
                }
            }
            if (obj != null && obj.GetType().IsArray)
            {
                foreach (object item in ((System.Collections.IEnumerable)obj))
                {
                    if (item != null)
                    {
                        double v2 = ToDouble(item, double.NaN);
                        if (double.IsNaN(v2) == false)
                        {
                            return v2;
                        }
                    }
                }
                return defaultValue;
            }
            if (obj is string)
            {
                string s = (string)obj;
                if (string.IsNullOrEmpty(s))
                {
                    return defaultValue;
                }
                double v2 = defaultValue;
                if (double.TryParse(s, out v2))
                {
                    return v2;
                }
                else
                {
                    return defaultValue;
                }
            }
            double dbl = Convert.ToDouble(obj);
            if (double.IsNaN(dbl))
            {
                return defaultValue;
            }
            else
            {
                return dbl;
            }
        }
    }
}