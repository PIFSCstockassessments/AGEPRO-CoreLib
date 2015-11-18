using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGEPRO_struct
{
    public static class Extensions
    {
        public static T[] SplitArray<T>(this T[] array, int index, int length)
        {
            T[] slice = new T[length];
            Array.Copy(array, index, slice, 0, length);
            return slice;
        }
    }
}
