using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class Objects
{
   
    public struct CraftingStation {
        public Vector3 pos;
        public int stationType;
    }

    public struct Container {
        public Vector3 pos;
        public int containerType;
    }

    /*
    public struct SpawnPoint {
        public Vector3 pos;
        public Vector3[] route;
    }
    */

    public static CraftingStation[] craftingStations;
    public static Container[] container;

    public static void load(string path) {
        string craftingStationFile = path + "objects_craftingStation.dat";
        string containerFile = path + "objects_container";

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file;

        if (File.Exists(craftingStationFile)) {
            file = File.Open(craftingStationFile, FileMode.Open);
            craftingStations = (CraftingStation[])bf.Deserialize(file);
            file.Close();
        }
        if (File.Exists(containerFile)) {
            file = File.Open(containerFile, FileMode.Open);
            container = (Container[]) bf.Deserialize(file);
            file.Close();
        }

    }

    public static void save(string path) {
        string craftingStationFile = path + "objects_craftingStation.dat";
        string containerFile = path + "objects_container";
        if (!File.Exists(craftingStationFile)) {
            Directory.CreateDirectory(Path.GetDirectoryName(craftingStationFile));
        }
        if (!File.Exists(containerFile)) {
            Directory.CreateDirectory(Path.GetDirectoryName(containerFile));
        }


        BinaryFormatter bf = new BinaryFormatter();
        FileStream file;

        file = File.Open(craftingStationFile, FileMode.OpenOrCreate);
        bf.Serialize(file, craftingStations);
        file.Close();

        file = File.Open(containerFile, FileMode.OpenOrCreate);
        bf.Serialize(file, container);
        file.Close();
    }
}
