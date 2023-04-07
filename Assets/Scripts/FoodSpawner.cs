using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodSpawner : MonoBehaviour
{
    [SerializeField] int foodLimit = 100;
    [SerializeField] int minX = -20;
    [SerializeField] int maxX = 20;
    [SerializeField] int minZ = -20;
    [SerializeField] int maxZ = 20;

    [SerializeField] float spawnRate = 0.1f;
    [SerializeField] float height = 0.1f;

    [SerializeField] GameObject foodPrefab;

    [SerializeField] GameObject parentObject;


    private int foodAmount = 0;

    void Update()
    {
        foodAmount = parentObject.transform.childCount;
        while(foodAmount < foodLimit)
        {
            GameObject food = Instantiate(foodPrefab, new Vector3(Random.Range(minX, maxX), height, Random.Range(minZ, maxZ)), Quaternion.identity);
            food.transform.SetParent(parentObject.transform);
            foodAmount = parentObject.transform.childCount;
        }
    }
}

    // // Start is called before the first frame update
    // void Start()
    // {
    //     StartCoroutine(foodSpawner(minX, maxX, minZ, maxZ, spawnRate, food, height, foodLimit));
    // }

    // Update is called once per frame


    // private IEnumerator foodSpawner(int minX, int maxX, int minZ, int maxZ, float spawnRate, GameObject prefab, float height, int foodLimit)
    // {
    //     while(foodAmount < foodLimit)
    //     {
    //         GameObject food = Instantiate(prefab, new Vector3(Random.Range(minX, maxX), height, Random.Range(minZ, maxZ)), Quaternion.identity);
    //         food.transform.SetParent(parentObject.transform);
    //         foodAmount++;
    //         yield return new WaitForSeconds(spawnRate);
    //     }
    // }