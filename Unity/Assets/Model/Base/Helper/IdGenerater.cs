namespace ETModel
{
    public static class IdGenerater
    {
        /// <summary>
        /// 当前的AppId
        /// </summary>
        public static long AppId { private get; set; }

        private static ushort value;

        /// <summary>
        /// 生成一个整个系统唯一的Id
        /// </summary>
        /// <returns></returns>
        public static long GenerateId()
        {
            long time = TimeHelper.ClientNowSeconds();

            return (AppId << 48) + (time << 16) + ++value;
        }

        /// <summary>
        /// 根据实例Id获取AppId
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static int GetAppIdFromId(long id)
        {
            return (int)(id >> 48);
        }
    }
}