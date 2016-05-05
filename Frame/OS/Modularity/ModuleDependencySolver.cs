using System;
using System.Globalization;
using System.Collections.Generic;
using Frame.OS.Modularity.Exceptions;

namespace Frame.OS.Modularity
{
    /// <summary>
    /// 模块依赖从属解析器，提供操作和解析模块及其依赖项模块的方法。
    /// </summary>
    public class ModuleDependencySolver
    {
        /// <summary>
        /// 表示一个主从模块依赖关系矩阵信息列表。
        /// </summary>
        private readonly ListDictionary<string, string> dependencyMatrix = new ListDictionary<string, string>();

        /// <summary>
        /// 表示一个模块集合列表。
        /// </summary>
        private readonly List<string> knownModules = new List<string>();

        public int ModuleCount
        {
            get { return this.dependencyMatrix.Count; }
        }

        public void AddModule(string name)
        {
            if (String.IsNullOrEmpty(name))
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture,
                    "提供的字符串参数name {0} 不可为null或空值。", "name"));
            AddToDependencyMatrix(name);
            AddToKnownModules(name);
        }

        public void AddDependency(string dependingModule, string dependentModule)
        {
            if (String.IsNullOrEmpty(dependingModule))
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture,
                    "提供的字符串参数dependingModule {0} 不可为null或空值。",
                    "dependingModule"));

            if (String.IsNullOrEmpty(dependentModule))
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture,
                    "提供的字符串参数dependentModule {0} 不可为null或空值。",
                    "dependentModule"));

            if (!knownModules.Contains(dependingModule))
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture,
                    "无法添加未知的模块依赖项{0}。",
                    dependingModule));
            AddToDependencyMatrix(dependentModule);
            this.dependencyMatrix.Add(dependentModule, dependingModule);
        }

        private void AddToDependencyMatrix(string module)
        {
            if (!this.dependencyMatrix.ContainsKey(module))
                this.dependencyMatrix.Add(module);
        }
        private void AddToKnownModules(string module)
        {
            if (!this.knownModules.Contains(module))
                this.knownModules.Add(module);
        }

        public string[]  Solve()
        {
            List<string> skip = new List<string>();
            while (skip.Count < this.dependencyMatrix.Count)
            {
                List<string> leaves = this.FindLeaves(skip);
                if (0 == leaves.Count && skip.Count < this.dependencyMatrix.Count)
                    throw new CyclicDependencyFoundException("至少在模块目录中存在一个重复依赖项."
                        + "应该尽量避免重复依赖。");
                skip.AddRange(leaves);
            }
            skip.Reverse();

            if (skip.Count > this.knownModules.Count)
            {
                string moduleNames = this.FindMissingModules(skip);
                throw new ModularityException(moduleNames, string.Format(CultureInfo.CurrentCulture, "缺少模块:{0}", moduleNames));
            }

            return skip.ToArray();
        }
        private List<string> FindLeaves(List<string> skip)
        {
            List<string> result = new List<string>();
            foreach (string precedent in this.dependencyMatrix.Keys)
            {
                if (skip.Contains(precedent))
                    continue;

                int count = 0;
                foreach (string dependent in this.dependencyMatrix[precedent])
                {
                    if (skip.Contains(dependent))
                        continue;
                    count++;
                }
                if (0 == count)
                    result.Add(precedent);
            }

            return result;
        }
        private string FindMissingModules(List<string> skip)
        {
            string missingModules = string.Empty;
            foreach (string module in skip)
            {
                if (!this.knownModules.Contains(module))
                {
                    missingModules += ", ";
                    missingModules += module;
                }
            }
            return missingModules.Substring(2);
        }
    }
}
