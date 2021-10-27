using NPOI.SS.UserModel;
using System;

namespace CtaLineaWebApi.Application.Utility
{
    public static class ExcelCellExtension
    {
        public static string GetStringValue(
            this ICell cell)
        {
            string result = null;
            switch (cell.CellType)
            {
                case CellType.Boolean:
                    result = cell.BooleanCellValue.ToString();
                    break;
                case CellType.Numeric:
                    result = cell.NumericCellValue.ToString();
                    break;
                case CellType.String:
                    result = cell.StringCellValue;
                    break;
            }
            return result;
        }
        public static DateTime? GetDateTimeValue(
            this ICell cell)
        {
            DateTime? result = null;
            switch (cell.CellType)
            {
                case CellType.Numeric:
                    result = cell.DateCellValue;
                    break;
            }
            return result;
        }
        public static decimal? GetDecimalValue(
            this ICell cell)
        {
            decimal? result = null;

            switch (cell.CellType)
            {
                case CellType.Numeric:
                    result = Convert.ToDecimal(cell.NumericCellValue);
                    break;
            }
            return result;
        }
        public static int? GetIntValue(
            this ICell cell)
        {
            int? result = null;

            switch (cell.CellType)
            {
                case CellType.Numeric:
                    result = Convert.ToInt32(cell.NumericCellValue);
                    break;
                case CellType.String:
                    result = Convert.ToInt32(cell.StringCellValue);
                    break;
            }
            return result;
        }
        public static  bool GetBooleanValue (
            this ICell cell)
        {
            bool result = false;
            switch (cell.CellType)
            {
                case CellType.Boolean:
                    result = cell.BooleanCellValue;
                    break;
                case CellType.Numeric:
                    result = cell.NumericCellValue != 0;
                    break;
                case CellType.String:
                    result = cell.StringCellValue.ToUpper() == "TRUE";
                    break;
            }
            return result;
        }




    }
}
