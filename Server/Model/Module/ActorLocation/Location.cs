namespace ETModel
{
    /// <summary>
    /// 位置处理实体
    /// </summary>
	public class Location : Entity
    {
        public string Address;

        public Location(long id, string address) : base(id)
        {
            this.Address = address;
        }
    }
}
