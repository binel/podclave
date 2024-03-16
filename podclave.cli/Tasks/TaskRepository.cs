
using Microsoft.Extensions.Logging;

namespace Podclave.Cli.Tasks;

public interface ITaskRespository
{
    public void AddTask(WorkTask t);

    public T GetLastTask<T>() where T : WorkTask;

    public WorkTask PopNextTask();
}

public class TaskRespository: ITaskRespository
{
    private readonly ILogger<TaskRespository> _logger;

    private readonly List<WorkTask> _tasks = new List<WorkTask>();

    public TaskRespository(ILogger<TaskRespository> logger)
    {
        _logger = logger;
    }

    public void AddTask(WorkTask t)
    {
        _tasks.Add(t);
    }

    public T GetLastTask<T>() where T : WorkTask
    {
        var task =  _tasks.Where(t => t is T)
                          .OrderByDescending(t => t.DoNotWorkBefore)
                          .FirstOrDefault();    

        return (T)task;    
    }

    public WorkTask PopNextTask()
    {
        var nextTask = _tasks.Where(t => t.DoNotWorkBefore < DateTime.UtcNow)
                                 .OrderBy(t => t.DoNotWorkBefore)
                                 .FirstOrDefault();

        if (nextTask != null)
        {
            _tasks.Remove(nextTask);
        }

        return nextTask;
    }
}