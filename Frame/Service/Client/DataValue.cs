using System;
using System.Data;
using Newtonsoft.Json;
using System.Reflection;
using System.Collections.Generic;
using Frame.Core.Extensions;
using Frame.Core.Reflection.Fast;
using Frame.Service.Client.Attributes;

namespace Frame.Service.Client
{
    /// <summary>
    /// 通用数据服务返回数据的类型，提供操作的属性和方法。
    /// </summary>
    public class DataValue
    {
        /// <summary>
        /// 执行命令影响的数据行数，默认为-1。
        /// </summary>
        private int _affectedRows = -1;

        /// <summary>
        /// 字段的名称。
        /// </summary>
        private string[] _columnNames = null;

        /// <summary>
        /// 行数据结果集。
        /// </summary>
        private object[][] _rows = null;

        /// <summary>
        /// 对应字段的数据类型
        /// </summary>
        private Type[] _columnTypes = null;

        /// <summary>
        /// 输出参数。
        /// </summary>
        private IDictionary<string, object> _out = null;

        /// <summary>
        /// 获取执行的文本指令。
        /// </summary>
        public string Command { get; internal set; }

        /// <summary>
        /// 获取或设置执行命令影响的数据行数。
        /// </summary>
        public int AffectRows
        {
            get { return _affectedRows; }
            set { _affectedRows = value; }
        }

        /// <summary>
        /// 获取或设置命令所有的行数，分页用。
        /// </summary>
        public int TotalRows { get; set; }

        /// <summary>
        /// 获取或设置字段的名称。
        /// </summary>
        [JsonProperty]
        public string[] ColumnNames
        {
            get
            {
                if (null == _columnNames)
                {
                    _columnNames = new string[0];
                }
                return _columnNames;
            }
            set { _columnNames = value; }
        }

        [JsonProperty]
        public Type[] ColumnTypes
        {
            get
            {
                if (null == _columnTypes)
                {
                    _columnTypes = new Type[0];
                }
                return _columnTypes;
            }
            set { _columnTypes = value; }
        }

        /// <summary>
        /// 获取或设置行数据结果集。
        /// </summary>
        [JsonProperty]
        public object[][] Rows
        {
            get
            {
                if (null == _rows)
                {
                    _rows = new object[][] { };
                }
                return _rows;
            }
            set { _rows = value; }
        }

        /// <summary>
        /// 获取或设置其他输出参数。
        /// </summary>
        public IDictionary<string, object> OutParams
        {
            get
            {
                if (null == _out)
                {
                    _out = new Dictionary<string, object>();
                }
                return _out;
            }
            set { _out = value; }
        }

        /// <summary>
        /// 获取服务异常。
        /// </summary>
        public Exception Exception { get; internal set; }

        /// <summary>
        /// 获取一个值，该值指示是否数据出错。
        /// </summary>
        public bool IsError
        {
            get { return null != Exception; }
        }

        // 命令名称与字段映射树对应关系
        private readonly static Dictionary<string, ColumnMapTreeNode> TreeCache = new Dictionary<string, ColumnMapTreeNode>();

