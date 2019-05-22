using System.Collections;
using UnityEngine;

/// <summary>
/// MonoBehavior of a chunk' GameObject to process Coroutines.
/// </summary>
public class ChunkMB: MonoBehaviour
{
	Chunk owner;
	public ChunkMB(){}

    /// <summary>
    /// Assigns the reference to the chunk who possesses this MonoBehavior.
    /// </summary>
    /// <param name="o"></param>
	public void SetOwner(Chunk o)
	{
		owner = o;
		//InvokeRepeating("SaveProgress",10,1000);
	}

    /// <summary>
    /// Coroutine to allow a squence of dropping a Block downwards like sand.
    /// </summary>
    /// <param name="b">Block to be dropped</param>
    /// <param name="bt">BlockType</param>
    /// <param name="maxDrop">Maximum number of drops</param>
    /// <returns></returns>
	public IEnumerator Drop(Block b, Block.BlockType bt, int maxDrop)
	{
		Block thisBlock = b;
		Block prevBlock = null;
		for(int i = 0; i < maxDrop; i++)
		{
			Block.BlockType previousType = thisBlock.blockType;
			if(previousType != bt)
				thisBlock.SetType(bt);
			if(prevBlock != null)
				prevBlock.SetType(previousType);

			prevBlock = thisBlock;
			b.owner.Redraw();
			
			yield return new WaitForSeconds(0.2f);
			Vector3 pos = thisBlock.position;
			
			thisBlock = thisBlock.GetBlock((int)pos.x,(int)pos.y-1,(int)pos.z);
			if(thisBlock.isSolid)
			{	
				yield break;
			}
		}
	}

    /// <summary>
    /// Saves the underlying chunk.
    /// </summary>
	private void SaveProgress()
	{
		if(owner.changed)
		{
			owner.Save();
			owner.changed = false;
		}
	}
}
