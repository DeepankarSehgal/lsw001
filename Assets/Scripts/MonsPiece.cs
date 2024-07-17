using ExitGames.Client.Photon;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


public enum MonsPieceType
{
    None = 0,
    demon = 1,
    angel = 2,
    drainer = 3,
    spirit = 4,
    mystic = 5,
    mana = 6,
    supermana = 7,
    bomb = 8,
    portion = 9,
    bombOrPortion = 10
}
[Serializable]
public class MonsPieceDataType 
{
    public int team;
    public int currentX;
    public int currentY;
    public MonsPieceType monsPieceType;

    public Vector3 desiredPos;
    public Vector3 desiredScale;

    public Vector2 resetPos;

    public bool isFainted;
    public int faintedTurnsRemaining;


    public bool isCarryingOppMana = false;
    public bool isCarryingSuperMana = false;
    public bool isCarryingBomb = false;
    public bool isCarryingPortion = false;
    public bool isHitBySpirit = false;
    public int whiteFaintGaps = 0;
    public int blackFaintGaps = 0;
    public bool mySpecialAbilityUsed = false;
    public bool onceAbilityUsed = false;
    public int itemChances = 5;
    public bool isCarryingMana = false;
    public bool isCarriedByDrainer = false;
    public bool isScored = false;

    public Vector2Int previousPosition = Vector2Int.zero;

    public static void Register()
    {
        PhotonPeer.RegisterType(typeof(MonsPieceDataType), (byte)'M', MyCustomTypeSerializer.Serialize, MyCustomTypeSerializer.Deserialize);
    }
}
public static class MyCustomTypeSerializer
{
    public static byte[] Serialize(object customObject)
    {
        MonsPieceDataType myCustomType = (MonsPieceDataType)customObject;
        using (MemoryStream ms = new MemoryStream())
        {
            using (BinaryWriter writer = new BinaryWriter(ms))
            {
                writer.Write(myCustomType.team);
                //writer.Write(myCustomType.currentX);
                //writer.Write(myCustomType.currentY);
                writer.Write(((int)myCustomType.monsPieceType));
                writer.Write((myCustomType.mySpecialAbilityUsed));
                writer.Write((myCustomType.onceAbilityUsed));
                writer.Write((myCustomType.itemChances));
            }
            return ms.ToArray();
        }
    }

    public static object Deserialize(byte[] data)
    {
        MonsPieceDataType myCustomType = new MonsPieceDataType();
        using (MemoryStream ms = new MemoryStream(data))
        {
            using (BinaryReader reader = new BinaryReader(ms))
            {
                myCustomType.team = reader.ReadInt32();
                myCustomType.monsPieceType = (MonsPieceType)reader.ReadInt32();
                myCustomType.mySpecialAbilityUsed = reader.ReadBoolean();
                myCustomType.onceAbilityUsed = reader.ReadBoolean();
                myCustomType.itemChances = reader.ReadInt32();
            }
        }
        return myCustomType;
    }
}


public class MonsPiece : MonoBehaviour
{
    //public int team;
    //public int currentX;
    //public int currentY;
    //public MonsPieceType monsPieceType;

    //public Vector3 desiredPos;
    //public Vector3 desiredScale;

    //public Vector2 resetPos;

    //public bool isFainted;
    //private int faintedTurnsRemaining;


    //public bool isCarryingOppMana = false;
    //public bool isCarryingSuperMana = false;
    //public bool isCarryingBomb = false;
    //public bool isCarryingPortion = false;
    //public bool isHitBySpirit = false;
    //public int whiteFaintGaps = 0;
    //public int blackFaintGaps = 0;
    //public bool mySpecialAbilityUsed = false;
    public MonsPieceDataType monsPieceDataType;
    private void Update()
    {
        transform.position = Vector2.Lerp(transform.position, monsPieceDataType.desiredPos, Time.deltaTime*10);
    }

    public virtual List<Vector2Int> GetAvailableMoves(ref MonsPiece[,] board,int tileCount)
    {
        List<Vector2Int> r = new List<Vector2Int>();
        r.Add(new Vector2Int(3, 3));    
        r.Add(new Vector2Int(3, 4));    
        r.Add(new Vector2Int(4, 3));    
        r.Add(new Vector2Int(4, 4));    
        return r;
    }

    public virtual void SetPosition(Vector2 position, bool force = false)
    {
      

        monsPieceDataType.desiredPos = position;
        print("My rpc synch data desired pos is local: " + position);
        if (force)
            transform.localPosition = monsPieceDataType.desiredPos;

    }

    public void FaintForTurns(int numTurns)
    {
        monsPieceDataType.isFainted = true;
        monsPieceDataType.faintedTurnsRemaining = numTurns;
    }

    public void UpdateFaintedTurns()
    {
        if (monsPieceDataType.isFainted)
        {
            monsPieceDataType.faintedTurnsRemaining--;
            if (monsPieceDataType.faintedTurnsRemaining <= 0)
            {
                monsPieceDataType.isFainted = false;
                gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
            }
        }
    }


}
