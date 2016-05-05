using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections;
using System.Globalization;
using System.Windows.Markup;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Frame.OS.Modularity;
using Frame.Core.Extensions;
using Frame.OS.Modularity.Exceptions;

namespace Frame.OS.WPF.Modularity
{
    /// <summary>
    /// 模块管理目录对象，定义声明了管理模块信息的属性和方法，这里实现了IModuleCatalog接口,它是ModuleInfo的容器,保存着所有Module的信息,
    /// 不仅会管理哪些Module需要加载,什么时候加载以什么顺序加载等问题,
    /// 还要检查各个Module之间是否存在着循环依赖、是否有重复的Module等等。
    /// 这些Module作为模块管理池对象(ModuleManager)的数据基础。
    /// </summary>
    public class ModuleCatalog : IModuleCatalog
    {
        private readonly ModuleCatalogItemCollection items;
        private bool isLoaded;
        protected bool Validated { get; set; }

        public ModuleCatalog()
        {
            this.items = new ModuleCatalogItemCollection();
            this.items.CollectionChanged += this.ItemsCollectionChanged;
        }

        /// <summary>
        /// 初始化模块列表，将模块列表集合装载到模块管理目录对象中。
        /// </summary>
        /// <param name="modules">模块列表集合。</param>
        public ModuleCatalog(IEnumerable<ModuleInfo> modules)
            : this()
        {
            if (modules == null)
                throw new System.ArgumentNullException("modules");
            foreach (ModuleInfo moduleInfo in modules)
            {
                this.Items.Add(moduleInfo);
            }
        }

        #region 属性

        /// <summary>
        /// 获取模块目录管理对象中的模块列表集合。
        /// </summary>
        public Collection<IModuleCatalogItem> Items
        {
            get { return this.items; }
        }

        /// <summary>
        /// 获取模块目录管理对象中属于同一组的模块元数据列表集合。
        /// 注意：这里的模块列表集合是一个模块组对象，该对象包含了多个模块元数据对象(ModuleInfo)。
        /// </summary>
        public IEnumerable<ModuleInfoGroup> Groups
        {
            get { return this.Items.OfType<ModuleInfoGroup>(); }
        }

        /// <summary>
        /// 获取模块目录管理对象中的排除掉ModuleInfoGroup中模块元数据列表集合的模块元数据列表集合。
        /// </summary>
        protected IEnumerable<ModuleInfo> GrouplessModules
        {
            get { return this.Items.OfType<ModuleInfo>(); }
        }

        #endregion

        #region 实现IModuleCatalog接口

        /// <summary>
        /// 获取模块目录管理对象中所有的模块元数据对象。
        /// </summary>
        public virtual IEnumerable<ModuleInfo> Modules
        {
            get { return this.GrouplessModules.Union(this.Groups.SelectMany(g => g)); }
        }

        public virtual IEnumerable<ModuleInfo> GetDependentModules(ModuleInfo moduleInfo)
        {
            this.EnsureCatalogValidated();

            return this.GetDependentModulesInner(moduleInfo);
        }
        protected virtual IEnumerable<ModuleInfo> GetDependentModulesInner(ModuleInfo moduleInfo)
        {
            return this.Modules.Where(dependantModule => moduleInfo.DependsOn.Contains(dependantModule.ModuleName));
        }

        public virtual IEnumerable<ModuleInfo> CompleteListWithDependencies(IEnumerable<ModuleInfo> modules)
        {
            if (modules == null)
            {
                throw new ArgumentNullException("modules");
            }

            this.EnsureCatalogValidated();

            List<ModuleInfo> completeList = new List<ModuleInfo>();
            List<ModuleInfo> pendingList = modules.ToList();
            while (pendingList.Count > 0)
            {
                ModuleInfo moduleInfo = pendingList[0];

                foreach (ModuleInfo dependency in this.GetDependentModules(moduleInfo))
                {
                    if (!completeList.Contains(dependency) && !pendingList.Contains(dependency))
                    {
                        pendingList.Add(dependency);
                    }
                }

                pendingList.RemoveAt(0);
                completeList.Add(moduleInfo);
            }

            IEnumerable<ModuleInfo> sortedList = this.Sort(completeList);
            return sortedList;
        }
        protected virtual IEnumerable<ModuleInfo> Sort(IEnumerable<ModuleInfo> modules)
        {
            foreach (string moduleName in SolveDependencies(modules))
            {
                yield return modules.First(m => m.ModuleName == moduleName);
            }
        }

        public virtual void Initialize()
        {
            if (!this.isLoaded)
                this.Load();
            this.Validate();
        }

        public virtual void AddModule(ModuleInfo moduleInfo)
        {
            this.items.Add(moduleInfo);
        }
        public ModuleCatalog AddModule(Type moduleType, params string[] dependsOn)
        {
            return this.AddModule(moduleType, InitializationMode.WhenAvailable, dependsOn);
        }
        public ModuleCatalog AddModule(Type moduleType, InitializationMode initializationMode, params string[] dependsOn)
        {
            if (moduleType == null) throw new System.ArgumentNullException("moduleType");
            return this.AddModule(moduleType.Name, moduleType.AssemblyQualifiedName, initializationMode, dependsOn);
        }
        public ModuleCatalog AddModule(string moduleName, string moduleType, params string[] dependsOn)
        {
            return this.AddModule(moduleName, moduleType, InitializationMode.WhenAvailable, dependsOn);
        }
        public ModuleCatalog AddModule(string moduleName, string moduleType, InitializationMode initializationMode, params string[] dependsOn)
        {
            return this.AddModule(moduleName, moduleType, null, initializationMode, dependsOn);
        }
        public ModuleCatalog AddModule(string moduleName, string moduleType, string refValue, InitializationMode initializationMode, params string[] dependsOn)
        {
            if (moduleName == null)
            {
                throw new ArgumentNullException("moduleName");
            }

            if (moduleType == null)
            {
                throw new ArgumentNullException("moduleType");
            }

            ModuleInfo moduleInfo = new ModuleInfo(moduleName, moduleType);
            moduleInfo.DependsOn.AddRange(dependsOn);
            moduleInfo.InitializationMode = initializationMode;
            moduleInfo.Ref = refValue;
            this.Items.Add(moduleInfo);
            return this;
        }

