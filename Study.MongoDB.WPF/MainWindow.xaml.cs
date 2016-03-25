using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Study.MongoDB.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        protected static IMongoClient _client;
        protected static IMongoDatabase _database;


        public MainWindow()
        {
            InitializeComponent();

            // connect to database
            _client = new MongoClient();
            _database = _client.GetDatabase("test");
        }



        async void tryAgg()
        {
            var collection = _database.GetCollection<BsonDocument>("restaurants");

            //var aggregate = collection.Aggregate().Group(new BsonDocument { { "_id", "$borough" }, { "count", new BsonDocument("$sum", 1) } });

            var aggregate = collection.Aggregate()
                .Match(new BsonDocument { { "borough", "Queens" }, { "cuisine", "Brazilian" } } )
                .Group(new BsonDocument { { "_id", "$address.zipcode" }, { "count", new BsonDocument("$sum", 1) } });

            var result = await aggregate.ToListAsync();

            // wrape the result
            var list = new List<BsonWrapper>();

            foreach (var item in result)
            {
                var w = new BsonWrapper(item);
                list.Add(w);
                //list.Items.Add(item.GetValue("_id").ToString() + " - " + item.GetValue("count").ToString());


            }

            this.DataContext = list;



        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            tryAgg();
        }
    }


    public class BsonWrapper
    {
        private BsonDocument mBsonDocument;

        public BsonWrapper(BsonDocument doc)
        {
            mBsonDocument = doc;
        }


        public string this[int idx] { 
            get {
                if (idx < mBsonDocument.Count())
                    return mBsonDocument.GetValue(idx).ToString();
                else
                    return null;
            }
        }
}


}
