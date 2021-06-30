namespace SyllabusZip.Services
{
    /// <summary>   The interface describing a CanvasService API </summary>
    public interface ICanvasService
    {
        bool TryGetCourses(out dynamic courses);
    }
}
