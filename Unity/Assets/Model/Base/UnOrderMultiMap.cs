using System.Collections.Generic;
using System.Linq;

namespace ETModel
{
    /// <summary>
    /// 无序的一对多的映射实现（内部带有缓存以减少GC回收）
    /// todo:这个可能有内存泄漏
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="K"></typeparam>
	public class UnOrderMultiMap<T, K>
    {
        /// <summary>
        /// 保存的项
        /// </summary>
		private readonly Dictionary<T, List<K>> dictionary = new Dictionary<T, List<K>>();

        /// <summary>
        /// 重用list 当前使用过的List，保存下来，以便重复使用。从而减少GC
        /// </summary>
        private readonly Queue<List<K>> queue = new Queue<List<K>>();

        /// <summary>
        /// 获取存储的所有项
        /// </summary>
        /// <returns></returns>
		public Dictionary<T, List<K>> GetDictionary()
        {
            return this.dictionary;
        }

        /// <summary>
        /// 添加一项
        /// </summary>
        /// <param name="t">项的Key</param>
        /// <param name="k">Value值</param>
        public void Add(T t, K k)
        {
            List<K> list;
            this.dictionary.TryGetValue(t, out list);
            if (list == null)
            {
                list = this.FetchList();
                this.dictionary[t] = list;
            }

            list.Add(k);
        }

        /// <summary>
        /// 获取已缓存的第一项
        /// </summary>
        /// <returns></returns>
        public KeyValuePair<T, List<K>> First()
        {
            return this.dictionary.First();
        }

        /// <summary>
        /// Key数量
        /// </summary>
        public int Count
        {
            get
            {
                return this.dictionary.Count;
            }
        }

        /// <summary>
        /// 从缓存中找一个可以重复使用的列表项，如果没有，则创建一个
        /// </summary>
        /// <returns></returns>
        private List<K> FetchList()
        {
            if (this.queue.Count > 0)
            {
                // 从回收得到的列表中拿到一项，以便重复使用
                List<K> list = this.queue.Dequeue();
                list.Clear();

                return list;
            }

            // 没有可以重复使用的项，则创建
            return new List<K>();
        }

        /// <summary>
        /// 回收一个可以再使用的项
        /// </summary>
        /// <param name="list"></param>
        private void RecycleList(List<K> list)
        {
            // 防止暴涨
            if (this.queue.Count > 100)
            {
                return;
            }

            list.Clear();
            this.queue.Enqueue(list);
        }

        /// <summary>
        /// 删除一个数据项
        /// </summary>
        /// <param name="t">待删除项的Key</param>
        /// <param name="k">待删除的对象</param>
        /// <returns></returns>
        public bool Remove(T t, K k)
        {
            List<K> list;
            this.dictionary.TryGetValue(t, out list);
            if (list == null)
            {
                return false;
            }
            if (!list.Remove(k))
            {
                return false;
            }
            if (list.Count == 0)
            {
                // 如果列表已被清空了，则加入到回收列表中，以便重复使用
                this.RecycleList(list);
                this.dictionary.Remove(t);
            }

            return true;
        }

        /// <summary>
        /// 通过Key删除
        /// </summary>
        /// <param name="t">待删除项的Key</param>
        /// <returns></returns>
        public bool Remove(T t)
        {
            List<K> list = null;
            this.dictionary.TryGetValue(t, out list);
            if (list != null)
            {
                // 如果列表已被清空了，则加入到回收列表中，以便重复使用
                this.RecycleList(list);
            }

            return this.dictionary.Remove(t);
        }

        /// <summary>
        /// 不返回内部的list,copy一份出来
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public K[] GetAll(T t)
        {
            List<K> list;
            this.dictionary.TryGetValue(t, out list);
            if (list == null)
            {
                return new K[0];
            }
            return list.ToArray();
        }

        /// <summary>
        /// 返回内部的list
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public List<K> this[T t]
        {
            get
            {
                List<K> list;
                this.dictionary.TryGetValue(t, out list);
                return list;
            }
        }

        /// <summary>
        /// 根据Key获取第一项，如果没有，则返回对应类型的默认值
        /// </summary>
        /// <param name="t">对应的Key</param>
        /// <returns></returns>
        public K GetOne(T t)
        {
            List<K> list;
            this.dictionary.TryGetValue(t, out list);
            if (list != null && list.Count > 0)
            {
                return list[0];
            }

            return default(K);
        }

        /// <summary>
        /// 检查是否包含对应项
        /// </summary>
        /// <param name="t">待检查项的Key</param>
        /// <param name="k">待检查项的Value</param>
        /// <returns></returns>
        public bool Contains(T t, K k)
        {
            List<K> list;
            this.dictionary.TryGetValue(t, out list);
            if (list == null)
            {
                return false;
            }
            return list.Contains(k);
        }

        /// <summary>
        /// 检查是否包含项
        /// </summary>
        /// <param name="t">待检查项的Key</param>
        /// <returns></returns>
        public bool ContainsKey(T t)
        {
            return this.dictionary.ContainsKey(t);
        }

        /// <summary>
        /// 清空所有项（被清理的项会被加入到回收集合中）
        /// </summary>
        public void Clear()
        {
            foreach (KeyValuePair<T, List<K>> keyValuePair in this.dictionary)
            {
                this.RecycleList(keyValuePair.Value);
            }

            this.dictionary.Clear();
        }
    }
}