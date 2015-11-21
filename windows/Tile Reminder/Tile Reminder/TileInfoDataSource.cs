using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using System.IO;

namespace Tile_Reminder
{
    class TileInfoDataSource
    {
        private const string FILENAME = "TileInfo.json";

        public static int latestID;

        public async static Task<List<TileInfo>> readAllTiles()
        {
            try
            {
                List<TileInfo> myData;
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(List<TileInfo>));

                using (var stream = await ApplicationData.Current.LocalFolder.OpenStreamForReadAsync(FILENAME))
                {
                    myData = (List<TileInfo>)serializer.ReadObject(stream);
                }
                return myData;
            }
            catch (Exception ex)
            {
                return new List<TileInfo>();
            }
        }

        public static async Task AddTile(TileInfo newTile)
        {

            List<TileInfo> myData = await readAllTiles();

            myData.Add(newTile);
            newTile.ID = myData.Count;

            latestID = newTile.ID;

            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(List<TileInfo>));

            using (var stream = await ApplicationData.Current.LocalFolder.OpenStreamForWriteAsync(FILENAME, CreationCollisionOption.ReplaceExisting))
            {
                serializer.WriteObject(stream, myData);
            }

        }
    }
}
