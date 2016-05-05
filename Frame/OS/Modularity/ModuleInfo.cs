using System;
using System.Collections.ObjectModel;

namespace Frame.OS.Modularity
{
    /// <summary>
    /// 记录模块信息的元数据对象。
    /// </summary>
    [Serializable]
    public class ModuleInfo : IModuleCatalogItem
    {
        #region 构造函数

        public ModuleInfo()
            : this(null, null, new string[0])
        {
        }

        /// <summary>
        /// 创建一个Module实例对象。
        /// </summary>
        /// <param name="name">模块的名称，也是该模块的唯一标识符或ID。</param>
        /// <param name="type">模块的类型。这里的类型也即模块类型的AssemblyQualifiedName。</param>
        /// <param name="dependsOn">模块依赖的其它模块的ModuleName的集合。
        /// 这个参数用于在加载该模块时，如果有其依赖的依赖模块没有加载的话，会先将依赖模块加载。</param>
        public ModuleInfo(string name, string type, params string[] dependsOn)
        {
            if (dependsOn == null)
                throw new System.ArgumentNullException("dependsOn");

            this.ModuleName = name;
            this.ModuleType = type;
            this.DependsOn = new Collection<string>();
            foreach (string dependency in dependsOn)
            {
                this.DependsOn.Add(dependency);
            }
        }

        /// <summary>
        /// 创建一个Module实例对象。
        /// </summary>
        /// <param name="name">模块的名称，也是该模块的唯一标识符或ID。</param>
        /// <param name="type">模块的类型。这里的类型也即模块类型的AssemblyQualifiedName。</param>
        public ModuleInfo(string name, string type)
            : this(name, type, new string[0])
        {
        }

        #endregion

        #region 属性

        /// <summary>
        /// 获取或设置该模块的名称，也是该模块的唯一标识符或ID。
        /// </summary>
        public string ModuleName { get; set; }

        /// <summary>
        /// 获取或设置该模块的类型。这里的类型也即模块类型的AssemblyQualifiedName。
        /// </summary>
        public string ModuleType { get; set; }

        /// <summary>
        /// 获取或设置该模块依赖的其它模块的ModuleName的集合。
        /// 此属性的作用在于在加载该模块时，如果该依赖模块列表中的模块没有加载初始化的话，必须先将依赖模块加载并初始化。
        /// </summary>
        public Collection<string> DependsOn { get; set; }

        /// <summary>
        /// 获取或设置该模块的初始化模式。
        /// </summary>
        public InitializationMode InitializationMode { get; set; }

        /// <summary>
        /// 获取或设置该模块存储的位置。
        /// </summary>
        public string Ref { get; set; }

        /// <summary>
        /// 获取或设置该模块的状态。
        /// </summary>
        public ModuleState State { get; set; }

        #endregion
    }
}
