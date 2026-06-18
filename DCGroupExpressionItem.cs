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
    /// Expression item group.
    /// </summary>
    [System.Runtime.InteropServices.ComVisible(false)]
    public class DCGroupExpressionItem : DCExpressionItem
    {
        public DCGroupExpressionItem()
        {
 
        }

        private DCExpressoinItemList _Items = new DCExpressoinItemList();
        /// <summary>
        /// Sub-item list.
        /// </summary>
        public DCExpressoinItemList Items
        {
            get
            {
                return _Items;
            }
            set
            {
                _Items = value;
            }
        }
        /// <summary>
        /// Evaluates the expression.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override object Eval(IDCExpressionContext context)
        {
            if (this.Items != null && this.Items.Count > 0)
            {
                return this.Items[0].Eval(context);
            }
            return null;
        }

        internal override void AddSubItem(DCExpressionItem item)
        {
            if (this._Items == null)
            {
                this._Items = new DCExpressoinItemList();
            }
            this._Items.Add(item);
        }

        public override DCExpressionItem Clone()
        {
            DCGroupExpressionItem resut = (DCGroupExpressionItem)this.MemberwiseClone();
            if (this._Items != null)
            {
                resut._Items = this._Items.CloneDeeply();
            }
            return resut;
        }

        public override void ToDebugString(int indentLevel, StringBuilder str)
        {
            string preFix = new string(' ', indentLevel * 3);
            if (this._Items != null && this._Items.Count > 0)
            {
                str.AppendLine(preFix + "(");
                foreach (var item in this._Items)
                {
                    item.ToDebugString(indentLevel + 1, str);
                }
                str.AppendLine(preFix + ")");
            }
            else
            {
                str.AppendLine(preFix + "( )");
            }
        }
        public override string ToString()
        {
            return "Group()";
        }
    }
}
