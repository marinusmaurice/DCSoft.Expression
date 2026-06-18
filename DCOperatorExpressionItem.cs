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
    /// Operator expression item.
    /// </summary>
    [System.Runtime.InteropServices.ComVisible(false)]
    public class DCOperatorExpressionItem : DCExpressionItem
    {
        public DCOperatorExpressionItem()
        {
        }

        private string _Text = null;
        /// <summary>
        /// Text.
        /// </summary>
        public string Text
        {
            get
            {
                return _Text;
            }
            set
            {
                _Text = value;
            }
        }
        private DCOperatorType _Operator = DCOperatorType.None;
        /// <summary>
        /// Operator type.
        /// </summary>
        public DCOperatorType Operator
        {
            get
            {
                return _Operator;
            }
            set
            {
                _Operator = value;
            }
        }

        private DCExpressionItem _Left = null;
        /// <summary>
        /// Left element.
        /// </summary>
        public DCExpressionItem Left
        {
            get
            {
                return _Left;
            }
            set
            {
                _Left = value;
            }
        }

        private DCExpressionItem _Right = null;
        /// <summary>
        /// Right element.
        /// </summary>
        public DCExpressionItem Rigth
        {
            get
            {
                return _Right;
            }
            set
            {
                _Right = value;
            }
        }

        private bool _IsLogicExpression = false;
        /// <summary>
        /// Whether this is a logical operation.
        /// </summary>
        public bool IsLogicExpression
        {
            get
            {
                return this._IsLogicExpression;
            }
            set
            {
                this._IsLogicExpression = value;
            }
        }

        /// <summary>
        /// Evaluates the expression.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override object Eval(IDCExpressionContext context)
        {
            switch (this.Operator)
            {
                #region Logical operations

                case DCOperatorType.And:
                    {
                        //Logical AND
                        bool v1 = GetBooleanValue(this.Left, context, false);
                        if( v1 == false )
                        {
                            return false;
                        }
                        bool v2 = GetBooleanValue(this.Rigth, context, false);
                        return v2;
                    }
                case DCOperatorType.Or:
                    {
                        //Logical OR
                        bool v1 = GetBooleanValue(this.Left, context, false);
                        if( v1 )
                        {
                            return true;
                        }
                        bool v2 = GetBooleanValue(this.Rigth, context, false);
                        return v2;
                        //return v1 && v2;
                    }
                #endregion
                #region Numeric comparison

                case DCOperatorType.Bigger:
                    {
                        // Greater than
                        return GetEqualResult(context) > 0;
                    }
                case DCOperatorType.BiggerOrEqual:
                    {
                        // Greater than or equal
                        return GetEqualResult(context) >= 0;
                    }
                case DCOperatorType.Equal:
                    {
                        // Equal
                        return GetEqualResult(context) == 0;
                    }
                case DCOperatorType.Less:
                    {
                        // Less than
                        return GetEqualResult(context) < 0;
                    }
                case DCOperatorType.LessOrEqual:
                    {
                        // Less than or equal
                        return GetEqualResult(context) <= 0;
                    }
                case DCOperatorType.Unequal:
                    {
                        // Not equal
                        return GetEqualResult(context) != 0;
                    }

                #endregion

                #region Math operations

                case DCOperatorType.Plus:
                    {
                        // Addition
                        double v1 = GetDoubleValue(this.Left, context, 0);
                        double v2 = GetDoubleValue(this.Rigth, context, 0);
                        return v1 + v2;
                    }
                case DCOperatorType.Minus:
                    {
                        // Subtraction
                        double v1 = GetDoubleValue(this.Left, context, 0);
                        double v2 = GetDoubleValue(this.Rigth, context, 0);
                        return v1 - v2;
                    }
                case DCOperatorType.Multi:
                    {
                        // Multiplication
                        double v1 = GetDoubleValue(this.Left, context, 0);
                        double v2 = GetDoubleValue(this.Rigth, context, 0);
                        return v1 * v2;
                    }
                case DCOperatorType.Negative:
                    {
                        // Negative number
                        double v1 = 0;
                        if (this.Rigth != null)
                        {
                            v1 = GetDoubleValue(this.Rigth, context, 0);
                        }
                        else if (this.Left != null)
                        {
                            v1 = GetDoubleValue(this.Left, context, 0);
                        }
                        return -v1;
                    }
                case DCOperatorType.Division:
                    {
                        // Division
                        double v1 = GetDoubleValue(this.Left, context, 0);
                        double v2 = GetDoubleValue(this.Rigth, context, 0);
                        if (v2 == 0)
                        {
                            return double.NaN;
                        }
                        else
                        {
                            return v1 / v2;
                        }
                    }
                case DCOperatorType.Mod:
                    {
                        // Modulo
                        double v1 = GetDoubleValue(this.Left, context, 0);
                        double v2 = GetDoubleValue(this.Rigth, context, 0);
                        if (v2 == 0)
                        {
                            return double.NaN;
                        }
                        else
                        {
                            return v1 % v2;
                        }
                    }

                #endregion

                default:
                    throw new NotSupportedException(this.Operator.ToString());
            }
         }

        /// <summary>
        /// Gets the comparison result.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private int GetEqualResult(IDCExpressionContext context)
        {
            object v1 = this.Left == null ? null : this.Left.Eval(context);
            object v2 = this.Rigth == null ? null : this.Rigth.Eval(context);
            if (v1 == v2)
            {
                return 0;
            }
            if ((v1 is int || v1 is double) && (v2 is int || v2 is double))
            {
                // Explicitly compare numbers
                double d1 = Convert.ToDouble(v1);
                double d2 = Convert.ToDouble(v2);
                return d1.CompareTo(d2);
            }
            if (v1 is string && v2 is string)
            {
                // Explicitly compare strings
                return string.Compare((string)v1, (string)v2);
            }

            if (v1 is float && v2 is float)
            {
                return ((float)v1).CompareTo((float)v2);
            }
            if (v1 is double && v2 is double)
            {
                return ((double)v1).CompareTo((double)v2);
            }

            bool hasContent1 = v1 != null && DBNull.Value.Equals(v1) == false ;
            bool hasContent2 = v2 != null && DBNull.Value.Equals(v2) == false ;

            if (hasContent1 && hasContent2  )
            {
                try
                {
                    // Both values are non-null
                    var t1 = v1.GetType();
                    var t2 = v2.GetType();
                    if (t1 == t2)
                    {
                        // Same type, compare directly
                        return System.Collections.Comparer.Default.Compare(v1, v2);
                    }
                    // Different types, use a common data type
                    var targetVT = (DCValueType)Math.Min((int)GetValueType(t1), (int)GetValueType(t2));
                    if (targetVT == DCValueType.Boolean)
                    {
                        bool b1 = Convert.ToBoolean(v1);
                        bool b2 = Convert.ToBoolean(v2);
                        return b1.CompareTo(b2);
                    }
                    else if (targetVT == DCValueType.Number)
                    {
                        double dbl1 = Convert.ToDouble(v1);
                        double dbl2 = Convert.ToDouble(v2);
                        return dbl1.CompareTo(dbl2);
                    }
                    else
                    {
                        string str1 = Convert.ToString(v1);
                        string str2 = Convert.ToString(v2);
                        return str1.CompareTo(str2);
                    }
                }
                catch (System.Exception ext)
                {
                    System.Diagnostics.Debug.WriteLine(ext.ToString());
                }
            }
            return hasContent1.CompareTo(hasContent2);
        }

        private DCValueType GetValueType(Type t)
        {
            if (t == typeof(string))
            {
                return DCValueType.String;
            }
            if (t == typeof(bool))
            {
                return DCValueType.Boolean;
            }
            if (t.IsValueType)
            {
                return DCValueType.Number;
            }
            return DCValueType.Number;
        }

        private double GetDoubleValue(DCExpressionItem item , IDCExpressionContext context , double defaultValue =0 )
        {
            if (item == null)
            {
                return defaultValue;
            }
            object v = item.Eval(context);
            return DCExpression.ToDouble(v , defaultValue);
        }
        private bool GetBooleanValue(DCExpressionItem item, IDCExpressionContext context , bool defaultValue = false )
        {
            if (item == null)
            {
                return defaultValue;
            }
            object v = item.Eval(context);
            return DCExpression.ToBoolean(v);
        }
        private string GetStringValue(DCExpressionItem item, IDCExpressionContext context )
        {
            if (item == null)
            {
                return null;
            }
            object v = item.Eval(context);
            return DCExpression.ToString(v);
        }

        internal override void AddSubItem(DCExpressionItem item)
        {
            if (this._Left == null)
            {
                this._Left = item;
            }
            else if (this._Right == null)
            {
                this._Right = item;
            }
            else
            {
                throw new NotSupportedException("Cannot add more than 2 sub-items.");
            }
        }
        public override DCExpressionItem Clone()
        {
            DCOperatorExpressionItem result = (DCOperatorExpressionItem)this.MemberwiseClone();
            if (this._Left != null)
            {
                result._Left = this._Left.Clone();
            }
            if (this._Right != null)
            {
                result._Right = this._Right.Clone();
            }
            return result;
        }
        public override void ToDebugString(int indentLevel, StringBuilder str)
        {
            string preFix = new string(' ', indentLevel * 3);
            str.AppendLine(preFix + this.Operator + "(" + this.Text + ")" + ( this.IsLogicExpression ? "[logic]":""));
            if (this.Left != null)
            {
                str.AppendLine(preFix + " >Left:");
                this.Left.ToDebugString(indentLevel + 1, str);
            }
            if (this.Rigth != null)
            {
                str.AppendLine(preFix + " >Right:");
                this.Rigth.ToDebugString(indentLevel + 1, str);
            }
        }
        public override string ToString()
        {
            return this.Operator.ToString() + ":" + this.Text ;
        }
    }

    /// <summary>
    /// Operator type.
    /// </summary>
    [System.Runtime.InteropServices.ComVisible(false)]
    public enum DCOperatorType
    {
        /// <summary>
        /// Invalid operation.
        /// </summary>
        None,
        /// <summary>
        /// Addition.
        /// </summary>
        Plus,
        /// <summary>
        /// Subtraction.
        /// </summary>
        Minus,
        /// <summary>
        /// Multiplication.
        /// </summary>
        Multi,
        /// <summary>
        /// Division.
        /// </summary>
        Division,
        /// <summary>
        /// Modulo.
        /// </summary>
        Mod,
        /// <summary>
        /// Logical AND.
        /// </summary>
        And,
        /// <summary>
        /// Logical OR.
        /// </summary>
        Or,
        /// <summary>
        /// Greater than.
        /// </summary>
        Bigger,
        /// <summary>
        /// Greater than or equal.
        /// </summary>
        BiggerOrEqual,
        /// <summary>
        /// Less than.
        /// </summary>
        Less ,
        /// <summary>
        /// Less than or equal.
        /// </summary>
        LessOrEqual,
        /// <summary>
        /// Equal.
        /// </summary>
        Equal,
        /// <summary>
        /// Not equal.
        /// </summary>
        Unequal,
        /// <summary>
        /// Negate.
        /// </summary>
        Negative
    }
}
