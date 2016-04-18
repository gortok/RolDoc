namespace RolDoc
{
    public class DataStore
    {
        private static StackExchange.Redis.ConnectionMultiplexer connection;
        private static Rol.Store store;
        private DataStore() { }
        public static Rol.Store Store
        {
            get
            {
                if (store == null)
                {
                    connection = StackExchange.Redis.ConnectionMultiplexer.Connect("localhost");
                    store = new Rol.Store(connection);
                }
                return store;
            }
        }
    }
}