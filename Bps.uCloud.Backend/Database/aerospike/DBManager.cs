namespace Bps.uCloud.Backend.Database
{
    using Aerospike.Client;

    public class DBManager
    {
        public DBManager(IAerospikeClient DB, string Namespace)
        {
            // Aerospike Nomenclature:
            //      Namespace   ~       Database
            //      Set         ~       Table
            //      Record      ~       Row
            //      Bin         ~       Column
            //      Key         ~       Unique ID, PK
            Gallery = new GalleryMapper(DB, Namespace);
            Items = new ItemMapper(DB, Namespace);
            // uCloud (namespace)
            // |_ Users (set):  PK (user_id)
            // |_ Gallery (set):PK (user_id:gallery_name)
            // |_ Items (set):  PK (user_id:gallery_name:item_name)
        }

        public GalleryMapper Gallery { get; protected set; }

        public ItemMapper Items { get; protected set; }

    }
}