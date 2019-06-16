using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

/// <summary>
/// The serializable BlockData class contains block information that are to be saved to a file.
/// </summary>
[Serializable]
class BlockData
{
	public Block.BlockType[,,] matrix;
	
    /// <summary>
    /// Empty constructor
    /// </summary>
	public BlockData(){}

    /// <summary>
    /// The constrcutor initializes its matrix for storing all blocks of the given chunk.
    /// </summary>
    /// <param name="b">3D block array (i.e. chunk)</param>
	public BlockData(Block[,,] b)
	{
		matrix = new Block.BlockType[World.chunkSize,World.chunkSize,World.chunkSize];
		for(int z = 0; z < World.chunkSize; z++)
			for(int y = 0; y < World.chunkSize; y++)
				for(int x = 0; x < World.chunkSize; x++)
				{
					matrix[x,y,z] = b[x,y,z].blockType;
				}
	}
}

/// <summary>
/// Chunk class that takes care of storing the information of the chunk's blocks.
/// It renders the chunk and provides functionality for saving, loading and updating the chunk.
/// </summary>
public class Chunk
{
	public Material cubeMaterial;   // Materia for solid blocks
	public Material fluidMaterial;  // Material for transparent blocks
	public Block[,,] chunkData;     // 3D Array containing all blocks of the chunk
	public GameObject chunk;        // GameObject that holds the mesh of the solid parts of the chunk
	public GameObject fluid;        // GameObject that holds the mesh of the transparent parts, like water, of the chunk
	public enum ChunkStatus
    {
        DRAW,                       // DRAW: data of the chunk has been created and needs to be rendered next
        DONE                        // DONE: Trees have been built and the chunk has been rendered
    };
	public ChunkStatus status;      // Current state of the chunk
	public ChunkMB mb;              // The MonoBehaviour of the Chunk
	BlockData bd;                   // 
	public bool changed = false;    // If a chunk got modified (e.g. a block got destroyed by the player), set this to true to redraw the chunk upon the next update.
    private bool[] isFilled = new bool[] { false, false, false, false, false, false };
    private string levelName = "default";

    /// <summary>
    /// Creates a file name for the to be saved or loaded chunk based on its position. On Windows machines the data is saved in AppData\LocalLow\DefaultCompany.
    /// </summary>
    /// <param name="v">Position of the chunk</param>
    /// <returns>Returns the file name of the to be saved or loaded chunk</returns>
	string BuildChunkFileName(Vector3 v)
	{
		return Application.dataPath + "/leveldata/"+levelName+"/Chunk_" + 
								(int)v.x + "_" +
									(int)v.y + "_" +
										(int)v.z + 
										"_" + World.chunkSize +
										"_" + World.radius +
										".dat";
	}

    /// <summary>
    /// Loads chunk data from file.
    /// </summary>
    /// <returns>Returns true if the file to be loaded exists</returns>
	private bool Load()
	{
		string chunkFile = BuildChunkFileName(chunk.transform.position);
		if(File.Exists(chunkFile))
		{
            
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(chunkFile, FileMode.Open);
			bd = new BlockData();
			bd.matrix = (Block.BlockType[,,]) bf.Deserialize(file);
			file.Close();

            chunkData = new Block[World.chunkSize, World.chunkSize, World.chunkSize];
            for (int z = 0; z < World.chunkSize; z++)
                for (int y = 0; y < World.chunkSize; y++)
                    for (int x = 0; x < World.chunkSize; x++) {
                        chunkData[x, y, z] = new Block(bd.matrix[x,y,z], new Vector3(x, y, z), this.chunk.gameObject, this);
                    }
            return true;
		}
		return false;
	}

    /// <summary>
    /// Writes chunk data to file.
    /// </summary>
	public void Save()
	{
        //check if chunk is empty
        bool empty = true;
        for (int z = 0; z < World.chunkSize; z++)
            for (int y = 0; y < World.chunkSize; y++)
                for (int x = 0; x < World.chunkSize; x++) {
                    if (bd.matrix[x, y, z] != Block.BlockType.AIR) {
                        empty = false;
                        z = World.chunkSize;
                        y = World.chunkSize;
                        x = World.chunkSize;
                    }
                }
        if (empty) { //if chunk is empty delete file
            string chunkFile = BuildChunkFileName(chunk.transform.position);
            if (File.Exists(chunkFile)) {
                File.Delete(chunkFile); 
            }
            if (File.Exists(chunkFile + ".meta")) {
                File.Delete(chunkFile + ".meta");
            }

        } else { //if chunk is not empty save data
            string chunkFile = BuildChunkFileName(chunk.transform.position);
            
            if (!File.Exists(chunkFile)) {
                Directory.CreateDirectory(Path.GetDirectoryName(chunkFile));
            }
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(chunkFile, FileMode.OpenOrCreate);
            bd = new BlockData(chunkData);
            bf.Serialize(file, bd.matrix);

            file.Close();
        }
	}

