using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FruitGeneration : MonoBehaviour
{
    private int force_vertical = 200;
    private int force_horizontal = 50;

    public float minSpawnDelay = 0.3f;
    public float maxSpawnDelay = 1.5f;
    private float timeToNextSpawn;

    public GameObject[] fruits;

    void CreateFruit(GameObject fruit_type, double spawn_angle) {
        spawn_angle = spawn_angle/180.0*System.Math.PI + System.Math.PI/2.0;
        float spawn_x = (float)System.Math.Cos(spawn_angle)*5.5f;
        float spawn_z = (float)System.Math.Sin(spawn_angle)*5.5f;

        GameObject fruit = Instantiate(fruit_type, new Vector3(spawn_x, 0.75f, spawn_z), Quaternion.identity);

        fruit.transform.Rotate(Random.Range(0, 359), Random.Range(0, 359), Random.Range(0, 359));

        float random = (float)(Random.Range(-100, 100))/100.0f;
        double target = spawn_angle - System.Math.PI + random*System.Math.PI/12.0f;

        float target_x = (float)System.Math.Cos(target)*6;
        float target_z = (float)System.Math.Sin(target)*6;
        fruit.GetComponent<Rigidbody>().AddForce(target_x*(force_horizontal+Random.Range(0, 15)), 
                                                force_vertical + Random.Range(0, 40), 
                                                target_z*(force_horizontal+Random.Range(0, 15)));
    }

    void Start()
    {
        ScheduleNextSpawn();
    }

    void ScheduleNextSpawn()
    {
        // Map range (0..345-ish) to a 0..1 difficulty factor
        float difficulty = Mathf.InverseLerp(0f, 345f, VariableHolder.range);

        // Lerp between max and min delay based on difficulty
        float baseDelay = Mathf.Lerp(maxSpawnDelay, minSpawnDelay, difficulty);

        // Add a bit of randomness around that delay
        timeToNextSpawn = Random.Range(baseDelay * 0.7f, baseDelay * 1.3f);
    }

    void Update()
    {
        timeToNextSpawn -= Time.deltaTime;

        if (timeToNextSpawn <= 0f)
        {
            // Pick fruit type
            GameObject fruit = fruits[Random.Range(0, fruits.Length)];

            // Random spawn angle within the range
            double spawn_angle = (double)(Random.Range(0, VariableHolder.range)) 
                               - (double)VariableHolder.range / 2.0;

            CreateFruit(fruit, spawn_angle);

            // Plan the next spawn
            ScheduleNextSpawn();
        }
    }
}