        #endregion
        
        public static ModuleCatalog CreateFromXaml(Stream xamlStream)
        {
            if (xamlStream == null)
            {
                throw new ArgumentNullException("xamlStream");
            }

            return XamlReader.Load(xamlStream) as ModuleCatalog;
        }

        public static ModuleCatalog CreateFromXaml(Uri builderResourceUri)
        {
            var streamInfo = System.Windows.Application.GetResourceStream(builderResourceUri);

            if ((streamInfo != null) && (streamInfo.Stream != null))
            {
                return CreateFromXaml(streamInfo.Stream);
            }

            return null;
        }

        public void Load()
        {
            this.isLoaded = true;
            this.InnerLoad();
        }

        /// <summary>
        /// 对模块目录管理对象中的模块信息进行一系列验证。
        /// </summary>
        public virtual void Validate()
        {
            this.ValidateUniqueModules();
            this.ValidateDependencyGraph();
            this.ValidateCrossGroupDependencies();
            this.ValidateDependenciesInitializationMode();

            this.Validated = true;
        }
        /// <summary>
        /// 检测模块集合中模块的唯一性,是否存在重复的模块。
        /// </summary>
        protected virtual void ValidateUniqueModules()
        {
            List<string> moduleNames = this.Modules.Select(m => m.ModuleName).ToList();

            // 查询获取出重复模块，这里表示在模块列表中存在至少两个相同名称的ModuleInfo对象
            string duplicateModule = moduleNames.FirstOrDefault(m => moduleNames.Count(c => c == m) > 1);
            if (null != duplicateModule)
                throw new DuplicateWaitObjectException(duplicateModule, string.Format(CultureInfo.CurrentCulture, "模块{0}被加载程序发现存在重复.", duplicateModule));
        }

        protected virtual void ValidateDependencyGraph()
        {
            SolveDependencies(this.Modules);
        }
        protected virtual void ValidateCrossGroupDependencies()
        {
            ValidateDependencies(this.GrouplessModules);
            foreach (ModuleInfoGroup group in this.Groups)
            {
                ValidateDependencies(this.GrouplessModules.Union(group));
            }
        }
        protected virtual void ValidateDependenciesInitializationMode()
        {
            ModuleInfo moduleInfo = this.Modules.FirstOrDefault(
                m => m.InitializationMode == InitializationMode.WhenAvailable
                    && this.GetDependentModulesInner(m).Any(
                    dependency => dependency.InitializationMode == InitializationMode.OnDemand));
            if (null != moduleInfo)
                throw new ModularityException(
                    moduleInfo.ModuleName,
                    String.Format(CultureInfo.CurrentCulture, "当程序启动时，模块 {0} 被标记为自动初始化, "
                        + "但是是作为按需即取式初始化. 修改该错误, "
                        + "设置该模块对象的InitializationMode属性为WhenAvailable, "
                        + "获取移除ModuleCatalog类中的验证机制.", moduleInfo.ModuleName));
        }

        protected static string[] SolveDependencies(IEnumerable<ModuleInfo> modules)
        {
            if (modules == null) throw new System.ArgumentNullException("modules");

            ModuleDependencySolver solver = new ModuleDependencySolver();

            foreach (ModuleInfo data in modules)
            {
                solver.AddModule(data.ModuleName);

                if (data.DependsOn != null)
                {
                    foreach (string dependency in data.DependsOn)
                    {
                        solver.AddDependency(data.ModuleName, dependency);
                    }
                }
            }

            if (solver.ModuleCount > 0)
            {
                return solver.Solve();
            }

            return new string[0];
        }

        protected static void ValidateDependencies(IEnumerable<ModuleInfo> modules)
        {
            if (modules == null)
                throw new System.ArgumentNullException("modules");

            List<string> moduleNames = modules.Select(m => m.ModuleName).ToList();
            foreach (ModuleInfo moduleInfo in modules)
            {
                // 判断是否moduleInfo的依赖模块集合不在moduleNames集合中
                if (null != moduleInfo.DependsOn && moduleInfo.DependsOn.Except(moduleNames).Any())
                    throw new ModularityException(
                        moduleInfo.ModuleName,
                        String.Format("模块 {0} 与依赖的其他模块项不属于同一个组。",
                        moduleInfo.ModuleName));
            }
        }

        public virtual ModuleCatalog AddGroup(InitializationMode initializationMode, string refValue, params ModuleInfo[] moduleInfos)
        {
            if (moduleInfos == null)
                throw new System.ArgumentNullException("moduleInfos");

            ModuleInfoGroup newGroup = new ModuleInfoGroup();
            newGroup.InitializationMode = initializationMode;
            newGroup.Ref = refValue;

            foreach (ModuleInfo info in moduleInfos)
            {
                newGroup.Add(info);
            }

            this.items.Add(newGroup);

            return this;
        }

        protected virtual void InnerLoad()
        {
        }

        private void ItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (this.Validated)
            {
                this.EnsureCatalogValidated();
            }
        }

        /// <summary>
        /// 确保模块目录中模块元数据信息合法性的验证。
        /// </summary>
        protected virtual void EnsureCatalogValidated()
        {
            if (!this.Validated)
            {
                this.Validate();
            }
        }
    }
}
