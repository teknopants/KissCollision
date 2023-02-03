using System;
using System.Diagnostics;
using UnityEngine;

public class Game : MonoBehaviour
{
    // This system is inspired by MoonTools.ECS:
    // https://gitea.moonside.games/MoonsideGames/MoonTools.ECS

    // Engines are executed in this order. Don't forget to add new engines here after you make em!
    public Engine[] engines = {
    };

    public bool LogPerformance = false;
    public float frameRate = 60;
    const float MAX_FRAME_RATE = 500;

    private long time = 0;
    private float frames = 0;
    private readonly Stopwatch stopWatch = new Stopwatch();
    private readonly Stopwatch runTimerStopWatch = new Stopwatch();

    public void Start()
    {
        stopWatch.Start();
    }

    public void Update()
    {
        stopWatch.Stop();

        time += stopWatch.ElapsedMilliseconds;
        stopWatch.Reset();

        frames = 0;

        frameRate = Mathf.Min(frameRate, MAX_FRAME_RATE);
        long max = (long)(1000 / frameRate);
        while(time >= max)
        {
            time -= max;
            frames += 1;
        }

        stopWatch.Start();

        // Update Engines
        for(var i = 0; i < frames; i ++)
        {
            foreach (Engine engine in engines)
            {
                if (!LogPerformance)
                {
                    engine.Run(1 / frameRate);
                    continue;
                }
                else
                {
                    // Log performance of individual engines
                    runTimerStopWatch.Start();
                    engine.Run(1 / frameRate);
                    runTimerStopWatch.Stop();
                    UnityEngine.Debug.Log(engine.ToString() + " ran in " + ((float)runTimerStopWatch.ElapsedTicks / TimeSpan.TicksPerMillisecond).ToString() + " ms");
                    runTimerStopWatch.Reset();
                }
            }
        }
    }
}

public abstract class Engine
{
    public abstract void Run(float dt);
}