
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardEffectsDelegate
{
    List<Func<IEnumerator>> CardsCoroutines = new();

    public void AddEffect(Func<IEnumerator> enumerator)
    {
        CardsCoroutines.Add(enumerator);
    }
    public void RemoveEffect(Func<IEnumerator> enumerator)
    {
        CardsCoroutines.Remove(enumerator);
    }
    public IEnumerator ActivateEffects()
    {
        foreach(Func<IEnumerator> effect in CardsCoroutines)
        {
            yield return effect();
        }
    }
}
