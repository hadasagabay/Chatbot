using BasePlugin.Interfaces;
using BasePlugin.Records;
using System.Collections.Generic;
using System.Text.Json;

public class ListPlugin : IPlugin
{
    public static string _Id = "list";
    public string Id => _Id;

    public PluginOutput Execute(PluginInput input)
    {
        List<string> list = new();

        if (string.IsNullOrEmpty(input.PersistentData) == false)
        {
            list = JsonSerializer.Deserialize<PersistentDataStructure>(input.PersistentData).List;
        }

        string message = input.Message.ToLower();

        if (message == "")
        {
            input.Callbacks.StartSession();
            return new PluginOutput("List started. Enter 'Add' to add task. Enter 'Delete' to delete task. Enter 'List' to view all list. Enter 'Exit' to stop.", input.PersistentData);
        }
        else if (message == "exit")
        {
            input.Callbacks.EndSession();
            return new PluginOutput("List stopped.", input.PersistentData);
        }
        else if (message.StartsWith("add"))
        {
            var str = input.Message.Substring("add".Length).Trim();
            list.Add(str);

            var data = new PersistentDataStructure(list);

            return new PluginOutput($"New task: {str}", JsonSerializer.Serialize(data));
        }
        else if (message.StartsWith("delete"))
        {
            var taskToDelete = message.Substring("delete".Length).Trim();
            if (string.IsNullOrEmpty(taskToDelete))
            {
                if (list.Count == 0)
                {
                    return new PluginOutput("Error: No tasks to delete.", input.PersistentData);
                }
                list.RemoveAt(list.Count - 1);
                return new PluginOutput("Deleted the last task.", JsonSerializer.Serialize(new PersistentDataStructure(list)));
            }
            else
            {
                if (list.Contains(taskToDelete))
                {
                    list.Remove(taskToDelete);
                    return new PluginOutput($"Deleted task: {taskToDelete}", JsonSerializer.Serialize(new PersistentDataStructure(list)));
                }
                else
                {
                    return new PluginOutput($"Error: Task '{taskToDelete}' not found.", input.PersistentData);
                }
            }
        }
        else if (message == "list")
        {
            if (list.Count == 0)
            {
                return new PluginOutput("The list is empty.", input.PersistentData);
            }
            string listtasks = string.Join("\r\n", list);
            return new PluginOutput($"All list tasks:\r\n{listtasks}", input.PersistentData);
        }
        else
        {
            return new PluginOutput("Error! Enter 'Add' to add task. Enter 'Delete' followed by task name to delete a task. Enter 'List' to view all list. Enter 'Exit' to stop.");
        }
    }
}
