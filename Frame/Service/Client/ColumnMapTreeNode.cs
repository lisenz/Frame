using System;
using System.Collections.Generic;
using Frame.Service.Client.Attributes;
using System.Reflection;

namespace Frame.Service.Client
{
    internal class ColumnMapTreeNode
    {
        /// <summary>
        /// 父节点。
        /// </summary>
        public ColumnMapTreeNode Parent { get; private set; }

        /// <summary>
        /// 子节点。
        /// </summary>
        public IList<ColumnMapTreeNode> Children { get; private set; }

        /// <summary>
        /// 目标节点的类型。
        /// </summary>
        private Type _targetType;

        /// <summary>
        /// 数据对象。
        /// </summary>
        public DataValue Value { get; internal set; }

        /// <summary>
        /// 是否是复杂类型。
        /// </summary>
        private bool IsComplexType { get; set; }

        /// <summary>
        /// 获取该节点的属性元数据。
        /// </summary>
        public PropertyInfo PropertyInfo { get; private set; }
        
        /// <summary>
        /// 获取字段名称。
        /// </summary>
        private string Column { get; set; }

        /// <summary>
        /// 获取静态解析方法名称。
        /// </summary>
        public string StaticParseMethod { get; private set; }

        /// <summary>
        /// 获取字段索引。
        /// </summary>
        public int ColumnIndex { get; private set; }

        /// <summary>
        /// 获取或设置目标节点的数据类型。
        /// </summary>
        public Type TargetType
        {
            get { return _targetType; }
            set
            {
                if (value == null || _targetType == value)
                {
                    return;
                }

                _targetType = value;

                if (HasColsClassMapAttribute(_targetType))
                {
                    BuildChildren();
                }

            }
        }

        /// <summary>
        /// 返回一个值，该值标识是否标注了DataEntity特性，表示复杂类型。
        /// </summary>
        /// <param name="type">要检测标注的类型。</param>
        /// <returns>若在参数type中搜索到DataEntity特性，则返回true；否则，返回false。</returns>
        private static bool HasColsClassMapAttribute(Type type)
        {
            var attrs = type.GetCustomAttributes(typeof(DataEntityAttribute), true);
            return attrs.Length > 0;
        }

        /// <summary>
        /// 生成该节点的子节点。
        /// </summary>
        private void BuildChildren()
        {
            this.Children = new List<ColumnMapTreeNode>();

            var props = _targetType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetProperty);

            foreach (var prop in props)
            {
                var node = new ColumnMapTreeNode
                {
                    Value = this.Value,
                    Parent = this,
                    TargetType = prop.PropertyType,
                    PropertyInfo = prop
                };

                FillColumnName(node);

                node.ColumnIndex = Array.IndexOf(Value.ColumnNames, node.Column);

                if (node.ColumnIndex < 0)
                {
                    // 如果不在当前Response中，则无需加入Tree中。
                    continue;
                }

                this.Children.Add(node);
            }
        }

        /// <summary>
        /// 对字段节点进行赋值填充。
        /// </summary>
        /// <param name="node">要赋值的字段节点对象。</param>
        private void FillColumnName(ColumnMapTreeNode node)
        {
            PropertyInfo prop = node.PropertyInfo;

            var propAttrs = prop.GetCustomAttributes(typeof(DataPropertyAttribute), true);

            string defaultColumnName = null; // ColumnMapAttribute中的CommandName为空表示适用于所有Command

            if (propAttrs.Length < 1)
            {
                node.Column = prop.Name;
            }
            else
            {
                foreach (DataPropertyAttribute propAttr in propAttrs)
                {
                    IsComplexType = propAttr.ComplexType;
                    if (IsComplexType)
                    {
                        if (!HasColsClassMapAttribute(node.TargetType))
                        {
                            throw new InvalidOperationException(string.Format("在该复杂类型的属性中没有标注DataEntity特性, 属性类型为:{0}。", node.TargetType));
                        }
                        // 如果是复杂类型，则没有对应的列名称，不需要再进行搜索
                        this.Children.Add(node);
                        break;
                    }

                    if (string.IsNullOrEmpty(propAttr.Command))
                    {
                        defaultColumnName = propAttr.ColumnName;
                        this.StaticParseMethod = propAttr.StaticParseMethod;
                        continue;
                    }

                    if (this.Value.Command == propAttr.Command)
                    {
                        this.StaticParseMethod = propAttr.StaticParseMethod;
                        if (!string.IsNullOrEmpty(propAttr.ColumnName))
                        {
                            node.Column = propAttr.ColumnName;
                        }
                        break;
                    }
                }
            }

            if (!IsComplexType && string.IsNullOrEmpty(node.Column))
            {
                // 如果进行了标记，但是又没有适用于当前Command的ColumnName，且则使用defaultColumnName
                // 如果defaultColumnName也为空，则使用属性名
                node.Column = string.IsNullOrEmpty(defaultColumnName) ? prop.Name : defaultColumnName;
            }
        }

        /// <summary>
        /// 递归处理节点。
        /// </summary>
        /// <param name="action">处理节点的方法。</param>
        internal void Visit(Action<ColumnMapTreeNode> action)
        {
            action(this);
            if (this.Children == null)
            {
                return;
            }
            foreach (var node in this.Children)
            {
                node.Visit(action);
            }
        }
    }
}
