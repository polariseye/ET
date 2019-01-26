namespace ETModel
{
    /// <summary>
    /// 类型帮助类
    /// </summary>
	public static class ObjectHelper
	{
        /// <summary>
        /// 交换两个对象
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="t1">对象1</param>
        /// <param name="t2">对象2</param>
		public static void Swap<T>(ref T t1, ref T t2)
		{
			T t3 = t1;
			t1 = t2;
			t2 = t3;
		}
	}
}