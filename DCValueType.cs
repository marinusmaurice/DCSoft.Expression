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
    /// Value type.
    /// </summary>
    [System.Runtime.InteropServices.ComVisible( false )]
    public enum DCValueType
    {
        /// <summary>
        /// Number.
        /// </summary>
        Number,
        /// <summary>
        /// String.
        /// </summary>
        String,
        /// <summary>
        /// Boolean.
        /// </summary>
        Boolean
    }
}
