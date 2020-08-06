using System;
using System.Collections.Generic;

namespace WasSagenSie
{
    internal class ResultSet : List<ResultSet>
    {
        public ResultSet(ResultType type, string content)
        {
            this.Type = type;
            this.Content = content;
        }

        public string Content { get; private set; }
        public ResultType Type { get; private set; }

        internal void Print(int level, Action<int, string, ResultType> p)
        {
            p(level, this.Content, this.Type);
            foreach (var result in this)
            {
                result.Print(level + 1, p);
            }
        }
    }
}