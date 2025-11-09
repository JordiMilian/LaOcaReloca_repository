
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
        foreach(Func<IEnumerator> effect in CardsCoroutines)
        {
            yield return effect();
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
        foreach (var effect in CardsCoroutines)
        {
            yield return effect(arg);
        }
    }
}
