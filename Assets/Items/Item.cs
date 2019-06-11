using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public static class Item
{
    public enum useType {
        UNDEF,
        GENERIC,
        ACTIVE,
        AMMO,
        OBJECT
    }

    public enum Type{
        //generic items (UNDEF muss als erstes)
        UNDEF,
        Battery,
        Nut,
        Ironplate,
        Flashlight,
        CristalBlue,
        CristalRed,

        Olli_ArmRight,
        Olli_ArmLeft,
        Olli_LegRight,
        Olli_LegLeft,
        Olli_Body,

        //Active Items ()
        Medkit,

        //Ammo (LASER_BLUE muss als erstes)
        LaserBlue,
        LaserRed

        //Objects ()
            
    }

    private static Texture2D[] sprites;
    private static Mesh[] itemMesh;
    private static Material[] itemMaterial;

    public static void loadSprites() {
        string[] names = Enum.GetNames(typeof(Type));
        sprites = new Texture2D[names.Length];
        for (int i = 0;i<names.Length;i++) {
            string filePath = Application.dataPath + "/Items/Sprites/Item_" + names[i] + ".png";
            if (File.Exists(filePath)) {
                sprites[i] = new Texture2D(128, 64);
                sprites[i].LoadImage(File.ReadAllBytes(filePath));
            }
        }
    }

    public static void loadModels() {
        string[] names = Enum.GetNames(typeof(Type));
        itemMaterial = new Material[names.Length];
        itemMesh = new Mesh[names.Length];
        for (int i = 0;i<names.Length;i++) {
            if (sprites[i]!=null) {
                itemMaterial[i] = new Material(Shader.Find("Sprites/Diffuse"));
                itemMaterial[i].mainTexture = sprites[i];
                itemMesh[i] = createItemMesh();
            }
        }
        
    }

    public static Texture2D getSprite(Type itemType) {
        if (sprites[(int)itemType] == null) {
            loadSprites();
        }
        else{
            return sprites[(int)itemType];
        }
        

        return null;
    }

    public static useType getUseType(Type itemType) {
        if ((int)itemType >= 1000) {
            return useType.OBJECT;
        } else {
            if ((int)itemType >= (int)Type.LaserBlue) {
                return useType.AMMO;
            } else {
                if ((int)itemType >= (int)Type.Medkit) {
                    return useType.ACTIVE;
                } else {
                    if ((int)itemType >= (int)Type.UNDEF) {
                        return useType.GENERIC;
                    } else {
                        return useType.UNDEF;
                    }
                }
            }
        }
    }

    private static Mesh createItemMesh() {
        Mesh itemMesh = new Mesh();

        //set vertices

        Vector3[] vertices = new Vector3[4];
        vertices[0] = new Vector3(-1, -0.5f, 0);
        vertices[1] = new Vector3(1, -0.5f, 0);
        vertices[2] = new Vector3(-1, 0.5f, 0);
        vertices[3] = new Vector3(1, 0.5f, 0);
        itemMesh.vertices = vertices;

        //set triangles

        int[] tri = new int[6];
        //  Lower left triangle.
        tri[0] = 0;
        tri[1] = 2;
        tri[2] = 1;
        //  Upper right triangle.   
        tri[3] = 2;
        tri[4] = 3;
        tri[5] = 1;
        itemMesh.triangles = tri;

        //set normals

        Vector3[] normals = new Vector3[4];

        normals[0] = -Vector3.forward;
        normals[1] = -Vector3.forward;
        normals[2] = -Vector3.forward;
        normals[3] = -Vector3.forward;

        itemMesh.normals = normals;

        //set UVs

        Vector2[] uv = new Vector2[4];

        uv[0] = new Vector2(0, 0);
        uv[1] = new Vector2(1, 0);
        uv[2] = new Vector2(0, 1);
        uv[3] = new Vector2(1, 1);

        itemMesh.uv = uv;

        return itemMesh;
    }

    public static ItemBehavior createItem(Type itemType,int amount,Vector3 position) {
        GameObject item = GameObject.CreatePrimitive(PrimitiveType.Sphere);

        if ((int)itemType >= (int)Type.LaserBlue) {
            item.AddComponent<AmmoBehavior>();
            item.GetComponent<ItemBehavior>().type = itemType;
            item.GetComponent<AmmoBehavior>().setValues();
        } else {
            item.AddComponent<ItemBehavior>();
            item.GetComponent<ItemBehavior>().type = itemType;
        }
        
        item.GetComponent<ItemBehavior>().useType = getUseType(itemType);
        item.GetComponent<ItemBehavior>().amount = amount;

        item.GetComponent<MeshFilter>().mesh = itemMesh[(int)itemType];

        item.GetComponent<MeshRenderer>().material = itemMaterial[(int) itemType];

        //item.GetComponent<MeshCollider>().sharedMesh = getItemMesh();
        item.GetComponent<SphereCollider>().isTrigger = true;

        if (position.x==0&&position.y==0&&position.z==0) {
            item.GetComponent<ItemBehavior>().take();
            item.transform.position = new Vector3(0, 0, 0);
        } else {
            item.GetComponent<ItemBehavior>().drop();
            item.transform.position = position;
        }
        

        
        return item.GetComponent<ItemBehavior>();
    }
}