        /// <summary>
        /// 将当前对象转换为泛型参数指定类型的对象。
        /// </summary>
        /// <typeparam name="T">要转换的类型。</typeparam>
        /// <returns>转换后的对象集合。</returns>
        public IList<T> ToObjects<T>() where T : new()
        {
            if (ColumnNames == null || ColumnNames.Length < 1)
            {
                throw new InvalidOperationException("未找到任何列。");
            }

            if (Rows == null || Rows.Length < 1)
            {
                throw new InvalidOperationException("未找到任何行。");
            }
            
            IList<T> list = new List<T>();

            var treeRoot = GetColumnMapTree(this, typeof(T));

            foreach (var row in Rows)
            {
                var target = new T();
                list.Add(target);
                var objectStack = new Dictionary<ColumnMapTreeNode, object> { { treeRoot, target } };

                var currentRow = row;
                treeRoot.Visit(node =>
                {
                    if (node.Parent == null)
                    {
                        return;
                    }

                    if (node.Children != null && node.Children.Count > 0)
                    {
                        var conInfo = node.TargetType.GetConstructor(new Type[0]);

                        if (conInfo != null)
                        {
                            var obj = conInfo.FastInvoke();

                            objectStack.Add(node, obj);

                            node.PropertyInfo.FastSetValue(objectStack[node.Parent], obj);
                        }
                        else
                        {
                            throw new InvalidOperationException(string.Format("缺少默认的构造函数。类型: {0}。", node.TargetType));
                        }
                    }
                    else
                    {
                        var obj = currentRow[node.ColumnIndex];
                        if (obj is string
                            && node.PropertyInfo.PropertyType != typeof(string))
                        {
                            SetValueViaParse(objectStack[node.Parent], node, obj as string);
                        }
                        else if (node.PropertyInfo.PropertyType == typeof(Int32) && obj is Int64)
                        {
                            node.PropertyInfo.FastSetValue(objectStack[node.Parent], Convert.ToInt32((Int64)obj));
                        }
                        else
                        {
                            node.PropertyInfo.FastSetValue(objectStack[node.Parent], obj);
                        }
                    }
                });
            }
            return list;
        }

        /// <summary>
        /// 将当前对象转换为泛型参数指定类型的对象。
        /// </summary>
        /// <typeparam name="T">要转换的类型。</typeparam>
        /// <returns>返回一个数据表。</returns>
        public DataTable ToTable()
        {
            if (ColumnNames == null || ColumnNames.Length < 1)
            {
                throw new InvalidOperationException("未找到任何列。");
            }

            if (Rows == null || Rows.Length < 1)
            {
                throw new InvalidOperationException("未找到任何行。");
            }

            DataTable dt = new DataTable();

            int colCount = _columnNames.Length;
            for (int index = 0; index < colCount; index++)
            {
                dt.Columns.Add(_columnNames[index], _columnTypes[index]);
            }
            foreach (var row in Rows)
            {
                var currentRow = row;
                DataRow drNew = dt.NewRow();

                for (int index = 0; index < colCount; index++)
                {
                    drNew[index] = currentRow[index];
                }

                dt.Rows.Add(drNew.ItemArray);
            }

            return dt;
        }

        /// <summary>
        /// 通过字段节点的目标节点对象的静态解析方法为指定对象赋值。
        /// </summary>
        /// <param name="obj">要赋值的对象。</param>
        /// <param name="node">字段节点对象。</param>
        /// <param name="s">字段节点的值。</param>
        private static void SetValueViaParse(object obj, ColumnMapTreeNode node, string s)
        {
            var parseMethodName = string.IsNullOrEmpty(node.StaticParseMethod) ? "Parse" : node.StaticParseMethod;

            var parse = node.TargetType.GetMethod(parseMethodName, BindingFlags.Static | BindingFlags.Public, 
                null, new Type[] { typeof(string) }, null);

            if (parse == null)
            {
                throw new InvalidOperationException(string.Format("{1} 缺少 public static void {0}(string s) 方法。", 
                    parseMethodName, node.TargetType));
            }

            node.PropertyInfo.FastSetValue(obj, parse.FastInvoke(null, s));
        }

        /// <summary>
        /// 从映射树中获取与指定命令键相关联的字段节点。
        /// </summary>
        /// <param name="value">数据值对象。</param>
        /// <param name="type">目标节点对象类型。</param>
        /// <returns>若查找到指定命令键，则返回对应的字段节点；否则，则根据形参构造一个新的字段节点返回，同时存入缓存列表。</returns>
        private static ColumnMapTreeNode GetColumnMapTree(DataValue value, Type type)
        {
            lock (TreeCache)
            {
                ColumnMapTreeNode node = null;
                if (TreeCache.TryGetValue(value.Command, out node))
                {
                    return node;
                }

                var tree = new ColumnMapTreeNode { Value = value, TargetType = type };
                TreeCache.Add(value.Command, tree);

                return tree;
            }
        }
    }
}