    /// <summary>
    /// If a block was destroyed upon player interaction, trigger the process of dropping sand for each sand block.
    /// </summary>
	public void UpdateChunk()
	{
		for(int z = 0; z < World.chunkSize; z++)
			for(int y = 0; y < World.chunkSize; y++)
				for(int x = 0; x < World.chunkSize; x++)
				{
					if(chunkData[x,y,z].blockType == Block.BlockType.SAND)
					{
						mb.StartCoroutine(mb.Drop(chunkData[x,y,z], 
										Block.BlockType.SAND, 
										20));
					}
				}
	}

    /// <summary>
    /// Builds the chunk from scatch or loads it from file. This functions does not draw the chunk.
    /// </summary>
	private void BuildChunk() {

        if (!Load()) {

            chunkData = new Block[World.chunkSize, World.chunkSize, World.chunkSize];
            for (int z = 0; z < World.chunkSize; z++)
                for (int y = 0; y < World.chunkSize; y++)
                    for (int x = 0; x < World.chunkSize; x++) {
                        chunkData[x, y, z] = new Block(Block.BlockType.AIR, new Vector3(x, y, z), this.chunk.gameObject, this);
                    }


            bd = new BlockData(chunkData);
        }
        status = ChunkStatus.DRAW;

    }

    /// <summary>
    /// Redraws this chunk by destroying all mesh and collision components and then creating new ones.
    /// </summary>
	public void Redraw()
	{
        GameObject.DestroyImmediate(chunk.GetComponent<MeshFilter>());
		GameObject.DestroyImmediate(chunk.GetComponent<MeshRenderer>());
		GameObject.DestroyImmediate(chunk.GetComponent<Collider>());
		GameObject.DestroyImmediate(fluid.GetComponent<MeshFilter>());
		GameObject.DestroyImmediate(fluid.GetComponent<MeshRenderer>());
		GameObject.DestroyImmediate(fluid.GetComponent<Collider>());
		DrawChunk();
	}

    /// <summary>
    /// Draws the chunk. If trees are not created yet, create them.
    /// The draw process creates meshes for all blocks and then combines them to a solid and a transparent mesh.
    /// </summary>
	public void DrawChunk() { 
		for(int z = 0; z < World.chunkSize; z++)
			for(int y = 0; y < World.chunkSize; y++)
				for(int x = 0; x < World.chunkSize; x++)
				{
					chunkData[x,y,z].Draw();
				}

        // Prepare solid chunk mesh
		CombineQuads(chunk.gameObject, cubeMaterial);
		MeshCollider collider = chunk.gameObject.AddComponent(typeof(MeshCollider)) as MeshCollider;
		collider.sharedMesh = chunk.transform.GetComponent<MeshFilter>().mesh;

        // Prepare transparent chunk mesh
		CombineQuads(fluid.gameObject, fluidMaterial);

		status = ChunkStatus.DONE;
	}


    /// <summary>
    /// Empty constructor.
    /// </summary>
	public Chunk(){}

	/// <summary>
    /// Initializes a chunk by providing a position, a material for blocks and a material for partially transparent blocks.
    /// </summary>
    /// <param name="position">Position of the chunk</param>
    /// <param name="c">The material for the solid blocks of the chunk</param>
    /// <param name="t">The material for the transparent blocks of the chunk</param>
	public Chunk (string levelName,Vector3 position, Material c, Material t)
    {
        // Create GameObjects holding the chunk's meshes
        this.levelName = levelName;
		chunk = new GameObject(World.BuildChunkName(position));         // solid chunk mesh, e.g. dirt blocks
		chunk.transform.position = position;
		fluid = new GameObject(World.BuildChunkName(position)+"_F");    // transparent chunk mesh, e.g. water blocks
		fluid.transform.position = position;

		mb = chunk.AddComponent<ChunkMB>();                             // Adds the chunk's Monobehaviour
		mb.SetOwner(this);
		cubeMaterial = c;
		fluidMaterial = t;
		BuildChunk();                                                   // Start building the chunk
	}
	
	public void CombineQuads(GameObject o, Material m)
	{
		// 1. Combine all children meshes
		MeshFilter[] meshFilters = o.GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];
        int i = 0;
        while (i < meshFilters.Length) {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            i++;
        }

        // 2. Create a new mesh on the parent object
        MeshFilter mf = (MeshFilter) o.gameObject.AddComponent(typeof(MeshFilter));
        mf.mesh = new Mesh();

        // 3. Add combined meshes on children as the parent's mesh
        mf.mesh.CombineMeshes(combine);

        // 4. Create a renderer for the parent
		MeshRenderer renderer = o.gameObject.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
		renderer.material = m;

