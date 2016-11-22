using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGEPRO.CoreLib
{
    public static class Extensions
    {
        public static T[] SplitArray<T>(this T[] array, int index, int length)
        {
            T[] slice = new T[length];
            Array.Copy(array, index, slice, 0, length);
            return slice;
        }

        public static System.Data.DataTable FillDBNullCellsWithZero (System.Data.DataTable dt)
        {
            for (int irow = 0 ; irow < dt.Rows.Count; irow++ )
            {
                foreach (System.Data.DataColumn jcol in dt.Columns)
                {
                    if ( dt.Rows[irow][jcol] == DBNull.Value)
                    {
                        dt.Rows[irow][jcol] = 0;
                    }
                }
            }
            return dt;
        }
    }
}
