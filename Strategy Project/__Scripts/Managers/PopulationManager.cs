using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopulationManager : MonoSingleton<PopulationManager>
{
    [SerializeField] private int currentPopulation;
    [SerializeField] private int populationLimit;
    public int GetCurrentPopulation()    {        return currentPopulation;     }
    public int GetPopulationLimit()    {        return populationLimit;    }
    public bool CanCreateUnit()
    {
        if (currentPopulation < populationLimit)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
    public void IncreaseCurrentPopulation(int population)
    {
        currentPopulation += population;
    }
    public void DecreaseCurrentPopulation(int population)
    {
        currentPopulation -= population;
    }
    public void IncreasePopulationLimit(int population)
    {
        populationLimit += population;
    }
    public void DecreasePopulationLimit(int population)
    {
        populationLimit -= population;
    }
}
