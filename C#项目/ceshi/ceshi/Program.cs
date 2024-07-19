using Python.Runtime;

public class PythonExecution
{
    public void ExecutePythonCode()
    {
        using (Py.GIL())
        {
            dynamic py = Py.Import("__main__");
            py.execute_code("print('Hello from Python!')");
        }
    }
}
