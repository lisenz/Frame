namespace Frame.DataStore.Utility
{
    /// <summary>
    /// 数据库访问执行器。
    /// </summary>
    internal struct DaoExecutor
    {
        /// <summary>
        /// 表示要对数据源执行的 SQL 语句或存储过程提供一个操作对象。
        /// </summary>
        public ISqlGeCommand Command { get; set; }

        /// <summary>
        /// 数据库业务访问对象。
        /// </summary>
        public BaseDao Dao { get; set; }
    }
}
