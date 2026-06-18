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
    /// Abstract expression item type. All expression items are derived from this type.
    /// </summary>
    [System.Runtime.InteropServices.ComVisible(false)]
    public abstract class DCExpressionItem
    {
        /// <summary>
        /// Initializes the object.
        /// </summary>
        public DCExpressionItem()
        {
        }
        /// <summary>
        /// Evaluates the expression.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public virtual object Eval(IDCExpressionContext context)
        {
            return null;
        }

        private int _Priority = 0;
        /// <summary>
        /// Operator precedence. Higher value means higher precedence.
        /// </summary>
        internal int Priority
        {
            get
            {
                return _Priority;
            }
            set
            {
                _Priority = value;
            }
        }

        private bool _Collapsed = false;
        /// <summary>
        /// Expression already collapsed.
        /// </summary>
        internal bool Collapsed
        {
            get
            {
                return _Collapsed;
            }
            set
            {
                _Collapsed = value;
            }
        }
        /// <summary>
        /// Clones the object.
        /// </summary>
        /// <returns>The clone.</returns>
        public virtual DCExpressionItem Clone()
        {
            throw new NotSupportedException("Clone()");
        }

        internal virtual void AddSubItem(DCExpressionItem item)
        {
        }

        public virtual void ToDebugString(int indentLevel, StringBuilder str)
        {
        }
    }
}
