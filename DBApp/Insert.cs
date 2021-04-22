using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBApp
{
    /// <summary>
    /// Класс, помогающий подставить переменные в строку в разное время.
    /// </summary>
    class Insert
    {
        /// <summary>
        /// Команда добавления в базу данных. Пример: Insert.InsertTable = "insert into [dbo].[workers] values('?','?','?')";
        /// </summary>
        /// <param name="insertCommand"></param>
        public string insertCommand;

        private List<string> vars = new List<string>();
        public Insert(){
        }

        /// <summary>
        /// Добавляет переменные, которые нужно подставить.
        /// </summary>
        /// <param name="list"></param>
        public void SetVars(List<string> list)
        {
            vars = list;
        }

        /// <summary>
        /// Подставляет переменные на место строки. Если переменных будет больше чем символов "?" в исходной строке, то лишние добавляться не будут.
        /// </summary>
        /// <returns></returns>
        public string GetTableCommand()
        {

            string result = insertCommand;
            int i = 0;
            while (result.Contains("?"))
            {
                int n = result.IndexOf("?");
                result = result.Remove(n, 1).Insert(n, vars[i]);
                i++;
            }
            return result;
        }
    }
}
