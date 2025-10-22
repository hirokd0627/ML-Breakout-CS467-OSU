using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickGeneration : MonoBehaviour
{
    public event System.Action OnGenerationComplete; // Can be used for ball holding script

    [Header("Player")]
    public bool human;
    public bool CPU;

    [Header("Brick Settings")]
    public GameObject brickPrefab; // Brick prefab object
    public bool brickGenAnimations; //toggle to enable/disable brick gen animation
    public int rows = 5; // Number of brick rows
    public int columns = 10; // Number of brick columns
    public float xSpacing = 0f; // Space between bricks (horiz)
    public float ySpacing = 0f; // Space between bricks (vert)
    public float spawnDelay = 0.02f; // Spawn delay for brick gen animation

    [Header("Layout Settings")]
    public Vector2 startPos = new Vector2(-0.0523f, 0.0024f); // Top-left corner of brick layout
    public Vector2 brickSize = new Vector2(0.482675f, 0.2672863f); // Width/height of each brick

    [Header("Color Settings")]
    public Color colorRow1 = Color.red;
    public Color colorRow2 = new Color(1f, 0.5f, 0f); //orange
    public Color colorRow3 = Color.green;
    public Color colorRow4 = new Color(0.4f, 0.8f, 1f); //green
    public Color colorRow5 = Color.yellow;
    public Color colorRow6 = Color.blue;

    void Start()
    {
        StartCoroutine(GenerateLevel()); //Start coroutine for brick generation animatino
    }

    IEnumerator GenerateLevel()
    {
        // Detect brick size automatically
        if (brickPrefab != null)
        {
            SpriteRenderer srPrefab = brickPrefab.GetComponent<SpriteRenderer>();
            if (srPrefab != null)
                brickSize = srPrefab.bounds.size;
        }

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                // Calculate position
                float x = startPos.x + (col * (brickSize.x + xSpacing));
                float y = startPos.y - (row * (brickSize.y + ySpacing));
                Vector3 brickPos = new Vector3(x, y, 0);

                // Spawn brick
                GameObject newBrick = Instantiate(
                    brickPrefab,
                    brickPos,
                    Quaternion.identity,
                    transform
                );

                // Color brick gradient by row
                SpriteRenderer sr = newBrick.GetComponent<SpriteRenderer>();
                BrickCollision bc = newBrick.GetComponent<BrickCollision>();
                if (sr != null)
                {
                    // Assign color based on row
                    if (row == 0)
                    {
                        sr.color = colorRow1;
                        if (bc != null)
                            bc.points = 7;
                    }
                    else if (row == 1)
                    {
                        sr.color = colorRow2;
                        if (bc != null)
                            bc.points = 7;
                    }
                    else if (row == 2)
                    {
                        sr.color = colorRow3;
                        if (bc != null)
                            bc.points = 4;
                    }
                    else if (row == 3)
                    {
                        sr.color = colorRow4;
                        if (bc != null)
                            bc.points = 4;
                    }
                    else if (row == 4)
                    {
                        sr.color = colorRow5;
                        if (bc != null)
                            bc.points = 1;
                    }
                    else
                    {
                        sr.color = colorRow6;
                        if (bc != null)
                            bc.points = 1;
                    }
                }

                // Animate brick spawn
                if (brickGenAnimations)
                {
                    StartCoroutine(AnimateBrickSpawn(newBrick));
                }
                // Delay between brick spawn
                if (brickGenAnimations)
                {
                    yield return new WaitForSeconds(spawnDelay);
                }
            }
        }
        OnGenerationComplete?.Invoke(); // Mark brick gen complete
    }

    IEnumerator AnimateBrickSpawn(GameObject brick)
    {
        float duration = 0.2f; // animation length
        float elapsed = 0f;

        Vector3 targetScale = brick.transform.localScale;
        brick.transform.localScale = Vector3.zero; // start small

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            t = Mathf.Sin(t * Mathf.PI * 0.5f); // ease-out
            brick.transform.localScale = Vector3.Lerp(Vector3.zero, targetScale, t);
            yield return null;
        }

        brick.transform.localScale = targetScale;
    }

    // Call this to clear all bricks (after death/completion)
    public void ClearLevel()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }
}
