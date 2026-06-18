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
    /// Constant expression item.
    /// </summary>
    [System.Runtime.InteropServices.ComVisible( false )]
    public class DCConstExpressionItem : DCExpressionItem
    {
        public DCConstExpressionItem()
        {
        }
        /// <summary>
        /// Initializes the object.
        /// </summary>
        /// <param name="v">The value.</param>
        /// <param name="vt">The data type.</param>
        public DCConstExpressionItem(object v, DCValueType vt)
        {
            this._ValueType = vt;
            if (vt == DCValueType.String)
            {
                this._StringValue = (string)v;
            }
            else if (vt == DCValueType.Number)
            {
                this._NumberValue = (double)v;
            }
            else if (vt == DCValueType.Boolean)
            {
                this._BooleanValue = (bool)v;
            }
        }
        private string _StringValue = null;
        /// <summary>
        /// String value.
        /// </summary>
        public string StringValue
        {
            get
            {
                return _StringValue;
            }
            set
            {
                _StringValue = value;
            }
        }

        private bool _BooleanValue = false;
        /// <summary>
        /// Boolean value.
        /// </summary>
        public bool BooleanValue
        {
            get
            {
                return _BooleanValue;
            }
            set
            {
                _BooleanValue = value;
            }
        }

        private double _NumberValue = 0;
        /// <summary>
        /// Numeric value.
        /// </summary>
        public double NumberValue
        {
            get
            {
                return _NumberValue;
            }
            set
            {
                _NumberValue = value;
            }
        }
        private DCValueType _ValueType = DCValueType.String;
        /// <summary>
        /// Data type.
        /// </summary>
        public DCValueType ValueType
        {
            get
            {
                return _ValueType;
            }
            set
            {
                _ValueType = value;
            }
        }
        /// <summary>
        /// Evaluates the expression.
        /// </summary>
        /// <param name="context">The context object.</param>
        /// <returns>The evaluation result.</returns>
        public override object Eval(IDCExpressionContext context)
        {
            if (this.ValueType == DCValueType.String)
            {
                return this._StringValue;
            }
            if (this.ValueType == DCValueType.Number)
            {
                return this._NumberValue;
            }
            if (this.ValueType == DCValueType.Boolean)
            {
                return this._BooleanValue;
            }
            throw new NotSupportedException(this.ValueType.ToString());
        }
        /// <summary>
        /// Outputs debug text.
        /// </summary>
        /// <param name="indentLevel"></param>
        /// <param name="str"></param>
        public override void ToDebugString(int indentLevel, StringBuilder str)
        {
            string txt = new string(' ', indentLevel * 3);
            if (this.ValueType == DCValueType.Number)
            {
                txt = txt + "Number:" + this.NumberValue;
            }
            else if (this.ValueType == DCValueType.String)
            {
                txt = txt + "String:" + this.StringValue;
            }
            else
            {
                txt = txt + "Boolean:" + this.BooleanValue;
            }
            str.AppendLine(txt);
        }
        public override string ToString()
        {
            if (this.ValueType == DCValueType.Number)
            {
                return "Number:" + this.NumberValue;
            }
            else if (this.ValueType == DCValueType.String)
            {
                return "String:" + this.StringValue;
            }
            else if (this.ValueType == DCValueType.Boolean)
            {
                return "Boolean:" + this.BooleanValue;
            }
            return this.ValueType.ToString();
        }
    }

    
}
