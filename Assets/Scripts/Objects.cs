using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;

public class Objects : MonoBehaviour
{
    [Serializable()]
    public struct ConstructPlace {
        public ConstructPlace(Vector3 position, int constructType) {
            pos = new float[] { position.x, position.y, position.z };
            type = constructType;
        }
        public float[] pos;
        public int type;      //0 = small, 1 = large
    }

    [Serializable()]
    public struct Container {
        public Container(Vector3 position, int containerType) {
            pos = new float[] { position.x, position.y, position.z };
            type = containerType;
        }
        public float[] pos;
        public int type;
    }

    /*
    public struct SpawnPoint {
        public Vector3 pos;
        public Vector3[] route;
    }
    */

    [SerializeField] private GameObject constructSmall;
    [SerializeField] private GameObject constructLarge;

    [SerializeField] private GameObject containerTiny;
    [SerializeField] private GameObject containerMedium;
    [SerializeField] private GameObject containerLarge;
    [SerializeField] private GameObject containerOlli;

    private ConstructPlace[] constructPlaces = new ConstructPlace[0];
    private Container[] container = new Container[0];

    private GameObject[] constructPlacesGO = new GameObject[0];
    private GameObject[] containerGO = new GameObject[0];

    void Start() {
        load(GetComponent<World>().LevelName);
        buildObjects();
    }

    public void load(string levelName) {
        string path = Application.dataPath + "/leveldata/" + levelName + "/";
        string constructPlacesFile = path + "objects_constructPlaces.obj";
        string containerFile = path + "objects_container.obj";

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file;

        if (File.Exists(constructPlacesFile)) {
            file = File.Open(constructPlacesFile, FileMode.Open);
            constructPlaces = (ConstructPlace[])bf.Deserialize(file);
            file.Close();
        }
        if (File.Exists(containerFile)) {
            file = File.Open(containerFile, FileMode.Open);
            container = (Container[]) bf.Deserialize(file);
            file.Close();
        }

    }

    public void save(string levelName) {
        string path = Application.dataPath + "/leveldata/" + levelName + "/";
        string constructPlacesFile = path + "objects_constructPlaces.obj";
        string containerFile = path + "objects_container.obj";
        if (!File.Exists(constructPlacesFile)) {
            Directory.CreateDirectory(Path.GetDirectoryName(constructPlacesFile));
        }
        if (!File.Exists(containerFile)) {
            Directory.CreateDirectory(Path.GetDirectoryName(containerFile));
        }


        BinaryFormatter bf = new BinaryFormatter();
        FileStream file;

        file = File.Open(constructPlacesFile, FileMode.OpenOrCreate);
        bf.Serialize(file, constructPlaces);
        file.Close();

        file = File.Open(containerFile, FileMode.OpenOrCreate);
        bf.Serialize(file, container);
        file.Close();
    }

    public void buildObjects() {
        if (constructPlaces != null) {
            constructPlacesGO = new GameObject[constructPlaces.Length];
            for (int i = 0; i < constructPlaces.Length; i++) {
                switch (constructPlaces[i].type) {
                    case 0 :
                        constructPlacesGO[i] = Instantiate(constructSmall, new Vector3(constructPlaces[i].pos[0], constructPlaces[i].pos[1], constructPlaces[i].pos[2]), new Quaternion());
                        break;

                    case 1 :
                        constructPlacesGO[i] = Instantiate(constructLarge, new Vector3(constructPlaces[i].pos[0], constructPlaces[i].pos[1], constructPlaces[i].pos[2]), new Quaternion());
                        break;
                }
            }
        }

        if (container != null) {
            containerGO = new GameObject[container.Length];
            for (int i = 0; i < container.Length; i++) {
                switch (container[i].type) {
                    case 0:
                        containerGO[i] = Instantiate(containerTiny, new Vector3(container[i].pos[0], container[i].pos[1], container[i].pos[2]), new Quaternion());
                        break;

                    case 1:
                        containerGO[i] = Instantiate(containerMedium, new Vector3(container[i].pos[0], container[i].pos[1], container[i].pos[2]), new Quaternion());
                        break;

                    case 2:
                        containerGO[i] = Instantiate(containerLarge, new Vector3(container[i].pos[0], container[i].pos[1], container[i].pos[2]), new Quaternion());
                        break;

                    case 3:
                        containerGO[i] = Instantiate(containerOlli, new Vector3(container[i].pos[0], container[i].pos[1], container[i].pos[2]), new Quaternion());
                        break;
                }
            }
        }
    }

