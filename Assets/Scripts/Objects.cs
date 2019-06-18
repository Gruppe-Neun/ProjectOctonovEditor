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

    [Serializable()]
    public struct LightSource {
        public LightSource(Vector3 position, float range, float intensity) {
            pos = new float[] { position.x, position.y, position.z };
            this.range = range;
            this.intensity = intensity;
        }
        public float[] pos;
        public float range;
        public float intensity;
    }

    [Serializable()]
    public struct SpawnPoint {
        public SpawnPoint(Vector3 position, int spawnType) {
            pos = new float[] { position.x, position.y, position.z };
            type = spawnType;
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

    [SerializeField] private GameObject lightSource;
    [SerializeField] private GameObject spawnPoint;

    private ConstructPlace[] constructPlaces = new ConstructPlace[0];
    private Container[] container = new Container[0];
    private LightSource[] light = new LightSource[0];
    private SpawnPoint[] spawn = new SpawnPoint[0];

    private GameObject[] constructPlacesGO = new GameObject[0];
    private GameObject[] containerGO = new GameObject[0];
    private GameObject[] lightGO = new GameObject[0];
    private GameObject[] spawnGO = new GameObject[0];

    void Start() {
        load(GetComponent<World>().LevelName);
        buildObjects();
    }

    public void load(string levelName) {
        string path = Application.dataPath + "/leveldata/" + levelName + "/";
        string constructPlacesFile = path + "objects_constructPlaces.obj";
        string containerFile = path + "objects_container.obj";
        string lightFile = path + "objects_light.obj";
        string spawnFile = path + "objects_spawn.obj";

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
        if (File.Exists(lightFile)) {
            file = File.Open(lightFile, FileMode.Open);
            light = (LightSource[])bf.Deserialize(file);
            file.Close();
        }
        if (File.Exists(spawnFile)) {
            file = File.Open(spawnFile, FileMode.Open);
            spawn = (SpawnPoint[])bf.Deserialize(file);
            file.Close();
        }

    }

    public void save(string levelName) {
        string path = Application.dataPath + "/leveldata/" + levelName + "/";
        string constructPlacesFile = path + "objects_constructPlaces.obj";
        string containerFile = path + "objects_container.obj";
        string lightFile = path + "objects_light.obj";
        string spawnFile = path + "objects_spawn.obj";

        if (!File.Exists(constructPlacesFile)) {
            Directory.CreateDirectory(Path.GetDirectoryName(constructPlacesFile));
        }
        if (!File.Exists(containerFile)) {
            Directory.CreateDirectory(Path.GetDirectoryName(containerFile));
        }
        if (!File.Exists(lightFile)) {
            Directory.CreateDirectory(Path.GetDirectoryName(lightFile));
        }
        if (!File.Exists(spawnFile)) {
            Directory.CreateDirectory(Path.GetDirectoryName(spawnFile));
        }

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file;

        file = File.Open(constructPlacesFile, FileMode.OpenOrCreate);
        bf.Serialize(file, constructPlaces);
        file.Close();

        file = File.Open(containerFile, FileMode.OpenOrCreate);
        bf.Serialize(file, container);
        file.Close();

        file = File.Open(lightFile, FileMode.OpenOrCreate);
        bf.Serialize(file, light);
        file.Close();

        file = File.Open(spawnFile, FileMode.OpenOrCreate);
        bf.Serialize(file, spawn);
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

        if (light != null) {
            lightGO = new GameObject[light.Length];
            for(int i = 0; i < light.Length; i++) {
                lightGO[i] = Instantiate(lightSource, new Vector3(light[i].pos[0], light[i].pos[1], light[i].pos[2]), new Quaternion());
                lightGO[i].GetComponent<Light>().intensity = light[i].intensity;
                lightGO[i].GetComponent<Light>().range = light[i].range;
            }
        }

        if (spawn != null) {
            spawnGO = new GameObject[spawn.Length];
            for(int i = 0;i < spawn.Length; i++) {
                spawnGO[i] = Instantiate(spawnPoint, new Vector3(spawn[i].pos[0], spawn[i].pos[1], spawn[i].pos[2]), new Quaternion());
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
                pos = new Vector3(pos.x, pos.y, pos.z);
                neuGO[constructPlacesGO.Length] = Instantiate(constructSmall, pos, new Quaternion());
                neu[constructPlaces.Length] = new ConstructPlace(pos,type);
                break;

            case 1:
                pos = new Vector3(pos.x, pos.y + 0.5f, pos.z);
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

    public void addLight(Vector3 pos, float range, float intensity) {
        GameObject[] neuGO = new GameObject[lightGO.Length + 1];
        LightSource[] neu = new LightSource[light.Length + 1];
        for (int i = 0; i < light.Length; i++) {
            neuGO[i] = lightGO[i];
            neu[i] = light[i];
        }
        neu[light.Length] = new LightSource(pos, range, intensity);
        neuGO[lightGO.Length] = Instantiate(lightSource, pos, new Quaternion());
        neuGO[lightGO.Length].GetComponent<Light>().intensity = intensity;
        neuGO[lightGO.Length].GetComponent<Light>().range = range;
        lightGO = neuGO;
        light = neu;
        save(GetComponent<World>().LevelName);
    }

    public void addSpawn(Vector3 pos, int type) {
        GameObject[] neuGO = new GameObject[spawnGO.Length + 1];
        SpawnPoint[] neu = new SpawnPoint[spawn.Length + 1];
        for (int i = 0; i < spawn.Length; i++) {
            neuGO[i] = spawnGO[i];
            neu[i] = spawn[i];
        }
        neu[spawn.Length] = new SpawnPoint(pos, type);
        neuGO[spawnGO.Length] = Instantiate(spawnPoint, pos, new Quaternion());
        spawnGO = neuGO;
        spawn = neu;
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
        for (int i = 0; i < lightGO.Length; i++) {
            if (obj.Equals(lightGO[i])) {
                GameObject[] neuGO = new GameObject[lightGO.Length - 1];
                LightSource[] neu = new LightSource[light.Length - 1];

                for (int a = 0; a < i; a++) {
                    neuGO[a] = lightGO[a];
                    neu[a] = light[a];
                }
                for (int b = i; b < neuGO.Length; b++) {
                    neuGO[b] = lightGO[b + 1];
                    neu[b] = light[b + 1];
                }
                lightGO = neuGO;
                light = neu;
                Destroy(obj);
                save(GetComponent<World>().LevelName);
                return;
            }
        }
        for (int i = 0; i < spawnGO.Length; i++) {
            if (obj.Equals(spawnGO[i])) {
                GameObject[] neuGO = new GameObject[spawnGO.Length - 1];
                SpawnPoint[] neu = new SpawnPoint[spawn.Length - 1];

                for (int a = 0; a < i; a++) {
                    neuGO[a] = spawnGO[a];
                    neu[a] = spawn[a];
                }
                for (int b = i; b < neuGO.Length; b++) {
                    neuGO[b] = spawnGO[b + 1];
                    neu[b] = spawn[b + 1];
                }
                spawnGO = neuGO;
                spawn = neu;
                Destroy(obj);
                save(GetComponent<World>().LevelName);
                return;
            }
        }
    }
}
