using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SImultaneousCoroutine 
{
    List<Func<IEnumerator>> CoroutinesList = new();
    private class CoroutineState { public bool isFinished; }

    public IEnumerator C_ExecuteCoroutines()
    {
        List<CoroutineState> coroutinesStates = new();

        foreach (var cor in CoroutinesList)
        {
            CoroutineState newState = new CoroutineState();
            coroutinesStates.Add(newState);

            GameController_Simple.Instance.StartCoroutine(RunCoroutine(cor, newState));
        }

        while (true)
        {
            bool isOver = true;
            foreach (CoroutineState state in coroutinesStates)
            {
                if (state.isFinished == false) { isOver = false; break; }
            }
            if (isOver) {yield break; }
            else { yield return null; }
        }
        
    }
    IEnumerator RunCoroutine(Func<IEnumerator> cor, CoroutineState state)
    {
        state.isFinished = false;
        yield return cor();
        state.isFinished = true;
    }

    public void AddCoroutine(Func<IEnumerator> enumerator)
    {
        CoroutinesList.Add(enumerator);
    }
    public void RemoveCoroutine(Func<IEnumerator> enumerator)
    {
        CoroutinesList.Remove(enumerator);
    }
}
