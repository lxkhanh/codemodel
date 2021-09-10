//-----------------------------------------------------------------------
// <copyright file="EncoderFactory.cs">
//     Copyright (c) emotive GmbH. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeModel.util
{
    /// <summary>
    /// Creates <seealso cref="CharsetEncoder"/> from a charset name.
    /// 
    public class EncoderFactory
    {
        public static Encoder createEncoder(string encodin)
        {
            Encoding cs = Encoding.GetEncoding(encodin);
            Encoder encoder = cs.GetEncoder();
            return encoder;
        }   

    }
}
