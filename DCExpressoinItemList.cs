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
    /// List of expression items.
    /// </summary>
    [System.Runtime.InteropServices.ComVisible(false)]
    public class DCExpressoinItemList : List<DCExpressionItem>
    {
        public DCExpressoinItemList()
        {
        }

        public DCExpressoinItemList CloneDeeply()
        {
            DCExpressoinItemList list = new DCExpressoinItemList();
            foreach (var item in this)
            {
                list.Add(item.Clone());
            }
            return list;
        }

        internal void ToDebugString(int indentLevel, StringBuilder str)
        {
            foreach (var item in this)
            {
                item.ToDebugString(indentLevel, str);
            }
        }
    }
}