		// 5. Delete all uncombined children
		foreach (Transform quad in o.transform) {
     		GameObject.Destroy(quad.gameObject);
 		}
	}

    //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    //Edit Methods
    //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    

    public void fill(bool solid, Block.BlockType block) {
        Block.BlockType fillType = Block.BlockType.AIR;
        if (solid) fillType = block;
        chunkData = new Block[World.chunkSize, World.chunkSize, World.chunkSize];
        for (int z = 0; z < World.chunkSize; z++)
            for (int y = 0; y < World.chunkSize; y++)
                for (int x = 0; x < World.chunkSize; x++) {
                    chunkData[x, y, z] = new Block(fillType, new Vector3(x, y, z), this.chunk.gameObject, this);
                }


        bd = new BlockData(chunkData);
        Redraw();
        Save();
    }

    public void setOuter(int site,Block.BlockType block) {



        Block.BlockType fillBlock = Block.BlockType.AIR;
        if (!this.isFilled[site]) {
            fillBlock = block;
            isFilled[site] = true;
        } else {
            isFilled[site] = false;
        }

        int[] minCo = new int[] { 0, 0, 0 };
        int[] maxCo = new int[] { World.chunkSize, World.chunkSize, World.chunkSize };

        switch (site) {
            case 0: //top
                minCo[1] = World.chunkSize-1;
                break;

            case 1: //front
                maxCo[2] = 1;
                break;

            case 2: //right
                minCo[0] = World.chunkSize-1;
                break;

            case 3: //back
                minCo[2] = World.chunkSize-1;
                break;

            case 4: //left
                maxCo[0] = 1;
                break;

            case 5: //bottom
                maxCo[1] = 1;
                break;


        }
        for (int z = minCo[2]; z < maxCo[2]; z++)
            for (int y = minCo[1]; y < maxCo[1]; y++)
                for (int x = minCo[0]; x < maxCo[0]; x++) {
                    chunkData[x, y, z] = new Block(fillBlock, new Vector3(x, y, z), this.chunk.gameObject, this);
                    bd = new BlockData(chunkData);
                }

        Redraw();
        Save();

    }

    public Block[,,] copyChunkData() {
        return chunkData;
    }

    public void pasteChunkData(Block[,,] data) {
        chunkData = data;
        bd = new BlockData(chunkData);
        Redraw();
        Save();
    }

    public void setChunkType(int type,Block.BlockType block) {

        switch (type) {
            case 0://stairs
                chunkData = new Block[World.chunkSize, World.chunkSize, World.chunkSize];
                for (int z = 0; z < World.chunkSize; z++)
                    for (int y = 1; y < World.chunkSize; y++)
                        for (int x = 0; x < World.chunkSize; x++) {
                            chunkData[x, y, z] = new Block(Block.BlockType.AIR, new Vector3(x, y, z), this.chunk.gameObject, this);
                        }
                for (int y = 0; y < World.chunkSize; y++)
                    for (int z = y; z < World.chunkSize; z++)
                        for (int x = 0; x < World.chunkSize; x++) {
                            chunkData[x, y, z] = new Block(block, new Vector3(x, y, z), this.chunk.gameObject, this);
                        }

                bd = new BlockData(chunkData);
                break;

        }

        Redraw();
        Save();
    }

    public void rotateChunk(int axis) {

        Block[,,] newChunkData = new Block[World.chunkSize, World.chunkSize, World.chunkSize];

        


        switch (axis) {
            case 0://z axis

                for (int z = 0; z < World.chunkSize; z++)
                    for (int y = 0; y < World.chunkSize; y++)
                        for (int x = 0; x < World.chunkSize; x++) {
                            newChunkData[x,y,z] = new Block(bd.matrix[World.chunkSize-1-y,x,z] , new Vector3(x, y, z), this.chunk.gameObject, this);
                        }
                break;

            case 1://y axis

                for (int z = 0; z < World.chunkSize; z++)
                    for (int y = 0; y < World.chunkSize; y++)
                        for (int x = 0; x < World.chunkSize; x++) {
                            newChunkData[x, y, z] = new Block(bd.matrix[World.chunkSize - 1-z, y, x], new Vector3(x, y, z), this.chunk.gameObject, this);
                        }
                break;

            case 2://x axis
                for (int z = 0; z < World.chunkSize; z++)
                    for (int y = 0; y < World.chunkSize; y++)
                        for (int x = 0; x < World.chunkSize; x++) {
                            newChunkData[x, y, z] = new Block(bd.matrix[x, World.chunkSize-1-z, y], new Vector3(x, y, z), this.chunk.gameObject, this);
                        }
                break;
        }

        chunkData = newChunkData;
        bd = new BlockData(chunkData);
        Redraw();
        Save();
    }
}
