using UnityEngine;
using System.Collections;
using static StringTools;
using System.Collections.Generic;
using System.Linq;
public class Toy_Bread : Toy_Profile
{
    List<TileController> modifiedOcas = new();
    [SerializeField] float multiplier = 2;
   public override void OnActivatedToy()
   {
        List<TileController> currentOcas = MathJ.GetAllTilesWithTag(TileTags.Oca,null,false);
        foreach (TileController oca in currentOcas)
        {
            ModifyOca(oca);
        }

        _boardController.OnAddedTile.AddListener(OnAddedTile);
        _boardController.OnRemovedTile.AddListener(OnRemovedTile);

   }
   public override void OnDeactivatedToy()
    {
        for (int i = modifiedOcas.Count -1; i >= 0; i--)
        {
            UnModifyOca(modifiedOcas[i]);
        }
        _boardController.OnAddedTile.RemoveListener(OnAddedTile);
        _boardController.OnRemovedTile.RemoveListener(OnRemovedTile);
    }

    void ModifyOca(TileController ocaTile){ ocaTile.BaseDamageModifiers += ocaModifier; modifiedOcas.Add(ocaTile); }
    void UnModifyOca(TileController ocaTile) { ocaTile.BaseDamageModifiers -= ocaModifier; modifiedOcas.Remove(ocaTile); }

    float ocaModifier(float dmg)
    {
        return dmg * 2;
    }
    void OnAddedTile( TileController newTile)
    {
        if(newTile._Info.tileTags.Contains(TileTags.Oca))
        {
            ModifyOca(newTile);
        }
    }
    void OnRemovedTile( TileController removedTile)
    {
        if(removedTile._Info.tileTags.Contains(TileTags.Oca))
        {
            UnModifyOca(removedTile);
        }
    }

  public override string GetTooltipDescription() { return $"OCAS deal x{multiplier} DMG"; }

}