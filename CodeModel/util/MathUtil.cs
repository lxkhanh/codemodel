//-----------------------------------------------------------------------
// <copyright file="MathUtil.cs">
//     Copyright (c) emotive GmbH. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeModel.util
{
    class MathUtil
    {
        private static readonly long NegativeZeroBits = BitConverter.DoubleToInt64Bits(-0.0);
        private static readonly long PositiveZeroBits = BitConverter.DoubleToInt64Bits(0.0);

        public static bool IsNegativeZero(object x)
        {
            if (x is double)
                return BitConverter.DoubleToInt64Bits((double)x) == NegativeZeroBits;
            return false;
        }

        public static bool IsPositiveZero(object x)
        {
            if (x is double)
                return BitConverter.DoubleToInt64Bits((double)x) == PositiveZeroBits;
            return false;
        }
    }
}
