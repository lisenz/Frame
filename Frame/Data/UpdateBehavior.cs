using System;

namespace Frame.Data
{
    /// <summary>
    /// 使用UpdateDataSet方法时提供的数据更新时碰到适配器命令的错误后的控制行为。
    /// </summary>
    public enum UpdateBehavior
    {
        /// <summary>
        /// 不影响更新命令的执行，当更新遇到错误时，不影响其他新增行的数据的操作。
        /// </summary>
        Standard,
        /// <summary>
        /// 当更新命令遇到错误时，继续更新其他行的数据。
        /// </summary>
        Continue,
        /// <summary>
        /// 当执行命令遇到错误时，回滚所有操作。
        /// </summary>
        Transactional
    }
}
