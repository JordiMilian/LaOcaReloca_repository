
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
    public IEnumerator C_ActivateEffects()
    {
        for (int i = CardsCoroutines.Count-1; i >= 0; i--)
        {
            yield return CardsCoroutines[i]();
        }
    }
}
public class CardEffectsDelegate<T>
{
    private readonly List<Func<T, IEnumerator>> CardsCoroutines = new();

    public void AddEffect(Func<T, IEnumerator> enumerator)
    {

        CardsCoroutines.Add(enumerator);
    }

    public void RemoveEffect(Func<T, IEnumerator> enumerator)
    {
        CardsCoroutines.Remove(enumerator);
    }

    public IEnumerator C_ActivateEffects(T arg)
    {
        for (int i = CardsCoroutines.Count - 1; i >= 0; i--)
        {
            yield return CardsCoroutines[i](arg);
        }
    }
}
