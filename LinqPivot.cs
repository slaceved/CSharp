using System.Linq;
using System.Data;
using System.Collections.Generic;

namespace Capstone_Game_Platform
{
    internal class LinqPivot
    {
        public DataTable Pivot(DataTable dt, DataColumn pivotColumn, string pivotName)
        {
            DataTable temp = dt.Copy();
            temp.Columns.Remove(pivotColumn.ColumnName);
            string[] dataColumnNames = temp.Columns.Cast<DataColumn>()
                .Select(c => c.ColumnName)
                .ToArray();

            DataTable result = new DataTable();
            DataColumn pivot = new DataColumn
            {
                ColumnName = pivotName
            };
            result.Columns.Add(pivot);
            result.DefaultView.ToTable(true, pivotName);

            DataRow dr = result.NewRow();
            for (int j = 0; j < temp.Columns.Count; j++)
            {
                result.Rows.Add(temp.Columns[j].ColumnName);
            }

            List<string> t = dt.AsEnumerable()
               .Select(r => r[pivotColumn.ColumnName].ToString())
               .Distinct()
               .ToList();
            t.ForEach(c => result.Columns.Add(c));

            for (int j = 0; j < t.Count; j++) //for each pivotColumn named value
            {
                foreach (string s in dataColumnNames) //for each dataColumnName
                {
                    //select value of dataColumnName string 
                    //where the pivotColum is equal to the current value from the list of pivotColum values
                    string value = (from row in dt.AsEnumerable()
                                    where row.Field<string>(pivotColumn.ColumnName) == t[j]
                                    select row.Field<string>(s)).SingleOrDefault();

                    DataRow pk = (from row in result.AsEnumerable()
                                  where row.Field<string>(pivotName) == s
                                  select row).SingleOrDefault();

                    pk.BeginEdit();
                    int index = pk.Table.Columns.IndexOf(t[j]);
                    pk[index] = value;
                    pk.EndEdit();
                    pk.AcceptChanges();
                }
            }
            return result;
        }
    }

    public static class DataRowExtensions
    {
        public static T GetCellValueByName<T>(this DataRow row, string columnName)
        {
            int index = row.Table.Columns.IndexOf(columnName);
            return (index < 0 || index > row.ItemArray.Count())
                      ? default(T)
                      : (T)row[index];
        }
    }
}