    public void addConstructPlace(Vector3 pos, int type) {
        GameObject[] neuGO = new GameObject[constructPlacesGO.Length + 1];
        ConstructPlace[] neu = new ConstructPlace[constructPlaces.Length + 1];
        for(int i = 0; i < constructPlaces.Length; i++) {
            neuGO[i] = constructPlacesGO[i];
            neu[i] = constructPlaces[i];
        }
        switch (type) {
            case 0:
                pos = new Vector3(pos.x, pos.y+1f, pos.z);
                neuGO[constructPlacesGO.Length] = Instantiate(constructSmall, pos, new Quaternion());
                neu[constructPlaces.Length] = new ConstructPlace(pos,type);
                break;

            case 1:
                pos = new Vector3(pos.x, pos.y + 1.5f, pos.z);
                neuGO[constructPlacesGO.Length] = Instantiate(constructLarge, pos, new Quaternion());
                neu[constructPlaces.Length] = new ConstructPlace(pos, type);
                break;
        }
        constructPlacesGO = neuGO;
        constructPlaces = neu;
        save(GetComponent<World>().LevelName);
    }

    public void addContainer(Vector3 pos, int type) {
        GameObject[] neuGO = new GameObject[containerGO.Length + 1];
        Container[] neu = new Container[container.Length + 1];
        for (int i = 0; i < container.Length; i++) {
            neuGO[i] = containerGO[i];
            neu[i] = container[i];
        }

        switch (type) {
            case 0:
                neuGO[containerGO.Length] = Instantiate(containerTiny, pos, new Quaternion());
                neu[container.Length] = new Container(pos, type);
                break;

            case 1:
                neuGO[containerGO.Length] = Instantiate(containerMedium, pos, new Quaternion());
                neu[container.Length] = new Container(pos, type);
                break;

            case 2:
                neuGO[containerGO.Length] = Instantiate(containerLarge, pos, new Quaternion());
                neu[container.Length] = new Container(pos, type);
                break;

            case 3:
                neuGO[containerGO.Length] = Instantiate(containerOlli, pos, new Quaternion());
                neu[container.Length] = new Container(pos, type);
                break;
        }
        containerGO = neuGO;
        container = neu;
        save(GetComponent<World>().LevelName);
    }

    public void deleteObject(GameObject obj) {
        for (int i = 0; i < constructPlacesGO.Length; i++) {
            if (obj.Equals(constructPlacesGO[i])){
                GameObject[] neuGO = new GameObject[constructPlacesGO.Length - 1];
                ConstructPlace[] neu = new ConstructPlace[constructPlaces.Length - 1];

                for (int a = 0; a < i; a++) {
                    neuGO[a] = constructPlacesGO[a];
                    neu[a] = constructPlaces[a];
                }
                for (int b = i; b < neuGO.Length; b++) {
                    neuGO[b] = constructPlacesGO[b+1];
                    neu[b] = constructPlaces[b+1];
                }
                constructPlacesGO = neuGO;
                constructPlaces = neu;
                Destroy(obj);
                save(GetComponent<World>().LevelName);
                return;
            } 
        }
        for(int i = 0; i < containerGO.Length; i++) {
            if (obj.Equals(containerGO[i])) {
                GameObject[] neuGO = new GameObject[containerGO.Length - 1];
                Container[] neu = new Container[container.Length - 1];

                for (int a = 0; a < i; a++) {
                    neuGO[a] = containerGO[a];
                    neu[a] = container[a];
                }
                for (int b = i; b < neuGO.Length; b++) {
                    neuGO[b] = containerGO[b + 1];
                    neu[b] = container[b + 1];
                }
                containerGO = neuGO;
                container = neu;
                Destroy(obj);
                save(GetComponent<World>().LevelName);
                return;
            }
        }
    }
}
