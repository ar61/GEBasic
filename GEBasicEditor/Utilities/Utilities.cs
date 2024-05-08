// Copyright (c) Abhinav Rathod. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GEBasicEditor.Utilities
{
    public static class MathUtil
    {
        public static float Epsilon => 0.00001f;

        public static bool IsSameAs(this float value, float other)
        {
            return Math.Abs(other - value) < Epsilon;
        }

        public static bool? IsSameAs(this float? value, float? other)
        {
            if(!value.HasValue || !other.HasValue) return false;
            return Math.Abs(value.Value - other.Value) < Epsilon;
        }
    }
}
