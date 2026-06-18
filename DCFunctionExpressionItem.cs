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
    /// Function expression item.
    /// </summary>
    [System.Runtime.InteropServices.ComVisible(false)]
    public class DCFunctionExpressionItem : DCExpressionItem
    {
        public DCFunctionExpressionItem()
        {
        }

        private string _Name = null;
        /// <summary>
        /// Name.
        /// </summary>
        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                _Name = value;
            }
        }

        private DCExpressoinItemList _Parameters = new DCExpressoinItemList();
        /// <summary>
        /// Parameter list.
        /// </summary>
        public DCExpressoinItemList Parameters
        {
            get
            {
                return _Parameters;
            }
            set
            {
                _Parameters = value;
            }
        }
        /// <summary>
        /// Evaluates the expression.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override object Eval(IDCExpressionContext context)
        {
            object[] pvs = null;
            if (_Parameters != null && _Parameters.Count > 0)
            {
                pvs = new object[this._Parameters.Count];
                for (int iCount = 0; iCount < this._Parameters.Count; iCount++)
                {
                    pvs[iCount] = this._Parameters[iCount].Eval(context);
                }
            }
            object result = context.ExecuteFunction(this.Name, pvs);
            return result;
        }

        internal override void AddSubItem(DCExpressionItem item)
        {
            if (_Parameters == null)
            {
                _Parameters = new DCExpressoinItemList();
            }
            _Parameters.Add(item);
        }

        public override DCExpressionItem Clone()
        {
            DCFunctionExpressionItem item = (DCFunctionExpressionItem)this.MemberwiseClone();
            if (this._Parameters != null)
            {
                item._Parameters = this._Parameters.CloneDeeply();
            }
            return item;
        }
        public override void ToDebugString(int indentLevel, StringBuilder str)
        {
            string preFix = new string(' ', indentLevel * 3);
            if (this.Parameters != null && this.Parameters.Count > 0)
            {
                str.AppendLine(preFix + "function " + this.Name);
                str.AppendLine(preFix + "(");
                foreach (var item in this.Parameters)
                {
                    item.ToDebugString(indentLevel + 1, str);
                }
                str.AppendLine(preFix + ")");
            }
            else
            {
                str.AppendLine(preFix + this.Name + "( )");
            }
        }
        public override string ToString()
        {
            return "function " + this.Name + "( )";
        }
    }
    /// <summary>
    /// Function type.
    /// </summary>
    [System.Runtime.InteropServices.ComVisible(false)]
    public enum DCFunctionType
    {
        /// <summary>
        /// String function.
        /// </summary>
        String,
        /// <summary>
        /// Numeric function.
        /// </summary>
        Number,
        /// <summary>
        /// Aggregate function.
        /// </summary>
        Group
    }
}
